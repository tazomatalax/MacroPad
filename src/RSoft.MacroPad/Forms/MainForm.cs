using RSoft.MacroPad.BLL.Infrastructure.Protocol.Mappers;
using RSoft.MacroPad.Infrastructure;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace RSoft.MacroPad.Forms;

public partial class MainForm : Form
{
    private KeyboardLayout[] _layouts = [];
    private readonly LayoutParser _parser = new();
    private readonly IUsb _usb = new HidLibUsb();
    private readonly ConfigurationReader _configReader = new();
    private readonly ComposerRepository _composerRepository = new();

    public MainForm()
    {
        InitializeComponent();
        InitializeLayouts();
        InitializeUsb();
    }

    #region Init

    private void InitializeUsb()
    {
        var config = _configReader.Read("config.txt");
        if (config is not null)
            _usb.SupportedDevices = config.SupportedDevices;

        _usb.OnConnected += OnDeviceConnected;
    }

    private void OnDeviceConnected(object? sender, EventArgs e)
    {
        var layout = _layouts.FirstOrDefault(l =>
            l.Products?.Any(p => p.VendorId == _usb.VendorId && p.ProductId == _usb.ProductId) == true);

        if (layout is not null)
        {
            keyboardVisual1.KeyboardLayout = layout;
            keyboardFunction1.KeyboardLayout = layout;
        }

        lblCommStatus.Text = $"Connected: ({_usb.VendorId}:{_usb.ProductId}) Protocol: {_usb.ProtocolType}.id{_usb.Version}";

        ShowDisclaimerIfNeeded();
    }

    private void ShowDisclaimerIfNeeded()
    {
        if (!TestedProducts.IsTested(_usb.VendorId, _usb.ProductId))
        {
            using var disclaimer = new DisclaimerForm();
            disclaimer.ShowDialog();
        }
    }

    private void SetUsbStatus(bool connected)
    {
        lblStatus.Text = connected ? "Connected" : "Disconnected";
        lblStatus.BackColor = connected ? Color.FromArgb(0, 128, 0) : Color.FromArgb(128, 0, 0);
        tsSend.Enabled = connected;
    }

    private void InitializeLayouts()
    {
        _layouts = _parser.Parse("layouts.txt");

        if (tsLayout.DropDown is ToolStripDropDownMenu dropDown)
            dropDown.ShowImageMargin = false;

        tsLayout.DropDownItems.Clear();
        tsLayout.DropDownItems.AddRange(_layouts.Select(CreateLayoutMenuItem).ToArray());
    }

    private ToolStripMenuItem CreateLayoutMenuItem(KeyboardLayout layout)
    {
        var menuItem = new ToolStripMenuItem
        {
            Text = layout.Name,
            AutoSize = true,
            Tag = layout
        };

        menuItem.Click += (s, e) =>
        {
            StopRecording(s, e);
            keyboardVisual1.KeyboardLayout = layout;
            keyboardFunction1.KeyboardLayout = layout;
        };

        return menuItem;
    }

    private void Tick(object? sender, EventArgs e) => SetUsbStatus(_usb.Connect());

    private void StopRecording(object? sender, EventArgs e) => keyboardFunction1.StopRecording();

    #endregion

    private void tsSend_Click(object? sender, EventArgs e)
    {
        StopRecording(sender, e);

        if (keyboardVisual1.SelectedAction == InputAction.None)
        {
            MessageBox.Show("Please select a key or knob action to map!");
            return;
        }

        var composer = _composerRepository.Get(_usb.ProtocolType, _usb.Version);
        var reports = GetReportsForCurrentFunction(composer);

        HidLog.ClearLog();

        var success = true;
        foreach (var report in reports)
        {
            if (!_usb.Write(report))
            {
                success = false;
                break;
            }
        }

        lblCommStatus.Text = success ? "Writing successful" : "Write failed";
        lblCommStatus.Text += $" [{DateTime.Now:T}]";
    }

    private IEnumerable<Report> GetReportsForCurrentFunction(ReportComposer composer)
    {
        return keyboardFunction1.Function switch
        {
            Model.SetFunction.LED => composer.Led(
                keyboardVisual1.Layer,
                keyboardFunction1.LedMode,
                keyboardFunction1.LedColor),

            Model.SetFunction.KeySequence => GetKeySequenceReports(composer),

            Model.SetFunction.MediaKey => composer.Media(
                keyboardVisual1.SelectedAction,
                keyboardVisual1.Layer,
                MediaKeyMapper.Map((VirtualKey)keyboardFunction1.MediaKey)),

            Model.SetFunction.Mouse => composer.Mouse(
                keyboardVisual1.SelectedAction,
                keyboardVisual1.Layer,
                keyboardFunction1.MouseButton,
                keyboardFunction1.MouseModifier),

            _ => []
        };
    }

    private IEnumerable<Report> GetKeySequenceReports(ReportComposer composer)
    {
        var currentLayout = PInvoke.GetKeyboardLayout(0);
        var enUsLayout = PInvoke.LoadKeyboardLayout("00000409", ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_ACTIVATE);

        try
        {
            var keyMappings = keyboardFunction1.KeySequence.Select(s =>
            {
                var virtualKey = (VirtualKey)PInvoke.MapVirtualKeyEx(
                    (uint)s.ScanCode,
                    MAP_VIRTUAL_KEY_TYPE.MAPVK_VSC_TO_VK,
                    enUsLayout);

                return (
                    KeyCodeMapper.Map(virtualKey),
                    ModifierMapper.Map(s.ShiftL, s.ShiftR, s.AltL, s.AltR, s.CtrlL, s.CtrlR, s.WinL, s.WinR)
                );
            });

            return composer.Key(
                keyboardVisual1.SelectedAction,
                keyboardVisual1.Layer,
                keyboardFunction1.Delay,
                keyMappings);
        }
        finally
        {
            PInvoke.ActivateKeyboardLayout(currentLayout, ACTIVATE_KEYBOARD_LAYOUT_FLAGS.KLF_ACTIVATE);
        }
    }

    private void tsAbout_Click(object? sender, EventArgs e)
    {
        StopRecording(sender, e);
        using var aboutBox = new AboutBox();
        aboutBox.ShowDialog();
    }

    private void tsSetParams_Click(object? sender, EventArgs e)
    {
        using var connectionForm = new ConnectionForm(_usb);
        connectionForm.ShowDialog();
    }
}
