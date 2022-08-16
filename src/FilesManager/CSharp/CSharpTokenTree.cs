using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AD.FilesManager.CSharp
{
    /// <summary>
    /// Contains <see cref="SyntaxTree"/> and ErrorMessage if <see cref="SyntaxTree"/> is null
    /// </summary>
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
