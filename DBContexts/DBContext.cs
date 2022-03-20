using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ImageTool.DBModels;

namespace ImageTool.DBContexts
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<CollectionImageLink> CollectionImageLinks { get; set; }
        public virtual DbSet<Exif> Exifs { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<FolderImageLink> FolderImageLinks { get; set; }
        public virtual DbSet<Image> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.ToTable("Collection");

                entity.HasIndex(e => e.Id, "Collection_index_7");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<CollectionImageLink>(entity =>
            {
                entity.ToTable("CollectionImageLink");

                entity.HasIndex(e => e.ImageId, "CollectionImageLink_index_10");

                entity.HasIndex(e => e.Id, "CollectionImageLink_index_8");

                entity.HasIndex(e => e.CollectionId, "CollectionImageLink_index_9");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.CollectionId).HasColumnType("int(11)");

                entity.Property(e => e.ImageId).HasColumnType("int(11)");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.CollectionImageLinks)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CollectionImageLink_ibfk_1");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.CollectionImageLinks)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CollectionImageLink_ibfk_2");
            });

            modelBuilder.Entity<Exif>(entity =>
            {
                entity.ToTable("Exif");

                entity.HasIndex(e => e.Id, "Exif_index_6");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Aperture).HasMaxLength(128);

                entity.Property(e => e.DateTime).HasMaxLength(128);

                entity.Property(e => e.ExposureTime).HasMaxLength(128);

                entity.Property(e => e.FocalLength).HasMaxLength(128);

                entity.Property(e => e.Iso)
                    .HasColumnType("int(11)")
                    .HasColumnName("ISO");

                entity.Property(e => e.LensInfo).HasMaxLength(128);

                entity.Property(e => e.Manufacturer).HasMaxLength(128);

                entity.Property(e => e.Model).HasMaxLength(128);
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.ToTable("Folder");

                entity.HasIndex(e => e.Id, "Folder_index_0");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.PhotoSetId)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<FolderImageLink>(entity =>
            {
                entity.ToTable("FolderImageLink");

                entity.HasIndex(e => e.Id, "FolderImageLink_index_1");

                entity.HasIndex(e => e.FolderId, "FolderImageLink_index_2");

                entity.HasIndex(e => e.ImageId, "FolderImageLink_index_3");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.FolderId).HasColumnType("int(11)");

                entity.Property(e => e.ImageId).HasColumnType("int(11)");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.FolderImageLinks)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FolderImageLink_ibfk_1");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.FolderImageLinks)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FolderImageLink_ibfk_2");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.HasIndex(e => e.Id, "Image_index_4");

                entity.HasIndex(e => e.ExifId, "Image_index_5");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.ExifId).HasColumnType("int(11)");

                entity.Property(e => e.LargePath)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.OriginalPath)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.PhotoId)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.ThumbnailPath)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.HasOne(d => d.Exif)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.ExifId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Image_ibfk_1");
            });
        }
    }
}