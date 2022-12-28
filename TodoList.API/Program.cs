global using ToDoListApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

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
        // укзывает, будет ли валидироваться издатель при валидации токена
        ValidateIssuer = true,
        // строка, представляющая издателя
        ValidIssuer = builder.Configuration["JWT:ISSUER"],

        // будет ли валидироваться потребитель токена
        ValidateAudience = true,
        // установка потребителя токена
        ValidAudience = builder.Configuration["JWT:ISSUER"],
        // будет ли валидироваться время существования
        ValidateLifetime = true,

        // установка ключа безопасности
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:KEY"])),
        // валидация ключа безопасности
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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
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
