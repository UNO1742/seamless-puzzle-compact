using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SeamlessPuzzle.Utils
{
    public static class ClipSlimLink
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetNamedPipeServerProcessId(SafePipeHandle pipe, out uint processId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AllowSetForegroundWindow(uint processId);
        public static async Task<string> InsertAsync(string file)
        {
            return await Task.Run(async () =>
            {
                string sid = WindowsIdentity.GetCurrent().User!.Value;
                using var pipe = new NamedPipeClientStream(".", "ClipSlim.Collage.v172." + sid, PipeDirection.InOut, PipeOptions.Asynchronous);
                try { pipe.Connect(200); }
                catch (TimeoutException)
                {
                    string root = Directory.GetParent(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar))!.FullName;
                    string engine = Path.Combine(root, "ClipSlim-v1.7.2.exe");
                    if (!File.Exists(engine)) throw new FileNotFoundException("找不到配套轻图，请从桌面新版快捷方式启动拼图工具。", engine);
                    Process.Start(new ProcessStartInfo(engine) { WorkingDirectory = root, UseShellExecute = false });
                    pipe.Connect(10000);
                }
                // The user's click belongs to this foreground process. Hand its foreground
                // allowance to the actual pipe server, including when ClipSlim was already open.
                uint serverId;
                if (GetNamedPipeServerProcessId(pipe.SafePipeHandle, out serverId))
                    AllowSetForegroundWindow(serverId);
                using var writer = new StreamWriter(pipe, new UTF8Encoding(false), 4096, true) { AutoFlush = true };
                using var reader = new StreamReader(pipe, Encoding.UTF8, false, 4096, true);
                await writer.WriteLineAsync(file);
                string? reply = await reader.ReadLineAsync().WaitAsync(TimeSpan.FromSeconds(90));
                if (reply == null) throw new IOException("轻图连接已断开，请检查是否已经插入，避免重复操作。");
                if (reply.StartsWith("ERR|")) throw new InvalidOperationException(reply.Substring(4));
                if (reply.StartsWith("READY|")) return reply.Substring(6);
                if (reply.StartsWith("SENT|")) return reply.Substring(5);
                if (!reply.StartsWith("OK|")) throw new IOException("轻图返回了无法识别的结果。");
                return reply.Substring(3);
            });
        }
    }
}
