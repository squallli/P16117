﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class Employee
    {
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
		public string GroupName { get; set; }
    }
}