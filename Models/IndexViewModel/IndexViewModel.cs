using System;
using System.Collections.Generic;

namespace AMS.Models.IndexViewModel
{
    public class IndexViewModel
    {
        public int TotalUser { get; set; }
        public int TotalActive { get; set; }
        public int TotalInActive { get; set; }
        public List<UserProfile> listUserProfile { get; set; }
        public int TotalAsset { get; set; }
        public int TotalAssignedAsset { get; set; }
        public int TotalUnAssignedAsset { get; set; }
        public int TotalEmployee { get; set; }
        public int TotalAssetRequest { get; set; }
        public int TotalIssue { get; set; }
    }
}
