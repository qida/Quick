using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Kindle_Quick.Fun
{
  static  class SendKey
    {
    
        [DllImport("user32.dll", EntryPoint = "keybd_event")]

        public static extern void keybd_event(
        byte bVk, //虚拟键值
        byte bScan,// 一般为0
        int dwFlags, //这里是整数类型 0 为按下，2为释放
        int dwExtraInfo //这里是整数类型 一般情况下设成为0
        );

        #region 模拟按键

        public static void PressWave()
        {
            keybd_event(192, 0, 0, 0);
            keybd_event(192, 0, 2, 0);
        }
            public static void Play()
            {
                keybd_event(179, 0, 0, 0);
                keybd_event(179, 0, 2, 0);
            }

            public static void Stop()
            {
                keybd_event(178, 0, 0, 0);
                keybd_event(178, 0, 2, 0);
            }

            public static void Last()
            {
                keybd_event(177, 0, 0, 0);
                keybd_event(177, 0, 2, 0);
            }

            public static void Next()
            {
                keybd_event(176, 0, 0, 0);
                keybd_event(176, 0, 2, 0);
            }
        #endregion
    }
}
