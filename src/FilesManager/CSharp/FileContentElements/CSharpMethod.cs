using AD.FilesManager.Common;

namespace AD.FilesManager.CSharp.FileContentElements
{
    public class CSharpMethod : IMethod
    {
        public CSharpParameter[] Parameters { get; init; }
        public string Name { get; init; }
        public string ReturnType { get; init; }
        //public string AccessModifiers { get; init; } Modifiers need to be added as classes
        public string Description { get; init; }

        public CSharpMethod(CSharpParameter[] parameters, string name, string returnType, string? description = null)
        {
            Parameters = parameters;
            Name = name;
            ReturnType = returnType;
            Description = description ?? string.Empty;
        }
    }
}
