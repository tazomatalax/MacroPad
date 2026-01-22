namespace RSoft.MacroPad.BLL.Infrastructure.Model;

/// <summary>
/// Defines the communication protocol types supported by macro pad devices.
/// </summary>
public enum ProtocolType
{
    /// <summary>
    /// Legacy protocol used by older devices (primarily the 3-button 1-knob keypad).
    /// </summary>
    Legacy = 0,

    /// <summary>
    /// Extended protocol used by most modern macro pad devices.
    /// </summary>
    Extended = 1
}
