using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Lib.Annotations;

namespace Lib
{
    public interface IReturnString
    {
        string ToString();
    }
    public class StrPatt : List<object>, IReturnString, ICloneable // позволяет многоуровневые замены строк ...{...{}...}...{}...
    {
        public string source;
        private StrPatt thisClone;
        public StrPatt(){source = "";}
        public StrPatt(string source, params object[] pars):base(4)
        {
            this.source = source;
            if (pars!=null)
            AddRange(pars);
        }
        public static string strFromObj(object obj)
        {
            // заменяем все элементы типа известных типов на их строковые представлоения
            if (obj == null) return ""; 
            if (obj.GetType() == typeof(StrPatt))
                return ((StrPatt) obj).ToString();
            if (obj.GetType() == typeof (StrSepararorPatt))
                return ((StrSepararorPatt) obj).ToString();
            return obj.ToString();   
        }
        protected string[] getStrParsArr() // возвращает строковый массив, раскрывая вложенные объекты тапа шблонов
        {
            string[] rez = new string[Count];
            int i = 0;
            foreach (object item in this)
                rez[i++] = strFromObj(item);
            return rez;
        }
        public override string ToString()
        {
            var parArr = getStrParsArr();
            return (parArr.Length > 0) ? String.Format(source, parArr) : source;
        }
        public object Clone()
        {
            var clone = new StrPatt(source);
            foreach (object o in this)
                clone.Add(GetClone(o));
            return clone;
        }
        public static object GetClone(object obj)
        {
            if (obj is string) return String.Copy(obj.ToString());
            if (obj is StrSepararorPatt) return ((StrSepararorPatt)obj).Clone();
            if (obj is StrPatt) return ((StrPatt)obj).Clone();
            ICloneable c = obj as ICloneable;
            return c != null ? c.Clone() : obj;
        }
        public static string quot(string str)
        {
            return "'" + str + "'";
        }
        public static string QuotePars(IEnumerable<string> pars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string par in pars)
                sb.AppendFormat("'{0}',", par);
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        public void Reset(object par)
        {
            Clear();
            Add(par);
        }
        public void Reset(object[] pars)
        {
            Clear();
            AddRange(pars);
        }
        public void SaveState() // запоминает свой текущий клон
        {
            thisClone = Clone() as StrPatt;
        }

        public void RestoreState()
        {
            source = thisClone.source;
            Reset(thisClone.ToArray());
        }


    }

    public class StrSepararorPatt : StrPatt, IReturnString
    {
        public StrSepararorPatt(string source,  params object[] pars) :base (source, pars){}
        public string ToString()
        {
            if (source == null || Count == 0) return "";
            StringBuilder rez = new StringBuilder();
            int lastNotEmptyParIndex = 0;
            object[] pars = ToArray();
            for (int i = Count-1; i > -1; i--) // ищем с конца непустой параметр
            {
                if (strFromObj(pars[i]) != "")
                {
                    lastNotEmptyParIndex = i;
                    break;
                }
            }
            for (int i = 0; i < pars.Length; i++)
            {
                string partCodition = strFromObj(pars[i]);
                if (partCodition != "") // добавляем непустые
                    rez.Append((i < lastNotEmptyParIndex) ? partCodition + source : partCodition);  // после последнего AND не добавляем   
            }
            return rez.ToString();
        }
        public object Clone()
        {
            var clone = new StrSepararorPatt(source);
            foreach (object o in this)
                clone.Add(GetClone(o));
            return clone;
        }

    }

    public class SqlSELECT
    {
        public string PriorWhere = null;
        public bool EnablePartition = true;
        public bool distinct;
        public SqlSELECT(object select, string from, object where = null):this()
        {
            if (select != null)
            {
                if (select.GetType().IsArray)
                SELECT.AddRange((object[])select);
                else SELECT.Add(select);
            }
            FROM = from;
            if (where != null)
            {
                var whereArr = where as IEnumerable<object>;
                if (whereArr == null) WHERE.Add(where);
                else WHERE.AddRange(whereArr);
            }
        }

        public SqlSELECT()
        {
            SELECT = new StrSepararorPatt(", ", new object[0]);
            WHERE = new StrSepararorPatt(" AND ", new object[0]);
            WhereLst = new SortedList<string, object>();
        }
        public StrSepararorPatt SELECT;
        public string FROM;
        public StrSepararorPatt WHERE;
        public SortedList<string, object> WhereLst; 
        public string ORDER_BY;
        public string GROUP_BY;
        public virtual string ToString()
        {
            return getString();
        }
        public object Clone()
        {
            var newWhere = WHERE.Clone() as StrSepararorPatt;
            var clone = new SqlSELECT(SELECT.ToArray(), FROM, newWhere.ToArray());
            var newWhereLst = new SortedList<string, object>();
            foreach (var pair in WhereLst)
            {
                var val = StrPatt.GetClone(pair.Value);
                newWhereLst.Add(pair.Key, val);
            }
            clone.WhereLst = newWhereLst;
            if (PriorWhere != null) clone.PriorWhere = String.Copy(PriorWhere);
            if (ORDER_BY != null) clone.ORDER_BY = String.Copy(ORDER_BY);
            if (GROUP_BY != null) clone.GROUP_BY = String.Copy(GROUP_BY);
            clone.EnablePartition = EnablePartition;
            clone.distinct = distinct;
            return clone;
        }

        public void SetWhereByKey(string key, object val)
        {
            if (!WhereLst.ContainsKey(key)) WhereLst.Add(key, val);
            else WhereLst[key] = val;
        }
        protected string getWhereStr()
        {
            var where = (StrSepararorPatt)WHERE.Clone();
            where.AddRange(WhereLst.Values);
            if (PriorWhere == null) return String.Format(" WHERE {0} ", where.ToString());
            string wherePatt = " WHERE {0} AND ({1})";
            return String.Format(wherePatt, PriorWhere, where.ToString());
        }
        // возвращает объединенный запрос (единая для всех запросов часть toString)
        protected string getString()
        {
           string select = distinct ? "DISTINCT " + SELECT.ToString() : SELECT.ToString();
            return String.Format("SELECT {0}", select) +  
                String.Format(" FROM {0}", FROM) + 
                getWhereStr() + 
                (string.IsNullOrEmpty(ORDER_BY) ? "" : String.Format(" ORDER BY {0}", ORDER_BY)) +
                (string.IsNullOrEmpty(GROUP_BY) ? "" : String.Format(" GROUP BY {0}", GROUP_BY));        
        }
    }
}