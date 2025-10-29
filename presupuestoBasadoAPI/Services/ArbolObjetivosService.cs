using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Interfaces;
using presupuestoBasadoAPI.Models;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Data;

namespace presupuestoBasadoAPI.Services
{
    public class ArbolObjetivosService : IArbolObjetivosService
    {
        private readonly AppDbContext _context;

        public ArbolObjetivosService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ArbolObjetivosDto?> GetUltimoAsync(string userId)
        {
            var entity = await _context.ArbolObjetivos
                .Include(a => a.Componentes)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            if (entity == null) return null;

            return new ArbolObjetivosDto
            {
                Id = entity.Id,
                Fin = entity.Fin,
                ObjetivoCentral = entity.ObjetivoCentral,
                Componentes = entity.Componentes.Select(c => new ComponenteObjetivoDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Medios = c.Medios.ToList(),
                    Resultados = c.Resultados.ToList()
                }).ToList()
            };
        }

        public async Task<ArbolObjetivosDto> CrearAsync(ArbolObjetivosDto dto, string userId)
        {
            var entity = new ArbolObjetivos
            {
                Fin = dto.Fin,
                ObjetivoCentral = dto.ObjetivoCentral,
                UserId = userId,
                Componentes = dto.Componentes.Select(c => new ComponenteObjetivo
                {
                    Nombre = c.Nombre,
                    Medios = c.Medios,
                    Resultados = c.Resultados
                }).ToList()
            };

            _context.ArbolObjetivos.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }
    }
}
