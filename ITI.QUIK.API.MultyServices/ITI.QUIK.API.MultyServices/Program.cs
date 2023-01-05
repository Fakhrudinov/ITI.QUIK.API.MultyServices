using ChildHttpMatrixRepository;
using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.Discounts;
using LogicCore;
using MailService;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
    c.SwaggerDoc(
        "v1", 
        new OpenApiInfo 
        { 
            Title = builder.Configuration.GetSection("SMTPMailConfigurations:ServerType").Value + " Quik API", 
            Version = "v1" 
        }));

// add child API connections adress
builder.Services.Configure<HttpConfigurations>(
    builder.Configuration.GetSection("HttpConfigurations"));

// add child APIs repository
builder.Services.AddTransient<IHttpMatrixRepository, HttpMatrixRepository>();
builder.Services.AddTransient<IHttpQuikRepository, HttpQuikRepository>();
builder.Services.AddTransient<HttpApiExecutiveRepository>();

// add core level
builder.Services.AddTransient<ICore, Core>();
builder.Services.AddTransient<ICoreKval, CoreKval>();
builder.Services.AddTransient<ICoreSingleServices, CoreSingleServices>();
builder.Services.AddTransient<ICoreDiscounts, CoreDiscounts>();
builder.Services.Configure<DiscountsSettings>(
    builder.Configuration.GetSection("DiscountsSettings"));

// add Email sender
builder.Services.AddTransient<IEMail, EMail>();
builder.Services.Configure<SMTPMailConfig>(
    builder.Configuration.GetSection("SMTPMailConfigurations"));

// add string for pubring.txk key cleaning and paths to files
builder.Services.Configure<CoreSettings>(
    builder.Configuration.GetSection("CoreSettings"));

builder.Services.Configure<LimLImCreationSettings>(
    builder.Configuration.GetSection("LimLImCreationSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
