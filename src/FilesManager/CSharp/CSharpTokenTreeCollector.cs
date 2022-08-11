using AD.Aids.Factories;
using AD.FilesManager.Common;
using AD.FilesManager.CSharp.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace AD.FilesManager.CSharp
{
    public sealed class CSharpTokenTreeCollector : ITokenTreeCollector
    {
        private class CSharpSyntaxTreeReader
        {
            private ILogger<CSharpSyntaxTreeReader> _logger;

            public CSharpSyntaxTreeReader(ILogger<CSharpSyntaxTreeReader> logger)
            {
                _logger = logger;
            }

            public void ExtractClasses(IEnumerable<CSharpTokenTree> tokenTreeArray, out List<IClass> cSharpClasses)
            {
                cSharpClasses = new List<IClass>();

                foreach (var tokenTree in tokenTreeArray)
                {
                    if (tokenTree.ErrorMessage != string.Empty)
                    {
                        _logger.LogError(tokenTree.ErrorMessage);
                        continue;
                    }

                    cSharpClasses.AddRange(ExtractClassesImpl(tokenTree.SyntaxTree!));
                }
            }

            private CSharpClass[] ExtractClassesImpl(in SyntaxTree syntaxTree)
            {
                var containsNamespaces = SyntaxNodeHelper.TryGetNameSpaceSyntaxis(syntaxTree, out List<NamespaceDeclarationSyntax> namespaceSyntaxis);

                if (containsNamespaces == false)
                {
                    _logger.LogWarning("SyntaxTree {FilePath} doesn't contain any namespace.", syntaxTree.FilePath);
                    return Array.Empty<CSharpClass>();
                }

                var containsClasses = SyntaxNodeHelper.TryGetClassDeclarationSyntaxis(syntaxTree, out List<ClassDeclarationSyntax> classSyntaxis);
                CSharpClass[] cSharpClasses = new CSharpClass[classSyntaxis.Count];

                if (containsClasses == false)
                {
                    _logger.LogWarning("{filePath} doesn't contain any class", syntaxTree.FilePath);

                    return Array.Empty<CSharpClass>();
                }

                for (int i = 0; i < classSyntaxis.Count; i++)
                {
                    try
                    {
                        CSharpClass cSharpClass = GetCSharpClass(classSyntaxis[i]);
                        cSharpClasses[i] = cSharpClass;

                        _logger.LogInformation("Namespace of class {className} is {namespaceName}", cSharpClass.Name, cSharpClass.NamespaceName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error occured. ", ex.Message);
                        throw;
                    }
                }

                return cSharpClasses;
            }

            private CSharpClass GetCSharpClass(in ClassDeclarationSyntax classSyntax)
            {
                var succeeded = SyntaxNodeHelper.TryGetFullClassPath(classSyntax, out string? namespaceName);
                if (succeeded == false)
                {
                    throw new Exception("Couldn't get namespace");
                }

                succeeded = SyntaxNodeHelper.TryGetClassName(classSyntax, out string className);
                if (succeeded == false)
                {
                    throw new Exception("Couldn't get class name");
                }

                CSharpField[] fields = SyntaxNodeHelper.GetCSharpFieldArray(classSyntax);
                CSharpProperty[] properties = SyntaxNodeHelper.GetCSharpPropertyArray(classSyntax);
                return null;
                //succeeded &= SyntaxNodeHelper.TryGetMethods(classSyntax, out CSharpMethod[] methods);
            }
        }

        private ILogger<CSharpTokenTreeCollector> _logger;
        private readonly IEnumerable<CSharpTokenTree> _syntaxTreeArray;
        private readonly CSharpTokenTreeGenerator _generator;
        private readonly CSharpSyntaxTreeReader _syntaxTreeReader;
        private List<IClass>? _classesList;
        public List<IClass>? ClassesList 
        { 
            get => _classesList; 
            private set
            {
                _classesList = value!;
            }
        }

        public string DirectoryPath { get; private set; }

        public CSharpTokenTreeCollector(MainFactory mainFactory, string directoryPath)
        {
            _logger = mainFactory.CreateLogger<CSharpTokenTreeCollector>();
            DirectoryPath = directoryPath;
            _generator = new CSharpTokenTreeGenerator();
            _syntaxTreeArray = _generator.CreateSyntaxTreeArray(DirectoryPath);
            _syntaxTreeReader = new CSharpSyntaxTreeReader(mainFactory.CreateLogger<CSharpSyntaxTreeReader>());
        }

        public void FillClasses()
        {
            _syntaxTreeReader.ExtractClasses(_syntaxTreeArray, out _classesList); 
        }
    }
}
