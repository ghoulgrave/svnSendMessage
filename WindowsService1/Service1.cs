using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 60000;//执行间隔时间,单位为毫秒  
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer1_Elapsed);

        }
        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string week = DateTime.Now.DayOfWeek.ToString("d");
            if (week == "0" || week == "6")
            {
            }
            else
            {
                // 得到 hour minute second  如果等于某个值就开始执行某个程序。  
                int intHour = e.SignalTime.Hour;
                int intMinute = e.SignalTime.Minute;
                int intSecond = e.SignalTime.Second;
                // 定制时间； 比如 在8：30 ：10 的时候执行某个函数  
                int iHour = 8;
                int iMinute = 30;

                // 设置　每天的8：30开始执行程序  
                if (intHour == iHour && intMinute == iMinute)
                {
                    Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    string name = config.AppSettings.Settings["names"].Value;
                    string svnUrl = config.AppSettings.Settings["svnUrl"].Value;
                    

                    foreach (string str in name.Split(','))
                    {
                        Thread.Sleep(10000);
                        this.doSomething(svnUrl+str.Trim(),str);
                    }
                }
            }

        }

        private void doSomething(string svnUrl,String name)
        {
            int iNum = 0;
            string retString = "";
            bool flag = true;
            while (flag)
            {
                iNum ++;
                try
                {
                    //string serviceAddress = "http://svn.gtmap.cn/message/getMobile?userName=" + name;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(svnUrl);
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=UTF-8";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                    retString = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
                catch (Exception e)
                {
                    retString = e.ToString();
                }
                FileStream fs = new FileStream(@"D:\SVNTXT.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine("短信服务已请求:" + DateTime.Now.ToString() + ":" + name + ":");
                sw.WriteLine(retString + "\n");
                sw.Flush();
                sw.Close();
                fs.Close();
                if (@"今天已经获取过密码,请查收短信" != retString)
                {
                    Thread.Sleep(10000);
                    if (iNum > 10) {
                        Thread.Sleep(120000);
                    }
                }
                else
                {
                    flag = false;
                }
            }
        }


        protected override void OnStop()
        {

        }
    }
}
