using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Merona
{
    public class DumpCallStack
    {
        public static void Dump()
        {
            StackTrace traces = new StackTrace(1);
            
            foreach(var trace in traces.GetFrames())
            {
                Console.WriteLine("{0} : {1}", trace.GetMethod(), trace.GetFileLineNumber());
            }
        }
    }
}
