using CodeGeneratorHelpers.Core;
using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis;


var generator = CodeGenerator.Create("CodeGeneratorHelpers.Core");


await generator.ExecuteOnEachFileAsync("models", filePattern: "*Data.cs", execution: PrintModelsAsync);


async Task PrintModelsAsync(CodeMetadata metadata)
{
    string className = metadata?.Classes?.FirstOrDefault()?.Name ?? string.Empty;
    string interfaceName = metadata?.Interfaces?.FirstOrDefault()?.Name ?? string.Empty;
    string enumName = metadata?.Enums?.FirstOrDefault()?.Name ?? string.Empty;

    if (className.Length + interfaceName.Length > 0)
    {
        string text = @$"
    Model name: {className}
    Interface: {interfaceName}
    Enum: {enumName}";

        var name = new FileInfo(metadata.SourceFilePath).Name;

        await generator.WriteAllTextToFileAsync($"{name}.txt", text);
    }

}

