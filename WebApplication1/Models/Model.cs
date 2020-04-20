using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class Model : DbContext
    {
        public Model()
        {
        }

        public Model(DbContextOptions<Model> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrador> Administrador { get; set; }
        public virtual DbSet<Aluguer> Aluguer { get; set; }
        public virtual DbSet<Artigo> Artigo { get; set; }
        public virtual DbSet<Denuncias> Denuncias { get; set; }
        public virtual DbSet<Localizacao> Localizacao { get; set; }
        public virtual DbSet<Utilizador> Utilizador { get; set; }
        public virtual DbSet<Venda> Venda { get; set; }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.

                optionsBuilder.UseSqlServer("Data Source = LENOVO-JOANA;Initial Catalog = LI4; Integrated Security = True");
            } 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.Email);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.ContaBancaria).HasColumnName("contaBancaria");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(45)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Aluguer>(entity =>
            {
                entity.HasKey(e => e.IdAluguer);

                entity.Property(e => e.IdAluguer)
                    .HasColumnName("idAluguer")
                    .ValueGeneratedNever();

                entity.Property(e => e.DataFim)
                    .HasColumnName("dataFim")
                    .HasColumnType("date");

                entity.Property(e => e.DataInicio)
                    .HasColumnName("dataInicio")
                    .HasColumnType("date");

                entity.Property(e => e.Duracao).HasColumnName("duracao");

                entity.Property(e => e.IdArtigo).HasColumnName("idArtigo");

                entity.Property(e => e.IdRent)
                    .IsRequired()
                    .HasColumnName("idRent")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.IdUtilizador)
                    .IsRequired()
                    .HasColumnName("idUtilizador")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Quantidade)
                    .IsRequired()
                    .HasColumnName("quantidade")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Preco).HasColumnName("preco");

                entity.HasOne(d => d.IdArtigoNavigation)
                    .WithMany(p => p.Aluguer)
                    .HasForeignKey(d => d.IdArtigo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Aluguer_Artigo");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Aluguer)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Aluguer_Utilizador");
            });

            modelBuilder.Entity<Artigo>(entity =>
            {
                entity.HasKey(e => e.IdArtigo);

                entity.Property(e => e.IdArtigo)
                    .HasColumnName("idArtigo")
                    .ValueGeneratedNever();

                entity.Property(e => e.Categoria)
                    .IsRequired()
                    .HasColumnName("categoria")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.Etiquetas)
                    .IsRequired()
                    .HasColumnName("etiquetas")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.IdDono)
                    .IsRequired()
                    .HasColumnName("idDono")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Modo)
                    .IsRequired()
                    .HasColumnName("modo")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Preco).HasColumnName("preco");

                entity.Property(e => e.Quantidade).HasColumnName("quantidade");

                entity.Property(e => e.Descricao).HasColumnName("descricao");

                entity.Property(e => e.Pontuacao).HasColumnName("pontuacao");

                entity.HasOne(d => d.IdDonoNavigation)
                    .WithMany(p => p.Artigo)
                    .HasForeignKey(d => d.IdDono)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Artigo_Utilizador");
            });
            modelBuilder.Entity<AluguerInfo>(entity =>
            {
                entity.HasKey(e => e.IdArtigo);

                entity.Property(e => e.IdArtigo)
                    .HasColumnName("idArtigo")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdAluguer)
                    .HasColumnName("idAluguer");

                entity.Property(e => e.NomeArtigo)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Preco).HasColumnName("preco");

                entity.Property(e => e.Quantidade).HasColumnName("quantidade");

                entity.Property(e => e.DataFim)
                   .HasColumnName("dataFim")
                   .HasColumnType("date");

                entity.Property(e => e.DataInicio)
                    .HasColumnName("dataInicio")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.CodPostal)
                    .IsRequired()
                    .HasColumnName("codPostal")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.NPorta).HasColumnName("nPorta");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasColumnName("rua")
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Telemovel).HasColumnName("telemovel");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasColumnName("tipo")
                    .HasMaxLength(45)
                    .IsFixedLength();


            });
            modelBuilder.Entity<VendaInfo>(entity =>
            {
                entity.HasKey(e => e.IdArtigo);

                entity.Property(e => e.IdArtigo)
                    .HasColumnName("idArtigo")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdVenda)
                    .HasColumnName("idVenda");

                entity.Property(e => e.NomeArtigo)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Preco).HasColumnName("preco");

                entity.Property(e => e.Quantidade).HasColumnName("quantidade");
                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.CodPostal)
                    .IsRequired()
                    .HasColumnName("codPostal")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.NPorta).HasColumnName("nPorta");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasColumnName("rua")
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Telemovel).HasColumnName("telemovel");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasColumnName("tipo")
                    .HasMaxLength(45)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Denuncias>(entity =>
            {
                entity.HasKey(e => e.IdDenuncia);

                entity.Property(e => e.IdDenuncia)
                    .HasColumnName("idDenuncia")
                    .ValueGeneratedNever();

                entity.Property(e => e.Administrador)
                    .IsRequired()
                    .HasColumnName("administrador")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(400)
                    .IsFixedLength();

                entity.Property(e => e.IdAutor)
                    .IsRequired()
                    .HasColumnName("idAutor")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.IdDenunciado)
                    .IsRequired()
                    .HasColumnName("idDenunciado")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.HasOne(d => d.AdministradorNavigation)
                    .WithMany(p => p.Denuncias)
                    .HasForeignKey(d => d.Administrador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Denuncias_Administrador");
            });

            modelBuilder.Entity<Localizacao>(entity =>
            {
                entity.HasKey(e => e.CodigoPostal);

                entity.Property(e => e.CodigoPostal)
                    .HasColumnName("codigoPostal")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Distrito)
                    .IsRequired()
                    .HasColumnName("distrito")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Freguesia)
                    .IsRequired()
                    .HasColumnName("freguesia")
                    .HasMaxLength(45)
                    .IsFixedLength();

               
            });

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasKey(e => e.Email);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Administrador)
                    .IsRequired()
                    .HasColumnName("administrador")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Cc).HasColumnName("cc");

                entity.Property(e => e.CodPostal)
                    .IsRequired()
                    .HasColumnName("codPostal")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.ContaBancaria).HasColumnName("contaBancaria");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.NPorta).HasColumnName("nPorta");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasColumnName("rua")
                    .HasMaxLength(200)
                    .IsFixedLength();

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Pontuacao).HasColumnName("pontuacao");

                entity.Property(e => e.Telemovel).HasColumnName("telemovel");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasColumnName("tipo")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.HasOne(d => d.AdministradorNavigation)
                    .WithMany(p => p.Utilizador)
                    .HasForeignKey(d => d.Administrador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Utilizador_Administrador");

                entity.HasOne(d => d.CodPostalNavigation)
                    .WithMany(p => p.Utilizador)
                    .HasForeignKey(d => d.CodPostal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Utilizador_Localizacao");
            });

            modelBuilder.Entity<Venda>(entity =>
            {
                entity.HasKey(e => e.IdVenda);

                entity.Property(e => e.IdVenda)
                    .HasColumnName("idVenda")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdArtigo).HasColumnName("idArtigo");

                entity.Property(e => e.IdRent)
                    .IsRequired()
                    .HasColumnName("idRent")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.IdUtilizador)
                    .IsRequired()
                    .HasColumnName("idUtilizador")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Quantidade)
                    .IsRequired()
                    .HasColumnName("quantidade")
                    .HasMaxLength(45)
                    .IsFixedLength();

                entity.Property(e => e.Preco).HasColumnName("preco");

                entity.HasOne(d => d.IdArtigoNavigation)
                    .WithMany(p => p.Venda)
                    .HasForeignKey(d => d.IdArtigo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Venda_Artigo");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Venda)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Venda_Utilizador");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
