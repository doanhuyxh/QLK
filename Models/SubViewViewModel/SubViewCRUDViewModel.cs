using System.Collections;
using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.DisplayFieldOfTableViewModel;
using AMS.Models.DisplaySubViewFieldOfTableViewModel;
using Services;
using Windows.Graphics;

namespace AMS.Models.SubViewViewModel
{
    public class SubViewCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        public int? OjbectFieldId { get; set; }
        public string TenTruong { get; set; }
        public int? DoLon { get; set; }
        public bool HienThiTrongBang { get; set; }
        public bool BatBuocNhap { get; set; }
        public bool? TreeView { get; set; }

        public string KieuDuLieu { get; set; }
        public string Label { get; set; }
        public List<DisplaySubViewFieldOfTableCRUDViewModel>? ListFieds { get; set; }
        public List<DisplaySubViewFieldOfTable>? ListDisplaySubViewFieldOfTable { get; set; }

        public static implicit operator SubViewCRUDViewModel(SubViewField _SubView)
        {
            return new SubViewCRUDViewModel
            {
                Id = _SubView.Id,
                Label = _SubView.Label,
                OjbectFieldId = _SubView.OjbectFieldId,
                TenTruong = _SubView.TenTruong,
                HienThiTrongBang = _SubView.HienThiTrongBang,
                BatBuocNhap = _SubView.BatBuocNhap,
                DoLon = _SubView.DoLon,
                KieuDuLieu = _SubView.KieuDuLieu,
                CreatedDate = _SubView.CreatedDate,
                ModifiedDate = _SubView.ModifiedDate,
                CreatedBy = _SubView.CreatedBy,
                ModifiedBy = _SubView.ModifiedBy,
                Cancelled = _SubView.Cancelled,
            };
        }

        public static implicit operator SubViewField(SubViewCRUDViewModel vm)
        {
            return new SubViewField
            {
                Id = vm.Id,
                OjbectFieldId = vm.OjbectFieldId,
                Label = vm.Label,
                TenTruong = vm.TenTruong,
                DoLon = vm.DoLon,
                KieuDuLieu = vm.KieuDuLieu,
                HienThiTrongBang = vm.HienThiTrongBang,
                BatBuocNhap = vm.BatBuocNhap,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
