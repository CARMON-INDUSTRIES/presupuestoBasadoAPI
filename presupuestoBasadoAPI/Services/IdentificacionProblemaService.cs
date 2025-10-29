using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public class IdentificacionProblemaService : IIdentificacionProblemaService
    {
        private readonly AppDbContext _context;

        public IdentificacionProblemaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<IdentificacionProblema>> ObtenerTodosAsync(string userId)
        {
            return await _context.IdentificacionProblemas
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<IdentificacionProblema?> ObtenerPorIdAsync(int id, string userId)
        {
            return await _context.IdentificacionProblemas
                                 .Where(x => x.Id == id && x.UserId == userId)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IdentificacionProblema?> ObtenerUltimoAsync(string userId)
        {
            return await _context.IdentificacionProblemas
                                 .Where(x => x.UserId == userId)
                                 .OrderByDescending(x => x.Id)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IdentificacionProblema> CrearAsync(IdentificacionProblemaDto dto, string userId)
        {
            var entidad = new IdentificacionProblema
            {
                DiagnosticoSituacionActual = dto.DiagnosticoSituacionActual,
                ProblemaCentral = dto.ProblemaCentral,
                EvidenciaProblema = dto.EvidenciaProblema,
                UserId = userId // 🔹 asociar al usuario loggeado
            };

            _context.IdentificacionProblemas.Add(entidad);
            await _context.SaveChangesAsync();

            return entidad;
        }
    }
}
