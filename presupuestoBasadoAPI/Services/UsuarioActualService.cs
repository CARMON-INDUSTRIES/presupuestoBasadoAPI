using Microsoft.AspNetCore.Identity;
using presupuestoBasadoAPI.Models;
using System.Security.Claims;

public interface IUsuarioActualService
{
    Task<string?> ObtenerUserIdAsync();
    Task<bool> EsSuperAdminAsync();
}

public class UsuarioActualService : IUsuarioActualService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActualService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> ObtenerUserIdAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public async Task<bool> EsSuperAdminAsync()
    {
        var userId = await ObtenerUserIdAsync();
        if (string.IsNullOrEmpty(userId)) return false;

        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario == null) return false;

        var roles = await _userManager.GetRolesAsync(usuario);
        return roles.Contains("SuperAdmin");
    }
}
