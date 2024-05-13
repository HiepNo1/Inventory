﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;

namespace Model.ViewModels
{
    public class CreateReceiptViewModel
    {
        public Supplier Supplier { get; set; }
        public Employee Employee { get; set; }
        public List<ReceiptDetail> LstReceiptDetail { get; set; }
        public Receipt Receipt { get; set; }
        public List<ProductDetailInReceipt> Products { get; set; }
        public string SearchProduct { get; set; }
        public string SearchSupplier { get; set; }
        public string SearchEmployee { get; set; }
    }
}
