using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
namespace Model.ViewModels
{
    public class AllPermission
    {
        public List<PermissionViewModel> ProductPermission { get; set; }
        public List<PermissionViewModel> SupplierPermission { get; set; }
        public List<PermissionViewModel> CustomerPermission { get; set; }
        public List<PermissionViewModel> CategoryPermission { get; set; }
        public List<PermissionViewModel> UserPermission { get; set; }
        public List<PermissionViewModel> OrderPermission { get; set; }
        public List<PermissionViewModel> ReceiptPermission { get; set; }
        public List<PermissionViewModel> EmployeePermission { get; set; }
        public List<PermissionViewModel> StatisticPermission { get; set; }
    }
}
