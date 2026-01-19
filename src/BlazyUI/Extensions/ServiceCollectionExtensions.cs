using Microsoft.Extensions.DependencyInjection;
using TailwindMerge.Extensions;

namespace BlazyUI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazyUI(this IServiceCollection services)
    {
        services.AddTailwindMerge();
        services.AddScoped<IModalService, ModalService>();
        services.AddScoped<IToastService, ToastService>();
        return services;
    }
}
