using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public class ConsoleLogger : ILogger
    {
        public void WriteError(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteInformation(string message)
        {
            Console.WriteLine(message);
        }

    }
}
