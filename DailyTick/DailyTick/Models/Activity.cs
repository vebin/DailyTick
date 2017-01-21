using System;
using System.Collections.Generic;
using Realms;

namespace DailyTick.Models
{
    public class Activity : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset StopTime { get; set; }
        public string Subject { get; set; }
        public string Memo { get; set; }

        public IList<Tag> Tags { get; }
    }
}
