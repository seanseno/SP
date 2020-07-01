﻿using IS.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IS.Database.Entities
{
    public class Stocks : GlobalExtender
    {
        public int? Id { get; set; }
        public string ProductId { get; set; }
        public int? Stock { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int Active { get; set; }
        public string ProductName { get; set; }
    }
}
