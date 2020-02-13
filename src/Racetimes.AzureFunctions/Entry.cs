using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EventFlow.Configuration;
using EventFlow;
using Racetimes.Domain.Command;
using System.Threading;
using Racetimes.Domain.Identity;
using Racetimes.AzureFunctions.Models;
using EventFlow.Queries;
using Racetimes.ReadModel.EntityFramework;
using System.Linq;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Options;

namespace Racetimes.AzureFunctions
{
    public class Entry
    {
        private readonly ICommandBus _eventFlow;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IEventGridClient _eventGrid;

        // TODO : Move this to Singleton lifecycle in Startup
        private IEventGridClient CreateEventGridClient(IOptions<EventGrid> eventGridOptions)
        {
            var eventGridEndpoint = eventGridOptions.Value?.Endpoint;
            var eventGridApiKey = eventGridOptions.Value?.Endpoint;

            Console.Write($"Creating Event Grid Client for {eventGridEndpoint} with <{eventGridApiKey}>.");

            TopicCredentials domainKeyCredentials = new TopicCredentials(eventGridApiKey);
            EventGridClient client = new EventGridClient(domainKeyCredentials);
            return client;
        }

        public Entry(IRootResolver resolver, IOptions<EventGrid> eventGridOptions)
        {
            _eventFlow = resolver.Resolve<ICommandBus>();
            _queryProcessor = resolver.Resolve<IQueryProcessor>();
            _eventGrid = CreateEventGridClient(eventGridOptions);
        }

        [FunctionName("GetEntries")]
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "entry")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var evts = _queryProcessor.Process(new GetAllEntriesQuery(), CancellationToken.None);

            if (evts == null)
            {
                return new NotFoundObjectResult($"Cannot get entries");
            }

            return new OkObjectResult(
                from evt in evts                                      
                select
                new EntryDTO
                {
                    EventId = evt.Id,
                    Discipline = evt.Discipline,
                    Name = evt.Competitor,
                    TimeInMillis = evt.TimeInMillis
                }
            );
        }

        [FunctionName("GetEntry")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "entry/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if(string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult("Entry id is missing.");
            }

            var evt = _queryProcessor.Process(new ReadModelByIdQuery<EntryReadModel>(EntryId.With(id)), CancellationToken.None);

            if (evt == null)
            {
                return new NotFoundObjectResult($"Entry {id} does not exist");
            }

            return new OkObjectResult(new EntryDTO
            {
                EventId = evt.Id,
                Discipline = evt.Discipline,
                Name = evt.Competitor,
                TimeInMillis = evt.TimeInMillis
            });
        }

        [FunctionName("PostEntry")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "entry")] HttpRequest req,
            ILogger log)
        {
            EntryDTO data = await GetEntryData(req);
            if (data == null)
            {
                return new BadRequestObjectResult("Must provide Entry details in POST body.");
            }

            var competitionId = CompetitionId.With(data.CompetitionId);
            var entryId = EntryId.New;

            RecordEntryCommand recordEntryCommand = new RecordEntryCommand(competitionId, entryId, data.Discipline, data.Name, data.TimeInMillis);
            var result = await _eventFlow.PublishAsync(recordEntryCommand, CancellationToken.None);

            return result?.IsSuccess == true
                ? (ActionResult)new OkObjectResult($"{recordEntryCommand.EntryId}")
                : new BadRequestObjectResult("Cannot create new entry");
        }

        [FunctionName("PutEntry")]
        public async Task<IActionResult> Put(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "entry/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            EntryDTO data = await GetEntryData(req);
            if (data == null)
            {
                return new BadRequestObjectResult("Must provide Entry details in POST body.");
            }

            var competitionId = CompetitionId.With(data.CompetitionId);
            var entryId = EntryId.With(id);

            CorrectEntryTimeCommand correctEntryTimeCommand = new CorrectEntryTimeCommand(competitionId, entryId, data.TimeInMillis);
            var result = await _eventFlow.PublishAsync(correctEntryTimeCommand, CancellationToken.None);

            return result?.IsSuccess == true
                ? (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(correctEntryTimeCommand)}")
                : new BadRequestObjectResult($"Cannot or did not update entry {id}");
        }

        private static async Task<EntryDTO> GetEntryData(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic rawData = JsonConvert.DeserializeObject(requestBody);
            if (rawData == null)
            {
                return null;
            }

            var data = new EntryDTO
            {
                CompetitionId = rawData?.CompetitionId ?? CompetitionId.New,
                Discipline = rawData?.Discipline,
                Name = rawData?.Name,
                TimeInMillis = rawData?.TimeInMillis ?? -1
            };

            return data;
        }
    }
}
