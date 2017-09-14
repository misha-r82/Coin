using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Btr.History;

namespace Btr
{
    public class Market
    {
        public Market(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public PlnCouse.CouseItem[] CourseData;
        public double Qmax { get; private set; }
        public void LoadHistory(DateTime from, DateTime to)
        {
            var course = new PlnCouse();
            CourseData = course.GetCouse(Name, from, to, new TimeSpan(0, 10, 0)).ToArray();
            for (int i = 0; i < CourseData.Length - 1; i++)
                CourseData[i].delta = CourseData[i + 1].course - CourseData[i].course;
        }
    }
}
