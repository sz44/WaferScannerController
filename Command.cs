using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMProjectTektronix
{
    public class Command
    {
        public string Ascii { get; set; }

        public string Type { get; set; }
        public bool Read { get; set; } 
        public bool Display { get; set; }

        public string Data { get; set; } = null;

        public TaskCompletionSource<string> TSC { get; set; } = new TaskCompletionSource<string>();

        public Command(string command, bool read=true, bool display=false, string type="")
        {
            Ascii = command;
            Type = type;
            Read = read;
            Display = display; 
        }
    }

   
}
