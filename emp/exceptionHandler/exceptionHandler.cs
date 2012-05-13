using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace EMP
{
    public class ExceptionHandler
    {
        /// <summary>
        ///    The different levels of exceptions
        /// </summary>
        public enum ExceptionLevel
        {
            FatalError,
            Error,
            Warning,
            Notice            
        }
        /// <summary>
        ///    Handles exceptions
        /// </summary>
        /// <param name="message">
        ///    The error message to display to the user or add to the log.
        /// </param>
        /// <param name="eLevel">
        ///    The severity of the error, a <see cref="ExeptionLevel"/> value. Deafult is <see cref="ExceptionLevel.Notice"/>.
        /// </param>
        /// <param name="exception">
        ///    The <see cref="Exception"/> object returned by the runtime or usercode.
        /// </param>
        static public void TriggerException(String message, ExceptionLevel eLevel = ExceptionLevel.Notice, Exception exception = null)
        {
            //MessageBox.Show(message+"\r\n"+eLevel.ToString());      obsolete
            //include logging instead
            Ex
        }
    }
}
