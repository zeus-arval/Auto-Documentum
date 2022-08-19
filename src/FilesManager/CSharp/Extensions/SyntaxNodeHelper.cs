using AD.FilesManager.CSharp.FileContentElements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AD.FilesManager.CSharp.CSharpFormats;

namespace AD.FilesManager.CSharp.Extensions
{
    internal static class SyntaxNodeHelper
    {
        /// <summary>
        /// Tries to get parent of <paramref name="syntaxNode"/> and to put it to <paramref name="result"/>
        /// </summary>
        /// <typeparam name="T">Every <see cref="SyntaxNode"/></typeparam>
        /// <returns>True, if parent was found and was put to <paramref name="result"/></returns>
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

        /// <summary>
        /// Tries to gets full path of class
        /// </summary>
        /// <returns>True, if class path was found and put to <paramref name="name"/></returns>
        internal static bool TryGetFullClassPath(ClassDeclarationSyntax? classSyntax, out string? name)
        {
            name = null;
            SyntaxNode? node = classSyntax;

            do
            {
                try
                {
                    node = ReturnParent(node, out string? parentName);

                    if (parentName == string.Empty || name is null)
                    {
                        string nameText = parentName! == string.Empty ? name! : parentName!;
                        name = $"{nameText}";
                    }
                    else
                    {
                        name = $"{parentName}.{name}";

                    }
                } 
                catch (Exception)
                {
                    name = string.Empty;
                    return false;
                }

            } while (node != null);

            return true;
        }

        /// <summary>
        /// Returns parent and fills <paramref name="parentName"/>
        /// </summary>
        /// <returns>Parent of <paramref name="syntaxNode"/></returns>
        /// <exception cref="Exception">If parent is not namespace or class exception is thrown</exception>
        internal static SyntaxNode? ReturnParent(SyntaxNode? syntaxNode, out string? parentName)
        {
            var parent = syntaxNode?.Parent;
            parentName = string.Empty;

            if (parent is null || parent is CompilationUnitSyntax)
            {
                return null;
            }

            if (parent is NamespaceDeclarationSyntax namespaceSyntax)
            {
                if (namespaceSyntax.Name is IdentifierNameSyntax identifierName)
                {
                    parentName = identifierName.Identifier.Text;
                }
                else if (namespaceSyntax.Name is QualifiedNameSyntax qualifiedName)
                {
                    parentName = CollectNameFromQualifiedNameSyntax(qualifiedName);
                }
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

        /// <summary>
        /// recursion for returning full namespace name
        /// </summary>
        /// <returns>Full path to class</returns>
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

        /// <summary>
        /// Tries to get List<<see cref="ClassDeclarationSyntax"/>>
        /// </summary>
        /// <returns>True, if any <see cref="ClassDeclarationSyntax"/> was found, else false</returns>
        internal static bool TryGetClassDeclarationSyntaxis(SyntaxTree syntaxTree, out List<ClassDeclarationSyntax> classSyntaxis)
        {
            SyntaxNode? root = syntaxTree.GetRoot();
            classSyntaxis = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            return classSyntaxis.Any();
        }

        /// <summary>
        /// Tries to get the name of class
        /// </summary>
        /// <returns>True, if <see cref="ClassDeclarationSyntax?"/> contains a name</returns>
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

        /// <summary>
        /// Tries to get <see cref="NamespaceDeclarationSyntax"/>
        /// </summary>
        /// <returns>True, if <see cref="SyntaxTree"/> contains any namespace</returns>
        internal static bool TryGetNamespaceSyntaxis(SyntaxTree syntaxTree, out List<NamespaceDeclarationSyntax> namespaces)
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

        /// <summary>
        /// Gets an Array of <see cref="CSharpField"/>
        /// </summary>
        /// <returns><see cref="CSharpField[]"/></returns>
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

        /// <summary>
        /// Gets an Array of <see cref="CSharpProperty"/>
        /// </summary>
        /// <returns><see cref="CSharpProperty[]"/></returns>
        internal static CSharpProperty[] GetCSharpPropertyArray(ClassDeclarationSyntax classSyntax)
        {
            //TODO check if all kind of properties could be used
            List<PropertyDeclarationSyntax> propertySyntaxis = classSyntax.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            if (propertySyntaxis.Count == 0)
            {
                return Array.Empty<CSharpProperty>();
            }

            CSharpProperty[] propertyArray = new CSharpProperty[propertySyntaxis.Count];

            for(int i = 0; i < propertySyntaxis.Count; i++)
            {
                PropertyDeclarationSyntax? propertyDeclaration = propertySyntaxis[i];
                string? propertyType = null;
                string? propertyName = propertyDeclaration.Identifier.Text;
                var type = propertyDeclaration.Type;

                propertyType = ReturnTypeName(type);

                //TODO Add recursion for nullable and other types
                if (propertyType is null || propertyName is null)
                {
                    continue;
                }

                //TODO Description of field needs to be taken as well
                propertyArray[i] = new CSharpProperty(propertyType, propertyName, string.Empty);
            }

            return propertyArray;
        }

        /// <summary>
        /// A recursion for getting a full tye name
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Type name like: List<string[]></returns>
        private static string ReturnTypeName(TypeSyntax type)
        {
            if (type is null)
            {
                return string.Empty;
            }

            if (type is NullableTypeSyntax nullableType)
            {
                return string.Format(NULLABLE_OPERATOR, ReturnTypeName(nullableType.ElementType));
            }
            else if (type is ArrayTypeSyntax arrayType)
            {
                return string.Format(ARRAY_OPERATOR, ReturnTypeName(arrayType.ElementType));
            }
            else if (type is PredefinedTypeSyntax predefinedType)
            {
                return predefinedType.Keyword.Text;
            }
            else if (type is IdentifierNameSyntax identifierName)
            {
                return identifierName.Identifier.Text;
            }
            else if (type is GenericNameSyntax genericName)
            {
                string name = genericName.Identifier.Text;
                List<string> argumentNameList = new List<string>();

                foreach (var argument in genericName.TypeArgumentList.Arguments)
                {
                    argumentNameList.Add(ReturnTypeName(argument));
                }

                return string.Format(GENERIC_OPERATOR, name, String.Join(',', argumentNameList));
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets an Array of <see cref="CSharpMethod"/>
        /// </summary>
        /// <returns><see cref="CSharpMethod[]"/></returns>
        internal static CSharpMethod[] GetCSharpMethodArray(ClassDeclarationSyntax classSyntax)
        {
            List<MethodDeclarationSyntax> methodSyntaxis = classSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            if (methodSyntaxis.Count == 0)
            {
                return Array.Empty<CSharpMethod>();
            }

            CSharpMethod[] methodsArray = new CSharpMethod[methodSyntaxis.Count];

            for(int i = 0; i < methodSyntaxis.Count; i++)
            {
                MethodDeclarationSyntax? methodSyntax = methodSyntaxis[i];
                string? methodName = methodSyntax.Identifier.Text;
                CSharpParameter[] parameters = GetCSharpParameters(methodSyntax);
                string? returnType = (methodSyntax.ReturnType as PredefinedTypeSyntax)?.Keyword.Text;
                if (methodName is null && returnType is null)
                {
                    continue;
                }

                methodsArray[i] = new CSharpMethod(parameters, methodName!, returnType!);
            }
            return methodsArray;

        }

        /// <summary>
        /// Gets an Array of <see cref="CSharpParameter"/>
        /// </summary>
        /// <returns><see cref="CSharpParameter[]"/></returns>
        internal static CSharpParameter[] GetCSharpParameters(MethodDeclarationSyntax method)
        {
            var parameterSyntaxis = method.ParameterList.Parameters;

            if (parameterSyntaxis.Count == 0)
            {
                return Array.Empty<CSharpParameter>();
            }

            CSharpParameter[] parameters = new CSharpParameter[parameterSyntaxis.Count];

            for (int i = 0; i < parameterSyntaxis.Count; i++)
            {
                string parameterName = String.Empty;
                string parameterType;
                
                var parameter = parameterSyntaxis[i];
                parameterType = parameter.Identifier.Text;

                if (parameter.Type is PredefinedTypeSyntax predefinedSyntax)
                {
                    parameterName = predefinedSyntax.Keyword.Text;
                }
                else if (parameter.Type is IdentifierNameSyntax identifierSyntax)
                {
                    parameterName = identifierSyntax.Identifier.Text;
                }

                parameters[i] = new CSharpParameter(parameterName, parameterType, string.Empty);
            }
            return parameters;
        }
    }
}
