using System;
using System.Collections.Generic;
using System.Windows.Automation;
using TornadoScript.ScriptCore.Game;

namespace TornadoScript.ScriptMain.Utility
{
    public class SoundManager : ScriptExtension
    {
        private List<WavePlayer> sounds = new List<WavePlayer>();

        public SoundManager()
        {
            SetupWindowHandling();
        }

        private void SetupWindowHandling()
        {
#if !DEBUG
            AutomationFocusChangedEventHandler handler = SoundManager_OnWindowFocusChange;

            Automation.AddAutomationFocusChangedEventHandler(handler);
#endif
        }

        public void Add(WavePlayer sound)
        {
            sounds.Add(sound);
        }

        private void SoundManager_OnWindowFocusChange(object source, AutomationFocusChangedEventArgs e)
        {
            var focusedHandle = new IntPtr(AutomationElement.FocusedElement.Current.NativeWindowHandle);
            var mainWindowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            if (focusedHandle == mainWindowHandle)
            {
                foreach (var sound in sounds)
                {
                    if (sound.IsPaused())
                        sound.Play();
                }
            }

            else
            {
                foreach (var sound in sounds)
                {
                    if (sound.IsPlaying())
                        sound.Pause();
                }
            }
        }

        public override void OnUpdate(int gameTime)
        {
            foreach (var sound in sounds)
            {
                sound.Update();
            }

            base.OnUpdate(gameTime);
        }

        public override void Dispose()
        {
#if !DEBUG
            Automation.RemoveAutomationFocusChangedEventHandler(SoundManager_OnWindowFocusChange);
#endif

            base.Dispose();
        }
    }
}
