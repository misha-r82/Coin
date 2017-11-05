using System;

namespace Btr
{
    public struct CourseItem
    {
        public CourseItem(DateTime date, double course, double delta)
        {
            this.date = date;
            this.course = course;
            this.delta = delta;
        }
        public DateTime date;
        public double course;
        public double delta;
        public override string ToString()
        {
            return string.Format("{0} {1} d={2}", date, course, delta);
        }
    }

}