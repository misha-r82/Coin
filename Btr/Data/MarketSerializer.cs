using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin.History;

namespace Coin.Files
{
    public class MarketSerializer
    {
        public const int VER = 0;
        private const char SEPARATOR = '_';
        private const string FILE_EXT = "mar";
        public static string MarDataDir { get; set; } = "c:\\Markt\\";

        private static string GetFileName(Market market)
        {
            return MarDataDir + market.Api.Name + SEPARATOR + market.Name + '.' + FILE_EXT;
        }
        public static void SerializeMarket(Market market)
        {
            using (FileStream stream = new FileStream(GetFileName(market), FileMode.Create))
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
                        writer.Write(item.vol);
                    }
                }
                
            }
        }

        public static Market DeserializeMarket(Market market)
        {
            string name = "";
            var data = new List<CourseItem>();
            try
            {
                using (FileStream stream = new FileStream(GetFileName(market), FileMode.Open))
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
                                double vol = reader.ReadDouble();
                                var item = new CourseItem(new DateTime(ticks), course, delta, vol);
                                data.Add(item);
                            }
                        }
                    }
                    catch (EndOfStreamException e) { }
                }
                market.CourseData = data.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return market;

        }
    }
}
