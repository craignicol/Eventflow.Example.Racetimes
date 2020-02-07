using System;
using System.Collections.Generic;
using System.Text;

namespace Racetimes.AzureFunctions.Models
{
    public class EntryDTO
    {
        public string CompetitionId { get; internal set; }
        public string Discipline { get; internal set; }
        public string Name { get; internal set; }
        public int TimeInMillis { get; internal set; }
        public string EventId { get; internal set; }
    }
}
