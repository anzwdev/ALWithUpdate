using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public interface ILogger
    {

        void WriteInformation(string message);
        void WriteError(string message);

    }
}
