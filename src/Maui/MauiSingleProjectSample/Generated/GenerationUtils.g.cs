using MauiSingleProjectSample;
using MauiSingleProjectSample.Pages;
using MauiSingleProjectSample.ViewModels;

namespace SampleNamespace;

public static class GenerationUtils
{
    
    public static IServiceCollection AddGeneratedInjections(this IServiceCollection services)
        => services.AddTransient<MainPage>()
                   .AddTransient<FirstPage>()
                   .AddTransient<SecondPage>()
                   .AddTransient<ThirdPage>()
                   .AddTransient<ThirdViewModel>()
                   .AddTransient<SecondViewModel>()
                   .AddTransient<FirstViewModel>();

}