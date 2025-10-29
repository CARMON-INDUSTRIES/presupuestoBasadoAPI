using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Data;
using presupuestoBasadoAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using presupuestoBasadoAPI.Dto;

namespace presupuestoBasadoAPI.Services
{
    public class DisenoIntervencionPublicaService : IDisenoIntervencionPublicaService
    {
        private readonly AppDbContext _context;

    public DisenoIntervencionPublicaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DisenoIntervencionPublicaDto>> GetAllAsync(string userId)
        {
            return await _context.DisenoIntervencionPublicas
                .Where(d => d.UserId == userId)
                .Include(d => d.Componentes)
                    .ThenInclude(c => c.Acciones)
                .Include(d => d.Componentes)
                    .ThenInclude(c => c.Resultado)
                .Select(d => new DisenoIntervencionPublicaDto
                {
                    Id = d.Id,
                    EtapasIntervencion = d.EtapasIntervencion,
                    EscenariosFuturosEsperar = d.EscenariosFuturosEsperar,
                    Componentes = d.Componentes.Select(c => new ComponenteDto
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Acciones = c.Acciones.Select(a => new AccionDto
                        {
                            Id = a.Id,
                            Descripcion = a.Descripcion,
                            Cantidad = a.Cantidad
                        }).ToList(),
                        Resultado = c.Resultado != null
                            ? new ResultadoDto { Id = c.Resultado.Id, Descripcion = c.Resultado.Descripcion }
                            : null
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<DisenoIntervencionPublicaDto?> GetByIdAsync(int id, string userId)
        {
            var d = await _context.DisenoIntervencionPublicas
                .Where(x => x.UserId == userId && x.Id == id)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Acciones)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Resultado)
                .FirstOrDefaultAsync();

            if (d == null) return null;

            return new DisenoIntervencionPublicaDto
            {
                Id = d.Id,
                EtapasIntervencion = d.EtapasIntervencion,
                EscenariosFuturosEsperar = d.EscenariosFuturosEsperar,
                Componentes = d.Componentes.Select(c => new ComponenteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Acciones = c.Acciones.Select(a => new AccionDto
                    {
                        Id = a.Id,
                        Descripcion = a.Descripcion,
                        Cantidad = a.Cantidad
                    }).ToList(),
                    Resultado = c.Resultado != null
                        ? new ResultadoDto { Id = c.Resultado.Id, Descripcion = c.Resultado.Descripcion }
                        : null
                }).ToList()
            };
        }

        public async Task<DisenoIntervencionPublicaDto> CreateAsync(DisenoIntervencionPublicaDto dto, string userId)
        {
            var d = new DisenoIntervencionPublica
            {
                UserId = userId,
                EtapasIntervencion = dto.EtapasIntervencion,
                EscenariosFuturosEsperar = dto.EscenariosFuturosEsperar,
                Componentes = dto.Componentes.Select(c => new Componente
                {
                    Nombre = c.Nombre,
                    Acciones = c.Acciones.Select(a => new Accion
                    {
                        Descripcion = a.Descripcion,
                        Cantidad = a.Cantidad,
                        UserId = userId
                    }).ToList(),
                    Resultado = c.Resultado != null
                        ? new Resultado { Descripcion = c.Resultado.Descripcion, UserId = userId }
                        : null,
                    UserId = userId
                }).ToList()
            };

            _context.DisenoIntervencionPublicas.Add(d);
            await _context.SaveChangesAsync();

            dto.Id = d.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, DisenoIntervencionPublicaDto dto, string userId)
        {
            var d = await _context.DisenoIntervencionPublicas
                .Where(x => x.UserId == userId && x.Id == id)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Acciones)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Resultado)
                .FirstOrDefaultAsync();

            if (d == null) return false;

            d.EtapasIntervencion = dto.EtapasIntervencion;
            d.EscenariosFuturosEsperar = dto.EscenariosFuturosEsperar;

            // Limpiar dependencias viejas
            _context.Acciones.RemoveRange(d.Componentes.SelectMany(c => c.Acciones));
            _context.Resultados.RemoveRange(d.Componentes.Where(c => c.Resultado != null).Select(c => c.Resultado));
            _context.Componentes.RemoveRange(d.Componentes);
            await _context.SaveChangesAsync();

            // Reemplazar con nuevos componentes
            d.Componentes = dto.Componentes.Select(c => new Componente
            {
                Nombre = c.Nombre,
                Acciones = c.Acciones.Select(a => new Accion
                {
                    Descripcion = a.Descripcion,
                    Cantidad = a.Cantidad,
                    UserId = userId
                }).ToList(),
                Resultado = c.Resultado != null
                    ? new Resultado { Descripcion = c.Resultado.Descripcion, UserId = userId }
                    : null,
                UserId = userId
            }).ToList();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var d = await _context.DisenoIntervencionPublicas
                .Where(x => x.UserId == userId && x.Id == id)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Acciones)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Resultado)
                .FirstOrDefaultAsync();

            if (d == null) return false;

            _context.DisenoIntervencionPublicas.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DisenoIntervencionPublicaDto?> GetUltimoAsync(string userId)
        {
            var d = await _context.DisenoIntervencionPublicas
                .Where(x => x.UserId == userId)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Acciones)
                .Include(x => x.Componentes)
                    .ThenInclude(c => c.Resultado)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (d == null) return null;

            return new DisenoIntervencionPublicaDto
            {
                Id = d.Id,
                EtapasIntervencion = d.EtapasIntervencion,
                EscenariosFuturosEsperar = d.EscenariosFuturosEsperar,
                Componentes = d.Componentes.Select(c => new ComponenteDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Acciones = c.Acciones.Select(a => new AccionDto
                    {
                        Id = a.Id,
                        Descripcion = a.Descripcion,
                        Cantidad = a.Cantidad
                    }).ToList(),
                    Resultado = c.Resultado != null
                        ? new ResultadoDto { Id = c.Resultado.Id, Descripcion = c.Resultado.Descripcion }
                        : null
                }).ToList()
            };
        }
    }

}
