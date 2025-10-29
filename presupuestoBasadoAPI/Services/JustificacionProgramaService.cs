using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public class JustificacionProgramaService : IJustificacionProgramaService
    {
        private readonly AppDbContext _context;

        public JustificacionProgramaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<JustificacionPrograma>> ObtenerTodosAsync(string userId)
        {
            return await _context.JustificacionProgramas
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<JustificacionPrograma?> ObtenerPorIdAsync(int id, string userId)
        {
            return await _context.JustificacionProgramas
                                 .Where(x => x.Id == id && x.UserId == userId)
                                 .FirstOrDefaultAsync();
        }

        public async Task<JustificacionPrograma?> ObtenerUltimoAsync(string userId)
        {
            return await _context.JustificacionProgramas
                                 .Where(x => x.UserId == userId)
                                 .OrderByDescending(x => x.Id)
                                 .FirstOrDefaultAsync();
        }

        public async Task<JustificacionPrograma> CrearAsync(JustificacionProgramaDto dto, string userId)
        {
            var entidad = new JustificacionPrograma
            {
                RelevanciaSocial = dto.RelevanciaSocial,
                AlineacionPlaneacion = dto.AlineacionPlaneacion,
                ContribucionSolucion = dto.ContribucionSolucion,
                UserId = userId // 🔹 asociar al usuario loggeado
            };

            _context.JustificacionProgramas.Add(entidad);
            await _context.SaveChangesAsync();

            return entidad;
        }
    }
}
