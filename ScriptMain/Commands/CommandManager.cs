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
            AddCommand("spawn", Commands.SpawnVortex);
            AddCommand("summon", Commands.SummonVortex);
            AddCommand("set", Commands.SetVar);
            AddCommand("reset", Commands.ResetVar);
            AddCommand("ls", Commands.ListVars);
            AddCommand("list", Commands.ListVars);
            AddCommand("help", Commands.ShowHelp);
            AddCommand("?", Commands.ShowHelp);
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

        public void AddCommand(string name, Func<string[], string> command)
        {
            _commands.Add(name, command);
        }
    }
}
