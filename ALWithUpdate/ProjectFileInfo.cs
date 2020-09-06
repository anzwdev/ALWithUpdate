using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public class ProjectFileInfo
    {

        public string ProjectFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public SyntaxTree SyntaxTree { get; set; }

    }
}
