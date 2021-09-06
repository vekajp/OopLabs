using System;

namespace Isu.Services
{
    public class CourseNumber
    {
        public CourseNumber(int number)
        {
            if (number > 9) throw new ArgumentOutOfRangeException("number");
            Number = number;
        }

        public int Number
        {
            get;
        }
    }
}