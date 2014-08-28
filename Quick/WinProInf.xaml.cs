using Kindle_Quick.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Kindle_Quick
{
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class WinProInf : WindowBase
    {
        public WinProInf()
        {
            InitializeComponent();
       
            this.Loaded += Window2_Loaded;
          
        }

      
        private void SaveXML()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(filePath);
            XmlNode root = xml.SelectSingleNode("programs");
            root.RemoveAll();
            while (root.HasChildNodes)
            {

            }
            foreach (Program program in autoCompletionList)
            {
                XmlElement Program = xml.CreateElement("program");
                XmlElement name = xml.CreateElement("name");
                XmlElement link = xml.CreateElement("link");
                XmlElement pingying = xml.CreateElement("pinying");
                if (program.Name != string.Empty)
                {
                    name.InnerText = program.Name;
                    pingying.InnerText = program.PingYing;
                    link.InnerText = program.Link;
                    Program.AppendChild(name);
                    Program.AppendChild(pingying);
                    Program.AppendChild(link);
                    root.AppendChild(Program);
                }
            }
            xml.Save(filePath);
        }    
        void Window2_Loaded(object sender, RoutedEventArgs e)
        {
            loadXML();
            this.dataGrid1.ItemsSource = autoCompletionList;         
            this.YesButton.Content = "确 定";
            this.YesButton.Width = 60;
            this.NoButton.Content = "取 消";
            this.NoButton.Width = 60;
            this.YesButton.Click += (ss, ee) =>
            {
                SaveXML();
                this.Close();
            };
            this.NoButton.Click += (ss, ee) =>
            {
                this.Close();
            };
        } 
        private static ObservableCollection<Program> autoCompletionList = new ObservableCollection<Program>();
        public int loadXML()
        {
            int countPrograms = 0;
            XmlDocument xml = new XmlDocument();
            xml.Load(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
            XmlNode allpeople = xml.SelectSingleNode("programs");
            XmlNodeList xpeople = allpeople.ChildNodes;
            autoCompletionList.Clear();
            foreach (XmlNode xnf in xpeople)
            {
                XmlElement xe = (XmlElement)xnf;
                XmlNodeList xnf1 = xe.ChildNodes;
                if (xnf1[2].InnerText !=string.Empty &&File.Exists(xnf1[2].InnerText))
                {
                    autoCompletionList.Add(new Program { Name = xnf1[0].InnerText.ToString(), PingYing = xnf1[1].InnerText, Link = xnf1[2].InnerText });
                    countPrograms++;
                }

            }
            return countPrograms;
        }


    }
}
