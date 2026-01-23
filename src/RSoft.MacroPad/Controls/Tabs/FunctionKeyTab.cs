using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using RSoft.MacroPad.Infrastructure;
using RSoft.MacroPad.Model;

namespace RSoft.MacroPad.Controls.Tabs
{
    public partial class FunctionKeyTab : UserControl
    {
        private Keys key = Keys.F13;

        public Keys Key
        {
            get => key;
            set
            {
                key = value;
                UpdateControls();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KeyStroke KeyStroke
        {
            get => new KeyStroke { Key = key, Operation = KeyStrokeOperation.Press };
            set
            {
                if (value != null)
                {
                    key = value.Key;
                    UpdateControls();
                }
            }
        }

        public FunctionKeyTab()
        {
            InitializeComponent();

            rbF13.Tag = Keys.F13;
            rbF14.Tag = Keys.F14;
            rbF15.Tag = Keys.F15;
            rbF16.Tag = Keys.F16;
            rbF17.Tag = Keys.F17;
            rbF18.Tag = Keys.F18;

            UpdateControls();
        }

        private void KeyChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            key = (Keys)rb.Tag;
        }

        private void UpdateControls()
        {
            foreach (var rb in Controls.As<RadioButton>())
            {
                if ((Keys)rb.Tag == key)
                {
                    rb.Checked = true;
                    break;
                }
            }
        }
    }
}
