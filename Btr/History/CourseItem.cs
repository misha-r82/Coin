using System;

namespace Coin
{
    public struct CourseItem
    {
        public CourseItem(DateTime date, double course, double delta, double vol)
        {
            this.date = date;
            this.course = course;
            this.delta = delta;
            this.vol = vol;
        }
        public DateTime date;
        public double course;
        public double delta;
        public double vol;
        public override string ToString()
        {
            return string.Format("{0} {1} d={2}", date, course, delta);
        }
    }

}