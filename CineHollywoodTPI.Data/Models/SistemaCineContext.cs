using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Models;

public partial class SistemaCineContext : DbContext
{
    public SistemaCineContext(DbContextOptions<SistemaCineContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actores { get; set; }

    public virtual DbSet<Beneficio> Beneficios { get; set; }

    public virtual DbSet<Butaca> Butacas { get; set; }

    public virtual DbSet<CanalVentum> CanalVenta { get; set; }

    public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; }

    public virtual DbSet<Clasificacion> Clasificaciones { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<ClienteMembresium> ClienteMembresia { get; set; }

    public virtual DbSet<Complejo> Complejos { get; set; }

    public virtual DbSet<Contacto> Contactos { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<DetalleVentaConfiterium> DetalleVentaConfiteria { get; set; }

    public virtual DbSet<Director> Directores { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EmpleadoTurno> EmpleadoTurnos { get; set; }

    public virtual DbSet<Entrada> Entrada { get; set; }

    public virtual DbSet<EstadoReserva> EstadoReservas { get; set; }

    public virtual DbSet<EstadoUsuario> EstadoUsuarios { get; set; }

    public virtual DbSet<FormaPago> FormaPagos { get; set; }

    public virtual DbSet<Formato> Formatos { get; set; }

    public virtual DbSet<Funcion> Funcion { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<Idioma> Idiomas { get; set; }

    public virtual DbSet<Localidad> Localidades { get; set; }

    public virtual DbSet<Pai> Pais { get; set; }

    public virtual DbSet<Pelicula> Pelicula { get; set; }

    public virtual DbSet<PeliculaIdioma> PeliculaIdiomas { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PrecioTipoSala> PrecioTipoSalas { get; set; }

    public virtual DbSet<ProductoConfiterium> ProductoConfiteria { get; set; }

    public virtual DbSet<Promocion> Promociones { get; set; }

    public virtual DbSet<Provincium> Provincia { get; set; }

    public virtual DbSet<Puesto> Puestos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<RolUsuario> RolUsuarios { get; set; }

    public virtual DbSet<Sala> Salas { get; set; }

    public virtual DbSet<Sucursal> Sucursales { get; set; }

    public virtual DbSet<TipoContacto> TipoContactos { get; set; }

    public virtual DbSet<TipoEspectador> TipoEspectadors { get; set; }

    public virtual DbSet<TipoMembresium> TipoMembresia { get; set; }

    public virtual DbSet<TipoSala> TipoSalas { get; set; }

    public virtual DbSet<TokenVerificacion> TokenVerificacions { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VentaxFormaPago> VentaxFormaPagos { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.IdActor).HasName("PK__ACTOR__F86BE7178E398702");

            entity.ToTable("ACTOR");

            entity.Property(e => e.IdActor).HasColumnName("id_actor");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Beneficio>(entity =>
        {
            entity.HasKey(e => e.IdBeneficio).HasName("PK__BENEFICI__E83C083171D8DF93");

            entity.ToTable("BENEFICIO");

            entity.Property(e => e.IdBeneficio).HasColumnName("id_beneficio");
            entity.Property(e => e.AplicaAJubilado)
                .HasDefaultValue(false)
                .HasColumnName("aplica_a_jubilado");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Butaca>(entity =>
        {
            entity.HasKey(e => e.IdButaca).HasName("PK__BUTACA__EB75799325B7F405");

            entity.ToTable("BUTACA");

            entity.Property(e => e.IdButaca).HasColumnName("id_butaca");
            entity.Property(e => e.Fila)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("fila");
            entity.Property(e => e.Habilitada).HasColumnName("habilitada");
            entity.Property(e => e.IdSala).HasColumnName("id_sala");
            entity.Property(e => e.Numero).HasColumnName("numero");

            entity.HasOne(d => d.IdSalaNavigation).WithMany(p => p.Butacas)
                .HasForeignKey(d => d.IdSala)
                .HasConstraintName("FK__BUTACA__id_sala__5DCAEF64");
        });

        modelBuilder.Entity<CanalVentum>(entity =>
        {
            entity.HasKey(e => e.IdCanal).HasName("PK__CANAL_VE__D4C01CC871A3D32A");

            entity.ToTable("CANAL_VENTA");

            entity.Property(e => e.IdCanal).HasColumnName("id_canal");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(e => e.IdCategoriaProd).HasName("PK__CATEGORI__56FB6ED7D79D7AE5");

            entity.ToTable("CATEGORIA_PRODUCTO");

            entity.Property(e => e.IdCategoriaProd).HasColumnName("id_categoria_prod");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Clasificacion>(entity =>
        {
            entity.HasKey(e => e.IdClasificacion).HasName("PK__CLASIFIC__257A4E1B57E7AD75");

            entity.ToTable("CLASIFICACION");

            entity.Property(e => e.IdClasificacion).HasColumnName("id_clasificacion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__CLIENTE__677F38F5CC0C4911");

            entity.ToTable("CLIENTE");

            entity.Property(e => e.IdCliente)
                .ValueGeneratedNever()
                .HasColumnName("id_cliente");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Altura).HasColumnName("altura");
            entity.Property(e => e.Calle)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("calle");
            entity.Property(e => e.EsJubilado)
                .HasDefaultValue(false)
                .HasColumnName("es_jubilado");

            entity.HasOne(d => d.IdClienteNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CLIENTE__id_clie__1332DBDC");
        });

        modelBuilder.Entity<ClienteMembresium>(entity =>
        {
            entity.HasKey(e => e.IdClienteMembresia).HasName("PK__CLIENTE___C01BE7CECEA0DF2C");

            entity.ToTable("CLIENTE_MEMBRESIA");

            entity.Property(e => e.IdClienteMembresia).HasColumnName("id_cliente_membresia");
            entity.Property(e => e.EntradasGratisUsadasMes)
                .HasDefaultValue(0)
                .HasColumnName("entradas_gratis_usadas_mes");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.FechaProximoCobro).HasColumnName("fecha_proximo_cobro");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdTipoMembresia).HasColumnName("id_tipo_membresia");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.ClienteMembresia)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__CLIENTE_M__id_cl__17036CC0");

            entity.HasOne(d => d.IdTipoMembresiaNavigation).WithMany(p => p.ClienteMembresia)
                .HasForeignKey(d => d.IdTipoMembresia)
                .HasConstraintName("FK__CLIENTE_M__id_ti__17F790F9");
        });

        modelBuilder.Entity<Complejo>(entity =>
        {
            entity.HasKey(e => e.IdComplejo).HasName("PK__COMPLEJO__FE0B73FB6C38A05D");

            entity.ToTable("COMPLEJO");

            entity.Property(e => e.IdComplejo).HasColumnName("id_complejo");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Complejos)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("FK__COMPLEJO__id_suc__5535A963");
        });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.HasKey(e => e.IdContacto).HasName("PK__CONTACTO__099A52B816623641");

            entity.ToTable("CONTACTO");

            entity.Property(e => e.IdContacto).HasColumnName("id_contacto");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.IdTipoContacto).HasColumnName("id_tipo_contacto");
            entity.Property(e => e.Valor)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Contactos)
                .HasForeignKey(d => d.IdPersona)
                .HasConstraintName("FK__CONTACTO__id_per__1DB06A4F");

            entity.HasOne(d => d.IdTipoContactoNavigation).WithMany(p => p.Contactos)
                .HasForeignKey(d => d.IdTipoContacto)
                .HasConstraintName("FK__CONTACTO__id_tip__1EA48E88");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PK__DEPARTAM__64F37A161E5DC148");

            entity.ToTable("DEPARTAMENTO");

            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<DetalleVentaConfiterium>(entity =>
        {
            entity.HasKey(e => e.IdDetalleConfiteria).HasName("PK__DETALLE___1FD4740FB4850174");

            entity.ToTable("DETALLE_VENTA_CONFITERIA");

            entity.Property(e => e.IdDetalleConfiteria).HasColumnName("id_detalle_confiteria");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVentaConfiteria)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__DETALLE_V__id_pr__41EDCAC5");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVentaConfiteria)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__DETALLE_V__id_ve__40F9A68C");
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.HasKey(e => e.IdDirector).HasName("PK__DIRECTOR__6B65E2A274FE0825");

            entity.ToTable("DIRECTOR");

            entity.Property(e => e.IdDirector).HasColumnName("id_director");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK__EMPLEADO__88B513941BDB6B4D");

            entity.ToTable("EMPLEADO");

            entity.Property(e => e.IdEmpleado)
                .ValueGeneratedNever()
                .HasColumnName("id_empleado");
            entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.IdPuesto).HasColumnName("id_puesto");
            entity.Property(e => e.Legajo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("legajo");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdDepartamento)
                .HasConstraintName("FK__EMPLEADO__id_dep__2739D489");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Empleado)
                .HasForeignKey<Empleado>(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EMPLEADO__id_emp__25518C17");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdPuesto)
                .HasConstraintName("FK__EMPLEADO__id_pue__2645B050");
        });

        modelBuilder.Entity<EmpleadoTurno>(entity =>
        {
            entity.HasKey(e => new { e.IdEmpleado, e.IdTurno, e.Fecha }).HasName("PK__EMPLEADO__C63CE0BE2255E6BD");

            entity.ToTable("EMPLEADO_TURNO");

            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");
            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.Fecha).HasColumnName("fecha");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.EmpleadoTurnos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EMPLEADO___id_em__2BFE89A6");

            entity.HasOne(d => d.IdTurnoNavigation).WithMany(p => p.EmpleadoTurnos)
                .HasForeignKey(d => d.IdTurno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EMPLEADO___id_tu__2CF2ADDF");
        });

        modelBuilder.Entity<Entrada>(entity =>
        {
            entity.HasKey(e => e.IdEntrada).HasName("PK__ENTRADA__167CD61B78E5688E");

            entity.ToTable("ENTRADA");

            entity.Property(e => e.IdEntrada).HasColumnName("id_entrada");
            entity.Property(e => e.IdButaca).HasColumnName("id_butaca");
            entity.Property(e => e.IdFuncion).HasColumnName("id_funcion");
            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.IdTipoEspectador).HasColumnName("id_tipo_espectador");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.PrecioFinal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_final");

            entity.HasOne(d => d.IdButacaNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdButaca)
                .HasConstraintName("FK__ENTRADA__id_buta__51300E55");

            entity.HasOne(d => d.IdFuncionNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdFuncion)
                .HasConstraintName("FK__ENTRADA__id_func__503BEA1C");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK__ENTRADA__id_rese__4F47C5E3");

            entity.HasOne(d => d.IdTipoEspectadorNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdTipoEspectador)
                .HasConstraintName("FK__ENTRADA__id_tipo__5224328E");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__ENTRADA__id_vent__4E53A1AA");
        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.HasKey(e => e.IdEstadoReserva).HasName("PK__ESTADO_R__D1D0D7E0A49436B1");

            entity.ToTable("ESTADO_RESERVA");

            entity.Property(e => e.IdEstadoReserva).HasColumnName("id_estado_reserva");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<EstadoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdEstadoUsuario).HasName("PK__ESTADO_U__CEFB9B899A99617E");

            entity.ToTable("ESTADO_USUARIO");

            entity.Property(e => e.IdEstadoUsuario).HasColumnName("id_estado_usuario");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<FormaPago>(entity =>
        {
            entity.HasKey(e => e.IdFormaPago).HasName("PK__FORMA_PA__DA9B39EE0F897711");

            entity.ToTable("FORMA_PAGO");

            entity.Property(e => e.IdFormaPago).HasColumnName("id_forma_pago");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Formato>(entity =>
        {
            entity.HasKey(e => e.IdFormato).HasName("PK__FORMATO__8C0C34B78E854A92");

            entity.ToTable("FORMATO");

            entity.Property(e => e.IdFormato).HasColumnName("id_formato");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Funcion>(entity =>
        {
            entity.HasKey(e => e.IdFuncion).HasName("PK__FUNCION__F6DE6FC9DF291763");

            entity.ToTable("FUNCION");

            entity.Property(e => e.IdFuncion).HasColumnName("id_funcion");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.IdFormato).HasColumnName("id_formato");
            entity.Property(e => e.IdPelicula).HasColumnName("id_pelicula");
            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");
            entity.Property(e => e.IdSala).HasColumnName("id_sala");

            entity.HasOne(d => d.IdFormatoNavigation).WithMany(p => p.Funcions)
                .HasForeignKey(d => d.IdFormato)
                .HasConstraintName("FK__FUNCION__id_form__04E4BC85");

            entity.HasOne(d => d.IdPeliculaNavigation).WithMany(p => p.Funcions)
                .HasForeignKey(d => d.IdPelicula)
                .HasConstraintName("FK__FUNCION__id_peli__02FC7413");

            entity.HasOne(d => d.IdPromocionNavigation).WithMany(p => p.Funcions)
                .HasForeignKey(d => d.IdPromocion)
                .HasConstraintName("FK__FUNCION__id_prom__05D8E0BE");

            entity.HasOne(d => d.IdSalaNavigation).WithMany(p => p.Funcions)
                .HasForeignKey(d => d.IdSala)
                .HasConstraintName("FK__FUNCION__id_sala__03F0984C");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.IdGenero).HasName("PK__GENERO__99A8E4F9E358B44E");

            entity.ToTable("GENERO");

            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Idioma>(entity =>
        {
            entity.HasKey(e => e.IdIdioma).HasName("PK__IDIOMA__0BA1525F96E40681");

            entity.ToTable("IDIOMA");

            entity.Property(e => e.IdIdioma).HasColumnName("id_idioma");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Localidad>(entity =>
        {
            entity.HasKey(e => e.IdLocalidad).HasName("PK__LOCALIDA__9A5E82AAA31CE1FD");

            entity.ToTable("LOCALIDAD");

            entity.Property(e => e.IdLocalidad).HasColumnName("id_localidad");
            entity.Property(e => e.IdProvincia).HasColumnName("id_provincia");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdProvinciaNavigation).WithMany(p => p.Localidads)
                .HasForeignKey(d => d.IdProvincia)
                .HasConstraintName("FK__LOCALIDAD__id_pr__4F7CD00D");
        });

        modelBuilder.Entity<Pai>(entity =>
        {
            entity.HasKey(e => e.IdPais).HasName("PK__PAIS__0941A3A74085F298");

            entity.ToTable("PAIS");

            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Pelicula>(entity =>
        {
            entity.HasKey(e => e.IdPelicula).HasName("PK__PELICULA__B5017F4D5DF2362E");

            entity.ToTable("PELICULA");

            entity.Property(e => e.IdPelicula).HasColumnName("id_pelicula");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.DuracionMinutos).HasColumnName("duracion_minutos");
            entity.Property(e => e.IdClasificacion).HasColumnName("id_clasificacion");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdClasificacionNavigation).WithMany(p => p.Peliculas)
                .HasForeignKey(d => d.IdClasificacion)
                .HasConstraintName("FK__PELICULA__id_cla__6B24EA82");

            entity.HasMany(d => d.IdActors).WithMany(p => p.IdPeliculas)
                .UsingEntity<Dictionary<string, object>>(
                    "PeliculaActor",
                    r => r.HasOne<Actor>().WithMany()
                        .HasForeignKey("IdActor")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PELICULA___id_ac__7A672E12"),
                    l => l.HasOne<Pelicula>().WithMany()
                        .HasForeignKey("IdPelicula")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PELICULA___id_pe__797309D9"),
                    j =>
                    {
                        j.HasKey("IdPelicula", "IdActor").HasName("PK__PELICULA__DA87C13C12F31143");
                        j.ToTable("PELICULA_ACTOR");
                        j.IndexerProperty<int>("IdPelicula").HasColumnName("id_pelicula");
                        j.IndexerProperty<int>("IdActor").HasColumnName("id_actor");
                    });

            entity.HasMany(d => d.IdDirectors).WithMany(p => p.IdPeliculas)
                .UsingEntity<Dictionary<string, object>>(
                    "PeliculaDirector",
                    r => r.HasOne<Director>().WithMany()
                        .HasForeignKey("IdDirector")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PELICULA___id_di__7E37BEF6"),
                    l => l.HasOne<Pelicula>().WithMany()
                        .HasForeignKey("IdPelicula")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PELICULA___id_pe__7D439ABD"),
                    j =>
                    {
                        j.HasKey("IdPelicula", "IdDirector").HasName("PK__PELICULA__83B721678D19006D");
                        j.ToTable("PELICULA_DIRECTOR");
                        j.IndexerProperty<int>("IdPelicula").HasColumnName("id_pelicula");
                        j.IndexerProperty<int>("IdDirector").HasColumnName("id_director");
                    });

            entity.HasMany(d => d.IdGeneros).WithMany(p => p.IdPeliculas)
                .UsingEntity<Dictionary<string, object>>(
                    "PeliculaGenero",
                    r => r.HasOne<Genero>().WithMany()
                        .HasForeignKey("IdGenero")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PELICULA___id_ge__6EF57B66"),
                    l => l.HasOne<Pelicula>().WithMany()
                        .HasForeignKey("IdPelicula")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__PELICULA___id_pe__6E01572D"),
                    j =>
                    {
                        j.HasKey("IdPelicula", "IdGenero").HasName("PK__PELICULA__3C9BF102D888C44E");
                        j.ToTable("PELICULA_GENERO");
                        j.IndexerProperty<int>("IdPelicula").HasColumnName("id_pelicula");
                        j.IndexerProperty<int>("IdGenero").HasColumnName("id_genero");
                    });
        });

        modelBuilder.Entity<PeliculaIdioma>(entity =>
        {
            entity.HasKey(e => new { e.IdPelicula, e.IdIdioma }).HasName("PK__PELICULA__55BB6A688AA686A9");

            entity.ToTable("PELICULA_IDIOMA");

            entity.Property(e => e.IdPelicula).HasColumnName("id_pelicula");
            entity.Property(e => e.IdIdioma).HasColumnName("id_idioma");
            entity.Property(e => e.EsDoblada).HasColumnName("es_doblada");

            entity.HasOne(d => d.IdIdiomaNavigation).WithMany(p => p.PeliculaIdiomas)
                .HasForeignKey(d => d.IdIdioma)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PELICULA___id_id__72C60C4A");

            entity.HasOne(d => d.IdPeliculaNavigation).WithMany(p => p.PeliculaIdiomas)
                .HasForeignKey(d => d.IdPelicula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PELICULA___id_pe__71D1E811");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__PERSONA__228148B03A07B01B");

            entity.ToTable("PERSONA");

            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NroDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nro_documento");
        });

        modelBuilder.Entity<PrecioTipoSala>(entity =>
        {
            entity.HasKey(e => e.IdPrecio).HasName("PK__PRECIO_T__A70EF6EDC22B7728");

            entity.ToTable("PRECIO_TIPO_SALA");

            entity.Property(e => e.IdPrecio).HasColumnName("id_precio");
            entity.Property(e => e.FechaDesde).HasColumnName("fecha_desde");
            entity.Property(e => e.FechaHasta).HasColumnName("fecha_hasta");
            entity.Property(e => e.IdTipoSala).HasColumnName("id_tipo_sala");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");

            entity.HasOne(d => d.IdTipoSalaNavigation).WithMany(p => p.PrecioTipoSalas)
                .HasForeignKey(d => d.IdTipoSala)
                .HasConstraintName("FK__PRECIO_TI__id_ti__60A75C0F");
        });

        modelBuilder.Entity<ProductoConfiterium>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__PRODUCTO__FF341C0D182C46A1");

            entity.ToTable("PRODUCTO_CONFITERIA");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdCategoriaProd).HasColumnName("id_categoria_prod");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioActual)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_actual");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.IdCategoriaProdNavigation).WithMany(p => p.ProductoConfiteria)
                .HasForeignKey(d => d.IdCategoriaProd)
                .HasConstraintName("FK__PRODUCTO___id_ca__3E1D39E1");
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK__PROMOCIO__F89308E02ED3646E");

            entity.ToTable("PROMOCION");

            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.PorcentajeDescuento)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentaje_descuento");
        });

        modelBuilder.Entity<Provincium>(entity =>
        {
            entity.HasKey(e => e.IdProvincia).HasName("PK__PROVINCI__66C18BFDDD047FB1");

            entity.ToTable("PROVINCIA");

            entity.Property(e => e.IdProvincia).HasColumnName("id_provincia");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdPaisNavigation).WithMany(p => p.Provincia)
                .HasForeignKey(d => d.IdPais)
                .HasConstraintName("FK__PROVINCIA__id_pa__4CA06362");
        });

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.HasKey(e => e.IdPuesto).HasName("PK__PUESTO__123AAB9988E199F1");

            entity.ToTable("PUESTO");

            entity.Property(e => e.IdPuesto).HasColumnName("id_puesto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__RESERVA__423CBE5DB79B5C99");

            entity.ToTable("RESERVA");

            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.FechaHoraReserva)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora_reserva");
            entity.Property(e => e.FechaHoraVencimiento)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora_vencimiento");
            entity.Property(e => e.IdButaca).HasColumnName("id_butaca");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdEstadoReserva).HasColumnName("id_estado_reserva");
            entity.Property(e => e.IdFuncion).HasColumnName("id_funcion");

            entity.HasOne(d => d.IdButacaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdButaca)
                .HasConstraintName("FK__RESERVA__id_buta__489AC854");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__RESERVA__id_clie__46B27FE2");

            entity.HasOne(d => d.IdEstadoReservaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEstadoReserva)
                .HasConstraintName("FK__RESERVA__id_esta__498EEC8D");

            entity.HasOne(d => d.IdFuncionNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdFuncion)
                .HasConstraintName("FK__RESERVA__id_func__47A6A41B");
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__ROL_USUA__6ABCB5E062B82309");

            entity.ToTable("ROL_USUARIO");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.IdSala).HasName("PK__SALA__D18B015B2247D872");

            entity.ToTable("SALA");

            entity.Property(e => e.IdSala).HasColumnName("id_sala");
            entity.Property(e => e.IdComplejo).HasColumnName("id_complejo");
            entity.Property(e => e.IdTipoSala).HasColumnName("id_tipo_sala");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdComplejoNavigation).WithMany(p => p.Salas)
                .HasForeignKey(d => d.IdComplejo)
                .HasConstraintName("FK__SALA__id_complej__59FA5E80");

            entity.HasOne(d => d.IdTipoSalaNavigation).WithMany(p => p.Salas)
                .HasForeignKey(d => d.IdTipoSala)
                .HasConstraintName("FK__SALA__id_tipo_sa__5AEE82B9");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal).HasName("PK__SUCURSAL__4C758013F11677E1");

            entity.ToTable("SUCURSAL");

            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Calle)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("calle");
            entity.Property(e => e.IdLocalidad).HasColumnName("id_localidad");
            entity.Property(e => e.Numero).HasColumnName("numero");

            entity.HasOne(d => d.IdLocalidadNavigation).WithMany(p => p.Sucursals)
                .HasForeignKey(d => d.IdLocalidad)
                .HasConstraintName("FK__SUCURSAL__id_loc__52593CB8");
        });

        modelBuilder.Entity<TipoContacto>(entity =>
        {
            entity.HasKey(e => e.IdTipoContacto).HasName("PK__TIPO_CON__CA854C0D95469C8C");

            entity.ToTable("TIPO_CONTACTO");

            entity.Property(e => e.IdTipoContacto).HasColumnName("id_tipo_contacto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<TipoEspectador>(entity =>
        {
            entity.HasKey(e => e.IdTipoEspectador).HasName("PK__TIPO_ESP__9A032DCF4A8036E8");

            entity.ToTable("TIPO_ESPECTADOR");

            entity.Property(e => e.IdTipoEspectador).HasColumnName("id_tipo_espectador");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<TipoMembresium>(entity =>
        {
            entity.HasKey(e => e.IdTipoMembresia).HasName("PK__TIPO_MEM__DA773810EBD41AD7");

            entity.ToTable("TIPO_MEMBRESIA");

            entity.Property(e => e.IdTipoMembresia).HasColumnName("id_tipo_membresia");
            entity.Property(e => e.EntradasGratisPorMes).HasColumnName("entradas_gratis_por_mes");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioMensual)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_mensual");

            entity.HasMany(d => d.IdBeneficios).WithMany(p => p.IdTipoMembresia)
                .UsingEntity<Dictionary<string, object>>(
                    "MembresiaBeneficio",
                    r => r.HasOne<Beneficio>().WithMany()
                        .HasForeignKey("IdBeneficio")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MEMBRESIA__id_be__0E6E26BF"),
                    l => l.HasOne<TipoMembresium>().WithMany()
                        .HasForeignKey("IdTipoMembresia")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MEMBRESIA__id_ti__0D7A0286"),
                    j =>
                    {
                        j.HasKey("IdTipoMembresia", "IdBeneficio").HasName("PK__MEMBRESI__D4F4F893F30E9780");
                        j.ToTable("MEMBRESIA_BENEFICIO");
                        j.IndexerProperty<int>("IdTipoMembresia").HasColumnName("id_tipo_membresia");
                        j.IndexerProperty<int>("IdBeneficio").HasColumnName("id_beneficio");
                    });
        });

        modelBuilder.Entity<TipoSala>(entity =>
        {
            entity.HasKey(e => e.IdTipoSala).HasName("PK__TIPO_SAL__1C51F0134F7E78C4");

            entity.ToTable("TIPO_SALA");

            entity.Property(e => e.IdTipoSala).HasColumnName("id_tipo_sala");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<TokenVerificacion>(entity =>
        {
            entity.HasKey(e => e.IdToken).HasName("PK__TOKEN_VE__3C2FA9C41DA7054E");

            entity.ToTable("TOKEN_VERIFICACION");

            entity.Property(e => e.IdToken).HasColumnName("id_token");
            entity.Property(e => e.FechaExpiracion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_expiracion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.Usado).HasColumnName("usado");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TokenVerificacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__TOKEN_VER__id_us__5F7E2DAC");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.IdTurno).HasName("PK__TURNO__C68E7397E3690C78");

            entity.ToTable("TURNO");

            entity.Property(e => e.IdTurno).HasColumnName("id_turno");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.HoraDesde).HasColumnName("hora_desde");
            entity.Property(e => e.HoraHasta).HasColumnName("hora_hasta");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__4E3E04ADDE922ADB");

            entity.ToTable("USUARIO");

            entity.HasIndex(e => e.IdPersona, "UQ__USUARIO__228148B189839654").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__USUARIO__AB6E61647AC95970").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdEstadoUsuario).HasColumnName("id_estado_usuario");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");

            entity.HasOne(d => d.IdEstadoUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEstadoUsuario)
                .HasConstraintName("FK__USUARIO__id_esta__5CA1C101");

            entity.HasOne(d => d.IdPersonaNavigation).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.IdPersona)
                .HasConstraintName("FK__USUARIO__id_pers__5AB9788F");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__USUARIO__id_rol__5BAD9CC8");
        });

        modelBuilder.Entity<VentaxFormaPago>(entity =>
        {
            entity.HasKey(e => e.IdVentaxformapago).HasName("PK__VentaxFo__7BB05E5DE5251963");

            entity.ToTable("VentaxFormaPago");

            entity.Property(e => e.IdVentaxformapago).HasColumnName("id_ventaxformapago");
            entity.Property(e => e.IdFormaPago).HasColumnName("id_forma_pago");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            entity.HasOne(d => d.IdFormaPagoNavigation).WithMany(p => p.VentaxFormaPagos)
                .HasForeignKey(d => d.IdFormaPago)
                .HasConstraintName("FK__VentaxFor__id_fo__395884C4");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.VentaxFormaPagos)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__VentaxFor__id_ve__3864608B");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__VENTA__459533BFE0E5A975");

            entity.ToTable("VENTA");

            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.IdCanal).HasColumnName("id_canal");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdEmpleado).HasColumnName("id_empleado");

            entity.HasOne(d => d.IdCanalNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdCanal)
                .HasConstraintName("FK__VENTA__id_canal__339FAB6E");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__VENTA__id_client__31B762FC");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__VENTA__id_emplea__32AB8735");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
