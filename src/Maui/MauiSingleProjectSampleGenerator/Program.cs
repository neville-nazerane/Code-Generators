using CodeGeneratorHelpers.Maui;



await new MauiCodeGenerationBuilder()
                   .SetTargetAppPath("MauiSingleProjectSample")
                   
                   .GenerateAsync();