using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;

namespace presupuestoBasadoAPI.Services
{
    public class AnalisisEntornoService : IAnalisisEntornoService
    {
        private readonly AppDbContext _context;

        public AnalisisEntornoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AnalisisEntorno>> ObtenerTodosAsync(string userId)
        {
            return await _context.AnalisisEntorno
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<AnalisisEntorno?> ObtenerPorIdAsync(int id, string userId)
        {
            return await _context.AnalisisEntorno
                                 .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task<AnalisisEntorno> CrearAsync(AnalisisEntornoDto dto, string userId)
        {
            var entidad = new AnalisisEntorno
            {
                FactoresInternos = dto.FactoresInternos,
                FactoresExternos = dto.FactoresExternos,
                RiesgosOportunidades = dto.RiesgosOportunidades,
                FechaCreacion = DateTime.Now,
                UserId = userId // 🔗 Asociar al usuario actual
            };

            _context.AnalisisEntorno.Add(entidad);
            await _context.SaveChangesAsync();

            return entidad;
        }
    }
}
