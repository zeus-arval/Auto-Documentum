using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AD.FilesManager.CSharp.Extensions
{
    internal static class SyntaxNodeHelper
    {
        internal static bool TryGetParentSyntax<T>(SyntaxNode? syntaxNode, out T? result)
        where T : SyntaxNode
        {
            // set defaults
            result = null;

            if (syntaxNode == null)
            {
                return false;
            }

            try
            {
                syntaxNode = syntaxNode.Parent;

                if (syntaxNode == null)
                {
                    return false;
                }

                if (syntaxNode.GetType() == typeof(T))
                {
                    result = syntaxNode as T;
                    return true;
                }

                return TryGetParentSyntax<T>(syntaxNode, out result);
            }
            catch
            {
                return false;
            }
        }

        internal static bool TryGetFullClassPath(ClassDeclarationSyntax? classSyntax, out string? name)
        {
            name = null;
            SyntaxNode? node = classSyntax;

            do
            {
                try
                {
                    node = FillParentName(node, out string? parentName);
                    name = $"{parentName}.{name}";
                } 
                catch (Exception)
                {
                    name = string.Empty;
                    return false;
                }

            } while (node != null);

            return true;
        }

        internal static SyntaxNode? FillParentName(SyntaxNode? syntaxNode, out string? parentName)
        {
            var parent = syntaxNode?.Parent;
            if (parent is null || parent is CompilationUnitSyntax)
            {
                parentName = string.Empty;
                return null;
            }

            if (parent is NamespaceDeclarationSyntax namespaceSyntax)
            {
                parentName = CollectNameFromQualifiedNameSyntax((QualifiedNameSyntax)namespaceSyntax.Name);
            }
            else if (parent is ClassDeclarationSyntax parentClassSyntax)
            {
                parentName = parentClassSyntax.Identifier.Text;
            }
            else
            {
                throw new Exception("Parent has invalid type");
            }

            return parent;
        }

        internal static string CollectNameFromQualifiedNameSyntax(QualifiedNameSyntax? qualifiedNameSyntax)
        {
            if (qualifiedNameSyntax is null)
            {
                return string.Empty;
            }

            string? left = GetLeft(qualifiedNameSyntax.Left);
            string? right = ((IdentifierNameSyntax)qualifiedNameSyntax.Right).Identifier.Text;

            return $"{left}.{right}";
        }

        internal static string GetLeft(NameSyntax left)
        {
            if (left is QualifiedNameSyntax qualifiedLeft)
            {
                return CollectNameFromQualifiedNameSyntax(qualifiedLeft);
            }

            return ((IdentifierNameSyntax) left).Identifier.Text;
        }


        internal static bool TryGetClassDeclarationSyntaxis(SyntaxTree syntaxTree, out List<ClassDeclarationSyntax> classSyntaxis)
        {
            SyntaxNode? root = syntaxTree.GetRoot();
            classSyntaxis = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            return classSyntaxis.Any();
        }

        internal static bool TryGetClassName(ClassDeclarationSyntax? classSyntax, out string name)
        {
            name = string.Empty;

            if (classSyntax is null)
            {
                return false;
            }

            name = classSyntax.Identifier.Text;
            return true;
        }

        internal static bool TryGetNameSpaceSyntaxis(SyntaxTree syntaxTree, out List<NamespaceDeclarationSyntax> namespaces)
        {
            namespaces = new List<NamespaceDeclarationSyntax>();
            var root = syntaxTree.GetCompilationUnitRoot();

            foreach(MemberDeclarationSyntax? member in root.Members)
            {
                if (member is NamespaceDeclarationSyntax @namespace)
                {
                    namespaces.Add(@namespace);
                }
            }

            return namespaces.Count > 0;
        }

        internal static CSharpField[] GetCSharpFieldArray(ClassDeclarationSyntax classSyntax)
        {
            //TODO check if all kind of properties could be used
            var fieldSyntaxis = classSyntax.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();

            if (fieldSyntaxis.Count() == 0)
            {
                return Array.Empty<CSharpField>();
            }

            CSharpField[] fieldArray = new CSharpField[fieldSyntaxis.Count()];

            for (int i = 0; i < fieldSyntaxis.Count; i++)
            {
                string? fieldType = null;

                VariableDeclarationSyntax? fieldDeclaration = fieldSyntaxis[i].Declaration;

                if (fieldDeclaration.Type is ArrayTypeSyntax arraySyntax)
                {
                    fieldType = (arraySyntax.ElementType as PredefinedTypeSyntax)?.Keyword.Text;
                }
                else if (fieldDeclaration.Type is PredefinedTypeSyntax predefinedSyntax)
                {
                    fieldType = predefinedSyntax.Keyword.Text;
                }

                var variables = fieldDeclaration.Variables;
                fieldArray = new CSharpField[variables.Count];

                for (int variableNum = 0; variableNum < variables.Count; variableNum++)
                {
                    string? fieldName = variables[variableNum].Identifier.Text; 
                
                    if (fieldType is null || fieldName is null)
                    {
                        fieldArray = Array.Empty<CSharpField>();
                        return fieldArray;
                    }

                    //TODO Description of field needs to be taken as well
                    fieldArray[variableNum] = new CSharpField(fieldType, fieldName, string.Empty);
                }
            }
            return fieldArray;
        }

        internal static CSharpProperty[] GetCSharpPropertyArray(ClassDeclarationSyntax classSyntax)
        {
            //TODO check if all kind of properties could be used
            List<PropertyDeclarationSyntax> propertySyntaxis = classSyntax.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            if (propertySyntaxis.Count() == 0)
            {
                return Array.Empty<CSharpProperty>();
            }

            CSharpProperty[] propertyArray = new CSharpProperty[propertySyntaxis.Count()];

            for(int i = 0; i < propertySyntaxis.Count(); i++)
            {
                PropertyDeclarationSyntax? propertyDeclaration = propertySyntaxis[i];
                string? propertyType = null;
                string? propertyName = propertyDeclaration.Identifier.Text;

                if (propertyDeclaration.Type is PredefinedTypeSyntax typeSyntax)
                {
                    propertyType = typeSyntax.Keyword.Text;
                }
                else if (propertyDeclaration.Type is ArrayTypeSyntax arrayTypeSyntax && arrayTypeSyntax.ElementType is PredefinedTypeSyntax predefinedSyntax)
                {
                    propertyType = $"{predefinedSyntax.Keyword.Text}[]";
                }
                else if (propertyDeclaration.Type is IdentifierNameSyntax identifierNameSyntax)
                {
                    propertyType = identifierNameSyntax.Identifier.Text;
                }

                if (propertyType is null || propertyName is null)
                {
                    continue;
                }

                //TODO Description of field needs to be taken as well
                propertyArray[i] = new CSharpProperty(propertyType, propertyName, string.Empty);
            }

            return propertyArray;
        }
    }
}
