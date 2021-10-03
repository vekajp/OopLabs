using System;

namespace IsuExtra.TimeEntities
{
    public class TimeSpan
    {
        private Time _begin;
        private Time _end;

        public TimeSpan(Time begin, Time end)
        {
            if (begin >= end)
            {
                throw new ArgumentException("Invalid time frame", nameof(begin));
            }

            _begin = begin;
            _end = end;
        }

        public TimeSpan(AcademicClass lesson)
        {
            switch (lesson)
            {
                case AcademicClass.One:
                    _begin = new Time(8, 20);
                    _end = new Time(9, 50);
                    break;
                case AcademicClass.Two:
                    _begin = new Time(10, 0);
                    _end = new Time(11, 30);
                    break;
                case AcademicClass.Three:
                    _begin = new Time(11, 40);
                    _end = new Time(13, 10);
                    break;
                case AcademicClass.Four:
                    _begin = new Time(13, 30);
                    _end = new Time(15, 00);
                    break;
                case AcademicClass.Five:
                    _begin = new Time(15, 20);
                    _end = new Time(16, 50);
                    break;
                case AcademicClass.Six:
                    _begin = new Time(17, 00);
                    _end = new Time(18, 30);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lesson), lesson, null);
            }
        }

        public bool Intersects(TimeSpan other)
        {
            return (_begin <= other._end && _end >= other._end) || (_end >= other._begin && _begin <= other._begin);
        }
    }
}