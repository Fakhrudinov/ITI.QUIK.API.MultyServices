using ChildHttpApiRepository;
using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add child API connections adress
builder.Services.Configure<HttpConfigurations>(
    builder.Configuration.GetSection("HttpConfigurations"));

// add child APIs repository
builder.Services.AddTransient<IHttpApiRepository, HttpApiRepository>();

// add string for pubring.txk key cleaning
builder.Services.Configure<PubringKeyIgnoreWords>(
    builder.Configuration.GetSection("PubringKeyIgnoreWords"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
