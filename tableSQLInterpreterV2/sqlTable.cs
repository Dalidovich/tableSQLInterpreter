using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tableSQLInterpreterV2;

namespace tableSQLInterpreterV2
{
    public class sqlTable
    {
        public int id;
        public string title;
        public string titleRu;
        public List<string> text;
        public List<sqlField> sqlFields = new List<sqlField>();
        public InsertSystem insertSystem;
        public sqlTable(List<string> text,int i)
        {
            this.id = i;
            this.text = text;
            this.titleRu = text[0].Trim();
            this.title = "_"+additionalFunction.createCamelCase(additionalFunction.WordFormattingOnlyLetterAndSpase(additionalFunction.TranslateText(titleRu))).Trim();
            this.sqlFields = getFields();
            this.insertSystem = new InsertSystem(sqlFields, id);
        }
        private List<sqlField> getFields()
        {
            List<sqlField> a = new List<sqlField>();
            foreach (var item in text)
            {
                if (item.Length!=0 && !item.Contains("таблица"))
                {
                    a.Add(new sqlField(item));
                }
            }
            return a;
        }
    }
}
