using AD.Aids.Factories;
using AD.FilesManager.CSharp;

namespace ConsoleForTesting
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string filePath = @"C:\Users\a.valdna\source\repos\TestClassReading\Test";
            MainFactory factory = new MainFactory();
            CSharpDiagramElementsBuilder collector = new CSharpDiagramElementsBuilder(factory, filePath);
            collector.PrintClassListInfo();
        }
    }
}
