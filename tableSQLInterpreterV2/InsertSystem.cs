using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tableSQLInterpreterV2
{
    public class InsertSystem
    {
        List<string> type;
        int count;
        string line;
        public List<string> model;
        public InsertSystem(List<sqlField> l,int i)
        {
            this.count = 3;
            this.type = l.Select(x => x.type).ToList();
            this.line = getLine(this.type);
            this.model = allLines(i);
        }
        private string getLine(List<string> l)
        {
            List<string> a = new List<string>();
            foreach (var item in l)
            {
                switch (item)
                {
                    case "int":
                        a.Add("00,");
                        break;
                    case "varchar(15)":
                        a.Add("\"text\",");
                        break;
                    case "bool":
                        a.Add("true,");
                        break;
                    case "date":
                        a.Add("\"2022-04-01\",");
                        break;
                    case "time":
                        a.Add("10-00-00,");
                        break;
                    case "year":
                        a.Add("2000,");
                        break;
                }
            }
            return "("+String.Join(" ",a).Trim(',')+")";
        }
        private List<string> allLines(int id)
        {
            List<string> a = new List<string>();
            a.Add($"INSERT INTO table{id + 1} VALUES");
            for (int i = 0; i < count; i++)
            {
                if(count==i+1)
                {
                    a.Add(line + ";");
                }
                else
                {
                    a.Add(line + ",");
                }
            }
            return a;
        }
    }
}
