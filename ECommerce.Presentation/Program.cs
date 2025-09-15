using ECommerce.Presentation.Handlers;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Presentation.Interfaces.Api;
using ECommerce.Presentation.Interfaces.Auth;
using ECommerce.Presentation.Interfaces.UI;
using ECommerce.Presentation.Services;
using ECommerce.Presentation.UI;
using ECommerce.Presentation.UI.Operations.Addresses;
using ECommerce.Presentation.UI.Operations.Auth;
using ECommerce.Presentation.UI.Operations.Categories;
using ECommerce.Presentation.UI.Operations.Products;
using ECommerce.Presentation.UI.Operations.Sales;
using ECommerce.Presentation.UI.Operations.Users;
using Microsoft.Extensions.Hosting;
using Serilog;


LoggingSetup();

try
{
    await Startup();
}
catch (Exception ex)
{
    Log.Fatal($"Application terminated unexpectedly: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}




async Task Startup()
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureLogging((_, log) => log.AddSerilog(dispose: true))
        .ConfigureServices((ctx, services) =>
        {
            var baseUrl = ctx.Configuration["ApiSettings:BaseUrl"];

            services.AddScoped<IAddressApiService, AddressApiService>();
            services.AddScoped<ICategoriesApiService, CategoriesApiService>();
            services.AddScoped<ILoginApiService, LoginApiService>();
            services.AddScoped<IProductsApiService, ProductsApiService>();
            services.AddScoped<ISalesApiService, SalesApiService>();
            services.AddScoped<IUserApiService, UserApiService>();
            services.AddSingleton<IAccessUI, AccessUI>();
            services.AddSingleton<IAddressUI, AddressUI>();
            services.AddSingleton<IApp, App>();
            services.AddSingleton<ICategoriesUI, CategoriesUI>();
            services.AddSingleton<IProductsUI, ProductsUI>();
            services.AddSingleton<ISalesUI, SalesUI>();
            services.AddSingleton<IUsersUI, UsersUI>();
            services.AddSingleton<IAuthTokenHolder, AuthTokenHolder>();
            services.AddTransient<AuthHeaderHandler>();

            services.AddHttpClient("Client", client =>
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept
                        .Add(new("application/json"));
                })
                .AddHttpMessageHandler<AuthHeaderHandler>();
            
        })
        .Build();

    var app = host.Services.GetRequiredService<IApp>();
    await app.RunAsync();

}
void LoggingSetup()
{
    var loggingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    Directory.CreateDirectory(loggingDirectory);
    var filePath = Path.Combine(loggingDirectory, "app-.txt");
    var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
            path: filePath,
            rollingInterval: RollingInterval.Day,
            outputTemplate: outputTemplate)
        .CreateLogger();
}