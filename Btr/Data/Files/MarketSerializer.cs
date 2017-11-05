using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Btr.History;

namespace Btr.Files
{
    public class MarketSerializer
    {
        public const int VER = 0;
        public static void SerializeMarket(Market market, string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(VER);
                    writer.Write(market.Name);
                    foreach (CourseItem item in market.CourseData)
                    {
                        writer.Write(item.date.Ticks);
                        writer.Write(item.course);
                        writer.Write(item.delta);
                    }
                }
                
            }
        }

        public static Market DeserializeMarket(string file)
        {
            string name = "";
            var data = new List<CourseItem>();
            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int ver = reader.ReadInt32();
                        name = reader.ReadString();
                        while (true)
                        {
                            var ticks = reader.ReadInt64();
                            var course = reader.ReadDouble();
                            double delta = reader.ReadDouble();
                            var item = new CourseItem(new DateTime(ticks), course, delta);
                            data.Add(item);
                        }
                    }
                }
                catch (EndOfStreamException e) { }
            }
            var market = new Market(name);
            market.CourseData = data.ToArray();
            return market;

        }
    }
}
