using System;
using System.Diagnostics;
using System.Reflection;
<<<<<<< HEAD
using System.IO;
=======
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

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
