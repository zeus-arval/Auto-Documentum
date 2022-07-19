// See https://aka.ms/new-console-template for more information
using AD.Soft.FilesManager;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");
LoggerFactory factory = new LoggerFactory();


FileReader reader = new FileReader(LoggerFactoryExtensions.CreateLogger<FileReader>(factory);

string filePath = "";
reader.ReadFile(filePath);