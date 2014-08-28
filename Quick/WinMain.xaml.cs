using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Kindle_Quick.Entity;
using System.Xml;
using Kindle_Quick.Fun;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.Windows.Controls;
using System.Diagnostics;

namespace Kindle_Quick
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WinMain : Window
    {
        /// <summary>
        /// Cities DataSource
        /// </summary>
        //private static readonly City[] dataSource = new City[] {
        //    new City{CityID=1, Name="Toronto",KeyWords=new string[]{"123","456"}}, 
        //    new City{CityID=2, Name="Montreal",KeyWords=new string[]{"a","b"}}, 
        //    new City{CityID=3, Name="Edmonton",KeyWords=new string[]{"c","d"}} 

        //};
        private static ObservableCollection<Program> autoCompletionList = new ObservableCollection<Program>();

        public static System.Windows.Media.Color CreateColor(string nameColor)
        {
            System.Windows.Media.Color c = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(nameColor);
            return c;
        }

        public WinMain()
        {
            InitializeComponent();
        }
        HotKey hotKey;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hotKey = new HotKey(this, HotKey.KeyFlags.MOD_ALT, System.Windows.Forms.Keys.None);
            hotKey.OnHotKey += new HotKey.OnHotKeyEventHandler(hotKey_OnHotKey);
            this.WindowState = WindowState.Minimized;
            this.Visibility = Visibility.Hidden;
        }

        private static void LoadDLL()
        {
            string filedllPath = AppDomain.CurrentDomain.BaseDirectory + "WPFToolkit.dll";
            if (!File.Exists(filedllPath))
            {
                FileStream fs = new FileStream(filedllPath, FileMode.CreateNew, FileAccess.Write);
                //  byte[] buffer = Kindle_Quick.Properties.Resources.WPFToolkit;
                byte[] buffer = new byte[] { };
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }

        public int loadXML()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
            if (!File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(Kindle_Quick.Properties.Resources.config);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            int countPrograms = 0;
            XmlDocument xml = new XmlDocument();
            xml.Load(filePath);
            autoCompletionList.Clear();
            XmlNode allpeople = xml.SelectSingleNode("programs");
            XmlNodeList xpeople = allpeople.ChildNodes;
            foreach (XmlNode xnf in xpeople)
            {
                XmlElement xe = (XmlElement)xnf;
                XmlNodeList xnf1 = xe.ChildNodes;
                if (File.Exists(xnf1[2].InnerText))
                {
                    autoCompletionList.Add(new Program { Name = xnf1[0].InnerText.ToString(), PingYing = xnf1[1].InnerText, Link = xnf1[2].InnerText, Icon = IconToBitmap.ToBitmapSource(System.Drawing.Icon.ExtractAssociatedIcon(xnf1[2].InnerText).ToBitmap()) });
                    countPrograms++;
                }

            }
            return countPrograms;
        }
        /// <summary>
        /// occurs when the user stops typing after a delayed timespan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void autoCities_PatternChanged(object sender, Kindle_Quick.Controls.AutoComplete.AutoCompleteArgs args)
        {
            //check
            if (string.IsNullOrEmpty(args.Pattern))
                args.CancelBinding = true;
            else
                args.DataSource = WinMain.GetCities(args.Pattern).ToList();
            //typeof(Colors).GetProperties();
        }
        /// <summary>
        /// Get a list of cities that follow a pattern
        /// </summary>
        /// <returns></returns>
        private static ObservableCollection<Program> GetCities(string Pattern)
        {
            // match on contain (could do starts with)
            ObservableCollection<Program> ocity = new ObservableCollection<Program>();
            ocity = new ObservableCollection<Program>(WinMain.autoCompletionList.
                    Where((program, match) => program.Name.ToLower().StartsWith(Pattern.ToLower()) || program.PingYing.ToLower().StartsWith(Pattern.ToLower())));
            if (ocity.Count == 0)
            {
                ocity = new ObservableCollection<Program>(WinMain.autoCompletionList.
                    Where((program, match) => program.Name.ToLower().Contains(Pattern.ToLower()) || program.PingYing.ToLower().Contains(Pattern.ToLower())));
            }
            return ocity;
        }

        void wd_Closed(object sender, EventArgs e)
        {
            loadXML();
        }
        public static bool ReadRegEdit(string keyName, string filePath, bool state)
        {
            RegistryKey pregkey;
            try
            {
                pregkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey("Microsoft", true).OpenSubKey("Windows", true).OpenSubKey("CurrentVersion", true).OpenSubKey("Run", true);
                if (null != pregkey)
                {
                    pregkey.SetValue(keyName, filePath);
                    pregkey.Close();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }
       
        public static bool SetAutoRun(string keyName, string filePath, bool state)
        {
            try
            {
                RegistryKey HKLM = Registry.CurrentUser;
                RegistryKey Run = HKLM.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (state)
                {
                    Run.SetValue(keyName, filePath);
                }
                else
                {
                    Run.DeleteValue(keyName);
                }
                HKLM.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("拒绝"))
                {
                    MessageBox.Show("请以管理员身份运行");
                }
                return false;
            }
            return true;
        }




        TimeSpan ts = TimeSpan.Zero;
        private void hotKey_OnHotKey()
        {
            if (DateTime.Now.TimeOfDay - ts < new TimeSpan(0, 0, 0, 0, 300))
            {
                ts = new TimeSpan();
                if (this.WindowState == WindowState.Normal)
                {
                    WinHidden();
                }
                else
                {
                    WinShow();
                }
            }

            ts = DateTime.Now.TimeOfDay;
        }

        private void WinShow()
        {
            Random rd = new Random();
            this.Resources.Remove("color");
            this.Resources.Add("color", new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)rd.Next(128, 256), (byte)rd.Next(256), (byte)rd.Next(256), (byte)rd.Next(256))));
            this.Resources.Remove("popupcolor");
            this.Resources.Add("popupcolor", new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)rd.Next(10, 66), (byte)rd.Next(256), (byte)rd.Next(256), (byte)rd.Next(256))));
            this.Show();
            this.WindowState = WindowState.Normal;
            this.autoCities.Focus();
           KeyState.SetState(KeyState.VirtualKeys.VK_CAPITAL, true);
        }

        private void WinHidden()
        {
           KeyState.SetState(KeyState.VirtualKeys.VK_CAPITAL, false);
            this.WindowState = WindowState.Minimized;
            this.Visibility = Visibility.Hidden;
        }
        private void add(string namep, string linkp)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
            XmlNode root = xml.SelectSingleNode("programs");
            XmlElement program = xml.CreateElement("program");
            XmlElement name = xml.CreateElement("name");
            XmlElement link = xml.CreateElement("link");
            XmlElement pingying = xml.CreateElement("pinying");
            name.InnerText = namep;
            link.InnerText = linkp;
            pingying.InnerText = PinYing.LineStr(namep);
            program.AppendChild(name);
            program.AppendChild(pingying);
            program.AppendChild(link);
            root.AppendChild(program);
            xml.Save(AppDomain.CurrentDomain.BaseDirectory + "config.xml");
        }

        private void WindowMain_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
                foreach (string s in files)
                {
                    if (System.IO.File.Exists(s))
                    {
                        string fileName = s.Substring(s.LastIndexOf(@"\") + 1, s.LastIndexOf(".") - s.LastIndexOf(@"\") - 1);
                        add(fileName, s);
                        autoCompletionList.Add(new Program { Name = fileName, PingYing = PinYing.LineStr(fileName), Link = s, Icon = IconToBitmap.ToBitmapSource(System.Drawing.Icon.ExtractAssociatedIcon(s).ToBitmap()) });
                    }
                }
            }
        }
        #region 提示ICON
        System.Windows.Forms.NotifyIcon notifyIcon;
        private void icon()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            //            this.notifyIcon.BalloonTipText = "单击 ~ 波浪键 呼叫我"; //设置程序启动时显示的文本
            this.notifyIcon.BalloonTipText = "双击 Alt键 呼叫我"; //设置程序启动时显示的文本
            this.notifyIcon.Text = "快闪启动 双击退出";//最小化到托盘时，鼠标点击时显示的文本
            this.notifyIcon.Icon = Kindle_Quick.Properties.Resources.flash;//程序图标
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.MouseClick += notifyIcon_MouseClick;
            this.notifyIcon.ShowBalloonTip(1000);
        }

        void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButton.Right))
            {
                MessageBox.Show("right");
            }
            else
            {
                WinShow();
            }
        }
        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void autoCities_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isRightCtrl = false;
            System.Windows.Input.KeyboardDevice kd = e.KeyboardDevice;
            if ((kd.GetKeyStates(Key.RightCtrl) & System.Windows.Input.KeyStates.Down) > 0)
            {
                isRightCtrl = true;//激活打开文件位置功能
            }


            if (e.Key == Key.Enter && this.autoCities.Text != string.Empty)
            {
                switch (this.autoCities.Text.Trim().ToUpper())
                {
                    case "HELP":

                        break;
                    case "SET":
                        this.autoCities.Text = string.Empty;
                        WinProInf wd = new WinProInf();
                        wd.WindowState = WindowState.Normal;
                        wd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        wd.ShowInTaskbar = false;
                        wd.Closed += wd_Closed;
                        wd.Topmost = true;
                        wd.ShowDialog();
                        break;
                    case "EXIT":
                        this.Close();
                        break;
                    case "RELOAD":
                        loadXML();
                        break;
                    case "UPDATE":
                        Process proc = new System.Diagnostics.Process();
                        proc.StartInfo.FileName = "http://pan.baidu.com/s/1CGL36";
                        proc.Start();
                        break;
                    default:
                        break;
                }
                if (autoCities.Items.Count > 0)
                {
                    if (autoCities.SelectedIndex == -1)
                    {
                        autoCities.SelectedIndex = 0;
                    }

                    Program city = (Program)(autoCities.SelectedItem);
                    WinHidden();
                    try
                    {
                        if (isRightCtrl)
                        {
                            FileInfo f = new FileInfo(city.Link);
                            Process.Start(f.Directory.ToString());
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(city.Link);
                            this.autoCities.Text = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("用户已取消操作"))
                        {
                            MessageBox.Show("文件已失效");
                        }
                        else
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            else if (e.Key == Key.Escape)
            {
                WinHidden();

            }

            else
            {

            }
        }

        private void WindowMain_Initialized(object sender, EventArgs e)
        {

            loadXML();
            icon();
            ReadRegEdit("KINDLE", System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, true);
        }

        private void WindowMain_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                KeyState.SetState(KeyState.VirtualKeys.VK_CAPITAL, true);
            }
       
        }
    }
}
