using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpAsm
{
    class Program
    {
        static void Main(string[] args)
        {
            x86Executer x86 = new x86Executer();

            Console.WriteLine(string.Format("EFLAGS: {0:X}", x86.GetEFlags()));

            Console.WriteLine(string.Format("Addition of (25,5): {0}", x86.x86Add(25, 5)));

            Console.Read();
        }
    }
}
