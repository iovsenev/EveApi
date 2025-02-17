using Eve.Infrastructure;
using Eve.Application;
using Eve.Api.Minddlewares;
using Eve.Api;
using Eve.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddConsole()
           .AddFilter("Microsoft", LogLevel.Warning)
           .AddFilter("System", LogLevel.Warning);
});

builder.Services.ConfigureIdentity(configuration);
builder.Services.AddScoped<AdminAdder>();

//builder.Services.AddHostedService<LoadOrdersBackgroundService>();
//builder.Services.AddHostedService<AppTokenAcessBackgroundService>();

builder.Services.AddCors( opt =>
{
    opt.AddPolicy("frontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowCredentials()
        .AllowAnyMethod();
    });
});


builder.Services.AddInfrastructure(configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using var scope = app.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<AdminAdder>();
await service.CreateAdmin();


//app.UseHttpsRedirection();

app.UseCors("frontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ClientIdMiddleware>();

app.MapControllers();

app.Run();