using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace presupuestoBasadoAPI.Services
{
    public class DeterminacionJustificacionObjetivosService : IDeterminacionJustificacionObjetivosService
    {
        private readonly AppDbContext _context;

        public DeterminacionJustificacionObjetivosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeterminacionJustificacionObjetivosDto>> GetAllAsync(string userId)
        {
            return await _context.DeterminacionJustificacionObjetivo
                .Where(d => d.UserId == userId)
                .Select(d => new DeterminacionJustificacionObjetivosDto
                {
                    Id = d.Id,
                    ObjetivosEspecificos = d.ObjetivosEspecificos,
                    RelacionOtrosProgramas = d.RelacionOtrosProgramas
                })
                .ToListAsync();
        }

        public async Task<DeterminacionJustificacionObjetivosDto> GetByIdAsync(int id, string userId)
        {
            var d = await _context.DeterminacionJustificacionObjetivo
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (d == null) return null;

            return new DeterminacionJustificacionObjetivosDto
            {
                Id = d.Id,
                ObjetivosEspecificos = d.ObjetivosEspecificos,
                RelacionOtrosProgramas = d.RelacionOtrosProgramas
            };
        }

        public async Task<DeterminacionJustificacionObjetivosDto> CreateAsync(DeterminacionJustificacionObjetivosDto dto, string userId)
        {
            var d = new DeterminacionJustificacionObjetivos
            {
                ObjetivosEspecificos = dto.ObjetivosEspecificos,
                RelacionOtrosProgramas = dto.RelacionOtrosProgramas,
                UserId = userId
            };

            _context.DeterminacionJustificacionObjetivo.Add(d);
            await _context.SaveChangesAsync();

            dto.Id = d.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, DeterminacionJustificacionObjetivosDto dto, string userId)
        {
            var d = await _context.DeterminacionJustificacionObjetivo
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (d == null) return false;

            d.ObjetivosEspecificos = dto.ObjetivosEspecificos;
            d.RelacionOtrosProgramas = dto.RelacionOtrosProgramas;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var d = await _context.DeterminacionJustificacionObjetivo
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (d == null) return false;

            _context.DeterminacionJustificacionObjetivo.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DeterminacionJustificacionObjetivosDto> GetUltimoAsync(string userId)
        {
            var d = await _context.DeterminacionJustificacionObjetivo
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (d == null) return null;

            return new DeterminacionJustificacionObjetivosDto
            {
                Id = d.Id,
                ObjetivosEspecificos = d.ObjetivosEspecificos,
                RelacionOtrosProgramas = d.RelacionOtrosProgramas
            };
        }
    }
}
