using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            CmdParameters parameters = new CmdParameters(args);
            if (!parameters.HasParameters)
                return;

            //find AL compiler path
            if (String.IsNullOrWhiteSpace(parameters.ALExtensionPath))
            {
                string compilerFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                compilerFolder = Path.Combine(compilerFolder, ".vscode", "extensions");
                compilerFolder = Directory
                    .GetDirectories(compilerFolder, "ms-dynamics-smb.al-*")
                    .OrderBy(p => p)
                    .LastOrDefault();
                if (String.IsNullOrWhiteSpace(compilerFolder))
                {
                    Console.WriteLine("Cannot find Microsoft AL Extension folder. Please specify it manually using -alcompiler parameter.");
                    return;
                }
                parameters.ALExtensionPath = compilerFolder;
            }

            DevToolsServer server = new DevToolsServer(parameters.ALExtensionPath);

            ILogger logger = new ConsoleLogger();

            WithStatementsProjectConverter projectConverter = new WithStatementsProjectConverter(
                parameters.ProjectPath,
                parameters.OutputPath,
                logger);
            projectConverter.ConvertProject();

            logger.WriteInformation("Completed");

            Console.ReadLine();
        }
    }
}
