using System.IO;
using System.Threading;
using RSoft.MacroPad.Forms;

namespace RSoft.MacroPad;

internal static class Program
{
    private const string ErrorLogFile = "error.log";

    [STAThread]
    public static void Main()
    {
        try
        {
            ApplicationConfiguration.Initialize();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            HandleException("Main()", ex);
        }
    }

    private static void HandleException(string header, Exception? ex)
    {
        var timestamp = DateTime.Now.ToString("g");
        var exceptionDetails = ex is null
            ? "Exception object was null"
            : $"{ex.GetType().FullName}\r\n{ex.Message}\r\n{ex.StackTrace}";

        var logEntry = $"[{timestamp}] Exception at {header}\r\n{exceptionDetails}\r\n\r\n";

        try
        {
            File.AppendAllText(ErrorLogFile, logEntry);
        }
        catch
        {
            // If we can't write to the log, there's nothing more we can do
        }
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        => HandleException($"Application_Thread ({sender})", e.Exception);

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        => HandleException($"CurrentDomain ({sender})", e.ExceptionObject as Exception);
}
