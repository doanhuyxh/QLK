﻿using System;

namespace AMS.Models.CommonViewModel
{
    public class JsonResultViewModel
    {
        public int Id { get; set; }
        public string AlertMessage { get; set; }
        public string CurrentURL { get; set; }
        public bool IsSuccess { get; set; }
        public dynamic ModelObject { get; set; }
    }
}
