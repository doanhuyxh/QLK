using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AMS.Models;
using AMS.Models.CommonViewModel;

namespace AMS.Data
{
    public class ApplicationDbContext : AuditableIdentityContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ItemDropdownListViewModel>().HasNoKey();
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }

        public DbSet<SMTPEmailSetting> SMTPEmailSetting { get; set; }
        public DbSet<SendGridSetting> SendGridSetting { get; set; }
        public DbSet<DefaultIdentityOptions> DefaultIdentityOptions { get; set; }
        public DbSet<LoginHistory> LoginHistory { get; set; }
        //AMS
        public DbSet<CreateObject> CreateObject { get; set; }
        public DbSet<CompanyInfo> CompanyInfo { get; set; }
        public DbSet<UserInfoFromBrowser> UserInfoFromBrowser { get; set; }
        public DbSet<ItemDropdownListViewModel> ItemDropdownListViewModel { get; set; }
        public DbSet<ObjectField> ObjectField { get; set; }
        public DbSet<ViewItemTemplate> ViewItemTemplate { get; set; }
        public DbSet<DisplayFieldOfTable> DisplayFieldOfTable { get; set; }
        public DbSet<RelationshipTable> RelationshipTable { get; set; }
        public DbSet<QuanLyMenu> QuanLyMenu { get; set; }



        public DbSet<NhomNguoiDung> NhomNguoiDung { get; set; }


        public DbSet<QuanLyMenuType> QuanLyMenuType { get; set; }
        public DbSet<SubViewField> SubViewField { get; set; }
        public DbSet<DisplaySubViewFieldOfTable> DisplaySubViewFieldOfTable { get; set; }
        public DbSet<DanhMucLoaiThuoc> DanhMucLoaiThuoc { get; set; }
        public DbSet<DanhMucDonVi> DanhMucDonVi { get; set; }
        public DbSet<DanhMucNuocSanXuat> DanhMucNuocSanXuat { get; set; }
        public DbSet<QuanLyKho> QuanLyKho { get; set; }
        public DbSet<NhapKho> NhapKho { get; set; }
        public DbSet<LoaiKho> LoaiKho { get; set; }
        public DbSet<DanhSachCon> DanhSachCon { get; set; }
        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Config> Config { get; set; }
        public DbSet<QuanLyDonViSanXuat> QuanLyDonViSanXuat { get; set; }
        public DbSet<QuanLyNhomNguoiDung> QuanLyNhomNguoiDung { get; set; }
        public DbSet<NguyenLieu> NguyenLieu { get; set; }
        public DbSet<NhomKhachHang> NhomKhachHang { get; set; }
        public DbSet<KhachHang> KhachHang { get; set; }
        public DbSet<KeHoachSanXuat> KeHoachSanXuat { get; set; }
        public DbSet<TheoDoiChatLuong> TheoDoiChatLuong { get; set; }
        public DbSet<QuanLyNguoiDung> QuanLyNguoiDung { get; set; }
        public DbSet<QuanLyKhoXuong> QuanLyKhoXuong { get; set; }
        public DbSet<XayDungCongThuc> XayDungCongThuc { get; set; }
        public DbSet<ListNguyenLieuInCongThuc> ListNguyenLieuInCongThuc { get; set; }
        public DbSet<NguyenLieuNhapKho> NguyenLieuNhapKho { get; set; }
        public DbSet<NguyenLieuXuatKho> NguyenLieuXuatKho { get; set; }
        public DbSet<KiemKho> KiemKho { get; set; }
        public DbSet<XuatKho> XuatKho { get; set; }
        public DbSet<QuanLyKhoXuongThucTe> QuanLyKhoXuongThucTe { get; set; }
        public DbSet<KhoXuongThucTe> KhoXuongThucTe { get; set; }
        public DbSet<HistoryNhapXuatNguyenLieu> HistoryNhapXuatNguyenLieu { get; set; }
        public DbSet<NguyenLieuKeHoach> NguyenLieuKeHoach { get; set; }
        public DbSet<NguyenLieuKiemKho> NguyenLieuKiemKho { get; set; }
        public DbSet<NguyenLieuNhapKhoLyThuyet> NguyenLieuNhapKhoLyThuyet { get; set; }
        public DbSet<NhapKhoLyThuyet> NhapKhoLyThuyet { get; set; }
        public DbSet<QuanLyThanhPham> QuanLyThanhPham { get; set; }
        public DbSet<NhapThanhPham> NhapThanhPham { get; set; }
        public DbSet<XuatThanhPham> XuatThanhPham { get; set; }
        public DbSet<ThanhPhamNhapKho> ThanhPhamNhapKho { get; set; }
        public DbSet<ThanhPhamXuatKho> ThanhPhamXuatKho { get; set; }
        public DbSet<Customproduct> Customproduct { get; set; }
        public DbSet<CustomField> CustomField { get; set; }
        public DbSet<CustomFieldInputValue> CustomFieldInputValue { get; set; }
        public DbSet<CustomFieldTotal> CustomFieldTotal { get; set; }
		public DbSet<ThongBaoChenhLechNguyenLieu> ThongBaoChenhLechNguyenLieu { get; set; }
		public DbSet<PhieuNhapThanhPham> PhieuNhapThanhPham { get; set; }
        public DbSet<ListNhapThanhPham> ListNhapThanhPham { get; set; }
        public DbSet<ListXuatThanhPham> ListXuatThanhPham { get; set; }

        public DbSet<PhieuXuatThanhPham> PhieuXuatThanhPham { get; set; }
        public DbSet<NhapKhoLyThuyet2> NhapKhoLyThuyet2 { get; set; }
        public DbSet<NguyenLieuNhapKhoLyThuyet2> NguyenLieuNhapKhoLyThuyet2 { get; set; }
        //[DBSET]

    }
}
