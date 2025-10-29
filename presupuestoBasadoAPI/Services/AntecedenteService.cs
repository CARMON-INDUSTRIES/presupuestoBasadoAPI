using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace presupuestoBasadoAPI.Services
{
    public class AntecedenteService : IAntecedenteService
    {
        private readonly AppDbContext _context;

        public AntecedenteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AntecedenteDto>> GetAllAsync(string userId)
        {
            return await _context.Antecedentes
                .Where(a => a.UserId == userId)
                .Select(a => new AntecedenteDto
                {
                    Id = a.Id,
                    DescripcionPrograma = a.DescripcionPrograma,
                    ContextoHistoricoNormativo = a.ContextoHistoricoNormativo,
                    ProblematicaOrigen = a.ProblematicaOrigen,
                    ExperienciasPrevias = a.ExperienciasPrevias
                })
                .ToListAsync();
        }

        public async Task<AntecedenteDto?> GetByIdAsync(int id, string userId)
        {
            var a = await _context.Antecedentes
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (a == null) return null;

            return new AntecedenteDto
            {
                Id = a.Id,
                DescripcionPrograma = a.DescripcionPrograma,
                ContextoHistoricoNormativo = a.ContextoHistoricoNormativo,
                ProblematicaOrigen = a.ProblematicaOrigen,
                ExperienciasPrevias = a.ExperienciasPrevias
            };
        }

        public async Task<AntecedenteDto> CreateAsync(AntecedenteDto dto, string userId)
        {
            var a = new Antecedente
            {
                DescripcionPrograma = dto.DescripcionPrograma,
                ContextoHistoricoNormativo = dto.ContextoHistoricoNormativo,
                ProblematicaOrigen = dto.ProblematicaOrigen,
                ExperienciasPrevias = dto.ExperienciasPrevias,
                UserId = userId
            };

            _context.Antecedentes.Add(a);
            await _context.SaveChangesAsync();

            dto.Id = a.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, AntecedenteDto dto, string userId)
        {
            var a = await _context.Antecedentes
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (a == null) return false;

            a.DescripcionPrograma = dto.DescripcionPrograma;
            a.ContextoHistoricoNormativo = dto.ContextoHistoricoNormativo;
            a.ProblematicaOrigen = dto.ProblematicaOrigen;
            a.ExperienciasPrevias = dto.ExperienciasPrevias;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var a = await _context.Antecedentes
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (a == null) return false;

            _context.Antecedentes.Remove(a);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AntecedenteDto?> GetUltimoAsync(string userId)
        {
            var a = await _context.Antecedentes
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (a == null) return null;

            return new AntecedenteDto
            {
                Id = a.Id,
                DescripcionPrograma = a.DescripcionPrograma,
                ContextoHistoricoNormativo = a.ContextoHistoricoNormativo,
                ProblematicaOrigen = a.ProblematicaOrigen,
                ExperienciasPrevias = a.ExperienciasPrevias
            };
        }
    }
}
