using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace TornadoScript.ScriptCore
{
    /// <summary>
    /// Static logger class that allows direct logging of anything to a text file
    /// </summary>
    public static class Logger
    {
        public static void Log(string format, params object[] args)
        {
            File.AppendAllText("TornadoScript.log", "[" + DateTime.Now + "]  " + string.Format(format, args) + Environment.NewLine);
        }
    }
}
