using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using SlotService.API.REST.Common;
using SlotService.API.REST.Mappers;
using SlotService.API.REST.Model;
using SlotService.API.REST.Validators;
using SlotService.Application.API;
using SlotService.Application.API.Common;
using SlotService.Storage.Helper;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddAuthentication("BasicAuthentication")
       .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",null);

builder.Services
       .AddScoped<IValidator<Appointment>, AppointmentValidator>()
       .AddScoped(_ => MessageDispatcherFactory.Create())
       .AddScoped<IMapper<WeekSchedule, GetWeekAvailabilityResponse>,
            GetWeeklyAvailabilityResponseMapper>();


builder.Services.AddControllers()
       .AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new PascalCaseNamingPolicy();
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(SetSwaggerOptions);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var devDataLoader = new DevDataLoader();
    devDataLoader.Load();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

return;

void SetSwaggerOptions(SwaggerGenOptions options)
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "Slot service API",
            Description = "API to check slots availability and book appointments."
        });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.AddSecurityDefinition(
        "basic",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            In = ParameterLocation.Header
        });
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                },
                new string[] { }
            }
        });
}

public partial class Program;
