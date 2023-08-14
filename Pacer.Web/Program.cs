using Pacer.Web;
using Pacer.Data.Services;
using Pacer.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication / Authorisation via extension methods 
builder.Services.AddCookieAuthentication();
builder.Services.AddPolicyAuthorisation();

// Connection to Heroku MySql database
var herokuConnectionString = builder.Configuration.GetConnectionString("HerokuMySqlConnection");
var uri = new Uri(herokuConnectionString);
var userInfo = uri.UserInfo.Split(':');
var user = userInfo[0];
var password = userInfo[1];
var host = uri.Host;
var database = uri.AbsolutePath.Trim('/');
var standardConnectionString = $"Server={host};Database={database};User ID={user};Password={password};";

// Configure Database Context
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(standardConnectionString, ServerVersion.AutoDetect(standardConnectionString)));



// Add Services to DI   
builder.Services.AddTransient<IUserService,UserServiceDb>();
builder.Services.AddTransient<IMailService,SmtpMailService>();
builder.Services.AddTransient<ITrainingPlanService, TrainingPlanServiceDb>();
builder.Services.AddTransient<IRunningProfileService, RunningProfileServiceDb>();
builder.Services.AddTransient<IWorkoutFactory, WorkoutFactory>();
builder.Services.AddTransient<IRaceTimePredictor, RaceTimePredictor>();
builder.Services.AddTransient<IWorkoutPaceCalculator, WorkoutPaceCalculator>();
builder.Services.AddSingleton<IWeatherService, WeatherService>();

builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApi"));


builder.Services.AddHttpClient();

// ** Required to enable asp-authorize Taghelper **            
builder.Services.AddHttpContextAccessor(); 

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var bindAddress = port == "5000" ? "localhost" : "*";
app.Urls.Add($"http://{bindAddress}:{port}");


var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application is starting up...");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else 
{
    // seed users in development mode - using service provider to get UserService from DI
    // using var scope = app.Services.CreateScope();
    // Seeder.Seed(scope.ServiceProvider.GetService<IUserService>(), scope.ServiceProvider.GetService<IRunningProfileService>(), scope.ServiceProvider.GetService<ITrainingPlanService>());
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ** configure cors to allow full cross origin access to any webapi end points **
app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// ** turn on authentication/authorisation **
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

