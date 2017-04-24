using System;
using System.Collections.Generic;
using System.Linq;
using TornadoScript.Frontend;
using ScriptCore;

namespace TornadoScript.Commands
{
    /// <summary>
    /// todo: add this to core script implementation
    /// </summary>
    public class CommandManager : ScriptExtension
    {
        private Dictionary<string, Func<string[], string>> _commands =
            new Dictionary<string, Func<string[], string>>();

        private FrontendManager _frontendMgr;

        public CommandManager()
        {
            AddCommand("set", SetVar);

            AddCommand("reset", ResetVar);
        }

        internal override void OnThreadAttached()
        {
            _frontendMgr = ScriptThread.GetOrCreate<FrontendManager>();

            _frontendMgr.Events["textadded"] += OnInputEvent;

            base.OnThreadAttached();
        }

        /// <summary>
        /// Execute a command by its name
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public void OnInputEvent(object sender, ScriptEventArgs e)
        {
            string cmd = (string)e.Data;

            if (cmd.Length > 0)
            {
                var stringArray = cmd.Split(' ');

                string command = stringArray[0].ToLower();

                Func<string[], string> func;

                if (_commands.TryGetValue(command, out func))
                {
                    string[] args = stringArray.Skip(1).ToArray();

                    string text = func?.Invoke(args);

                    if (text.Length > 0)
                    {
                        _frontendMgr.WriteLine(text);
                    }

                    else _frontendMgr.WriteLine("Success");
                }
            }
        }

        string SetVar(params string[] args)
        {
            if (args.Length < 2) return "SetVar: Invalid format.";

            string varName = args[0];

            int i;

            if (int.TryParse(args[1], out i))
            {
                var foundVar = ScriptThread.GetVar<int>(varName);

                if (foundVar != null)
                {
                    if (!ScriptThread.SetVar(varName, i))
                    {
                        return "Failed to set the (integer) variable. Is it readonly?";
                    }

                    return null;
                }
            }

            float f;

            if (float.TryParse(args[1], out f))
            {
                var foundVar = ScriptThread.GetVar<float>(varName);

                if (foundVar != null)
                {
                    if (!ScriptThread.SetVar(varName, f))
                    {
                        return "Failed to set the (float) variable. Is it readonly?";
                    }

                    return null;
                }
            }

            bool b;

            if (bool.TryParse(args[1], out b))
            {
                var foundVar = ScriptThread.GetVar<bool>(varName);

                if (foundVar != null)
                {
                    if (!ScriptThread.SetVar(varName, b))
                    {
                        return "Failed to set the (bool) variable. Is it readonly?";
                    }

                    return null;
                }
            }

            return "Variable '" + args[0] + "' not found.";
        }

        string ResetVar(params string[] args)
        {
            if (args.Length < 1) return "ResetVar: Invalid format.";

            string varName = args[0];

            int i;

            if (int.TryParse(args[1], out i))
            {
                var foundVar = ScriptThread.GetVar<int>(varName);

                if (foundVar != null)
                {
                    foundVar.Value = foundVar.Default;

                    return null;
                }
            }

            float f;

            if (float.TryParse(args[1], out f))
            {
                var foundVar = ScriptThread.GetVar<float>(varName);

                if (foundVar != null)
                {
                    foundVar.Value = foundVar.Default;

                    return null;
                }
            }

            bool b;

            if (bool.TryParse(args[1], out b))
            {
                var foundVar = ScriptThread.GetVar<bool>(varName);

                if (foundVar != null)
                {
                    foundVar.Value = foundVar.Default;

                    return null;
                }
            }

            return "Variable '" + args[0] + "' not found.";
        }

        public void AddCommand(string name, Func<string[], string> command)
        {
            _commands.Add(name, command);
        }
    }
}
