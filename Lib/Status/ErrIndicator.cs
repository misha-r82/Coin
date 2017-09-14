using System.ComponentModel;
using Lib.Annotations;
using Timer = System.Threading.Timer;

namespace Lib.Process
{
    public class ErrIndicator : INotifyPropertyChanged
    {
        private string errMsg;
        private int errTime, curTime, errCount, errMaxCount;
        private Timer timer;
        private bool cansel;
        private bool errEqure;
        #region Fields
        public int CurTime
        {
            get { return curTime; }
            set
            {
                if (value == curTime) return;
                curTime = value;
                OnPropertyChanged("CurTime");
            }
        }

        public string ErrMsg
        {
            get { return errMsg; }
            set
            {
                if (value == errMsg) return;
                errMsg = value;
                OnPropertyChanged("ErrMsg");
            }
        }

        public int ErrTime
        {
            get { return errTime; }
            set
            {
                if (value == errTime) return;
                errTime = value;
                OnPropertyChanged("ErrTime");
            }
        }
        public bool Cansel
        {
            get { return cansel; }
            set
            {
                if (value.Equals(cansel)) return;
                cansel = value;
                OnPropertyChanged("Cansel");
            }
        }

        public int ErrCount
        {
            get { return errCount; }
            set
            {
                if (value == errCount) return;
                errCount = value;
                OnPropertyChanged("ErrCount");
            }
        }

        public int ErrMaxCount
        {
            get { return errMaxCount; }
            set
            {
                if (value == errMaxCount) return;
                errMaxCount = value;
                OnPropertyChanged("ErrMaxCount");
            }
        }

        public bool ErrEqure
        {
            get { return errEqure; }
            set
            {
                if (value.Equals(errEqure)) return;
                errEqure = value;
                OnPropertyChanged("ErrEqure");
            }
        }
        public bool IsErrTime { get { return errTime != curTime; } }
#endregion
        public void SetErr(string msg, int time = 5)
        {
            ErrMsg = msg;
            ErrTime = time;
            CurTime = 0;
            timer = new Timer(OnTimer, null, 1000, 1000);
        }
        public void Reset()
        {
            ErrEqure = false;
            Cansel = false;
            ResetTime();
        }
        public void ResetTime()
        {
            timer.Dispose();
            ErrTime = CurTime = 0;                
        }
        private void OnTimer(object obj)
        {
            if (++CurTime == ErrTime) ResetTime();       
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
