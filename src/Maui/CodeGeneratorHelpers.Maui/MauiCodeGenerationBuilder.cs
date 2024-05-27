using CodeGeneratorHelpers.Core;
using CodeGeneratorHelpers.Core.Models;
using CodeGeneratorHelpers.Maui.InternalUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Maui
{
    public partial class MauiCodeGenerationBuilder
    {

        private string _targetAppPath;
        private string _generationFolder = "Generated";
        private Regex _pageRegex = DefaultPageRegex();
        private Regex _viewModelRegex = DefaultViewModelRegex();

        #region Regex

        [GeneratedRegex(@"(.*)(Page|View)")]
        private static partial Regex DefaultPageRegex();

        [GeneratedRegex(@"(.*)(PageModel|ViewModel)")]
        private static partial Regex DefaultViewModelRegex();

        #endregion

        public MauiCodeGenerationBuilder SetTargetAppPath(string path)
        {
            _targetAppPath = path;
            return this;
        }

        public MauiCodeGenerationBuilder SetGenerationFolder(string path)
        {
            _generationFolder = path;
            return this;
        }


        public async Task GenerateAsync()
        {
            var generator = CodeGenerator.Create(_targetAppPath, _generationFolder, true);

            var viewModels = new Dictionary<string, ClassMetadata>();
            var pages = new Dictionary<string, ClassMetadata>();

            // Fetch pages and viewmodel metadata
            await foreach (var code in generator.GetFilesMetaInBatchesAsync())
            {
                var classes = code.SelectMany(code => code.Classes);

                foreach (var cls in classes)
                {
                    var viewModelMatch = _viewModelRegex.Match(cls.Name);
                    if (viewModelMatch.Success)
                    {
                        var pageName = viewModelMatch.Groups[1].Value;
                        viewModels[pageName] = cls;
                    }
                    else
                    {
                        var pageMatch = _pageRegex.Match(cls.Name);
                        if (pageMatch.Success)
                        {
                            var pageName = pageMatch.Groups[1].Value;
                            pages[pageName] = cls;
                        }
                    }

                }

            }

            var utilClassCode = GenerateUtilClass(pages.Values, viewModels.Values, "SampleNamespace");

            await generator.WriteAllTextToFileAsync("GenerationUtils.g.cs", utilClassCode);

            var chunks = pages.Chunk(10);

            foreach (var chunk in chunks)
            {
                var tasks = chunk.Select(async page =>
                {
                    if (viewModels.TryGetValue(page.Key, out var viewModelCode))
                        await GenerateForPageAsync(generator, page.Key, page.Value, viewModelCode);
                });

                await Task.WhenAll(tasks);
            }

        }

        private async Task GenerateForPageAsync(CodeGenerator generator,
                                               string name,
                                               ClassMetadata page,
                                               ClassMetadata viewModel)
        {

            var code = $@"

namespace {page.Namespace};

public partial class {page.Name} 
{{


// HELLO WORLD



}}

";

            await generator.WriteAllTextToFileAsync($"{page.Name}.g.cs", code);

        }


    }
}
