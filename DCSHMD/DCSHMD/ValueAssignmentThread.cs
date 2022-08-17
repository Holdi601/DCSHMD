using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace DCSHMD
{
    public class ValueAssignmentThread
    {
        public bool quit = false;
        public bool bench = false;
        public void RefresherGo()
        {
            Stopwatch sw = new Stopwatch();
            while (!quit)
            {
                General.overlay.Dispatcher.Invoke(new Action(() =>
                {
                    foreach(KeyValuePair<string, DP_Windows> kvp in General.SourceElements)
                    {
                        if(kvp.Value is DP_Windows_Text)
                        {
                            ((DP_Windows_Text)kvp.Value).UpdateValue();
                        }
                    }
                }));
                if(bench)
                    General.Write(sw.ElapsedMilliseconds.ToString());
                sw.Restart();
                Thread.Sleep(8);
            }
        }
    }
}
