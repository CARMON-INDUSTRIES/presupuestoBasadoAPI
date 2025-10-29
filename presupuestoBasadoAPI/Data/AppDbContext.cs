using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using presupuestoBasadoAPI.Models;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProgramaPresupuestario> Programas { get; set; }
    public DbSet<IdentificacionProblema> IdentificacionProblemas { get; set; }
    public DbSet<JustificacionPrograma> JustificacionProgramas { get; set; }
    public DbSet<PoblacionObjetivo> PoblacionObjetivo { get; set; }
    public DbSet<AnalisisEntorno> AnalisisEntorno { get; set; }
    public DbSet<UnidadAdministrativa> UnidadAdministrativas { get; set; }
    public DbSet<Entidad> Entidad { get; set; }
    public DbSet<AlineacionEstado> AlineacionesEstado { get; set; }
    public DbSet<AlineacionMunicipio> AlineacionesMunicipio { get; set; }
    public DbSet<Ramo> Ramos { get; set; }
    public DbSet<ClasificacionFuncional> ClasificacionesFuncionales { get; set; }

    //anexo 1
    public DbSet<Antecedente> Antecedentes { get; set; }
    public DbSet<IdentificacionDescripcionProblema> IdentificacionDescripcionProblemas { get; set; }
    public DbSet<DeterminacionJustificacionObjetivos> DeterminacionJustificacionObjetivo { get; set; }
    public DbSet<Cobertura> Coberturas { get; set; }
    public DbSet<DisenoIntervencionPublica> DisenoIntervencionPublicas { get; set; }
    public DbSet<ProgramaSocial> ProgramaSocial { get; set; }
    public DbSet<ProgramaSocialCategoria> ProgramaSocialCategorias { get; set; }
    public DbSet<PadronBeneficiarios> PadronBeneficiarios { get; set; }
    public DbSet<ReglasOperacion> ReglasOperacion { get; set; }
    public DbSet<Componente> Componentes { get; set; }
    public DbSet<Accion> Acciones { get; set; }
    public DbSet<Resultado> Resultados { get; set; }
    public DbSet<EfectoSuperior> EfectosSuperiores { get; set; }
    public DbSet<ArbolObjetivos> ArbolObjetivos { get; set; }
    public DbSet<AnalisisAlternativas> AnalisisAlternativas { get; set; } = default!;
    public DbSet<AlternativaEvaluacion> AlternativasEvaluacion { get; set; } = default!;
    public DbSet<MatrizIndicadores> MatricesIndicadores { get; set; }
    public DbSet<FilaMatriz> FilaMatriz { get; set; }

    public DbSet<FichaIndicador> Fichas { get; set; }
    public DbSet<IndicadorDetalle> IndicadoresDetalle { get; set; }
    public DbSet<MetaProgramada> MetasProgramadas { get; set; }
    public DbSet<LineaAccion> LineasAccion { get; set; }

    public DbSet<Indicador> Indicadores { get; set; }
    public DbSet<Meta> Metas { get; set; }
    public DbSet<LineaBase> LineasBase { get; set; }
    public DbSet<ProgramacionMeta> ProgramacionesMetas { get; set; }
    public DbSet<ReglasOperacionDetalle> ReglasOperacionDetalles { get; set; }
    public DbSet<AcuerdoEstatal> AcuerdoEstatal { get; set; }
    public DbSet<ObjetivoEstatal> ObjetivoEstatal { get; set; }
    public DbSet<EstrategiaEstatal> EstrategiaEstatal { get; set; }
    public DbSet<LineaDeAccionEstatal> LineaDeAccionEstatal { get; set; }

    // === Municipal ===
    public DbSet<AcuerdoMunicipal> AcuerdoMunicipal { get; set; }
    public DbSet<ObjetivoMunicipal> ObjetivoMunicipal { get; set; }
    public DbSet<EstrategiaMunicipal> EstrategiaMunicipal { get; set; }
    public DbSet<LineaDeAccionMunicipal> LineaDeAccionMunicipal { get; set; }

    // === Clasificación Funcional ===
    public DbSet<Finalidad> Finalidad { get; set; }
    public DbSet<Funcion> Funcion { get; set; }
    public DbSet<Subfuncion> SubFuncion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AlternativaEvaluacion>()
            .HasOne(a => a.Analisis)
            .WithMany(x => x.Alternativas)
            .HasForeignKey(a => a.AnalisisAlternativasId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UnidadAdministrativa>()
           .HasMany(u => u.Usuarios)
           .WithOne(u => u.UnidadAdministrativa)
           .HasForeignKey(u => u.UnidadAdministrativaId)
           .OnDelete(DeleteBehavior.Restrict); // O Cascade

        modelBuilder.Entity<Entidad>()
   .HasMany(u => u.Usuarios)
   .WithOne(u => u.Entidad)
   .HasForeignKey(u => u.EntidadId)
   .OnDelete(DeleteBehavior.Restrict); // O Cascade

        modelBuilder.Entity<Componente>()
        .HasOne(c => c.Resultado)
        .WithOne(r => r.Componente)
        .HasForeignKey<Resultado>(r => r.ComponenteId)
        .OnDelete(DeleteBehavior.Cascade);

        // Aseguramos unicidad del FK (Resultado.ComponenteId)
        modelBuilder.Entity<Resultado>()
            .HasIndex(r => r.ComponenteId)
            .IsUnique();
    }

}
