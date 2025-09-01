using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TicTacToe.Client;
using TicTacToe.Client.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiUrl = builder.Configuration.GetConnectionString("ApiConnection")
   //?? "http://localhost:6543/";
   ?? "https://localhost:7654/";

Console.WriteLine($"API URL=!!!{apiUrl}!!!!");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<UserDataUpdateService>();
builder.Services.AddScoped<RefreshDataService>();
builder.Services.AddScoped<GameHubService>(provider => new GameHubService(apiUrl));

builder.Services.AddHttpClient("HttpClient", client =>
{
    client.BaseAddress = new Uri(apiUrl);
}).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("HttpClient"));

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
