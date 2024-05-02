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
                    List<Product> existingProducts = Session["SelectedProducts"] as List<Product> ?? new List<Product>();

                    foreach (var product in foundProducts)
                    {
                        if (!existingProducts.Any(p => p.ID == product.ID))
                        {
                            existingProducts.Add(product);
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
            List<Product> currentProducts = Session["SelectedProducts"] as List<Product> ?? new List<Product>();
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
            List<Product> products = Session["SelectedProducts"] as List<Product>;
            if (products != null)
            {
                var productToRemove = products.FirstOrDefault(p => p.ID == productId);
                if (productToRemove != null)
                {
                    products.Remove(productToRemove);
                    Session["SelectedProducts"] = products;
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Failed to remove product from session." });
        }
    }
}