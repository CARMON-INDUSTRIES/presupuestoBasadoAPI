using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public CuentasController(SignInManager<ApplicationUser> signInManager,
                             UserManager<ApplicationUser> userManager,
                             RoleManager<IdentityRole> roleManager, 
                             AppDbContext context,
                             IConfiguration config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _context = context;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.User);
        if (user == null)
            return Unauthorized("Usuario no encontrado.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Contraseña incorrecta.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return Ok(new
        {
            token,
            username = user.UserName,
            roles
        });
    }

    [Authorize]
    [HttpPost("Logout")]
    public IActionResult Logout()
    {

        return Ok(new { message = "Sesión cerrada correctamente" });
    }


    [HttpPost("Registro")]
    public async Task<IActionResult> Registro([FromBody] RegisterDto model)
    {
        // Validación mínima obligatoria
        if (string.IsNullOrWhiteSpace(model.User) || string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "El usuario y la contraseña son obligatorios." });

        // Crear el nuevo usuario (los demás campos son opcionales)
        var usuario = new ApplicationUser
        {
            UserName = model.User,
            Email = model.Email ?? string.Empty,
            NombreCompleto = model.NombreCompleto ?? string.Empty,
            Cargo = model.Cargo ?? string.Empty,
            Coordinador = model.Coordinador ?? string.Empty,
            UnidadesPresupuestales = model.UnidadesPresupuestales ?? string.Empty,
            ProgramaPresupuestario = model.ProgramaPresupuestario ?? string.Empty,
            NombreMatriz = model.NombreMatriz ?? string.Empty,
            UnidadAdministrativaId = model.UnidadAdministrativaId,
            EntidadId = model.EntidadId
        };

        // Crear el usuario en Identity
        var resultado = await _userManager.CreateAsync(usuario, model.Password);
        if (!resultado.Succeeded)
            return BadRequest(resultado.Errors);

        // Asignar rol si se proporcionó
        if (!string.IsNullOrEmpty(model.Rol))
        {
            if (!await _roleManager.RoleExistsAsync(model.Rol))
                await _roleManager.CreateAsync(new IdentityRole(model.Rol));

            await _userManager.AddToRoleAsync(usuario, model.Rol);
        }

        // Obtener roles y generar token
        var roles = await _userManager.GetRolesAsync(usuario);
        var token = GenerateJwtToken(usuario, roles);

        return Ok(new
        {
            message = $"Usuario registrado correctamente con el rol: {model.Rol}",
            token
        });
    }


    [HttpPut("CambiarPassword")]
    public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordDto model)
    {
        if (string.IsNullOrWhiteSpace(model.User) || string.IsNullOrWhiteSpace(model.Password))
            return BadRequest("Usuario o contraseña no pueden estar vacíos.");

        var user = await _userManager.FindByNameAsync(model.User);
        if (user == null)
            return NotFound("Usuario no encontrado.");

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Contraseña actualizada correctamente." });
    }

    [HttpPut("ActualizarPerfil")]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == model.User);

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (!string.IsNullOrWhiteSpace(model.Email))
                usuario.Email = model.Email;

            if (!string.IsNullOrWhiteSpace(model.NombreCompleto))
                usuario.NombreCompleto = model.NombreCompleto;

            if (!string.IsNullOrWhiteSpace(model.Cargo))
                usuario.Cargo = model.Cargo;

            if (!string.IsNullOrWhiteSpace(model.Coordinador))
                usuario.Coordinador = model.Coordinador;

            if (!string.IsNullOrWhiteSpace(model.UnidadesPresupuestales))
                usuario.UnidadesPresupuestales = model.UnidadesPresupuestales;

            if (!string.IsNullOrWhiteSpace(model.ProgramaPresupuestario))
                usuario.ProgramaPresupuestario = model.ProgramaPresupuestario;

            if (!string.IsNullOrWhiteSpace(model.NombreMatriz))
                usuario.NombreMatriz = model.NombreMatriz;


            _context.Users.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Perfil actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar perfil", error = ex.Message });
        }
    }




    [HttpGet("roles/{username}")]
    public async Task<IActionResult> GetRoles(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound("Usuario no encontrado.");

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUsuarioActual()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();

        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound("Usuario no encontrado.");

        return Ok(new
        {
            user.UserName,
            user.Email,
            user.NombreCompleto,
            user.Cargo,
            user.Coordinador,
            user.UnidadesPresupuestales,
            user.ProgramaPresupuestario,
            user.NombreMatriz,
            user.UnidadAdministrativaId,
            user.EntidadId
        });
    }

    // MÉTODO PRIVADO PARA GENERAR JWT
    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
