﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AdminViewUploadVm
    {
        public int RequestId { get; set; }
        
        public string Name { get; set; }


        public List<RequestWiseFile> requestWiseFiles { get; set; }
    }
}
