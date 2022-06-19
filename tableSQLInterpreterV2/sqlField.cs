using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tableSQLInterpreterV2
{
    public class sqlField
    {
        public string titleEn;
        public string titleRu;
        public string type;
        public string settings;
        public string comment;

        public sqlField(string comment)
        {
            this.comment = comment;
            this.titleRu = getTitleRu().Trim();
            this.titleEn = "_" + getTitleEn(titleRu).Trim();
            this.type = getType().Trim();
        }
        private string getTitleRu()
        {
            string title = comment;
            if (comment.Contains("("))
            {
                title = comment.Substring(0, comment.IndexOf("("));
                getSettings(comment.Substring(comment.IndexOf("(")));
            }
            title = additionalFunction.WordFormattingForRuTitle(title);
            return title;
        }
        private string getTitleEn(string t)=>additionalFunction.createCamelCase(additionalFunction.WordFormattingOnlyLetterAndSpase(additionalFunction.TranslateText(titleRu)));
        private string getType()
        {
            return additionalFunction.createFieldType(titleRu,additionalFunction.loadKeyWordsDictionaryForTypeField());
        }
        private void getSettings(string st)=>this.settings=additionalFunction.createSettingsForField(st);
    }
}
