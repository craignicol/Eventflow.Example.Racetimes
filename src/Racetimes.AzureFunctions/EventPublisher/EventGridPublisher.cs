using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Racetimes.AzureFunctions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Racetimes.AzureFunctions.EventPublisher
{
    public class EventGridPublisher
    {
        public static void Publish(IEventGridClient client, string endpoint, params EntryDTO[] entryEvents)
        {
            client.PublishEventsAsync(endpoint, GetEventsList(entryEvents)).GetAwaiter().GetResult();
            Console.Write("Published events to Event Grid domain.");
            Console.ReadLine();
        }

        private static IList<EventGridEvent> GetEventsList(IEnumerable<EntryDTO> entries)
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

