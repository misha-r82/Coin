using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lib.Process;
using YAK.Log;


namespace Lib
{
    public class DBManager
    {
        private const int RESULT_LIST_CAPACITY = 1024*16;
        private const int QUER_TO_UNION = 16;
        SqlConnection conn;
        private SqlConnectionStringBuilder sb;
        private int sql_timeout; // в секундах
        private StatusIndicator statusIndicator;
        private ErrIndicator errIndicator;
        private Logger logger;
        public StatusIndicator StatusIndicator
        {
            get { return statusIndicator; }
            set { statusIndicator = value; }
        }

        public static string FormatDateTime(DateTime date)
        {
            string rez = date.ToString("yyyy-MM-dd") + "T" + date.ToString("HH:mm:ss.fff");
            return rez;
        }

        public static string GetPeriodCond(DatePeriod period, string field)
        {
            string result = String.Format("{0} BETWEEN '{1}' AND '{2}'", 
                field, FormatDateTime(period.From), 
                FormatDateTime(period.To));
            return result;
        }
        public ErrIndicator ErrIndicator { get { return errIndicator; } }
        private void ResetStatus(int count)
        {
            if (statusIndicator != null)
            {
                statusIndicator.CountAll = count;
                statusIndicator.Current = 0;
            }            
        }

        public DBManager(SqlConnectionStringBuilder sb, int sql_timeout = 120)
        {
            this.sb = sb;
            this.sql_timeout = sql_timeout;
            statusIndicator = new StatusIndicator();
            errIndicator = new ErrIndicator();
            logger = new Logger(){FileName = "dbErr.txt"};
        }
        bool OpenConn()
        {
            try
            {
                sb.AsynchronousProcessing = true;
                conn = new SqlConnection(sb.ToString());
                conn.Open();
                //throw new Exception("test ex");
            }
            catch (Exception e)
            {
                ErrLog.CreateLog(this, e, conn.ConnectionString);
                CloseConn();
                return false;
            }
            return true;
        }
        void CloseConn()
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }

        public void execNonQuery(object command)
        {
            OpenConn();
            var cmd = command as SqlCommand;
            if (cmd != null) cmd.Connection = conn;
            if (cmd == null) cmd = new SqlCommand(command.ToString(), conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ErrLog.CreateLog(this, ex, cmd.CommandText);
                throw;
            }
            
            CloseConn();
        }


        public List<object[]> query(string sql)
        {
            List<object[]> Result = new List<object[]>(RESULT_LIST_CAPACITY);
            do
            {
                if (!OpenConn()) return null;
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = sql_timeout;
                try
                {
                    var ar = cmd.BeginExecuteReader();
                    while (!ar.IsCompleted)
                    {
                         if (StatusIndicator.status == StatusIndicator.StatusProc.IsCancel)
                        {
                            cmd.Cancel();
                            CloseConn();
                            throw new TaskCanceledException();
                        }        
                        Thread.Sleep(20);               
                    }


                    var dr = cmd.EndExecuteReader(ar);
                    if (dr.HasRows)
                        while (dr.Read())
                        {
                            object[] row = new object[dr.FieldCount];
                            dr.GetValues(row);
                            Result.Add(row);
                        }
                    dr.Close();
                    ErrIndicator.ErrEqure = false;
                }
                catch (Exception e)
                {
                    ErrLog.CreateLog(this,e,sql);
                    if (e as TaskCanceledException != null) throw e;
                    logger.CreateLog(true, e.Message, sql);
                    if (statusIndicator.Status == StatusIndicator.StatusProc.IsCancel) return null;
                    if (errIndicator.ErrCount++ < errIndicator.ErrMaxCount)
                        ErrIndicator.SetErr(e.Message);
                    else
                    {
                        Exception ex = new Exception("ошибка DbMan", e);
                        ex.Data.Add("sql", sql);
                        throw ex;
                    }                        
                }              
                CloseConn();
                while (ErrIndicator.IsErrTime)// ждем таймаут
                    if (ErrIndicator.Cansel)
                    {
                        errIndicator.Reset();
                        return new List<object[]>();
                    }
            } while (ErrIndicator.ErrEqure);           
            CloseConn();
            return Result;
        }
        
    }
}
