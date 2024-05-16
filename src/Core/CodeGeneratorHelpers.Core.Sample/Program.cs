using CodeGeneratorHelpers.Core;
using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis;


var generator = CodeGenerator.Create("CodeGeneratorHelpers.Core");


await generator.ExecuteOnEachFileAsync("models", filePattern: "*", execution: PrintModelsAsync);


async Task PrintModelsAsync(CodeMetadata metadata)
{
    string className = metadata?.Classes?.FirstOrDefault()?.ClassName ?? string.Empty;
    string interfaceName = metadata?.Interfaces?.FirstOrDefault()?.InterfaceName ?? string.Empty;
    string enumName = metadata?.Enums?.FirstOrDefault()?.EnumName ?? string.Empty;

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

