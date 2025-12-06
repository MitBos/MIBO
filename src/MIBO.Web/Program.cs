// Summary: Bootstraps the Blazor Server app, wiring services, static assets, middleware, routes, and API endpoints.
using System.Reflection;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MIBO.Web.Components;
using MIBO.Web.Endpoints;
using MIBO.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// When running the compiled exe directly, the SDK doesn't set ASPNETCORE_STATIC_WEB_ASSETS,
// which is required to serve _framework and _content files. Ensure it's set manually if missing.
var staticAssetsManifest = Path.Combine(
    AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.staticwebassets.runtime.json");
if (Environment.GetEnvironmentVariable("ASPNETCORE_STATIC_WEB_ASSETS") is null &&
    File.Exists(staticAssetsManifest))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_STATIC_WEB_ASSETS", staticAssetsManifest);
}

builder.WebHost.UseStaticWebAssets();

// Serve the framework bootstrap script even when running the apphost directly (e.g., double-clicking the exe).
var componentsVersion = typeof(Microsoft.AspNetCore.Components.ComponentBase).Assembly.GetName().Version;
var frameworkAssetsVersion = $"{componentsVersion?.Major}.{componentsVersion?.Minor}.{componentsVersion?.Build}";
var frameworkAssetsPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    ".nuget",
    "packages",
    "microsoft.aspnetcore.app.internal.assets",
    frameworkAssetsVersion,
    "_framework");

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMiboWebApp();
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
if (Directory.Exists(frameworkAssetsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frameworkAssetsPath),
        RequestPath = "/_framework",
        ContentTypeProvider = new FileExtensionContentTypeProvider()
    });
}
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapIntegrationApiEndpoints();
app.MapApiRegistryEndpoints();
app.MapEndpointRelationApiEndpoints();

app.Run();
