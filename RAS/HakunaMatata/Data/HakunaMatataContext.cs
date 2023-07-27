using System;
using HakunaMatata.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HakunaMatata.Data
{
    public partial class HakunaMatataContext : DbContext
    {
        public HakunaMatataContext()
        {
        }

        public HakunaMatataContext(DbContextOptions<HakunaMatataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AboutUs> AboutUs { get; set; }
        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Faq> Faq { get; set; }
        public virtual DbSet<Forum> Forum { get; set; }
        public virtual DbSet<Level> Level { get; set; }
        public virtual DbSet<Like> Like { get; set; }
        public virtual DbSet<Map> Map { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<HistoryPayment> HistoryPayment { get; set; }
        public virtual DbSet<Newspicture> Newspicture { get; set; }
        public virtual DbSet<Picture> Picture { get; set; }
        public virtual DbSet<Policy> Policy { get; set; }
        public virtual DbSet<Rating> Rating { get; set; }
        public virtual DbSet<RealEstate> RealEstate { get; set; }
        public virtual DbSet<RealEstateDetail> RealEstateDetail { get; set; }
        public virtual DbSet<RealEstateType> RealEstateType { get; set; }
        public virtual DbSet<SocialLogin> SocialLogin { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }
        public virtual DbSet<Wishlist> Wishlist { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=DESKTOP-8AASNAI;Database=RAS_final;User Id=sa;Password=12345678;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AboutUs>(entity =>
            {
                entity.ToTable("ABOUT_US");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("ntext");
            });

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.ToTable("AGENT");

                entity.Property(e => e.ActiveKey)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.AgentName).HasMaxLength(100);

                entity.Property(e => e.ConfirmPhoneNumber)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Facebook).HasMaxLength(100);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LoginName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ResetPasswordKey)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Zalo).HasMaxLength(100);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Agent)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AGENT__LevelId__00200768");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("CITY");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_AGENT");

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.ForumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comment_Forum");
            });

            modelBuilder.Entity<HistoryPayment>(entity =>
            {
                entity.HasKey(e => e.PaymentID); // Specify the primary key

                entity.Property(e => e.Coin)
                    .IsRequired();

                entity.Property(e => e.PaymentDate)
                    .IsRequired()
                    .HasColumnType("datetime");

            });





            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("DISTRICT");

                entity.Property(e => e.DistrictName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.District)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK__DISTRICT__CityId__02FC7413");
            });

            modelBuilder.Entity<Faq>(entity =>
            {
                entity.ToTable("FAQ");

                entity.Property(e => e.Answer).HasColumnType("ntext");

                entity.Property(e => e.Question).HasColumnType("ntext");
            });

            modelBuilder.Entity<Forum>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Forum)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Forum_AGENT");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("LEVEL");

                entity.Property(e => e.LevelName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Like)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Like_AGENT");

                entity.HasOne(d => d.Forum)
                    .WithMany(p => p.Like)
                    .HasForeignKey(d => d.ForumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Like_Forum");
            });

            modelBuilder.Entity<Map>(entity =>
            {
                entity.ToTable("MAP");

                entity.HasIndex(e => e.RealEstateId)
                    .HasName("UQ__MAP__C03786347F48CB15")
                    .IsUnique();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");

                entity.Property(e => e.Longtitude).HasColumnType("decimal(9, 6)");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Map)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK__MAP__CityId__04E4BC85");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Map)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK__MAP__DistrictId__07C12930");

                entity.HasOne(d => d.RealEstate)
                    .WithOne(p => p.Map)
                    .HasForeignKey<Map>(d => d.RealEstateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__MAP__RealEstateI__49C3F6B7");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Map)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK__MAP__WardId__0A9D95DB");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.NewsBody)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsFixedLength();

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_News_AGENT");
            });

            modelBuilder.Entity<Newspicture>(entity =>
            {
                entity.ToTable("NEWSPICTURE");

                entity.Property(e => e.NewsID).HasColumnName("NewsID");

                entity.Property(e => e.PictureName).HasMaxLength(50);

                entity.Property(e => e.URL)
                    .HasColumnName("URL")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.News)
                    .WithMany(p => p.Newspicture)
                    .HasForeignKey(d => d.NewsID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NEWSPICTURE_News");
            });

            modelBuilder.Entity<Picture>(entity =>
            {
                entity.ToTable("PICTURE");

                entity.Property(e => e.PictureName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("URL")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.RealEstate)
                    .WithMany(p => p.Picture)
                    .HasForeignKey(d => d.RealEstateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__PICTURE__RealEst__4CA06362");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.ToTable("POLICY");

                entity.Property(e => e.PolicyContent).HasColumnType("ntext");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("RATING");

                entity.HasOne(d => d.RealEstate)
                    .WithMany(p => p.Rating)
                    .HasForeignKey(d => d.RealEstateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RATING__RealEsta__0D7A0286");
            });

            modelBuilder.Entity<RealEstate>(entity =>
            {
                entity.ToTable("REAL_ESTATE");

                entity.Property(e => e.BeginTime).HasDefaultValueSql("('2019-12-21T00:00:00.0000000')");

                entity.Property(e => e.ExprireTime).HasColumnType("datetime");

                entity.Property(e => e.IsAvaiable)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.IsConfirm)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.PostTime).HasColumnType("datetime");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.RealEstate)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__REAL_ESTA__Agent__3F466844");

                entity.HasOne(d => d.ReaEstateType)
                    .WithMany(p => p.RealEstate)
                    .HasForeignKey(d => d.RealEstateTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__REAL_ESTA__ReaEs__3E52440B");
            });

            modelBuilder.Entity<RealEstateDetail>(entity =>
            {
                entity.ToTable("REAL_ESTATE_DETAIL");

                entity.HasIndex(e => e.RealEstateId)
                    .HasName("UQ__REAL_EST__C03786341696F05E")
                    .IsUnique();

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.HasPrivateWc).HasColumnName("HasPrivateWC");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SecurityCamera)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Title).HasMaxLength(300);

                entity.Property(e => e.WifiPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.RealEstate)
                    .WithOne(p => p.RealEstateDetail)
                    .HasForeignKey<RealEstateDetail>(d => d.RealEstateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__REAL_ESTA__RealE__5629CD9C");
            });

            modelBuilder.Entity<RealEstateType>(entity =>
            {
                entity.ToTable("REAL_ESTATE_TYPE");

                entity.Property(e => e.RealEstateTypeName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<SocialLogin>(entity =>
            {
                entity.HasKey(e => e.ProviderKey)
                    .HasName("PK__SOCIAL_L__8DE43C5E36C1E621");

                entity.ToTable("SOCIAL_LOGIN");

                entity.Property(e => e.ProviderKey).HasMaxLength(300);

                entity.Property(e => e.Provider)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SocialLogin)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SOCIAL_LO__UserI__10566F31");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.ToTable("WARD");

                entity.Property(e => e.WardName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Ward)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK__WARD__DistrictId__1332DBDC");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("date");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Wishlist)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_Wishlist_AGENT");

                entity.HasOne(d => d.RealEstate)
                    .WithMany(p => p.Wishlist)
                    .HasForeignKey(d => d.RealEstateId)
                    .HasConstraintName("FK_Wishlist_REAL_ESTATE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
