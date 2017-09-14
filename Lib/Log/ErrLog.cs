using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace YAK.Log
{
    public class ErrRec
    {

        public ErrRec(object source, Exception exeption, string data)
        {
            Date = DateTime.Now;
            SetMsg(exeption);
            Source = source.ToString();
            Exeption = exeption.ToString();
            Data = data;
        }

        public DateTime Date { get; }
        public string Source { get; }
        public string Msg { get; private set; }
        public string Exeption { get; }
        public string Data { get; }

        private void SetMsg(Exception exception)
        {
            StringBuilder sb = new StringBuilder(exception.Message);
            Exception ex = exception;
            while (ex != null)
            {
                sb.Append('\n' + ex.Message);
                ex = ex.InnerException;
            }
            Msg = sb.ToString();
        }

    }
    public static class ErrLog
    {
        private const string INSERT_PATT = @"INSERT INTO [dbo].[err_log]
        ([date],[source],[msg],[exeption],[user],[data])
        VALUES (@DATE,@source,@msg,@exeption,@user,@data)";
        public static DBManager DbMan { private get; set; }
        public static string UserRow { get; set; }
        public static void CreateLog(object source, Exception exeption, string data)
        {
            var rec = new ErrRec(source, exeption, data);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @date as datetime set @date = '{0}'\n", DBManager.FormatDateTime(rec.Date));
            sb.AppendFormat("DECLARE @source as [varchar](255) set @source = '{0}'\n", rec.Source);
            sb.AppendFormat("DECLARE @msg as [varchar](max) set @msg = '{0}'\n", rec.Msg);
            sb.AppendFormat("DECLARE @exeption as [varchar](max) set @exeption = '{0}'\n", rec.Exeption);
            sb.AppendFormat("DECLARE @user as [varchar](255) set @user = '{0}'\n", UserRow);
            sb.AppendFormat("DECLARE @data as [varchar](max) set @data = '{0}'\n", data);
            sb.Append(INSERT_PATT);
            string sql = string.Format(sb.ToString());
            try
            {
                DbMan.execNonQuery(sql);
            }
            catch (Exception ex)
            {

            }

        }
    }
}
