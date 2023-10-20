using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Models.ConfigViewModel
{
    public class ConfigCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public int Id { get; set; }
        
                                    [Display (Name = "config1")] 
                                    public string Config1 { get; set; }
                                
        public static implicit operator ConfigCRUDViewModel(Config _Config)
        {
            return new ConfigCRUDViewModel
            {
                Id = _Config.Id,
                Config1 = _Config.Config1,
                CreatedDate = _Config.CreatedDate,
                ModifiedDate = _Config.ModifiedDate,
                CreatedBy = _Config.CreatedBy,
                ModifiedBy = _Config.ModifiedBy,
                Cancelled = _Config.Cancelled,
            };
        }

        public static implicit operator Config(ConfigCRUDViewModel vm)
        {
            return new Config
            {
                Id = vm.Id,
                Config1 = vm.Config1,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
