using AD.FilesManager.CSharp;

namespace AD.FilesManager.Common
{
    public interface IDiagramElementsBuilder
    {
        List<IClass>? ClassesList { get; }
    }
}