using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace YAN2
{
    public static class Files
    {
        public static SortedList<Int32, Int32> LoadI32I32(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            BinaryReader br = new BinaryReader(fs);
            int readCount = (Int32)br.BaseStream.Length / sizeof(uint) / 2;
            var rez = new SortedList<Int32, Int32>();
            try
            {
                for (Int32 c = 0; c < readCount; c++)
                {
                    Int32 key = br.ReadInt32();
                    Int32 val = br.ReadInt32();
                    rez.Add(key, val);
                }
                return rez;
            }
            finally
            {
                br.Close();
                fs.Close();
            }
        }
        public static void SaveI32I32(string fn, IDictionary<Int32, Int32> data)
        {
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                foreach (Int32 key in data.Keys)
                {
                    bw.Write(key);
                    Int32 val = data[key];
                    bw.Write(val);
                }
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
        }

        public static SortedList<uint, SortedList<uint, int>> LoadList2D(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            BinaryReader br = new BinaryReader(fs);
            int readCount = (Int32)br.BaseStream.Length / sizeof(uint) / 3;
            var rez = new SortedList<uint, SortedList<uint, int>>();
            SortedList<uint, int> data = new SortedList<uint, int>();
            uint previosItemId = 0;
            bool newItem = false;
            try
            {
                for (Int32 c = 0; c < readCount; c++)
                {
                    uint itemId = br.ReadUInt32();
                    uint key = br.ReadUInt32();
                    int val = br.ReadInt32();
                    if (previosItemId != itemId && (!newItem))
                    {
                        rez.Add(previosItemId, data);
                        data = new SortedList<uint, int>();
                    }
                    data.Add(key, val);
                    previosItemId = itemId;
                }
                return rez;
            }
            finally
            {
                br.Close();
                fs.Close();
            }
        }
        public static void SaveList2D(string fn, SortedList<uint, SortedList<uint, int>> data)
        {
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                foreach (uint key in data.Keys)
                {
                    var Item = data[key];
                    foreach (KeyValuePair<uint, int> pair in Item)
                    {
                        bw.Write(key);
                        bw.Write(pair.Key);
                        bw.Write(pair.Value);                        
                    }
                }
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
        }
        public static SortedList<uint, Int32> LoadU32I32(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            BinaryReader br = new BinaryReader(fs);
            int readCount = (int)br.BaseStream.Length / sizeof(uint) / 2;
            var rez = new SortedList<uint, Int32>();
            try
            {
                for (int c = 0; c < readCount; c++)
                {
                    uint key = br.ReadUInt32();
                    int val = br.ReadInt32();
                    rez.Add(key, val);
                }
                return rez;
            }
            finally
            {
                br.Close();
                fs.Close();
            }
        }
        public static void SaveU32I32(string fn, IDictionary<uint, int> data)
        {
            FileStream fs = new FileStream(fn, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                foreach (uint key in data.Keys)
                {
                    bw.Write(key);
                    int val = data[key];
                    bw.Write(val);
                }
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
        }

        public static void saveCsv(string fn, IDictionary<uint, int> data)
        {
            StreamWriter sw = new StreamWriter(fn);
            foreach (var itm in data)
            {
                sw.WriteLine(itm.Key + ";" + itm.Value);
            }

        }
        public static List<string> GetListOfFiles(string dir, DateTime d1, DateTime d2)
        {
            var sl = new List<string>();
            DateTime dt = d1;
            do
            {
                string ss = DateToFname(dir, dt);
                if (!File.Exists(ss))
                {
                    //ToLog(tb, "Файла " + ss + " нет на диске!");
                }
                else
                {
                    sl.Add(ss);
                }
                dt = dt.AddDays(1);
            }
            while (dt <= d2);
            return sl;
        }
        public static string DateToFname(string dir, DateTime dt)
        {
            string ss;
            ss = dt.ToString("yyyyMMdd");
            ss = dir + ss + ".txt";
            return ss;
        }
    }
}
