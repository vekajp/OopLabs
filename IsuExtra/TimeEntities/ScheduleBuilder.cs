using System;
using System.Collections.Generic;
using System.Linq;

namespace IsuExtra.TimeEntities
{
    public class ScheduleBuilder
    {
        private List<Lesson> _lessons;
        public ScheduleBuilder()
        {
            _lessons = new List<Lesson>();
        }

        public void AddLesson(Lesson lesson)
        {
            if (LessonIntersects(lesson))
            {
                throw new ArgumentException("Lesson intersects with current schedule");
            }

            _lessons.Add(lesson);
        }

        public void AddLesson(DayOfWeek day, TimeSpan span, string teacher, int auditory)
        {
            Lesson lesson = new Lesson(day, span, teacher, auditory);
            AddLesson(lesson);
        }

        public Schedule MakeSchedule()
        {
            return new Schedule(_lessons);
        }

        public void Destroy()
        {
            _lessons.Clear();
        }

        private bool LessonIntersects(Lesson lesson)
        {
            return _lessons.Any(x => x.Intersects(lesson));
        }
    }
}