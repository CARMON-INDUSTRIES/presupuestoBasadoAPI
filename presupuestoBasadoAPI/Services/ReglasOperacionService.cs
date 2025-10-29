using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public class ReglasOperacionService : IReglasOperacionService
    {
        private readonly AppDbContext _context;

        public ReglasOperacionService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Ahora recibe el userId
        public async Task<ReglasOperacion> CrearAsync(ReglasOperacionDto dto, string userId)
        {
            var entidad = new ReglasOperacion
            {
                TieneReglasOperacion = dto.TieneReglasOperacion,
                ArchivoAdjunto = dto.ArchivoAdjunto,
                LigaInternet = dto.LigaInternet,
                UserId = userId                  
            };

            _context.ReglasOperacion.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        
        public async Task<ReglasOperacion?> ObtenerUltimoAsync(string userId)
        {
            return await _context.ReglasOperacion
                .Where(r => r.UserId == userId)            
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();
        }
    }
}
