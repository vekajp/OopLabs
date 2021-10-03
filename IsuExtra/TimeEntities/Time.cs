using System;

namespace IsuExtra.TimeEntities
{
    public class Time : IComparable<Time>
    {
        private DateTime _date;
        public Time(int hour, int minute)
        {
            _date = new DateTime(1, 1, 1, hour, minute, 0);
        }

        private int Hour => _date.Hour;

        private int Minute => _date.Minute;

        public static bool operator <=(Time lhs, Time rhs)
        {
            return lhs.Hour < rhs.Hour || (lhs.Hour == rhs.Hour && lhs.Minute <= rhs.Minute);
        }

        public static bool operator >=(Time lhs, Time rhs)
        {
            return lhs.Hour > rhs.Hour || (lhs.Hour == rhs.Hour && lhs.Minute >= rhs.Minute);
        }

        public int CompareTo(Time other)
        {
            if (Hour == other.Hour && Minute == other.Minute) return 0;
            if (Hour < other.Hour || (Hour == other.Hour && Minute < other.Minute)) return -1;
            return 1;
        }
    }
}