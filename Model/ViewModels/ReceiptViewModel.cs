using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels
{
    public class ReceiptViewModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierAddress { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhone { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeAddress { get; set; }
        public int? EmployeeGender { get; set; }
        public bool? Payment { get; set; }
        public bool PaymentStatus { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public DateTime CreateDate { get; set; }       
    }
}
