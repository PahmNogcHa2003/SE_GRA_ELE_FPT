using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public static class QuestTypes
    {
        public const string Distance = "Distance";
        public const string Trips = "Trips";
        public const string Duration = "Duration";

        public static readonly string[] All =
        {
            Distance, Trips, Duration
        };
    }

    public static class QuestScopes
    {
        public const string Daily = "Daily";
        public const string Weekly = "Weekly";
        public const string Monthly = "Monthly";

        public static readonly string[] All =
        {
            Daily, Weekly, Monthly
        };
    }

    public static class QuestStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
    }
}

