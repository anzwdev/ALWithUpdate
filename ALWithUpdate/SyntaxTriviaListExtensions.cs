using Microsoft.Dynamics.Nav.CodeAnalysis;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALWithUpdate
{
    public static class SyntaxTriviaListExtensions
    {

        public static SyntaxTriviaList NormalizeSyntaxTriviaList(this SyntaxTriviaList triviaList)
        {
            List<SyntaxTrivia> newList = new List<SyntaxTrivia>();
            
            for (int triviaIdx = 0; triviaIdx < triviaList.Count; triviaIdx++)
            {
                SyntaxTrivia trivia = triviaList[triviaIdx];
                bool addTrivia = true;
                switch (trivia.Kind)
                {
                    case SyntaxKind.WhiteSpaceTrivia:
                        addTrivia = (triviaIdx == (triviaList.Count - 1)) ||
                            (triviaList[triviaIdx + 1].Kind != SyntaxKind.EndOfLineTrivia);
                        break;
                    case SyntaxKind.EndOfLineTrivia:
                        addTrivia = (newList.Count == 0) ||
                            (newList[newList.Count - 1].Kind != SyntaxKind.EndOfLineTrivia);
                        break;
                }
                if (addTrivia)
                    newList.Add(triviaList[triviaIdx]);
            }
            return SyntaxFactory.TriviaList(newList);
        }


    }
}
