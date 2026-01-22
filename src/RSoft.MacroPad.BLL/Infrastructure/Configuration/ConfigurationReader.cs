using System.IO;
using System.Text.RegularExpressions;

namespace RSoft.MacroPad.BLL.Infrastructure.Configuration;

/// <summary>
/// Reads and parses device configuration from a configuration file.
/// </summary>
public sealed partial class ConfigurationReader
{
    [GeneratedRegex(@"^([0-9]+):([0-9]+),([a-zA-Z0-9\-_]+)(?:,([01]))?$", RegexOptions.Compiled)]
    private static partial Regex DeviceConfigLinePattern();

    /// <summary>
    /// Reads the configuration from the specified file.
    /// </summary>
    /// <param name="fileName">The path to the configuration file.</param>
    /// <returns>The parsed configuration, or null if the file cannot be read.</returns>
    public Configuration? Read(string fileName)
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(fileName)
                .Select(l => l.Trim())
                .ToArray();
        }
        catch
        {
            return null;
        }

        var devices = new List<SupportedDevice>();
        var lineNo = 0;

        foreach (var line in lines)
        {
            lineNo++;

            if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                continue;

            var match = DeviceConfigLinePattern().Match(line);
            if (match.Success)
            {
                var vid = ushort.Parse(match.Groups[1].Value);
                var pid = ushort.Parse(match.Groups[2].Value);
                var path = match.Groups[3].Value;
                var protocolType = ProtocolType.Extended;

                if (match.Groups.Count > 4 && !string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    var type = byte.Parse(match.Groups[4].Value);
                    protocolType = type == 0 ? ProtocolType.Legacy : ProtocolType.Extended;
                }

                devices.Add(new SupportedDevice(vid, pid, path, protocolType));
                continue;
            }

            throw new InvalidDataException($"Invalid line format in {fileName}({lineNo}): {line}");
        }

        return new Configuration { SupportedDevices = devices };
    }
}
