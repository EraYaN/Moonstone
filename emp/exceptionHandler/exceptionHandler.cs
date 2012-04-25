using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace EMP
{
    public class exceptionHandler
    {
        public enum exceptionLevel
        {
            EMP_FATAL_ERROR = 0,
            EMP_ERROR = 1,
            EMP_WARNING = 2,
            EMP_NOTICE = 3            
        }
        /// <summary>
        ///    Handles exceptions
        /// </summary>
        /// <param name="message">
        ///    The error message to display to the user or add to the log.
        /// </param>
        /// <param name="eLevel">
        ///    The severity of the error, a <see cref="exeptionLevel"/> value. Deafult is <see cref="exceptionLevel.EMP_NOTICE"/>.
        /// </param>
        /// <param name="exception">
        ///    The <see cref="Exception"/> object returned by the runtime or usercode.
        /// </param>
        static public void triggerException(String message, exceptionLevel eLevel = exceptionLevel.EMP_NOTICE, Exception exception = null)
        {
            MessageBox.Show(message+"\r\n"+eLevel.ToString());            
        }
    }
}
