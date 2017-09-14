using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Lib.Annotations;
using System.Windows.Forms;
using Microsoft;
using Application = System.Windows.Forms.Application;

namespace Lib
{
    public class StatusIndicator : INotifyPropertyChanged
    {
        public long countAll;
        public long current;
        public string currentName;
        public StatusProc status;
        private string userText;
        public int DisplayDivie = 1;
        private int displayCount = 0;
        private Stopwatch sw;
        public int maxIndicateMillisec = 100;
        private Progress<KVPair<object, PropertyChangedEventArgs>> progress;
        #region Fields
        public long Current
        {
            get { return current; }
            set
            {
                if (value == current) return;
                current = value;
                if (++displayCount == DisplayDivie)
                {
                    OnPropertyChanged("Current");
                    displayCount = 0;
                }
                    
            }
        }

        public long CountAll
        {
            get { return countAll; }
            set
            {
                if (value == countAll) return;
                countAll = value;
                if (displayCount < 2)
                    OnPropertyChanged("CountAll");
            }
        }

        public string CurrentName
        {
            get { return currentName; }
            set
            {
                if (value == currentName) return;
                currentName = value;
                if (displayCount < 2)
                    OnPropertyChanged("CurrentName");
            }
        }

        public StatusProc Status
        {
            get { return status; }
            set
            {
                if (value == status) return;
                status = value;
                OnPropertyChanged("Status");
            }
        }
        public string UserText
        {
            get { return userText; }
            set
            {
                if (value == userText) return;
                userText = value;
                OnPropertyChanged("UserText");
            }
        }
#endregion
        public enum StatusProc
        {
            Error,
            InProcess,
            IsCancel
        }


        public object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }  


        public StatusIndicator()
        {
            countAll = -1;
            current = -1;
            currentName = "";
            status = StatusProc.Error;
            sw = Stopwatch.StartNew();
            progress = new Progress<KVPair<object, PropertyChangedEventArgs>>(p => PropertyChanged(p.Key, p.Val));
        }

        public void ResetStatus()
        {
            countAll = -1;
            current = -1;
            currentName = "";
            status = StatusProc.InProcess;           
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (sw.Elapsed.Milliseconds > maxIndicateMillisec)
            {
                ///
                sw.Restart();
            }
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                ((IProgress<KVPair<object, PropertyChangedEventArgs>>) progress).Report(
                    new KVPair<object, PropertyChangedEventArgs>(this, new PropertyChangedEventArgs(propertyName)));
                //sContext.Send((a) => handler(this, new PropertyChangedEventArgs(propertyName)), null);
                ;
            }
            
        }
    }

    public class Logger : List<Logger.LogRec>
    {
        public List<LogRec> Log;
        public string Folder;
        public string FileName;

        public Logger()
        {
            Folder = @"C:\";
        }
        public struct LogRec
        {
            private DateTime time;
            private string rec;
            
            public string Rec { get { return rec; } }
            public DateTime Time { get { return time; } }
            public LogRec(string rec)
            {
                time = DateTime.Now;
                this.rec = rec;
            }
            public override string ToString()
            {
                return time + " : " + rec;
            }
        }

        public void CreateLog(string record, bool toFile = false)
        {
            LogRec rec = new LogRec(record);
            Add(rec);
            if (toFile)
                SaveLog(rec);
        }

        public void CreateLog(bool toFile, params object[] pars)
        {
            string str = "";
            for (int i = 0; i < pars.Length-1; i++)
            {
                str += pars[i] + " ";
            }
            str += pars[pars.Length - 1];
            CreateLog(str, toFile);
        }
        public void SaveLog(LogRec rec)
        {
            StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(Folder + @"\" + FileName, true);
                    sw.WriteLine(rec.ToString());
                }
                catch (Exception ex)
                {
                    sw = null;
                }                
                finally { sw.Close(); }

                

            
        }
    }
}
