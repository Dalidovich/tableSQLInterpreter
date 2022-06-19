using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace tableSQLInterpreterV2
{
    static class additionalFunction
    {
        public static string createCamelCase(string orig) => Char.ToLower(orig[0]) + String.Join("", orig.Split(' ').Select(x => Char.ToUpper(x[0]) + x.Substring(1))).Substring(1);
        public static string WordFormattingOnlyLetterAndSpase(string w)
        {
            string nw = "";
            foreach (var item in w)
            {
                if (Char.IsLetterOrDigit(item))
                {
                    nw += item;
                }
                if (Char.IsWhiteSpace(item))
                {
                    nw += item;
                }
            }
            return nw;
        }
        public static string WordFormattingForRuTitle(string w)
        {
            string nw = "";
            foreach (var item in w)
            {
                if (Char.IsLetterOrDigit(item) || item == '-')
                {
                    nw += item;
                }
                if (Char.IsWhiteSpace(item))
                {
                    nw += item;
                }
            }
            return nw;
        }
        public static void removeExtraSpaces(string[] t)
        {
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = replaceMoreSpaceOnOne(t[i]);
            }
        }
        public static string[] punctuationCleanText(string[] t)
        {
            List<string> cleanText = new List<string>();
            for (int i = 0; i < t.Length; i++)
            {
                string oneLine = "";
                foreach (var item in t[i])
                {
                    if ((Char.IsLetterOrDigit(item)) || (item == '\n'))
                    {
                        oneLine += item;
                    }
                    if (Char.IsWhiteSpace(item) || Char.IsPunctuation(item))
                    {
                        oneLine += item;
                    }
                }
                cleanText.Add(oneLine.Trim());
            }
            return cleanText.ToArray();
        }
        public static string replaceMoreSpaceOnOne(string t)
        {
            var a = t.Split(' ');
            List<string> b = new List<string>();
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Length != 0)
                    b.Add(a[i].Trim());
            }
            return String.Join(" ", b);
        }
        public static Dictionary<string, string> loadKeyWordsDictionaryForTypeField()   
        {
            var d = new Dictionary<string, string>();
            string path = Directory.GetCurrentDirectory();
            //MessageBox.Show(path+ "_ keyWordForFieldsType.txt");
            path += @"\keyWordForFieldsType.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //MessageBox.Show(line);
                    d.Add(line.Split(' ')[0], line.Split(' ')[1]);
                }
            }
            return d;
        }
        public static string TranslateText(string input)
        {
            try
            {
                string url = String.Format
                ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", "ru", "en", Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;
                var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
                var translationItems = jsonData[0];
                string translation = "";
                foreach (object item in translationItems)
                {
                    IEnumerable translationLineObject = item as IEnumerable;
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();
                    translationLineString.MoveNext();
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }
                if (translation.Length > 1) { translation = translation.Substring(1); };
                return translation;
            }
            catch(Exception e)
            {
                return createCamelCase(WordFormattingOnlyLetterAndSpase(input));
            }
        }
        public static string createFieldType(string t, Dictionary<string, string> keyWords)
        {
            string type = " varchar(15)";
            foreach (var item in keyWords)
            {
                if (t.Contains(item.Key))
                    type = $" {item.Value}";
            }
            return type;
        }
        public static string createSettingsForField(string t)
        {
            string sett = "";
            if (t == "") return sett;
            if (t.Contains("уник"))
            {
                sett = "primary key";
            }
            if (t.Contains("н0"))
            {
                sett += " not null";
            }
            if (t.Contains("авто"))
            {
                sett += " AUTO_INCREMENT";
            }
            return sett;
        }
    }
}
