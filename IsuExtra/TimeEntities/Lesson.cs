using System;

namespace IsuExtra.TimeEntities
{
    public class Lesson
    {
        private DayOfWeek _day;
        private TimeSpan _span;
        private string _teacher;
        private int _auditory;

        public Lesson(DayOfWeek day, TimeSpan span, string teacherName, int auditory)
        {
            _day = day;
            _span = span;
            _teacher = teacherName;
            _auditory = auditory;
        }

        public bool Intersects(Lesson other)
        {
            return _day == other._day && _span.Intersects(other._span);
        }
    }
}