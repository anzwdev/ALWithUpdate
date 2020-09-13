using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.SymbolReference;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public class WithIdentifiersSyntaxRewriter: SyntaxRewriter
    {
        public SemanticModel SemanticModel { get; set; }
 
        public WithIdentifiersSyntaxRewriter()
        {
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            bool skip =
                (node.Parent.Kind == SyntaxKind.PageField) &&
                (((PageFieldSyntax)node.Parent).Name == node);

            if (!skip)
            {

                IOperation operation = this.SemanticModel.GetOperation(node);
                if (operation != null)
                {
                    IOperation operationInstance = this.GetOperationInstance(operation);

                    if ((operationInstance != null) &&
                        (operationInstance.Syntax != null))
                    {
                        //part of with?
                        if ((operationInstance.Syntax.Parent != null) &&
                        (operationInstance.Syntax.Parent.Kind == SyntaxKind.WithStatement))
                        {
                            return SyntaxFactory.MemberAccessExpression(
                                (CodeExpressionSyntax)operationInstance.Syntax.WithoutTrivia(),
                                node.WithoutTrivia()).WithTriviaFrom(node);
                        }

                        //global variable reference?
                        else if ((operationInstance.Kind == OperationKind.GlobalReferenceExpression) &&
                            (node.Parent.Kind != SyntaxKind.MemberAccessExpression))
                        {
                            IGlobalReferenceExpression globalRef = (IGlobalReferenceExpression)operationInstance;
                            string name = globalRef.GlobalVariable.Name.ToString();

                            return SyntaxFactory.MemberAccessExpression(
                                SyntaxFactory.IdentifierName(name),
                                node.WithoutTrivia()).WithTriviaFrom(node);
                        }
                    }
                }
            }

            return base.VisitIdentifierName(node);
        }

        protected IOperation GetOperationInstance(IOperation operation)
        {
            if (operation != null)
            {
                switch (operation.Kind)
                {
                    case OperationKind.FieldAccess:
                        IFieldAccess fieldAccess = operation as IFieldAccess;
                        if (fieldAccess != null)
                            return fieldAccess.Instance;
                        break;
                    case OperationKind.InvocationExpression:
                        IInvocationExpression invocationExpression = operation as IInvocationExpression;
                        if (invocationExpression != null)
                            return invocationExpression.Instance;
                        break;
                }
            }
            return null;
        }

    }
}
