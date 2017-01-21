using System;
using System.Collections.Generic;
using Realms;

namespace DailyTick.Models
{
    public class ApplicationSettings : RealmObject
    {
        public DateTimeOffset LastStartTime { get; set; }
    }
}
