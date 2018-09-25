using System.Windows.Forms;
using System.Windows.Input;
using GTA;
using TornadoScript.ScriptCore.Game;
using TornadoScript.ScriptMain.Utility;

namespace TornadoScript.ScriptMain.Frontend
{
    public class FrontendManager : ScriptExtension
    {
        private readonly FrontendInput _input = new FrontendInput();

        private readonly FrontendOutput _output = new FrontendOutput();

        private bool _showingConsole;

        private bool _capsLock;

        public FrontendManager()
        {
            RegisterEvent("textadded");
        }

        internal override void OnThreadAttached()
        {
            Events["keydown"] += OnKeyDown;
            base.OnThreadAttached();
        }

        private void OnKeyDown(object sender, ScriptEventArgs e)
        {
<<<<<<< HEAD
            var keyArgs = e.Data as System.Windows.Forms.KeyEventArgs;
=======
            var keyArgs = e.Data as KeyEventArgs;
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48

            if (keyArgs == null) return;

            if (!ScriptThread.GetVar<bool>("enableconsole")) return;

            if (keyArgs.KeyCode == Keys.CapsLock)
            {
                _capsLock = !_capsLock;
            }

            if (!_showingConsole)
            {
                if (keyArgs.KeyCode == ScriptThread.GetVar<Keys>("toggleconsole"))
                {
                    ShowConsole();
                }
            }

            else
                GetConsoleInput(keyArgs);
        }

        public void ShowConsole()
        {
            if (_showingConsole) return;
            _input.Show();
            _output.Show();
            _output.DisableFadeOut();
            _showingConsole = true;
        }

        public void HideConsole()
        {
            if (!_showingConsole) return;
            _input.Clear();
            _input.Hide();
            _output.Hide();
            _output.EnableFadeOut();
            _showingConsole = false;
        }

        public void WriteLine(string format, params object[] args)
        {
            if (args == null)
                _output.WriteLine(format);
            else _output.WriteLine(format, args);
        }

<<<<<<< HEAD
        private void GetConsoleInput(System.Windows.Forms.KeyEventArgs e)
=======
        private void GetConsoleInput(KeyEventArgs e)
>>>>>>> 46660d5b9e2a5942c1c3eb32c40357e5d9abfc48
        {
            var key = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);

            var keyChar = Win32Native.GetCharFromKey(key, (e.Modifiers & Keys.Shift) != 0);
            
            var capsLock = System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock);

            if (char.IsLetter(keyChar))
            {
                if (capsLock || e.Shift)
                    keyChar = char.ToUpper(keyChar);
                else keyChar = char.ToLower(keyChar);

            }

            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                    {
                        var text = _input.GetText();

                        if (text.Length < 1)
                        {
                            HideConsole();
                        }

                        _input.RemoveLastChar();

                        return;
                    }

                    case Keys.Up:
                        _output.ScrollUp();
                        return;

                    case Keys.Down:
                        _output.ScrollDown();
                        return;

                    case Keys.Space:
                        _input.AddChar(' ');
                        return;

                    case Keys.Enter:
                        {
                            var text = _input.GetText();

                            NotifyEvent("textadded", new ScriptEventArgs(text));

                            _output.WriteLine(text);

                            _input.Clear();

                            _output.ScrollToTop();

                           // HideConsole();    

                            return;
                        }

                    case Keys.Escape:
                        HideConsole();
                        return;
                }
            }

            if (keyChar != ' ')
            {
                _input.AddChar(keyChar);
            }
        }

        public override void OnUpdate(int gameTime)
        {
            _input.Update(gameTime);

            _output.Update(gameTime);

            if (_showingConsole)
            {
                if (Game.IsControlJustPressed(0, (GTA.Control) 241) || 
                    Game.IsControlJustPressed(0, (GTA.Control) 188))
                {
                    _output.ScrollUp();
                }

                else if (Game.IsControlJustPressed(0, (GTA.Control) 242) ||
                         Game.IsControlJustPressed(0, (GTA.Control) 187))
                {
                    _output.ScrollDown();
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
