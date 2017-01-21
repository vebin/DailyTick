using System;
using System.Linq;
using Realms;

namespace DailyTick.Models
{
    public class Tag : RealmObject
    {
        public string Name { get; set; }
        public int UseTimes { get; set; }
        public DateTimeOffset LastUsedTime { get; set; }

        [Backlink(nameof(Activity.Tags))]
        public IQueryable<Activity> Activities { get; }
    }
}
