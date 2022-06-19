using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tableSQLInterpreterV2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void loadTxtFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            string fileText = System.IO.File.ReadAllText(filename);
            textBox1.Text = fileText.ToLower();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            loadTxtFile();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Lines=additionalFunction.punctuationCleanText(textBox1.Lines);
        }
        public int tableCount() => textBox1.Lines.Where(x => x.Contains("таблиц")).Count();
        public List<string> getStringLinesTable(int s,int e)
        {
            List<string> a= new List<string>();
            for (int i = s; i < e+1; i++)
            {
                a.Add(textBox1.Lines[i]);
            }
            return a;
        }
        public static List<sqlTable> sqlStruct = new List<sqlTable>();
        public static List<Thread> th = new List<Thread>();
        public static List<Task> t = new List<Task>();
        public delegate void Action();
        public delegate void Action<in T>(T obj);
        public List<sqlTable> createDocStruct()
        {
            th.Clear();
            sqlStruct.Clear();
            int start = -1;
            int end = -1;
            int id = 0;
            for(int i = 0;i<textBox1.Lines.Length;i++)
            {
                if (textBox1.Lines[i].Contains("таблица") && start==-1)
                {
                    start = i;
                }
                else if (textBox1.Lines[i].Contains("таблица") || i==textBox1.Lines.Length-1)
                {
                    end = i;
                }
                if (start != -1 && end != -1)
                {
                    textTable = getStringLinesTable(start, end);
                    th.Add(new Thread(createTable));
                    th[id].Start();
                    Thread.Sleep(100);
                    id++;
                    start = end;
                    end = -1;
                }
            }
            return sqlStruct;
        }
        public static List<string> textTable = new List<string>();
        public void createTable()
        {
            var s = textTable;
            sqlStruct.Add(new sqlTable(s, 1));
        }
        public bool checkSqlTextForDublicateTitle()
        {
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.DeselectAll();
            bool beRed = false;
            List<string> anyTitleField = new List<string>();
            List<string> anyTitleTable = new List<string>();
            var lines = richTextBox1.Lines;
            for (int i = 3; i < lines.Length; i++)
            {
                //MessageBox.Show($"i - {i}\nline[i] - {lines[i]}");
                string a = lines[i];//.Trim();
                if (lines[i].Contains("INSERT INTO ") && lines[i].Contains(" VALUES"))
                    return beRed;
                var start = a.IndexOf("_");
                if (start == -1)
                    continue;
                var len = a.Substring(a.IndexOf("_")).IndexOf(' ');
                a = a.Substring(start, len);

                if (lines[i].Contains("CREATE TABLE "))
                {
                    if (!anyTitleTable.Contains(a))
                    {
                        anyTitleTable.Add(a);
                    }
                    else
                    {
                        richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i) + start, a.Length);
                        richTextBox1.SelectionColor = Color.Red;
                        richTextBox1.DeselectAll();
                        beRed = true;
                    }
                    anyTitleField.Clear();
                }
                else
                {
                    if (!anyTitleField.Contains(a))
                    {
                        anyTitleField.Add(a);
                    }
                    else
                    {
                        richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(i) + start, a.Length);
                        richTextBox1.SelectionColor = Color.Red;
                        richTextBox1.DeselectAll();
                        beRed = true;
                    }
                }
            }
            return beRed;
        }
        public bool dolgo = false;
        public void OneThread()
        {
            Thread.Sleep(10000);
            if(dolgo)
            {
                MessageBox.Show("подождите идет обработка данных", "ВАЖНО!");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            dolgo = true;
            Thread thread1 = new Thread(OneThread);
            thread1.Start();
            var a = createDocStruct();
            while (th.Where(x=>x.IsAlive).Count()>0)
            {
                Thread.Sleep(1);
            }
            List<string> vivod = new List<string>();
            vivod.Add("drop database bd;");
            vivod.Add("create database bd;");
            vivod.Add("use bd;");
            //MessageBox.Show(toolStripComboBox1.SelectedItem.ToString());
            foreach (var item in a)
            {
                vivod.Add($"CREATE TABLE {item.title} (");
                for (int i = 0; i < item.sqlFields.Count; i++)
                {
                    if (toolStripComboBox1.SelectedItem.ToString() == "сверху")
                    {
                        vivod.Add($"    -- {item.sqlFields[i].titleRu}");
                    }
                    string t= ($"    {item.sqlFields[i].titleEn} {item.sqlFields[i].type} {item.sqlFields[i].settings}");
                    if (toolStripComboBox1.SelectedItem.ToString() == "сбоку")
                    {
                        t += "  -- " + item.sqlFields[i].titleRu;
                    }
                    if (i != item.sqlFields.Count - 1)
                        vivod.Add(t+",");
                    else
                        vivod.Add($"    {item.sqlFields[i].titleEn} {item.sqlFields[i].type} {item.sqlFields[i].settings}");
                    if (toolStripComboBox1.SelectedItem.ToString() == "снизу")
                    {
                        vivod.Add($"    -- {item.sqlFields[i].titleRu}");
                    }
                }
                vivod.Add(");");
            }
            vivod.Add("");
            foreach (var item in a)
            {
                vivod.AddRange(item.insertSystem.model);
            }
            richTextBox1.Lines = vivod.ToArray();
            if (checkSqlTextForDublicateTitle())
            {
                MessageBox.Show("красным помечены слова-дубликаты\nкоторые мы не смогли перевести иначе", "ВАЖНО!");
            }
            dolgo = false;
            if(thread1.IsAlive)
            {
                thread1.Suspend();
            }
            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            
        }
    }
}
