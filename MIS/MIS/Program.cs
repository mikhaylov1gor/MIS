using Microsoft.Extensions.Options;
using MIS.Services;
using MIS.Models;
using MIS.Models.DB;
using Microsoft.EntityFrameworkCore;
using MIS.Infrastucture;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using MIS.Middleware;
using Quartz;
using MIS.Infrastucture.Email;

var builder = WebApplication.CreateBuilder(args);

//quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory(); 
    var jobKey = new JobKey("MissedAppointmentsNotificationJob");

    q.AddJob<SkippedVisitsNotificationJob>(opts => opts.WithIdentity(jobKey));  
    q.AddTrigger(opts => opts
        .ForJob(jobKey)  
        .WithIdentity("SkippedVisitsNotificationJob-trigger")
        .WithCronSchedule("0 1 1 * * ?")); 
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//smtp
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//database
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MisDbContext>(options => options.UseSqlServer(connection));

//services
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IDictionaryService, DictionaryService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IInspectionService, InspectionService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<ISeedDataService, SeedDataService>();
builder.Services.AddScoped<ITokenBlackListService, TokenBlackListService>();
builder.Services.AddHostedService<TokenCleanupService>();


//jwt
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

//jwt configure
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]))
        };
    });

var app = builder.Build();

//seedData
using (var scope = app.Services.CreateScope())
{
    var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
    await seedDataService.SeedSpecialties();
    await seedDataService.SeedIcd10();
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// middleware
app.UseMiddlewareHandlerException();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
