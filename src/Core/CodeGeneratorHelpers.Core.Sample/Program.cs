using CodeGeneratorHelpers.Core;
using CodeGeneratorHelpers.Core.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using System.Collections.Immutable;


var generator = CodeGenerator.Create("CodeGeneratorHelpers.Core", 
                                     clearGenerationDestinationPath: true,
                                     generationDestinationPath: "Generated");


//await generator.ExecuteOnEachFileAsync("models",
//                                       filePattern: "*metadata.cs",
//                                       execution: PrintModelsAsync);


await generator.ExecuteOnEachFileAsync(execution: meta => generator.WriteAllTextToFileAsync($"{meta.SourceFilePath}.txt", $"Found {meta.Classes?.Count() ?? 0} classes"));


await generator.ExecuteOnEachFileAsync(ExecuteOnEachAsync);



var batches = generator.GetAllFilesMetadataInBatchesAsync();
var allInterfaceNames = new ConcurrentBag<string>();
await foreach (var batch in batches)
{
    // Get interface names of all files in batch
    var names = batch.SelectMany(b => b.Interfaces)
                     .Select(b => b.Name)
                     .ToImmutableArray();

    // add all names to a concurrent bag
    // this way you ensure only the relevant data stays in memory
    foreach (var name in names)
        allInterfaceNames.Add(name);
}


var files = generator.GetAllFilesMetadataAsAsyncEnumerable();
await foreach (var file in files)
{
    var names = file.Interfaces
                    .Select(b => b.Name)
                    .ToImmutableArray();

    foreach (var name in names)
        allInterfaceNames.Add(name);
}

var files2 = await generator.GetAllFilesMetadataAsync();

var names2 = files2.SelectMany(e => e.Enums)
                    .Select(b => b.Name)
                    .ToImmutableArray();



async Task ExecuteOnEachAsync(CodeMetadata meta)
{
    if (meta.SourceFilePath.EndsWith("Model.cs"))
    {
        var property = meta.Classes.SelectMany(c => c.Properties).First();

        var code = $@"
public class Extended{meta.Classes.First().Name}
{{
    public {property.Type.Name} {property.Name} {{ get; set; }}
}}";

        await generator.WriteAllTextToFileAsync($"Extended{meta.SourceFilePath}.cs", code);

    }
}

CodeMetadata metadata = await generator.GetFileMetadataAsync("MyFile.cs");

metadata.Classes.SelectMany(c => c.Properties);

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

        var name = new FileInfo(metadata.SourceFilePath).Name[..^3];

        await generator.WriteAllTextToFileAsync($"{name}.txt", text);
    }

}

