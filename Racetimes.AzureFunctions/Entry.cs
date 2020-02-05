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
        public async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "entry")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("GetEntry")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "entry/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return new NotFoundObjectResult($"Entry {id} does not exist");
        }

        [FunctionName("PostEntry")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "entry")] HttpRequest req,
            ILogger log)
        {
            // TODO : Get these as parameters
            var competitionId = CompetitionId.New;
            var entryId = EntryId.New;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var rnd = new Random();

            RecordEntryCommand recordEntryCommand = new RecordEntryCommand(competitionId, entryId, data?.discipline ?? "No Discipline", data?.name ?? "No Name", data?.time ?? rnd.Next(0, 100000));
            var result = await _eventFlow.PublishAsync(recordEntryCommand, CancellationToken.None);

            return result?.IsSuccess == true
                ? (ActionResult)new OkObjectResult($"{JsonConvert.SerializeObject(recordEntryCommand)}")
                : new BadRequestObjectResult("Cannot create new entry");
        }

        [FunctionName("PutEntry")]
        public async Task<IActionResult> Put(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "entry/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            return new NotFoundObjectResult($"Entry {id} does not exist");
        }
    }
}
