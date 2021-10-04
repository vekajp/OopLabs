using System.Collections.Generic;
using System.Linq;

namespace IsuExtra.TimeEntities
{
    public class Schedule
    {
        private List<Lesson> _lessons;
        public Schedule(List<Lesson> lessons)
        {
            _lessons = lessons;
        }

        public bool Intersects(Schedule other)
        {
            return _lessons.Any(other.IntersectsWithLesson);
        }

        private bool IntersectsWithLesson(Lesson otherLesson)
        {
            return _lessons.Any(lesson => lesson.Intersects(otherLesson));
        }
    }
}