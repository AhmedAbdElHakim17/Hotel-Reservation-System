﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRS_BussinessLogic.DTOs.Queries
{
    public class AuthDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
