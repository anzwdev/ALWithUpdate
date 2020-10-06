using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public class WithStatementsProjectConverter: ProjectConverter
    {

        public WithStatementsProjectConverter(string projectPath, string outputPath, ILogger logger) : base(projectPath, outputPath, logger)
        {
        }

        protected override SyntaxNode ProcessALFile(string filePath, SyntaxTree syntaxTree, SemanticModel semanticModel)
        {
            if (filePath.Contains("Pag50000.MySmallTableList.al"))
            {
                int f = 9999999;
            }

            //stage 1 - update calls
            WithIdentifiersSyntaxRewriter identifiersRewriter = new WithIdentifiersSyntaxRewriter();
            identifiersRewriter.SemanticModel = semanticModel;
            SyntaxNode newNode = identifiersRewriter.Visit(syntaxTree.GetRoot());

            //stage 2 - remove "with" statements
            WithRemoveSyntaxRewriter withRemoveSyntaxRewriter = new WithRemoveSyntaxRewriter();
            newNode = withRemoveSyntaxRewriter.Visit(newNode);

            return newNode;
        }


    }
}
