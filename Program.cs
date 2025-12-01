using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeApp;
using Blazored.LocalStorage;
using RecipeApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// LocalStorage（データ移行用に一時的に残す）
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<RecipeService>();

// Supabase サービス追加（この行を追加！）
builder.Services.AddScoped<SupabaseRecipeService>();

await builder.Build().RunAsync();