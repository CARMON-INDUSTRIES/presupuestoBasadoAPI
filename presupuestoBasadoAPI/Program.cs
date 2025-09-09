using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Services;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Data; // Para usar DbInitializer

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//  Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:9000", "http://localhost:9001")
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                      });
});

//  Registrar DbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Agregar Identity con soporte de roles (sin cookies)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Inyección de servicios
builder.Services.AddScoped<IProgramaService, ProgramaService>();
builder.Services.AddScoped<IIdentificacionProblemaService, IdentificacionProblemaService>();
builder.Services.AddScoped<IJustificacionProgramaService, JustificacionProgramaService>();
builder.Services.AddScoped<IPoblacionObjetivoService, PoblacionObjetivoService>();
builder.Services.AddScoped<IAnalisisEntornoService, AnalisisEntornoService>();
builder.Services.AddScoped<IAntecedenteService, AntecedenteService>();
builder.Services.AddScoped<IIdentificacionDescripcionProblemaService, IdentificacionDescripcionProblemaService>();
builder.Services.AddScoped<IDeterminacionJustificacionObjetivosService, DeterminacionJustificacionObjetivosService>();
builder.Services.AddScoped<ICoberturaService, CoberturaService>();
builder.Services.AddScoped<IDisenoIntervencionPublicaService, DisenoIntervencionPublicaService>();
builder.Services.AddScoped<IProgramaSocialService, ProgramaSocialService>();
builder.Services.AddScoped<IPadronBeneficiariosService, PadronBeneficiariosService>();
builder.Services.AddScoped<IReglasOperacionService, ReglasOperacionService>();
builder.Services.AddScoped<IArbolObjetivosService, ArbolObjetivosService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var jwtKey = builder.Configuration["Jwt:Key"]; //Jwt:Key en appsettings.json
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("No se encontró la clave Jwt:Key en appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Evita redirect a /Account/Login
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\":\"No autorizado o token inválido\"}");
        }
    };
});

builder.Services.AddAuthorization();

// Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

// primero Autenticación, luego Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Inicialización de roles al iniciar la app
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedRolesAsync(roleManager);
}

app.Run();
