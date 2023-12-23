using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NixpkgsShields;
using HostBuilder = Microsoft.Extensions.Hosting.HostBuilder;

await new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(
        (context, builder) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                builder.AddEnvironmentVariables();
                builder.AddJsonFile("local.settings.json", true);
            }
        })
    .ConfigureServices(
        (context, services) =>
        {
            services.AddSingleton<IGitHubClient, GitHubClient>();
            services.AddSingleton<BadgeService>();

            services.AddHttpClient<IGitHubClient, GitHubClient>(
                client =>
                {
                    client.BaseAddress = new Uri("https://api.github.com");
                    client.DefaultRequestHeaders.Add("User-Agent", "NixpkgsShields");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", context.Configuration["GitHub:Token"]);
                });
        })
    .Build()
    .RunAsync();
