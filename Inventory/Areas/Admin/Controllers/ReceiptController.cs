using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Model.Dao;
using Model.EF;
using Model.ViewModels;
using PagedList;

namespace Inventory.Areas.Admin.Controllers
{
    public class ReceiptController : BaseController
    {
        private InventoryDbContext db = new InventoryDbContext();
        private ReceiptDao receiptDao = new ReceiptDao();
        private ReceiptDetailDao receiptDetailDao = new ReceiptDetailDao();
        private SupplierDao supplierDao = new SupplierDao();
        private ProductDao productDao = new ProductDao();
        private EmployeeDao employeeDao = new EmployeeDao();
        // GET: Admin/Receipt
        [HasCredential(RoleID = "VIEW_RECEIPT")]
        public ActionResult Index(string searchString, bool? paymentStatus, DateTime? createDate, int page = 1, int pageSize = 5)
        {
            var receipt = receiptDao.GetList("", searchString, paymentStatus, createDate, page, pageSize);
            ViewBag.searchString = searchString;
            ViewBag.createDate = createDate != null ? createDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.PaymentStatus = paymentStatus;
            return View(receipt);
        }
        [HasCredential(RoleID = "DETAIL_RECEIPT")]
        public ActionResult Detail(long id)
        {
            var detail = receiptDao.Detail(id);
            ViewBag.lstReceiptDetail = receiptDetailDao.GetList(id);
            return View(detail);
        }

        [HasCredential(RoleID = "ADD_RECEIPT")]
        [HttpGet]
        public ActionResult Create(string searchSupplier, string searchProduct, string searchEmployee)
        {
            Session.Remove("SelectedReceipts");
            CreateReceiptViewModel receiptViewModel = new CreateReceiptViewModel();
            receiptViewModel.SearchProduct = searchProduct;
            receiptViewModel.SearchSupplier = searchSupplier;
            receiptViewModel.SearchEmployee = searchEmployee;
            return View(receiptViewModel);
        }

        [HasCredential(RoleID = "ADD_RECEIPT")]
        [HttpPost]
        public ActionResult Create(CreateReceiptViewModel receiptViewModel)
        {
            if (ModelState.IsValid)
            {

                if (receiptViewModel.LstReceiptDetail == null || receiptViewModel.LstReceiptDetail.Count == 0 || receiptViewModel.Supplier == null || receiptViewModel.Employee == null)
                {
                    TempData["message"] = new XMessage("danger", "Chưa đủ dữ liệu để lưu");
                    return View(receiptViewModel);
                }
                var receipt = new Receipt()
                {
                    SupplierID = receiptViewModel.Supplier.ID,
                    CreateBy = GetUserSession().UserName,
                    EmployeeID = receiptViewModel.Employee.ID,
                    Description = receiptViewModel.Receipt.Description,
                    PaymentStatus = receiptViewModel.Receipt.PaymentStatus,
                    Payment = receiptViewModel.Receipt.Payment

                };
                receiptDao.Insert(receipt);
                foreach (var detail in receiptViewModel.LstReceiptDetail)
                {
                    var receiptDetail = new ReceiptDetail
                    {
                        ReceiptID = receipt.ID,
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity,
                        ImportPrice = detail.ImportPrice
                    };
                    var product = productDao.GetByID(detail.ProductID);
                        product.Quantity += detail.Quantity;
                        productDao.Update(product);
                    db.ReceiptDetails.Add(receiptDetail);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Lưu đơn hàng thành công!" });

            }
            else
            {
                return View(receiptViewModel);
            }
        }

        [HttpPost]
        public ActionResult SearchSupplier(string searchSupplier)
        {
            var receiptViewModel = new CreateReceiptViewModel();
            receiptViewModel.SearchSupplier = searchSupplier;

            if (!string.IsNullOrEmpty(searchSupplier))
            {
                var foundSupplier = supplierDao.GetByPhone(searchSupplier);
                if (foundSupplier != null)
                {
                    receiptViewModel.Supplier = new Supplier
                    {
                        ID = foundSupplier.ID,
                        Name = foundSupplier.Name,
                        Phone = foundSupplier.Phone,
                        Address = foundSupplier.Address,
                        Email = foundSupplier.Email,
                    };
                }
                else
                {
                    TempData["messagePartial"] = new XMessage("danger", "Không tìm thấy khách hàng với số điện thoại vừa nhập !");
                }
            }

            return PartialView("_SupplierInfo", receiptViewModel.Supplier);
        }

        [HttpPost]
        public ActionResult SearchEmployee(string searchEmployee)
        {
            var receiptViewModel = new CreateReceiptViewModel();
            receiptViewModel.SearchSupplier = searchEmployee;

            if (!string.IsNullOrEmpty(searchEmployee))
            {
                var foundEmployee = employeeDao.GetByPhone(searchEmployee);
                if (foundEmployee != null)
                {
                    receiptViewModel.Employee = new Employee
                    {
                        ID = foundEmployee.ID,
                        Name = foundEmployee.Name,
                        Phone = foundEmployee.Phone,
                        Address = foundEmployee.Address,
                        Email = foundEmployee.Email,
                    };
                }
                else
                {
                    TempData["messagePartial"] = new XMessage("danger", "Không tìm thấy nhân viên với số điện thoại vừa nhập !");
                }
            }

            return PartialView("_EmployeeInfo", receiptViewModel.Employee);
        }

        [HttpPost]
        public ActionResult SearchProduct(string searchProduct)
        {
            var receiptViewModel = new CreateReceiptViewModel();
            receiptViewModel.SearchProduct = searchProduct;

            if (!string.IsNullOrEmpty(searchProduct))
            {
                var foundProducts = productDao.GetBySearch(searchProduct);
                if (foundProducts != null && foundProducts.Any())
                {
                    List<ProductDetailInReceipt> existingProducts = Session["SelectedReceipts"] as List<ProductDetailInReceipt> ?? new List<ProductDetailInReceipt>();

                    foreach (var product in foundProducts)
                    {
                        if (!existingProducts.Any(p => p.ProductID == product.ID))
                        {
                            existingProducts.Add(new ProductDetailInReceipt { 
                                ProductID = product.ID,
                                ImportPrice = 0,
                                ImportQuantity = 1,
                                TotalPrice = 0
                            });
                        }
                    }

                    Session["SelectedReceipts"] = existingProducts;

                    receiptViewModel.Products = existingProducts;
                    return PartialView("_ReceiptDetail", receiptViewModel);
                }
                else
                {
                    TempData["messagePartial"] = new XMessage("danger", "Không tìm thấy sản phẩm vừa nhập !");
                }
            }
            else
            {
                TempData["messagePartial"] = new XMessage("danger", "Vui lòng nhập tên hoặc mã sản phẩm.");
            }
            List<ProductDetailInReceipt> currentProducts = Session["SelectedReceipts"] as List<ProductDetailInReceipt> ?? new List<ProductDetailInReceipt>();
            receiptViewModel.Products = currentProducts;
            return PartialView("_ReceiptDetail", receiptViewModel);
        }

        [HasCredential(RoleID = "ADD_SUPPLIER")]
        [HttpPost]
        public ActionResult SaveNewSupplier(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                long exist = supplierDao.Insert(supplier);
                if (exist == 0)
                {
                    return Json(new { success = false, message = "Nhà cung cấp đã tồn tại trong hệ thống." });
                }
                else
                {
                    var supplierInfoHtml = RenderRazorViewToString("_SupplierInfo", supplier);
                    return Json(new { success = true, data = supplierInfoHtml });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        [HasCredential(RoleID = "ADD_PRODUCT")]
        [HttpPost]
        public ActionResult SaveNewProduct(Product product, HttpPostedFileBase fileImage)
        {
            if (ModelState.IsValid)
            {
                if (fileImage != null && fileImage.ContentLength > 0)
                {
                    string rootFolder = Server.MapPath("/Images/");
                    string pathImage = rootFolder + fileImage.FileName;
                    fileImage.SaveAs(pathImage);
                    product.Image = "/Images/" + fileImage.FileName;
                }
                product.CreateBy = GetUserSession().UserName;
                long id = productDao.Insert(product);
                if (id == 0)
                {
                    return Json(new { success = false, message = "Sản phẩm này đã tồn tại" });
                }
                if (id > 0)
                {
                    List<ProductDetailInReceipt> existingProducts = Session["SelectedReceipts"] as List<ProductDetailInReceipt> ?? new List<ProductDetailInReceipt>();

                    if (!existingProducts.Any(p => p.ProductID == product.ID))
                    {
                        existingProducts.Add(new ProductDetailInReceipt
                        {
                            ProductID = product.ID,
                            ImportPrice = 0,
                            ImportQuantity = 1
                        });
                    }

                    Session["SelectedReceipts"] = existingProducts;

                    var receiptViewModel = new CreateReceiptViewModel
                    {
                        Products = existingProducts
                    };
                    var productHtml = RenderRazorViewToString("_ReceiptDetail", receiptViewModel);
                    return Json(new { success = true, data = productHtml });
                }
                else
                {
                    return Json(new { success = false, message = "Thêm sản phẩm không thành công." });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            List<ProductDetailInReceipt> products = Session["SelectedReceipts"] as List<ProductDetailInReceipt>;
            if (products != null)
            {
                var productToRemove = products.FirstOrDefault(p => p.ProductID == productId);
                if (productToRemove != null)
                {
                    products.Remove(productToRemove);
                    Session["SelectedReceipts"] = products;
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Failed to remove product from session." });
        }

        [HttpPost]
        public ActionResult SaveProductDetails(ProductDetailInReceipt productDetail)
        {
            if (Session["SelectedReceipts"] == null)
            {
                Session["SelectedReceipts"] = new List<ProductDetailInReceipt>();
            }

            var productDetails = (List<ProductDetailInReceipt>)Session["SelectedReceipts"];

            var existingProductDetail = productDetails.FirstOrDefault(p => p.ProductID == productDetail.ProductID);

            if (existingProductDetail != null)
            {
                existingProductDetail.ImportQuantity = productDetail.ImportQuantity;
                existingProductDetail.ImportPrice = productDetail.ImportPrice;
                existingProductDetail.TotalPrice = productDetail.ImportQuantity * productDetail.ImportPrice;
            }
            else
            {
                productDetails.Add(productDetail);
            }

            return Json(new { success = true });
        }
    }
}