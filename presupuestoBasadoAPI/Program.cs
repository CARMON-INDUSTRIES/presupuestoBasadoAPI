using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Services;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Data;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 🔹 Cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Configuración de CORS (local + producción)
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins(
                "http://localhost:9000",
                "http://localhost:9001",
                "https://presupuesto-basado-frontend.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//  Registrar DbContext con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

//  Identity sin cookies
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//  Inyección de servicios personalizados
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
builder.Services.AddHttpClient();
builder.Services.AddScoped<IIAService, IAService>();
builder.Services.AddScoped<IUsuarioActualService, UsuarioActualService>();
builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddHttpContextAccessor();

//  Configuración de JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

//  Configuración de JWT
var jwtKey = builder.Configuration["Jwt:Key"];
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

//  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.UseCors(MyAllowSpecificOrigins);


app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();

    if (!string.IsNullOrEmpty(origin) && (
        origin == "https://presupuesto-basado-frontend.vercel.app" ||
        origin == "http://localhost:9000" ||
        origin == "http://localhost:9001"))
    {
        context.Response.Headers.TryAdd("Access-Control-Allow-Origin", origin);
        context.Response.Headers.TryAdd("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.TryAdd("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.TryAdd("Access-Control-Allow-Credentials", "true");
    }

    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204;
        return;
    }

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//  Inicialización de roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedRolesAsync(roleManager);
}

app.Run();
