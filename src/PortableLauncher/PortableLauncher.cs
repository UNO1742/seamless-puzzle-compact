using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
internal static class PortableLauncher
{
    [DllImport("user32.dll")] static extern bool ShowWindowAsync(IntPtr window, int command);
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr window);
    [STAThread]
    static void Main()
    {
        try
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string application = Path.Combine(root, "app", "SeamlessPuzzle.exe");
            // Keep one stable, version-independent desktop entry for future updates.
            try
            {
                dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
                dynamic shortcut = shell.CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "无缝拼图.lnk"));
                shortcut.TargetPath = Path.Combine(root, "启动无缝拼图.exe");
                shortcut.WorkingDirectory = root;
                shortcut.IconLocation = application;
                shortcut.Description = "紧凑拼图：读取剪贴板、置顶、横竖和宫格拼接";
                shortcut.Save();
                Marshal.ReleaseComObject(shortcut); Marshal.ReleaseComObject(shell);
            }
            catch { }
            foreach (Process existing in Process.GetProcessesByName("SeamlessPuzzle"))
            {
                try
                {
                    if (String.Equals(existing.MainModule.FileName, application, StringComparison.OrdinalIgnoreCase) && existing.MainWindowHandle != IntPtr.Zero)
                    {
                        ShowWindowAsync(existing.MainWindowHandle, 9);
                        SetForegroundWindow(existing.MainWindowHandle);
                        return;
                    }
                }
                catch { }
                finally { existing.Dispose(); }
            }
            ProcessStartInfo start = new ProcessStartInfo(application);
            start.WorkingDirectory = Path.GetDirectoryName(application);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;


            Process.Start(start);
        }
        catch (Exception ex) { MessageBox.Show("启动失败：" + ex.Message, "无缝拼图", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
