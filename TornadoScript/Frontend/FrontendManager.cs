using System;
using System.Windows.Forms;
using System.Windows.Input;
using ScriptCore;

namespace TornadoScript.Frontend
{
    public class FrontendManager : ScriptExtension
    {
        private FrontendInput _input = new FrontendInput();

        private FrontendOutput _output = new FrontendOutput();

        private bool _showingConsole = false;

        private bool _capsLock = false;

        private const Keys ToggleConsoleKey = Keys.T;

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
            KeyEventArgs keyArgs = e.Data as KeyEventArgs;

            if (ScriptThread.GetVar<bool>("enableconsole"))
            {
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

                else GetConsoleInput(keyArgs);
            }
        }

        public void ShowConsole()
        {
            if (!_showingConsole)
            {
                _input.Show();

                _output.Show();

                _output.DisableFadeOut();

                _showingConsole = true;
            }
        }

        public void HideConsole()
        {
            if (_showingConsole)
            {
                _input.Clear();

                _input.Hide();

                _output.Hide();

                _output.EnableFadeOut();

                _showingConsole = false;
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            if (args == null)
                _output.WriteLine(format);
            else _output.WriteLine(format, args);
        }

        private void GetConsoleInput(KeyEventArgs e)
        {
            var key = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);

            char keyChar = Win32Native.GetCharFromKey(key, e.Shift);

            if ((e.Modifiers & Keys.Shift) != 0)
            {
                switch (keyChar)
                {
                    case ',': keyChar = '<'; break;
                    case '.': keyChar = '>'; break;
                    case '/': keyChar = '?'; break;
                    case ';': keyChar = ':'; break;
                    case '\'': keyChar = '"'; break;
                    case '\\': keyChar = '|'; break;
                    case '[': keyChar = '{'; break;
                    case ']': keyChar = '}'; break;
                    case '1': keyChar = '!'; break;
                    case '2': keyChar = '@'; break;
                    case '3': keyChar = '#'; break;
                    case '4': keyChar = '$'; break;
                    case '5': keyChar = '%'; break;
                    case '6': keyChar = '^'; break;
                    case '7': keyChar = '&'; break;
                    case '8': keyChar = '*'; break;
                    case '9': keyChar = '('; break;
                    case '0': keyChar = ')'; break;
                    case '-': keyChar = '_'; break;
                    case '=': keyChar = '+'; break;
                    case '`': keyChar = '~'; break;
                    default: keyChar = char.ToUpper(keyChar); break;
                }
            }

            else if (char.IsLetterOrDigit(keyChar))
            {
                if (_capsLock || e.Modifiers.HasFlag(Keys.Shift))
                    keyChar = char.ToUpper(keyChar);
            }

            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        _input.AddChar(' ');
                        return;

                    case Keys.Back:
                        {
                            string text = _input.GetText();

                            if (text.Length < 1)
                            {
                                HideConsole();
                            }

                            _input.RemoveLastChar();

                            return;
                        }
                    case Keys.Enter:
                        {
                            string text = _input.GetText();

                            _output.WriteLine(text);

                            HideConsole();

                            NotifyEvent("textadded", new ScriptEventArgs(text));

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

            base.OnUpdate(gameTime);
        }
    }
}
