using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.CommandLine;
using Microsoft.Dynamics.Nav.CodeAnalysis.Diagnostics;
using Microsoft.Dynamics.Nav.CodeAnalysis.SymbolReference;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public class ProjectConverter
    {

        public string ProjectPath { get; set; }
        public string OutputPath { get; set; }
        protected ILogger Logger { get; set; }

        protected List<SyntaxTree> SyntaxTrees { get; private set; }
        protected List<ProjectFileInfo> ALFiles { get; private set; }
        protected Compilation Compilation { get; private set; }

        public ProjectConverter(string projectPath, string outputPath, ILogger logger)
        {
            this.ProjectPath = projectPath;
            this.OutputPath = outputPath;
            this.SyntaxTrees = null;
            this.Compilation = null;
            this.Logger = logger;
        }

        #region Project loading

        protected void LoadProject()
        {
            this.Logger.WriteInformation("Loading al files...");

            //load all syntax trees
            this.SyntaxTrees = new List<SyntaxTree>();
            this.ALFiles = new List<ProjectFileInfo>();
            this.LoadProjectALFiles(this.ProjectPath, this.OutputPath);

            List<Diagnostic> diagnostics = new List<Diagnostic>();

            //load project manifest
            string projectFile = Path.Combine(this.ProjectPath, "app.json");
            ProjectManifest manifest = ProjectManifest.ReadFromString(projectFile, File.ReadAllText(projectFile), diagnostics);

            this.Logger.WriteInformation("Preparing compilation...");

            //create compilation
            Compilation compilation = Compilation.Create("MyCompilation", manifest.AppManifest.AppPublisher,
                manifest.AppManifest.AppVersion, manifest.AppManifest.AppId,
                null, this.SyntaxTrees, 
                new CompilationOptions());

            compilation = compilation
                .WithReferenceLoader(new LocalCacheSymbolReferenceLoader(Path.Combine(this.ProjectPath, ".alpackages")))
                .WithReferences(manifest.GetAllReferences());

            this.Compilation = compilation;
        }


        protected void LoadProjectALFiles(string sourcePath, string destPath)
        {
            string[] filePaths = Directory.GetFiles(sourcePath, "*.al");
            for (int i=0; i<filePaths.Length; i++)
            {
                this.Logger.WriteInformation("Loading file " + filePaths[i]);

                string content = File.ReadAllText(filePaths[i]);
                SyntaxTree syntaxTree = SyntaxTree.ParseObjectText(content, filePaths[i]);
                
                this.SyntaxTrees.Add(syntaxTree);
                this.ALFiles.Add(new ProjectFileInfo()
                {
                    ProjectFilePath = filePaths[i],
                    OutputFilePath = Path.Combine(destPath, Path.GetFileName(filePaths[i])),
                    SyntaxTree = syntaxTree
                });
            }

            string[] dirPaths = Directory.GetDirectories(sourcePath);
            for (int i=0; i<dirPaths.Length; i++)
            {
                LoadProjectALFiles(dirPaths[i], Path.Combine(destPath, Path.GetFileName(dirPaths[i])));
            }
        }

        #endregion

        #region Main project conversion logic

        public void ConvertProject()
        {
            CheckParameters();
            PrepareOutputFolder();
            CopyNonALFiles(this.ProjectPath, this.OutputPath);
            LoadProject();
            ProcessALFiles();
        }

        #endregion

        #region Project files processing

        protected void ProcessALFiles()
        {
            foreach (ProjectFileInfo srcFile in this.ALFiles)
            {
                this.Logger.WriteInformation("Processing file " + srcFile.ProjectFilePath);

                SyntaxTree syntaxTree = srcFile.SyntaxTree;
                SemanticModel semanticModel = this.Compilation.GetSemanticModel(syntaxTree);

                SyntaxNode newRootNode = this.ProcessALFile(srcFile.ProjectFilePath, srcFile.SyntaxTree, semanticModel);
                File.WriteAllText(srcFile.OutputFilePath, newRootNode.ToFullString());
            }
        }

        protected virtual SyntaxNode ProcessALFile(string filePath, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            return syntaxTree.GetRoot();
        }

        #endregion

        #region File system operations

        protected void CheckParameters()
        {
            string fullSourcePath = Path.GetFullPath(this.ProjectPath);
            string fullDestPath = Path.GetFullPath(OutputPath);
            if (fullSourcePath.Equals(fullDestPath, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Project conversion error. Source and Destination paths are the same.");
        }

        protected void PrepareOutputFolder()
        {
            if (Directory.Exists(this.OutputPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(this.OutputPath);
                foreach (FileInfo file in directoryInfo.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                    dir.Delete(true);
            }
            else
            {
                Directory.CreateDirectory(this.OutputPath);
            }
        }

        protected void CopyNonALFiles(string sourcePath, string destPath)
        {
            //copy directories
            string[] directories = Directory.GetDirectories(sourcePath);
            foreach (string srcSubDir in directories)
            {
                string destSubDir = Path.Combine(destPath, Path.GetFileName(srcSubDir));
                Directory.CreateDirectory(destSubDir);
                CopyNonALFiles(srcSubDir, destSubDir);
            }

            //copy files
            string[] files = Directory.GetFiles(sourcePath, "*.*");
            foreach (string srcFile in files)
            {
                if (!srcFile.EndsWith(".al", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.Logger.WriteInformation("Copying file " + srcFile);
                    string destFile = Path.Combine(destPath, Path.GetFileName(srcFile));
                    File.Copy(srcFile, destFile);
                }
            }
        }

        #endregion

    }	
}
