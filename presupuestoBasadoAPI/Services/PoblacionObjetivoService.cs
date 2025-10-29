using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Services
{
    public class PoblacionObjetivoService : IPoblacionObjetivoService
    {
        private readonly AppDbContext _context;

        public PoblacionObjetivoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PoblacionObjetivo>> ObtenerTodosAsync(string userId)
        {
            return await _context.PoblacionObjetivo
                .Where(p => p.UserId == userId)  // 🔹 filtrar por usuario
                .ToListAsync();
        }

        public async Task<PoblacionObjetivo?> ObtenerPorIdAsync(int id)
        {
            return await _context.PoblacionObjetivo.FindAsync(id);
        }

        public async Task<PoblacionObjetivo> CrearAsync(PoblacionObjetivoDto dto)
        {
            var entidad = new PoblacionObjetivo
            {
                GrupoPoblacional = dto.GrupoPoblacional,
                AmbitoGeografico = dto.AmbitoGeografico,
                CriteriosSeleccion = dto.CriteriosSeleccion,
                FechaCreacion = DateTime.Now,
                UserId = dto.UserId // 🔹 asociar usuario
            };

            _context.PoblacionObjetivo.Add(entidad);
            await _context.SaveChangesAsync();

            return entidad;
        }

        public async Task<PoblacionObjetivo?> ObtenerUltimoAsync(string userId)
        {
            return await _context.PoblacionObjetivo
                .Where(p => p.UserId == userId) // 🔹 filtrar por usuario
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
        }
    }
}
