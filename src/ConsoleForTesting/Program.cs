// See https://aka.ms/new-console-template for more information
using AD.FilesManager;
using AD.Factories;

var mainFactory = new MainFactory();

FileReader reader = new FileReader(mainFactory.CreateLogger<FileReader>());

string filePath = @"C:\Users\a.valdna\source\repos\LearningTasks\Attributes\Program.cs";
reader.ReadFile(filePath, (line) => {
    Console.WriteLine(line);
});

FilesController controller = new FilesController(mainFactory.CreateLogger<FilesController>());

string filePattern = "*.cs";

controller.ReturnFilePathArray(@"C:\Users\a.valdna\source\repos\Bank\Bank", filePattern);