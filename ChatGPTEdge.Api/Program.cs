using ChatGPTEdge.Api;
using ChatGPTEdge.Api.Managers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

_ = builder.Logging.ClearProviders();
_ = builder.Logging.AddConsole();

// Configuration Bindings
_ = builder.Services.AddOptions<VisionOptions>().BindConfiguration(VisionOptions.Vision);

// Service Bindings
_ = builder.Services.AddTransient<IDenseCaptionManager, DenseCaptionManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
