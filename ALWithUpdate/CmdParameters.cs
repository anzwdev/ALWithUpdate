using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public class CmdParameters
    {

        public string ProjectPath { get; set; }
        public string OutputPath { get; set; }
        public string ALExtensionPath { get; set; }
        public bool HasParameters { get; set; }

        protected bool _helpDisplayed;

        public CmdParameters()
        {
            this.ProjectPath = null;
            this.OutputPath = null;
            this.ALExtensionPath = null;
            this._helpDisplayed = false;
        }

        public CmdParameters(string[] args): this()
        {
            this.Parse(args);
            this.HasParameters = (!String.IsNullOrWhiteSpace(this.ProjectPath)) && (!String.IsNullOrWhiteSpace(this.OutputPath));
            if (!this.HasParameters)
            {
                Console.WriteLine("Incorrect parameters");
                Console.WriteLine("");
                this.WriteHelp();
            }
        }

        protected void Parse(string[] args)
        {
            int idx = 0;
            while (idx < args.Length)
            {
                string val = args[idx];
                if (val.Equals("-source", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ProjectPath = SafeGetVal(args, idx + 1);
                    idx += 2;
                }
                else
                if (val.Equals("-dest", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.OutputPath = SafeGetVal(args, idx + 1);
                    idx += 2;
                }
                else
                if (val.Equals("-alextension", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ALExtensionPath = SafeGetVal(args, idx + 1);
                    idx += 2;
                }
                else
                if (val.Equals("-help", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ProjectPath = SafeGetVal(args, idx + 1);
                    idx += 2;
                }
                else
                if (val.Equals("-help", StringComparison.CurrentCultureIgnoreCase))
                {
                    idx++;
                    WriteHelp();
                }
                else
                    idx++;
            }
        }

        protected string SafeGetVal(string[] args, int idx)
        {
            return (idx < args.Length) ? args[idx] : "";
        }

        public void WriteHelp()
        {
            if (!this._helpDisplayed)
            {
                Console.WriteLine("AL With Update");
                Console.WriteLine("Removes all implicit and explicit WITH commands from an AL project");
                Console.WriteLine("Usage:");
                Console.WriteLine("");
                Console.WriteLine("ALWithUpdate -source <ProjectPath> -dest <OutputPath> [-alextension <MSALExtensionPath>]");
                Console.WriteLine("-source: path to the AL project");
                Console.WriteLine("-dest: path to output folder where converted al project will be saved");
                Console.WriteLine("-alextension: optional path to the main folder of Microsoft AL Extension. It is usually located in <UserProfile>\\.vscode\\extensions\\ms-dynamics-smb.al-<version>");

                this._helpDisplayed = true;
            }
        }


    }
}
