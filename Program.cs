using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;
namespace arrays
{


    class Program
    {
        static void Main(string[] args)
        {
            string ret = libnative.return_wstring();
            Console.WriteLine(ret);
        }
    }
}
