using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.SymbolReference;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
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
            IOperation operation = this.SemanticModel.GetOperation(node);
            if (operation != null)
            {
                IOperation operationInstance = this.GetOperationInstance(operation);
                if ((operationInstance != null) &&
                    (operationInstance.Syntax != null) &&
                    (operationInstance.Syntax.Parent != null) &&
                    (operationInstance.Syntax.Parent.Kind == SyntaxKind.WithStatement))
                {
                    return SyntaxFactory.MemberAccessExpression(
                        (CodeExpressionSyntax)operationInstance.Syntax.WithoutTrivia(),
                        node.WithoutTrivia()).WithTriviaFrom(node);
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
                        return ((IFieldAccess)operation).Instance;
                    case OperationKind.InvocationExpression:
                        return ((IInvocationExpression)operation).Instance;
                }
            }
            return null;
        }

    }
}
