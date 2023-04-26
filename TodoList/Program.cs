global using TodoList.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

string connect = $"Host={config["DataBase:Host"]};Port={config["DataBase:Port"]};Database={config["DataBase:DataBaseName"]};Username={config["DataBase:Username"]};Password={config["DataBase:Password"]}";

var contextOptions = new DbContextOptionsBuilder<ApplicationContext>()
    .UseNpgsql(connect)
    .Options;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:ISSUER"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ISSUER"],
        ValidateLifetime = true,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:KEY"])),
        ValidateIssuerSigningKey = true,
    };
    options.Events = new JwtBearerEvents()
    {
        OnTokenValidated = context =>
        {
            context.HttpContext.Items["username"] = context.Principal.Identity.Name;
            
            return Task.CompletedTask;
        } 
    };
});
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(connect);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
