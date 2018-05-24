using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellWrapper
{
    public class Build
    {


        [STAThreadAttribute]
        public static void Main(string[] args)
        {

            var a = new ShellWrapper();
            Console.ReadLine();
            GC.SuppressFinalize(a);
        }

    }
}
