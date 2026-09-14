using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using NoteBoard.Api.Middlewares;
using NoteBoard.Application.Common.Messages;
using NoteBoard.Application.DependencyInjection;
using NoteBoard.Data.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes.Add(
            "Bearer", 
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Name = "Bearer",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token to log in."
            });
            
        return Task.CompletedTask;
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Authentication:Issuer"];
        var audience = builder.Configuration["Authentication:Audience"];
        var secretKey = builder.Configuration["Authentication:SigningKey"];

        if (string.IsNullOrEmpty(issuer) 
            || string.IsNullOrEmpty(audience)
            || string.IsNullOrEmpty(secretKey) )
        {
            throw new InvalidOperationException(ApplicationMessages.AuthConfigurationError);
        }

        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authentication:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SigningKey"]!)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddDataDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Note Board API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}
else
{
    app.UseExceptionMiddleware();
}

app.UseCors(opts => opts
    .AllowAnyOrigin() //.WithOrigins(app.Configuration["AllowedHosts"] ?? string.Empty)
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.UseAuthentication();
app.UseAuthorization();
app.UseAuthenticationMiddleware();

app.MapControllers()
    .RequireAuthorization();

app.Run();