using System.Collections;
using System.ComponentModel.DataAnnotations;
using AMS.Models;
using AMS.Models.DisplayFieldOfTableViewModel;
using AMS.Models.SubViewViewModel;
using Services;

namespace AMS.Models.ObjectFieldViewModel
{
    public class ObjectFieldCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        public int CreateObjectId { get; set; }
        public string TenTruong { get; set; }
        public int? DoLon { get; set; }
        public bool HienThiTrongBang { get; set; }
        public bool BatBuocNhap { get; set; }
        public bool? TreeView { get; set; }
        public string KieuDuLieu { get; set; }
        public int? RowIndex { get; set; }
        public int? ColumnIndex { get; set; }
        public string Label { get; set; }
        public string? TenBang { get; set; }
        public string? LabelTenBang { get; set; }
        public bool? ShowSubItem { get; set; }
        public List<DisplayFieldOfTableCRUDViewModel>? ListFieds { get; set; }
        public List<int>? subItemsNeedDelete { get; set; }
        public List<SubViewCRUDViewModel>? sub_items { get; set; }
        public string? json_sub_items { get; set; }
        public List<DisplayFieldOfTable>? ListDisplayFieldOfTable { get; set; }

        public static implicit operator ObjectFieldCRUDViewModel(ObjectField _ObjectField)
        {
            return new ObjectFieldCRUDViewModel
            {
                Id = _ObjectField.Id,
                CreateObjectId = _ObjectField.CreateObjectId,
                TenBang = _ObjectField.TenBang,
                LabelTenBang = _ObjectField.LabelTenBang,
                Label = _ObjectField.Label,
                RowIndex = _ObjectField.RowIndex,
                ColumnIndex = _ObjectField.ColumnIndex,
                TenTruong = _ObjectField.TenTruong,
                HienThiTrongBang = _ObjectField.HienThiTrongBang,
                BatBuocNhap = _ObjectField.BatBuocNhap,
                DoLon = _ObjectField.DoLon,
                KieuDuLieu = _ObjectField.KieuDuLieu,
                CreatedDate = _ObjectField.CreatedDate,
                ModifiedDate = _ObjectField.ModifiedDate,
                CreatedBy = _ObjectField.CreatedBy,
                ModifiedBy = _ObjectField.ModifiedBy,
                Cancelled = _ObjectField.Cancelled,
            };
        }

        public static implicit operator ObjectField(ObjectFieldCRUDViewModel vm)
        {
            return new ObjectField
            {
                Id = vm.Id,
                CreateObjectId = vm.CreateObjectId,
                Label = vm.Label,
                LabelTenBang = vm.LabelTenBang,
                TenBang = vm.TenBang,
                RowIndex = vm.RowIndex,
                ColumnIndex = vm.ColumnIndex,
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
