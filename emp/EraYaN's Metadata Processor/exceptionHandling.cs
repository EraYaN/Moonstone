using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace EMP
{
    class exceptionHandling
    {
        public enum exceptionLevel
        {
            EMP_FATAL_ERROR = 0,
            EMP_ERROR = 1,
            EMP_WARNING = 2,
            EMP_NOTICE = 3            
        }
        static public void triggerExeption(String message, exceptionLevel eLevel = exceptionLevel.EMP_NOTICE, Exception exception = null)
        {
            MessageBox.Show(message+"\r\n"+eLevel.ToString());
        }
    }
}
