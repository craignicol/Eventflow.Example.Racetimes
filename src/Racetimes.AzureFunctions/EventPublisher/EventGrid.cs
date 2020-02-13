using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Racetimes.AzureFunctions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Racetimes.AzureFunctions.EventPublisher
{
    class EventGrid
    {
        static void Publish(params EntryDTO[] entryEvents)
        {
            // TODO: Enter values for <domain-name> and <region>. You can find this domain endpoint value
            // in the "Overview" section in the "Event Grid Domains" blade in Azure Portal.
            string domainEndpoint = "https://<YOUR-DOMAIN-NAME>.<REGION-NAME>-1.eventgrid.azure.net/api/events";

            // TODO: Enter value for <domain-key>. You can find this in the "Access Keys" section in the
            // "Event Grid Domains" blade in Azure Portal.
            string domainKey = "<YOUR-DOMAIN-KEY>";

            string domainHostname = new Uri(domainEndpoint).Host;
            TopicCredentials domainKeyCredentials = new TopicCredentials(domainKey);
            EventGridClient client = new EventGridClient(domainKeyCredentials);

            client.PublishEventsAsync(domainHostname, GetEventsList(entryEvents)).GetAwaiter().GetResult();
            Console.Write("Published events to Event Grid domain.");
            Console.ReadLine();
        }

        static IList<EventGridEvent> GetEventsList(IEnumerable<EntryDTO> entries)
        {
            List<EventGridEvent> eventsList = new List<EventGridEvent>();

            foreach (var entry in entries)
            {
                eventsList.Add(new EventGridEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Contoso.Items.ItemReceived",

                    // TODO: Specify the name of the topic (under the domain) to which this event is destined for.
                    // Currently using a topic name "domaintopic0"
                    Topic = "domaintopic",
                    Data = entry,
                    EventTime = DateTime.Now,
                    Subject = "BLUE",
                    DataVersion = "2.0"
                });
            }

            return eventsList;
        }
    }
}

