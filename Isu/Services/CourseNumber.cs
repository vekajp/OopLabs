using System;

namespace Isu.Services
{
    public class CourseNumber
    {
        private const int MinCourse = 1;
        private const int MaxCourse = 5;
        public CourseNumber(int number)
        {
            if (number is > MaxCourse or < MinCourse) throw new ArgumentOutOfRangeException(nameof(number));
            Number = number;
        }

        public int Number { get; }
    }
}