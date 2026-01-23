using System;
using System.Windows.Forms;

namespace RSoft.MacroPad.Infrastructure
{
    public enum WorkflowMode
    {
        VSCODE,
        MEDIA,
        WINDOW
    }

    public class ModeManager
    {
        private WorkflowMode _currentMode = WorkflowMode.VSCODE;
        private readonly WorkflowMode[] _modeCycle = { WorkflowMode.VSCODE, WorkflowMode.MEDIA, WorkflowMode.WINDOW };

        public WorkflowMode CurrentMode => _currentMode;

        public event EventHandler<WorkflowMode>? ModeChanged;

        public void CycleMode()
        {
            var currentIndex = Array.IndexOf(_modeCycle, _currentMode);
            var nextIndex = (currentIndex + 1) % _modeCycle.Length;
            _currentMode = _modeCycle[nextIndex];
            ModeChanged?.Invoke(this, _currentMode);
        }

        public void HandleFunctionKey(Keys key)
        {
            switch (_currentMode)
            {
                case WorkflowMode.VSCODE:
                    HandleVSCodeMode(key);
                    break;
                case WorkflowMode.MEDIA:
                    HandleMediaMode(key);
                    break;
                case WorkflowMode.WINDOW:
                    HandleWindowMode(key);
                    break;
            }
        }

        private void HandleVSCodeMode(Keys key)
        {
            switch (key)
            {
                case Keys.F13: // Key 1: Quick Open (Ctrl+P)
                    SendKeys.Send("^p");
                    break;
                case Keys.F14: // Key 2: Toggle Terminal (Ctrl+`)
                    SendKeys.Send("^`");
                    break;
                case Keys.F15: // Key 3: Format Document (Shift+Alt+F)
                    SendKeys.Send("+%f");
                    break;
                case Keys.F16: // Knob Left: Previous Tab (Ctrl+PgUp)
                    SendKeys.Send("^{PGUP}");
                    break;
                case Keys.F17: // Knob Right: Next Tab (Ctrl+PgDn)
                    SendKeys.Send("^{PGDN}");
                    break;
            }
        }

        private void HandleMediaMode(Keys key)
        {
            // For media keys, we'll need to use low-level APIs
            // For now, these can be left as placeholders
            // Teams mute: Win+Alt+K is complex, skip for now
        }

        private void HandleWindowMode(Keys key)
        {
            switch (key)
            {
                case Keys.F13: // Key 1: Task View (Win+Tab) - Not supported by SendKeys
                    break;
                case Keys.F14: // Key 2: Show Desktop (Win+D)
                    SendKeys.Send("^{ESC}d"); // Close approximation
                    break;
                case Keys.F15: // Key 3: Lock Screen (Win+L) - Not supported by SendKeys
                    break;
                case Keys.F16: // Knob Left: Alt+Tab
                    SendKeys.Send("%{TAB}");
                    break;
                case Keys.F17: // Knob Right: Alt+Shift+Tab
                    SendKeys.Send("%+{TAB}");
                    break;
            }
        }
    }
}
