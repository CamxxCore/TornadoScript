using System.Drawing;
using GTA;
using GTA.Native;

namespace TornadoScript.Frontend
{
    public class FrontendInput
    {
        const int TextActiveTime = 30000;

        const int CursorPulseSpeed = 300;

        private bool _cursorState = false;

        private bool _active = false;

        string _str = "";

        private int _lastCursorPulse = 0,
            _currentTextWidth = 0,
            _shownTime = 0;

        private UIText _text = new UIText("", new Point(14, 5), 0.3f);

        private UIRectangle _cursor = new UIRectangle(new Point(14, 5), new Size(1, 15), Color.Empty);

        private UIContainer _backsplash = new UIContainer(new Point(20, 20), new Size(600, 30), Color.Empty);

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public FrontendInput()
        {
            _backsplash.Items.Add(_text);
            _backsplash.Items.Add(_cursor);
        }

        /// <summary>
        /// Add a line of text to the input box.
        /// </summary>
        /// <param name="text"></param>
        public void AddLine(string text)
        {
            Show();

            _str = text;

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Add a single character to the input box.
        /// </summary>
        /// <param name="text"></param>
        public void AddChar(char c)
        {
            Show();

            _str += c;

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Gets the current text in the input box.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return _str;
        }

        /// <summary>
        /// Remove the last character from the input box.
        /// </summary>
        public void RemoveLastChar()
        {
            if (_str.Length > 0)
            {
                _str = _str.Substring(0, _str.Length - 1);

                _currentTextWidth = GetTextWidth();
            }
        }

        /// <summary>
        /// Show the input box.
        /// </summary>
        public void Show()
        {
            _backsplash.Color = Color.FromArgb(140, 52, 144, 2);

            SetCursorColor(Color.White);

            SetTextColor(Color.White);

            _shownTime = Game.GameTime;

            _active = true;
        }

        /// <summary>
        /// Hide the input box.
        /// </summary>
        public void Hide()
        {
            _backsplash.Color = Color.Empty;

            SetCursorColor(Color.Empty);

            SetTextColor(Color.Empty);

            _active = false;
        }

        /// <summary>
        /// Clear the active text.
        /// </summary>
        public void Clear()
        {
            _str = string.Empty;

            _currentTextWidth = GetTextWidth();
        }

        /// <summary>
        /// Set the text color.
        /// </summary>
        /// <param name="color"></param>
        private void SetTextColor(Color color)
        {
            _text.Color = color;
        }

        /// <summary>
        /// Set the cursor color.
        /// </summary>
        /// <param name="color"></param>
        private void SetCursorColor(Color color)
        {
            _cursor.Color = color;
        }

        private int GetTextWidth()
        {
            Function.Call((Hash)0x54CE8AC98E120CAB, "CELL_EMAIL_BCON");

            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, _str);

            Function.Call(Hash.SET_TEXT_FONT, (int)_text.Font);
            Function.Call(Hash.SET_TEXT_SCALE, _text.Scale, _text.Scale);

            return (int) (UI.WIDTH * Function.Call<float>((Hash)0x85F061DA64ED2F67, (int)_text.Font));
        }

        /// <summary>
        /// Update and draw the this <see cref="FrontendInput"/> box.
        /// </summary>
        public void Update(int gameTime)
        {
            if (_active)
            {
                Game.DisableAllControlsThisFrame(0);

                _text.Caption = _str;

                _cursor.Position = new Point(14 + _currentTextWidth, 7);

                if (gameTime - _lastCursorPulse > CursorPulseSpeed && _text.Color.A > 0)
                {
                    _cursorState = !_cursorState;

                    _cursor.Color = _cursorState ? Color.FromArgb(255, _cursor.Color) : Color.FromArgb(0, _cursor.Color);

                    _lastCursorPulse = gameTime;
                }

                _backsplash.Draw();
            }
        }
    }
}