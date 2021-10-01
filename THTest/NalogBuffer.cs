using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace THTest
{
    public class NalogBuffer
    {
        private DataTable tableNalog;
        private static NalogBuffer self;
        private NalogBuffer()
        {
            tableNalog = new DataTable();
            tableNalog.Columns.AddRange(new DataColumn[] {
                new DataColumn("INN", typeof(string)),
                new DataColumn("OGRN", typeof(string)),
                new DataColumn("NalogData", typeof(NalogInfo))
            });
        }
        public static NalogBuffer Init()
        {
            if (self == null)
            {
                self = new NalogBuffer();
            }
            return self;
        }
        public NalogInfo GetNalog(string data)
        {
            if (data.Length == 10 || data.Length == 12)
            {
                return GetNalogByColumnName(data,"INN");
            }
            if (data.Length == 13)
            {
                return GetNalogByColumnName(data, "OGRN");
            }
            throw new Exception("Invalid value. The length should be 10-13");
        }
        private NalogInfo GetNalogByColumnName(string searchingData,string nameOfColumn)
        {
            foreach (DataRow i in tableNalog.Rows)
            {
                if (i[nameOfColumn].ToString() == searchingData)
                {
                    NalogInfo searched = i["NalogData"] as NalogInfo;
                    if (searched.LastSearchingTime - DateTime.Now >= TimeSpan.FromMinutes(5))
                    {
                        DeleteRow(i);
                        return null;
                    }
                    return searched;
                }
            }
            return null;
        }
        public void ClearOldRows()
        {
            List<DataRow> oldData = new List<DataRow>();
            foreach (DataRow i in tableNalog.Rows)
            {
                NalogInfo searched = i["NalogData"] as NalogInfo;
                if (DateTime.Now - searched.LastSearchingTime >= TimeSpan.FromMinutes(5))
                {
                    oldData.Add(i);
                }
            }
            foreach (var i in oldData)
            {
                tableNalog.Rows.Remove(i);
            }
            tableNalog.AcceptChanges();
        }
        public void AddRow(NalogInfo nalog)
        {
            foreach (NalogInfoData i in nalog.data)
            {
                tableNalog.Rows.Add(i.inn, i.ogrn, nalog);
            }
            tableNalog.AcceptChanges();
        }
        private void DeleteRow(DataRow row)
        {
            tableNalog.Rows.Remove(row);
            tableNalog.AcceptChanges();
        }
    }
}
