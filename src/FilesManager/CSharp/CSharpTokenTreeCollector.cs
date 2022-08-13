using AD.Aids.Factories;
using AD.FilesManager.Common;
using AD.FilesManager.CSharp.Extensions;
using AD.FilesManager.CSharp.FileContentElements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using static AD.FilesManager.Common.LogMessages;

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
                    _logger.LogWarning(SYNTAX_TREE_DOESNOT_CONTAIN_ANY_NAMESPACE, syntaxTree.FilePath);
                    return Array.Empty<CSharpClass>();
                }

                var containsClasses = SyntaxNodeHelper.TryGetClassDeclarationSyntaxis(syntaxTree, out List<ClassDeclarationSyntax> classSyntaxis);
                CSharpClass[] cSharpClasses = new CSharpClass[classSyntaxis.Count];

                if (containsClasses == false)
                {
                    _logger.LogWarning(FILE_DOESNOT_CONTAIN_ANY_CLASS, syntaxTree.FilePath);

                    return Array.Empty<CSharpClass>();
                }

                for (int i = 0; i < classSyntaxis.Count; i++)
                {
                    try
                    {
                        cSharpClasses[i] = GetCSharpClass(classSyntaxis[i]);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(GENERIC_ERROR_OCCURED, ex.Message);
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
                    throw new Exception(CANNOT_GET_NAMESPACE_NAME);
                }

                succeeded = SyntaxNodeHelper.TryGetClassName(classSyntax, out string className);
                if (succeeded == false)
                {
                    throw new Exception(CANNOT_GET_CLASS_NAME);
                }

                CSharpField[] fields = SyntaxNodeHelper.GetCSharpFieldArray(classSyntax);
                CSharpProperty[] properties = SyntaxNodeHelper.GetCSharpPropertyArray(classSyntax);
                CSharpMethod[] methods = SyntaxNodeHelper.GetCSharpMethodArray(classSyntax);

                return new CSharpClass(namespaceName!, string.Empty, className, methods, fields, properties);
            }
        }

        private ILogger<CSharpTokenTreeCollector> _logger;
        private readonly IEnumerable<CSharpTokenTree> _syntaxTreeArray;
        private readonly CSharpTokenTreeGenerator _generator;
        private readonly CSharpSyntaxTreeReader _syntaxTreeReader;
        private List<IClass>? _classesList;
        public List<IClass>? ClassesList 
        { 
            get
            {
                if (_classesList is null || _classesList.Count == 0)
                {
                    FillClasses();
                }

                return _classesList;
            }
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

        private void FillClasses()
        {
            _syntaxTreeReader.ExtractClasses(_syntaxTreeArray, out _classesList); 
        }

        public void PrintClassListInfo()
        {
            foreach (CSharpClass csharpClass in ClassesList!) 
            {
                string classPath = csharpClass.NamespaceName + "." + csharpClass.Name;
                Console.WriteLine($"Class {classPath} has {csharpClass.Methods.Length} methods, {csharpClass.Fields.Length} fields and {csharpClass.Properties.Length} properties");
                Console.ForegroundColor = ConsoleColor.Green;
                if (csharpClass?.Methods.Count() != 0)
                {
                    Console.WriteLine("Methods:");
                    Array.ForEach(csharpClass!.Methods, (method) => Console.WriteLine($"Method {(method as CSharpMethod)!.Name} has {(method as CSharpMethod)!.Parameters.Length} params and its' return type is {(method as CSharpMethod)!.ReturnType}"));
                }
                if (csharpClass?.Fields.Count() != 0)
                {
                    Console.WriteLine("Fields:");
                    Array.ForEach(csharpClass!.Fields, (field) => Console.WriteLine($"Field {(field as CSharpField)!.Name} is of {(field as CSharpField)!.TypeName} type"));
                }

                if (csharpClass?.Properties.Count() != 0)
                {
                    Console.WriteLine("Properties:");
                    Array.ForEach(csharpClass!.Properties, (property) => Console.WriteLine($"Property {(property as CSharpProperty)!.Name} is of {(property as CSharpProperty)!.TypeName} type"));
                }
                Console.WriteLine("\n\n\n");
                Console.ResetColor();
            }
        }
    }
}
