using AmcrestMQTT;
using AmcrestMQTT.Models;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<MQTTWorker>();

builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SetCameras();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


void SetCameras()
{
    Camera.Set(app.Services.GetRequiredService<IOptions<Settings>>().Value);

}
