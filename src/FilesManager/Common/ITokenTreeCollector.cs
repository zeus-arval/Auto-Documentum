using AD.FilesManager.CSharp;

namespace AD.FilesManager.Common
{
    public interface ITokenTreeCollector
    {
        List<IClass>? ClassesList { get; }
    }
}