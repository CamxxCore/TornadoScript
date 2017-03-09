using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Static logger class that allows direct logging of anything to a text file
    /// </summary>
    public static class Logger
    {
        private static TextWriterTraceListener listener;

        static Logger()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            listener = new TextWriterTraceListener(string.Format("{0}.log", assembly.GetName().Name), "logger");
            Trace.Listeners.Add(listener);
        }

        /// <summary>
        /// Write a new entry to the application log file.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Log(string format, params object[] args)
        {
            listener.WriteLine("[" + DateTime.Now + "]  " + string.Format(format, args));
        }
    }
}
