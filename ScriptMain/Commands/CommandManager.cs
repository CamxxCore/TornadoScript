using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Frontend;

namespace TornadoScript.ScriptMain.Commands
{
    public class CommandManager : ScriptExtension
    {
        private readonly Dictionary<string, Func<string[], string>> _commands =
            new Dictionary<string, Func<string[], string>>();

        private FrontendManager _frontendMgr;

        public CommandManager()
        {
<<<<<<< HEAD
            AddCommand("spawn", Commands.SpawnVortex);
            AddCommand("summon", Commands.SummonVortex);
            AddCommand("set", Commands.SetVar);
            AddCommand("reset", Commands.ResetVar);
            AddCommand("ls", Commands.ListVars);
            AddCommand("list", Commands.ListVars);
            AddCommand("help", Commands.ShowHelp);
            AddCommand("?", Commands.ShowHelp);
=======
            AddCommand("set", SetVar);
            AddCommand("reset", ResetVar);
            AddCommand("ls", ListVars);
            AddCommand("list", ListVars);
            AddCommand("help", ShowHelp);
            AddCommand("?", ShowHelp);
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        }

        internal override void OnThreadAttached()
        {
            _frontendMgr = ScriptThread.GetOrCreate<FrontendManager>();
            _frontendMgr.Events["textadded"] += OnInputEvent;
            base.OnThreadAttached();
        }

        public void OnInputEvent(object sender, ScriptEventArgs e)
        {
            var cmd = (string)e.Data;

            if (cmd.Length <= 0) return;

            var stringArray = cmd.Split(' ');

            var command = stringArray[0].ToLower();

            if (!_commands.TryGetValue(command, out var func)) return;

            var args = stringArray.Skip(1).ToArray();

            var text = func?.Invoke(args);

            if (!string.IsNullOrEmpty(text))
                _frontendMgr.WriteLine(text);
        }

<<<<<<< HEAD
=======
        private static string SetVar(params string[] args)
        {
            if (args.Length < 2) return "SetVar: Invalid format.";

            var varName = args[0];


            if (int.TryParse(args[1], out var i))
            {
                var foundVar = ScriptThread.GetVar<int>(varName);

                if (foundVar != null)
                {
                    return !ScriptThread.SetVar(varName, i) ?
                        "Failed to set the (integer) variable. Is it readonly?" : null;
                }
            }

            if (float.TryParse(args[1], out var f))
            {
                var foundVar = ScriptThread.GetVar<float>(varName);

                if (foundVar != null)
                {
                    return !ScriptThread.SetVar(varName, f) ? 
                        "Failed to set the (float) variable. Is it readonly?" : null;
                }
            }

            if (bool.TryParse(args[1], out var b))
            {
                var foundVar = ScriptThread.GetVar<bool>(varName);

                if (foundVar != null)
                {
                    return !ScriptThread.SetVar(varName, b) ? 
                        "Failed to set the (bool) variable. Is it readonly?" : null;
                }
            }

            return "Variable '" + args[0] + "' not found.";
        }

        private static string ResetVar(params string[] args)
        {
            if (args.Length < 1) return "ResetVar: Invalid format.";

            var varName = args[0];


            if (int.TryParse(args[1], out var i))
            {
                var foundVar = ScriptThread.GetVar<int>(varName);

                if (foundVar != null)
                {
                    foundVar.Value = foundVar.Default;

                    return null;
                }
            }


            if (float.TryParse(args[1], out var f))
            {
                var foundVar = ScriptThread.GetVar<float>(varName);

                if (foundVar != null)
                {
                    foundVar.Value = foundVar.Default;

                    return null;
                }
            }

            if (bool.TryParse(args[1], out var b))
            {
                var foundVar = ScriptThread.GetVar<bool>(varName);

                if (foundVar == null) return "Variable '" + args[0] + "' not found.";

                foundVar.Value = foundVar.Default;

                return null;
            }

            return "Variable '" + args[0] + "' not found.";
        }

        private static string ListVars(params string[] args)
        {
            var foundCount = 0;

            var frontend = ScriptThread.Get<FrontendManager>();

            foreach (var var in ScriptThread.Vars)
            {
                frontend.WriteLine(var.Key + (var.Value.ReadOnly ? " (read-only) " : ""));

                foundCount++;
            }

            return "Found " + foundCount + " vars.";
        }

        private static string ShowHelp(params string[] args)
        {
            var frontend = ScriptThread.Get<FrontendManager>();

            frontend.WriteLine("~r~set~w~: Set a variable\t\t~r~reset~w~: Reset a variable\t\t~r~ls~w~: List all vars");

            return "Commands:";
        }

>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        public void AddCommand(string name, Func<string[], string> command)
        {
            _commands.Add(name, command);
        }
    }
}
