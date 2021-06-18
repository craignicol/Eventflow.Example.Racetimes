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

namespace Racetimes.AzureFunctions
{
    public class Entry
    {
        private readonly ICommandBus _eventFlow;
 
        public Entry(ICommandBus eventFlow)
        {
            _eventFlow = eventFlow;
        }

        [FunctionName("GetEntries")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "entry")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("GetEntry")]
        public static IActionResult Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "entry/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"C# HTTP trigger function processed a request {req.Path}.");

            return new NotFoundObjectResult($"Entry {id} does not exist");
        }

        [FunctionName("PostEntry")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "entry")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("PostEntry called");
            EntryDTO data = await GetEntryData(req);
            if (data == null)
            {
                return new BadRequestObjectResult("Must provide Entry details in POST body.");
            }

            var competitionId = CompetitionId.With(data.CompetitionId);
            var entryId = EntryId.New;

            RecordEntryCommand recordEntryCommand = new(competitionId, entryId, data.Discipline, data.Name, data.TimeInMillis);
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
            log.LogInformation($"PutEntry({id})");
            EntryDTO data = await GetEntryData(req);
            if (data == null)
            {
                return new BadRequestObjectResult("Must provide Entry details in POST body.");
            }

            var competitionId = CompetitionId.With(data.CompetitionId);
            var entryId = EntryId.With(id);

            CorrectEntryTimeCommand correctEntryTimeCommand = new(competitionId, entryId, data.TimeInMillis);
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
