using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebBanHang.Models
{
    public partial class TshopContext : DbContext
    {
        public TshopContext()
        {
        }

        public TshopContext(DbContextOptions<TshopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BaiViet> BaiViets { get; set; } = null!;
        public virtual DbSet<BaiVietSanPham> BaiVietSanPhams { get; set; } = null!;
        public virtual DbSet<Banner> Banners { get; set; } = null!;
        public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;
        public virtual DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = null!;
        public virtual DbSet<ChiTietSanPham> ChiTietSanPhams { get; set; } = null!;
        public virtual DbSet<DanhGium> DanhGia { get; set; } = null!;
        public virtual DbSet<DanhMuc> DanhMucs { get; set; } = null!;
        public virtual DbSet<DiaChi> DiaChis { get; set; } = null!;
        public virtual DbSet<DiaChiCuaHang> DiaChiCuaHangs { get; set; } = null!;
        public virtual DbSet<DonHang> DonHangs { get; set; } = null!;
        public virtual DbSet<DonHangKhuyenMai> DonHangKhuyenMais { get; set; } = null!;
        public virtual DbSet<GioHang> GioHangs { get; set; } = null!;
        public virtual DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; } = null!;
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; } = null!;
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; } = null!;
        public virtual DbSet<LuuVetDoiTra> LuuVetDoiTras { get; set; } = null!;
        public virtual DbSet<MauSac> MauSacs { get; set; } = null!;
        public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; } = null!;
        public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; } = null!;
        public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; } = null!;
        public virtual DbSet<SanPham> SanPhams { get; set; } = null!;
        public virtual DbSet<SanPhamKhuyenMai> SanPhamKhuyenMais { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public virtual DbSet<TaiKhoanKhuyenMai> TaiKhoanKhuyenMais { get; set; } = null!;
        public virtual DbSet<VaiTro> VaiTros { get; set; } = null!;
        public virtual DbSet<YeuCauDoiTra> YeuCauDoiTras { get; set; } = null!;
        public virtual DbSet<YeuThich> YeuThiches { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-TOPRUO27\\MAY1;Database=Tshop;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaiViet>(entity =>
            {
                entity.HasKey(e => e.MaBaiViet)
                    .HasName("PK__BaiViet__AEDD564753B0BF98");

                entity.ToTable("BaiViet");

                entity.Property(e => e.LinkBaiViet)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Loai).HasDefaultValueSql("((1))");

                entity.Property(e => e.TenBaiViet).HasMaxLength(255);
            });

            modelBuilder.Entity<BaiVietSanPham>(entity =>
            {
                entity.HasKey(e => e.MaBvsp)
                    .HasName("PK__BaiVietS__845612962D74B739");

                entity.ToTable("BaiVietSanPham");

                entity.HasIndex(e => new { e.MaBaiViet, e.MaSp }, "UQ_BVSP")
                    .IsUnique();

                entity.Property(e => e.MaBvsp).HasColumnName("MaBVSP");

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.HasOne(d => d.MaBaiVietNavigation)
                    .WithMany(p => p.BaiVietSanPhams)
                    .HasForeignKey(d => d.MaBaiViet)
                    .HasConstraintName("FK_BVSP_BaiViet");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.BaiVietSanPhams)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BVSP_SanPham");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.HasKey(e => e.MaBanner)
                    .HasName("PK__Banner__508B4A4920E2039C");

                entity.ToTable("Banner");

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TenBanner).HasMaxLength(100);

                entity.Property(e => e.ViTriBanner).HasMaxLength(50);

                entity.HasOne(d => d.MaBaiVietNavigation)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.MaBaiViet)
                    .HasConstraintName("FK_Banner_BaiViet");
            });

            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.HasKey(e => e.MaCtdh)
                    .HasName("PK__ChiTietD__1E4E40F0321C58B5");

                entity.ToTable("ChiTietDonHang");

                entity.Property(e => e.MaCtdh).HasColumnName("MaCTDH");

                entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.HasOne(d => d.MaCtspNavigation)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.MaCtsp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietDo__MaCTS__787EE5A0");

                entity.HasOne(d => d.MaDhNavigation)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.MaDh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietDon__MaDH__778AC167");
            });

            modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
            {
                entity.HasKey(e => e.MaCtpn)
                    .HasName("PK__ChiTietP__1E4E60751F76D05F");

                entity.ToTable("ChiTietPhieuNhap");

                entity.Property(e => e.MaCtpn).HasColumnName("MaCTPN");

                entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");

                entity.Property(e => e.MaPn).HasColumnName("MaPN");

                entity.HasOne(d => d.MaCtspNavigation)
                    .WithMany(p => p.ChiTietPhieuNhaps)
                    .HasForeignKey(d => d.MaCtsp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietPh__MaCTS__1332DBDC");

                entity.HasOne(d => d.MaPnNavigation)
                    .WithMany(p => p.ChiTietPhieuNhaps)
                    .HasForeignKey(d => d.MaPn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietPhi__MaPN__123EB7A3");
            });

            modelBuilder.Entity<ChiTietSanPham>(entity =>
            {
                entity.HasKey(e => e.MaCtsp)
                    .HasName("PK__ChiTietS__1E4FCECD980EDF4E");

                entity.ToTable("ChiTietSanPham");

                entity.HasIndex(e => new { e.MaSp, e.MaMau, e.MaSize }, "UQ_CTSP")
                    .IsUnique();

                entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");

                entity.Property(e => e.MaMau)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaSize)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.HasOne(d => d.MaMauNavigation)
                    .WithMany(p => p.ChiTietSanPhams)
                    .HasForeignKey(d => d.MaMau)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CTSP_MauSac");

                entity.HasOne(d => d.MaSizeNavigation)
                    .WithMany(p => p.ChiTietSanPhams)
                    .HasForeignKey(d => d.MaSize)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CTSP_Size");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.ChiTietSanPhams)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CTSP_SanPham");
            });

            modelBuilder.Entity<DanhGium>(entity =>
            {
                entity.HasKey(e => e.MaDanhGia)
                    .HasName("PK__DanhGia__AA9515BFD5B0AB28");

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.NgayDanhGia)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NoiDung).HasMaxLength(1000);

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhGia_SanPham");

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhGia_TaiKhoan");
            });

            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.HasKey(e => e.MaDanhMuc)
                    .HasName("PK__DanhMuc__B37508879595CF85");

                entity.ToTable("DanhMuc");

                entity.Property(e => e.MaDanhMuc)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
            });

            modelBuilder.Entity<DiaChi>(entity =>
            {
                entity.HasKey(e => e.MaDiaChi)
                    .HasName("PK__DiaChi__EB61213EF74297D0");

                entity.ToTable("DiaChi");

                entity.Property(e => e.DiaChiCuThe).HasMaxLength(255);

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.PhuongXa).HasMaxLength(100);

                entity.Property(e => e.QuanHuyen).HasMaxLength(100);

                entity.Property(e => e.SdtNguoiNhan)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("sdtNguoiNhan");

                entity.Property(e => e.TinhThanh).HasMaxLength(100);

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.DiaChis)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiaChi_TaiKhoan");
            });

            modelBuilder.Entity<DiaChiCuaHang>(entity =>
            {
                entity.HasKey(e => e.MaCh)
                    .HasName("PK__DiaChiCu__27258E007D046D37");

                entity.ToTable("DiaChiCuaHang");

                entity.Property(e => e.MaCh).HasColumnName("MaCH");

                entity.Property(e => e.Dc)
                    .HasMaxLength(100)
                    .HasColumnName("DC");

                entity.Property(e => e.LinkGgmap)
                    .HasColumnType("text")
                    .HasColumnName("linkGGmap");
            });

            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.MaDh)
                    .HasName("PK__DonHang__27258661556DE9BD");

                entity.ToTable("DonHang");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.MaPttt).HasColumnName("MaPTTT");

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.NgayDat)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TrangThai).HasMaxLength(30);

                entity.Property(e => e.TrangThaiThanhToan)
                    .HasMaxLength(30)
                    .HasDefaultValueSql("(N'Chưa thanh toán')");

                entity.HasOne(d => d.MaDiaChiNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaDiaChi)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DonHang__MaDiaCh__72C60C4A");

                entity.HasOne(d => d.MaPtttNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaPttt)
                    .HasConstraintName("FK_DonHang_PTTT");

                entity.HasOne(d => d.MaTaiXeNavigation)
                    .WithMany(p => p.DonHangMaTaiXeNavigations)
                    .HasForeignKey(d => d.MaTaiXe)
                    .HasConstraintName("FK_DonHang_TaiXe");

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.DonHangMaTkNavigations)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__DonHang__MaTK__71D1E811");
            });

            modelBuilder.Entity<DonHangKhuyenMai>(entity =>
            {
                entity.HasKey(e => new { e.MaDh, e.MaKm });

                entity.ToTable("DonHang_KhuyenMai");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.MaKm).HasColumnName("MaKM");

                entity.HasOne(d => d.MaDhNavigation)
                    .WithMany(p => p.DonHangKhuyenMais)
                    .HasForeignKey(d => d.MaDh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DHKM_DonHang");

                entity.HasOne(d => d.MaKmNavigation)
                    .WithMany(p => p.DonHangKhuyenMais)
                    .HasForeignKey(d => d.MaKm)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DHKM_KhuyenMai");
            });

            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.HasKey(e => new { e.MaTk, e.MaCtsp });

                entity.ToTable("GioHang");

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");

                entity.Property(e => e.NgayThem)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoLuong).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.MaCtspNavigation)
                    .WithMany(p => p.GioHangs)
                    .HasForeignKey(d => d.MaCtsp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GioHang_ChiTietSanPham");

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.GioHangs)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GioHang_TaiKhoan");
            });

            modelBuilder.Entity<HinhAnhSanPham>(entity =>
            {
                entity.HasKey(e => e.MaHa)
                    .HasName("PK__HinhAnhS__2725A6FDD49A47B8");

                entity.ToTable("HinhAnhSanPham");

                entity.Property(e => e.MaHa).HasColumnName("MaHA");

                entity.Property(e => e.DuongDanAnh).HasMaxLength(255);

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.HinhAnhSanPhams)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HinhAnh_SanPham");
            });

            modelBuilder.Entity<KhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaKm)
                    .HasName("PK__KhuyenMa__2725CF15BBEC7ED6");

                entity.ToTable("KhuyenMai");

                entity.Property(e => e.MaKm).HasColumnName("MaKM");

                entity.Property(e => e.CodeKm)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CodeKM");

                entity.Property(e => e.DonToiThieu).HasDefaultValueSql("((0))");

                entity.Property(e => e.LoaiGiam)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiKm)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LoaiKM");

                entity.Property(e => e.NgayBatDau).HasColumnType("datetime");

                entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");

                entity.Property(e => e.TenKm)
                    .HasMaxLength(100)
                    .HasColumnName("TenKM");

                entity.Property(e => e.TrangThai)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.MaLoai)
                    .HasName("PK__LoaiSanP__730A57590E23B75C");

                entity.ToTable("LoaiSanPham");

                entity.Property(e => e.MaLoai)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.MaDanhMuc)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenLoai).HasMaxLength(50);

                entity.HasOne(d => d.MaDanhMucNavigation)
                    .WithMany(p => p.LoaiSanPhams)
                    .HasForeignKey(d => d.MaDanhMuc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoaiSanPham_DanhMuc");
            });

            modelBuilder.Entity<LuuVetDoiTra>(entity =>
            {
                entity.HasKey(e => e.MaLuuVet)
                    .HasName("PK__LuuVetDo__385E843FD49AD330");

                entity.ToTable("LuuVetDoiTra");

                entity.Property(e => e.MaCtsp).HasColumnName("MaCTSP");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.NgayThem)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MaCtspNavigation)
                    .WithMany(p => p.LuuVetDoiTras)
                    .HasForeignKey(d => d.MaCtsp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuuVetDoiTra_ChiTietSanPham");

                entity.HasOne(d => d.MaDhNavigation)
                    .WithMany(p => p.LuuVetDoiTras)
                    .HasForeignKey(d => d.MaDh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuuVetDoiTra_DonHang");

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.LuuVetDoiTras)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LuuVetDoiTra_TaiKhoan");
            });

            modelBuilder.Entity<MauSac>(entity =>
            {
                entity.HasKey(e => e.MaMau)
                    .HasName("PK__MauSac__3A5BBB7D43BC8B7F");

                entity.ToTable("MauSac");

                entity.Property(e => e.MaMau)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenMau).HasMaxLength(50);
            });

            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.HasKey(e => e.MaNcc)
                    .HasName("PK__NhaCungC__3A185DEB1EE4DBF1");

                entity.ToTable("NhaCungCap");

                entity.Property(e => e.MaNcc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaNCC");

                entity.Property(e => e.DiaChi).HasMaxLength(255);

                entity.Property(e => e.SoDienThoai)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TenNcc)
                    .HasMaxLength(100)
                    .HasColumnName("TenNCC");
            });

            modelBuilder.Entity<PhieuNhap>(entity =>
            {
                entity.HasKey(e => e.MaPn)
                    .HasName("PK__PhieuNha__2725E7F08FCF83ED");

                entity.ToTable("PhieuNhap");

                entity.Property(e => e.MaPn).HasColumnName("MaPN");

                entity.Property(e => e.MaNcc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaNCC");

                entity.Property(e => e.NgayNhap)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MaNccNavigation)
                    .WithMany(p => p.PhieuNhaps)
                    .HasForeignKey(d => d.MaNcc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PhieuNhap__MaNCC__0F624AF8");
            });

            modelBuilder.Entity<PhuongThucThanhToan>(entity =>
            {
                entity.HasKey(e => e.MaPttt)
                    .HasName("PK__PhuongTh__B30A2802D79C37D6");

                entity.ToTable("PhuongThucThanhToan");

                entity.Property(e => e.MaPttt).HasColumnName("MaPTTT");

                entity.Property(e => e.TenPttt)
                    .HasMaxLength(50)
                    .HasColumnName("TenPTTT");
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.MaSp)
                    .HasName("PK__SanPham__2725081C546B9FF6");

                entity.ToTable("SanPham");

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.Property(e => e.MaLoai)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenSp)
                    .HasMaxLength(200)
                    .HasColumnName("TenSP");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaLoaiNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaLoai)
                    .HasConstraintName("FK_SanPham_LoaiSanPham");
            });

            modelBuilder.Entity<SanPhamKhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaSpkm);

                entity.ToTable("SanPhamKhuyenMai");

                entity.HasIndex(e => new { e.MaSp, e.MaKm }, "UQ_SanPham_KhuyenMai")
                    .IsUnique();

                entity.Property(e => e.MaSpkm).HasColumnName("MaSPKM");

                entity.Property(e => e.MaKm).HasColumnName("MaKM");

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.HasOne(d => d.MaKmNavigation)
                    .WithMany(p => p.SanPhamKhuyenMais)
                    .HasForeignKey(d => d.MaKm)
                    .HasConstraintName("FK_SPKM_KhuyenMai");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.SanPhamKhuyenMais)
                    .HasForeignKey(d => d.MaSp)
                    .HasConstraintName("FK_SPKM_SanPham");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.HasKey(e => e.MaSize)
                    .HasName("PK__Size__A787E7ED38BEE2E3");

                entity.ToTable("Size");

                entity.Property(e => e.MaSize)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TenSize).HasMaxLength(20);
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.MaTk)
                    .HasName("PK__TaiKhoan__27250070397FA4D2");

                entity.ToTable("TaiKhoan");

                entity.HasIndex(e => e.SoDienThoai, "UQ_TaiKhoan_SoDienThoai")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D10534CBB52ADD")
                    .IsUnique();

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.HanToken).HasColumnType("datetime");

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.MatKhau)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoDienThoai)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.TokenXacNhan).HasMaxLength(200);

                entity.Property(e => e.TrangThai)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.MaDiaChiNavigation)
                    .WithMany(p => p.TaiKhoans)
                    .HasForeignKey(d => d.MaDiaChi)
                    .HasConstraintName("FK_TaiKhoan_DiaChi");

                entity.HasOne(d => d.MaVaiTroNavigation)
                    .WithMany(p => p.TaiKhoans)
                    .HasForeignKey(d => d.MaVaiTro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TaiKhoan_VaiTro");
            });

            modelBuilder.Entity<TaiKhoanKhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaTkkm)
                    .HasName("PK__TaiKhoan__B50CD23D095D244E");

                entity.ToTable("TaiKhoanKhuyenMai");

                entity.HasIndex(e => new { e.MaTk, e.MaKm }, "UQ_TKKM")
                    .IsUnique();

                entity.Property(e => e.MaTkkm).HasColumnName("MaTKKM");

                entity.Property(e => e.MaKm).HasColumnName("MaKM");

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.NgaySuDung)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MaKmNavigation)
                    .WithMany(p => p.TaiKhoanKhuyenMais)
                    .HasForeignKey(d => d.MaKm)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TKKM_KhuyenMai");

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.TaiKhoanKhuyenMais)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TKKM_TaiKhoan");
            });

            modelBuilder.Entity<VaiTro>(entity =>
            {
                entity.HasKey(e => e.MaVaiTro)
                    .HasName("PK__VaiTro__C24C41CF0F325330");

                entity.ToTable("VaiTro");

                entity.HasIndex(e => e.TenVaiTro, "UQ__VaiTro__1DA558149E2C249B")
                    .IsUnique();

                entity.Property(e => e.TenVaiTro).HasMaxLength(50);
            });

            modelBuilder.Entity<YeuCauDoiTra>(entity =>
            {
                entity.HasKey(e => e.MaDoiTra)
                    .HasName("PK__YeuCauDo__5F75908CC2573F71");

                entity.ToTable("YeuCauDoiTra");

                entity.Property(e => e.LyDo).HasMaxLength(500);

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.NgayYeuCau)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaDhNavigation)
                    .WithMany(p => p.YeuCauDoiTras)
                    .HasForeignKey(d => d.MaDh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__YeuCauDoiT__MaDH__17036CC0");
            });

            modelBuilder.Entity<YeuThich>(entity =>
            {
                entity.HasKey(e => new { e.MaTk, e.MaSp })
                    .HasName("PK__YeuThich__F55750F192C72458");

                entity.ToTable("YeuThich");

                entity.Property(e => e.MaTk).HasColumnName("MaTK");

                entity.Property(e => e.MaSp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MaSP");

                entity.Property(e => e.NgayThem)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.YeuThiches)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_YeuThich_SanPham");

                entity.HasOne(d => d.MaTkNavigation)
                    .WithMany(p => p.YeuThiches)
                    .HasForeignKey(d => d.MaTk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_YeuThich_TaiKhoan");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
