using System;
using System.Diagnostics;

namespace Snrk.Github
{
    [System.ComponentModel.DesignerCategory("")]
    class CommandPrompt : Process, IDisposable
    {
        public CommandPrompt(bool createNoWindow = false)
        {
            StartInfo.FileName = "cmd.exe";
            //StartInfo.CreateNoWindow = createNoWindow;
            StartInfo.CreateNoWindow = false;
            StartInfo.RedirectStandardInput = true;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.UseShellExecute = false;
            Start();
        }
        
        ~CommandPrompt() { Dispose(); }
        public new void Dispose()
        {
            Close();
            base.Dispose();
        }

        public void WriteLine(string command, params object[] args0)
        {
            StandardInput.WriteLine(string.Format("{0} 1>NUL", command), args0);
        }

        public void RethrowError()
        {
            while (!StandardError.EndOfStream)
            {
                string line = StandardError.ReadLine();
                if (line.StartsWith("fatal") || line.StartsWith("error"))
                    throw new Exception(line);
            }
        }
    }
}
