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
            return (from lesson in _lessons from otherLesson in other._lessons where otherLesson.Intersects(lesson) select lesson).Any();
        }
    }
}