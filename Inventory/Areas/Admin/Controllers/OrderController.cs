using Model.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;
using Model.EF;
using Model.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedReceipts")]
    public class OrderController : BaseController
    {
        // GET: Admin/Order
        private InventoryDbContext db = new InventoryDbContext();
        private OrderDao orderDao = new OrderDao();
        private OrderDetailDao orderDetailDao = new OrderDetailDao();
        private ProductDao productDao = new ProductDao();
        private CustomerDao customerDao = new CustomerDao();

        [ClearSessions("SelectedProducts")]
        [HasCredential(RoleID = "VIEW_ORDER")]
        public ActionResult Index(string searchString, DateTime? createDate, int? status, int page = 1, int pageSize = 10)
        {
            var model = orderDao.ListAll("", searchString, createDate, status, page, pageSize);
            ViewBag.searchString = searchString;
            ViewBag.createDate = createDate != null ? createDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.status = status;
            return View(model);
        }
        [ClearSessions("SelectedProducts")]
        [HasCredential(RoleID = "VIEW_ORDER")]
        public ActionResult Trash(string searchString, DateTime? createDate, int? status, int page = 1, int pageSize = 10)
        {
            var model = orderDao.ListAll("Trash", searchString, createDate, status, page, pageSize);
            ViewBag.searchString = searchString;
            ViewBag.createDate = createDate != null ? createDate.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.status = status;
            return View(model);
        }
        [ClearSessions("SelectedProducts")]
        [HasCredential(RoleID = "DETAIL_ORDER")]
        public ActionResult Detail(long id)
        {
            var detail = orderDao.Detail(id);
            ViewBag.lstOrderDetail = orderDetailDao.GetList(id);
            return View(detail);
        }

        [ClearSessions("SelectedProducts")]
        [HasCredential(RoleID = "DETAIL_ORDER")]
        public ActionResult TrashDetail(long id)
        {
            var detail = orderDao.Detail(id);
            ViewBag.lstOrderDetail = orderDetailDao.GetList(id);
            return View(detail);
        }
        [HasCredential(RoleID = "DETAIL_ORDER")]
        public ActionResult Destroy(long id)
        {
            if (ModelState.IsValid)
            {
                int result = new OrderDao().Destroy(id);
                if (result == 0)
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng không tồn tại");
                    return RedirectToAction("Index");
                }
                if (result == 1)
                {
                    TempData["message"] = new XMessage("success", "Hủy đơn hàng thành công");
                    return RedirectToAction("Index");
                }
                if (result == 2)
                {
                    var orderDetail = orderDetailDao.GetList(id);
                    foreach (var detail in orderDetail)
                    {
                        var product = productDao.GetByID(detail.ProductID);
                        product.Quantity += detail.Quantity;
                        productDao.Update(product);
                    }
                    TempData["message"] = new XMessage("success", "Hủy đơn hàng thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng đang vận chuyển hoặc đã hoàn thành ! Không thể hủy");
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Detail", new { id = @id });
        }
        [HasCredential(RoleID = "DETAIL_ORDER")]
        public ActionResult Confirm(long id)
        {
            if (ModelState.IsValid)
            {
                int result = new OrderDao().Confirm(id);
                if (result == 0)
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng không tồn tại");
                    return RedirectToAction("Index");
                }
                if (result == 1)
                {
                    var orderDetail = orderDetailDao.GetList(id);
                    foreach (var detail in orderDetail)
                    {
                        var product = productDao.GetByID(detail.ProductID);
                        product.Quantity -= detail.Quantity;
                        productDao.Update(product);
                    }
                    TempData["message"] = new XMessage("success", "Đơn hàng đã được xác nhận thành công");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng đã được xác nhận hoặc đang vận chuyển hoặc đã hoàn thành !");
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Detail", new { id = @id });
        }

        [HasCredential(RoleID = "DETAIL_ORDER")]
        public ActionResult Transpost(long id)
        {
            if (ModelState.IsValid)
            {
                int result = new OrderDao().Transport(id);
                if (result == 0)
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng không tồn tại");
                    return RedirectToAction("Index");
                }
                if (result == 1)
                {
                    TempData["message"] = new XMessage("success", "Đơn hàng đã được đưa vào vận chuyển");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng đã hoàn thành ! Không thể vận chuyển");
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Detail", new { id = @id });
        }

        public ActionResult ReTrash(long id)
        {
            var order = orderDao.GetByID(id);
            if (order == null)
            {
                TempData["message"] = new XMessage("danger", "Đơn hàng không tồn tại");
                return RedirectToAction("Trash");
            }
            order.Status = 1;
            orderDao.UpdateStatus(order);
            TempData["message"] = new XMessage("success", "Đơn hàng đã được khôi phục thành công");
            return RedirectToAction("Trash");
        }

        [HasCredential(RoleID = "DETAIL_ORDER")]
        public ActionResult Success(long id)
        {
            if (ModelState.IsValid)
            {
                int result = new OrderDao().Success(id);
                if (result == 0)
                {
                    TempData["message"] = new XMessage("danger", "Đơn hàng không tồn tại");
                    return RedirectToAction("Index");
                }
                if (result == 1)
                {
                    TempData["message"] = new XMessage("success", "Đơn hàng đã hoàn thành");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = new XMessage("danger", "Không thành công");
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Detail", new { id = @id });
        }

        public ActionResult Delete(long id)
        {
            bool orderToDelete = orderDao.Delete(id);
            if (orderToDelete)
            {
                TempData["message"] = new XMessage("success", "Xóa đơn hàng hủy thành công");
            }
            else
            {
                TempData["message"] = new XMessage("danger", "Đơn hàng không tồn tại");
            }
            return RedirectToAction("Index");
        }

        [HasCredential(RoleID = "ADD_ORDER")]
        [HttpGet]
        public ActionResult Create(string searchCustomer, string searchProduct)
        {
            CreateOrderViewModel orderViewModel = new CreateOrderViewModel();
            orderViewModel.SearchProduct = searchProduct;
            orderViewModel.SearchCustomer = searchCustomer;
            return View(orderViewModel);
        }

        [HasCredential(RoleID = "ADD_ORDER")]
        [HttpPost]
        public ActionResult Create(CreateOrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {

                if (orderViewModel.LstOrderDetail == null || orderViewModel.LstOrderDetail.Count == 0 || orderViewModel.Customer == null)
                {
                    TempData["message"] = new XMessage("success", "Chưa đủ dữ liệu để lưu");
                    return View(orderViewModel);
                }
                var order = new Order()
                {
                    CustomerID = orderViewModel.Customer.ID,
                    CreateBy = GetUserSession().UserName,
                    EmployeeID = 3,
                    Description = orderViewModel.Order.Description,
                    ShippingFee = orderViewModel.Order.ShippingFee,
                    PaymentStatus = orderViewModel.Order.PaymentStatus,
                    ShippingStatus = orderViewModel.Order.ShippingStatus,
                };
                if (!orderViewModel.Order.ShippingStatus && orderViewModel.Order.PaymentStatus)
                {
                    order.Status = 4;
                }
                else
                {
                    order.Status = 2;
                }
                orderDao.Insert(order);
                foreach (var detail in orderViewModel.LstOrderDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.ID,
                        ProductID = detail.ProductID,
                        Quantity = detail.Quantity
                    };
                    var product = productDao.GetByID(detail.ProductID);
                    product.Quantity -= detail.Quantity;
                    productDao.Update(product);
                    db.OrderDetails.Add(orderDetail);
                    db.SaveChanges();
                }
                return Json(new { success = true, message = "Lưu đơn hàng thành công!" });

            }
            else
            {
                return View(orderViewModel);
            }
        }

        [HttpPost]
        public ActionResult SearchCustomer(string searchCustomer)
        {
            var orderViewModel = new CreateOrderViewModel();
            orderViewModel.SearchCustomer = searchCustomer;

            if (!string.IsNullOrEmpty(searchCustomer))
            {
                var foundCustomer = customerDao.GetByPhone(searchCustomer);
                if (foundCustomer != null)
                {
                    orderViewModel.Customer = new Customer
                    {
                        ID = foundCustomer.ID,
                        Name = foundCustomer.Name,
                        Phone = foundCustomer.Phone,
                        Address = foundCustomer.Address,
                        Email = foundCustomer.Email,
                        Gender = foundCustomer.Gender
                    };
                }
                else
                {
                    TempData["messagePartial"] = new XMessage("danger", "Không tìm thấy khách hàng với số điện thoại vừa nhập !");
                }
            }

            return PartialView("_CustomerInfo", orderViewModel.Customer);
        }

        [HttpPost]
        public ActionResult SearchProduct(string searchProduct)
        {
            var orderViewModel = new CreateOrderViewModel();
            orderViewModel.SearchProduct = searchProduct;

            if (!string.IsNullOrEmpty(searchProduct))
            {
                var foundProducts = productDao.GetBySearch(searchProduct);
                if (foundProducts != null && foundProducts.Any())
                {
                    List<ProductDetailInOrder> existingProducts = Session["SelectedProducts"] as List<ProductDetailInOrder> ?? new List<ProductDetailInOrder>();

                    foreach (var product in foundProducts)
                    {
                        if (!existingProducts.Any(p => p.ProductID == product.ID))
                        {
                            existingProducts.Add(new ProductDetailInOrder { 
                                ProductID = product.ID,
                                Quantity = 1,
                                Price = product.Price * (100 - product.Sale)/100,
                                TotalPrice = product.Price * (100 - product.Sale)/100
                            });
                        }
                    }

                    Session["SelectedProducts"] = existingProducts;

                    orderViewModel.Products = existingProducts;
                    return PartialView("_OrderDetail", orderViewModel);
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
            List<ProductDetailInOrder> currentProducts = Session["SelectedProducts"] as List<ProductDetailInOrder> ?? new List<ProductDetailInOrder>();
            orderViewModel.Products = currentProducts;
            return PartialView("_OrderDetail", orderViewModel);
        }

        [HttpPost]
        public ActionResult SaveNewCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                long exist = customerDao.Insert(customer);
                if (exist == 0)
                {
                    return Json(new { success = false, message = "Khách hàng đã tồn tại trong hệ thống." });
                }
                else
                {
                    var customerInfoHtml = RenderRazorViewToString("_CustomerInfo", customer);
                    return Json(new { success = true, data = customerInfoHtml });
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

        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            List<ProductDetailInOrder> products = Session["SelectedProducts"] as List<ProductDetailInOrder>;
            if (products != null)
            {
                var productToRemove = products.FirstOrDefault(p => p.ProductID == productId);
                if (productToRemove != null)
                {
                    products.Remove(productToRemove);
                    Session["SelectedProducts"] = products;
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Failed to remove product from session." });
        }

        [HttpPost]
        public ActionResult SaveProductDetails(ProductDetailInOrder productDetail)
        {
            if (Session["SelectedProducts"] == null)
            {
                Session["SelectedProducts"] = new List<ProductDetailInOrder>();
            }

            var productDetails = (List<ProductDetailInOrder>)Session["SelectedProducts"];

            var existingProductDetail = productDetails.FirstOrDefault(p => p.ProductID == productDetail.ProductID);

            if (existingProductDetail != null)
            {
                existingProductDetail.Quantity = productDetail.Quantity;
                existingProductDetail.Price = productDetail.Price;
                existingProductDetail.TotalPrice = productDetail.Quantity * productDetail.Price;
            }
            else
            {
                productDetails.Add(productDetail);
            }

            return Json(new { success = true });
        }

        [HasCredential(RoleID = "PRINT_ORDER")]
        public ActionResult ExportPDF(long id)
        {
            var order = orderDao.Detail(id);
            MemoryStream memoryStream = new MemoryStream();
            Document document = new Document();
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font font = new Font(bf, 12);
            Font boldFont = new Font(bf, 12, Font.BOLD);

            document.Add(new Paragraph($"Mã hóa đơn: {order.Code}", font));
            document.Add(new Paragraph($"Ngày tạo: {order.CreateDate.ToString("dd/MM/yyyy")}", font));

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"Thông tin khách hàng", boldFont));
            document.Add(new Paragraph($"Tên khách hàng: {order.CustomerName}", font));
            document.Add(new Paragraph($"SĐT: {order.CustomerPhone}", font));
            document.Add(new Paragraph($"Email: {order.CustomerEmail}", font));
            document.Add(new Paragraph($"Địa chỉ nhận: {order.CustomerAddress}", font));
            document.Add(new Paragraph($"Giới tính: {GetGenderText(order.Gender)}", font));

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"Chi tiết hóa đơn", boldFont));
            document.Add(new Paragraph(" "));
            PdfPTable table = new PdfPTable(5);
            float[] widths = new float[] { 36f, 17f, 15f, 15f, 17f };
            table.SetWidths(widths);
            table.AddCell(new PdfPCell(new Phrase("Sản phẩm", font)));
            table.AddCell(new PdfPCell(new Phrase("Giá tiền", font)));
            table.AddCell(new PdfPCell(new Phrase("Giảm giá", font)));
            table.AddCell(new PdfPCell(new Phrase("Số lượng", font)));
            table.AddCell(new PdfPCell(new Phrase("Tổng tiền", font)));
            var orderDetails = orderDetailDao.GetList(id);
            decimal totalMoney = 0;
            foreach (var detail in orderDetails)
            {
                table.AddCell(new PdfPCell(new Phrase(detail.ProductName, font)));
                table.AddCell(new PdfPCell(new Phrase(detail.ProductPrice.ToString("#,##0 đ"), font)));
                table.AddCell(new PdfPCell(new Phrase(detail.Sale + "%", font)));
                table.AddCell(new PdfPCell(new Phrase(detail.Quantity.ToString(), font)));
                decimal total = detail.ProductPrice * (decimal)(1 - 0.01 * detail.Sale) * detail.Quantity;
                table.AddCell(new PdfPCell(new Phrase(total.ToString("#,##0 đ"), font)));
                totalMoney += total;
            }
            decimal totalAll = totalMoney + order.ShippingFee;
            document.Add(table);

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph($"Tổng tiền: {totalMoney.ToString("#,##0 đ")}", font));
            document.Add(new Paragraph($"Phí vận chuyển: {order.ShippingFee.ToString("#,##0 đ")}", font));
            document.Add(new Paragraph($"Thành tiền: {totalAll.ToString("#,##0 đ")}", font));
            document.Close();
            return File(memoryStream.ToArray(), "application/pdf", "Đơn hàng " + order.Code.ToString() + "__" + DateTime.Now.ToString() +".pdf");

        }
        private string GetGenderText(int? gender)
        {
            switch (gender)
            {
                case 0:
                    return "Nữ";
                case 1:
                    return "Nam";
                default:
                    return "Không xác định";
            }
        }

    }
}