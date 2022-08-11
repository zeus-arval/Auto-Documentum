using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AD.FilesManager.CSharp
{
    public sealed class CSharpTokenTree
    {
        public string ErrorMessage { get; init; }
        public SyntaxTree? SyntaxTree { get; init; }
        public CSharpTokenTree(in string errorMessage, in SyntaxTree? syntaxTree)
        {
            ErrorMessage = errorMessage;
            SyntaxTree = syntaxTree ?? null;
        }
    }
}
