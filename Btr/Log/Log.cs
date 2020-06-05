using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr.Log
{
    public static class Log
    {

        public struct Record
        {
            public DateTime Date { get; }
            public string Type { get; }
            public string Message { get; }

            public Record(string type, string msg)
            {
                Date = DateTime.Now;
                Type = type;
                Message = msg;
            }

            public override string ToString()
            {
                return string.Format("{0:dd.MM.yyyy HH:mm}\t{1}\t{2}", Date, Type, Message);
            }
        }
        public static ObservableCollection<Record> Data;
        public static string Path { get; set; } = "Log.txt";
        static Log()
        {
            Data = new ObservableCollection<Record>();
        }

        public static void CreateLog(string type, string msg)
        {
            var rec = new Record(type, msg);
            Data.Add(rec); 
            StreamWriter sw = new StreamWriter(Path, true);
            sw.WriteLine(rec.ToString());
            sw.Close();
        }
    }
}
