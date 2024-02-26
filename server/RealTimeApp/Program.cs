using System.Reactive.Concurrency;

var builder = WebApplication.CreateBuilder(args);
const string corsPolicy = "CorsPolity";
// Add services to the container.
builder.Services.AddCors(options =>
    {
        options.AddPolicy(corsPolicy, _ =>
            _.WithOrigins("http://localhost:3000") // Allow only the React app
                   // .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()); // Important for SignalR
    });
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IPriceTicker, PriceTicker>();
builder.Services.AddSingleton<StockTickerHub>();
var scheduler = DefaultScheduler.Instance;
builder.Services.AddSingleton<IScheduler>(scheduler);
builder.Logging.AddConsole();


var app
 = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseCors(corsPolicy);
app.MapRazorPages(); // Register Razor Pages routes directly on app builder
app.MapHub<StockTickerHub>("/stockTickerHub"); // Register SignalR Hub directly on app builder

app.Run();

