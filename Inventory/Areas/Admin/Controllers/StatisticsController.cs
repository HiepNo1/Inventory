using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.EF;
using Model.ViewModels;
using System.Data.Entity;
using Model.Dao;
using OfficeOpenXml;
using System.IO;
using Inventory.Common;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class StatisticsController : BaseController
    {
        private InventoryDbContext db = new InventoryDbContext();
        private OrderDao orderDao = new OrderDao();

        [HasCredential(RoleID = "ORDERCOUNT_STATISTIC")]
        public ActionResult OrderQuantity(OrderStatisticsViewModel model)
        {
            ViewBag.StartDate = model.StartDate;
            ViewBag.EndDate = model.EndDate;
            ViewBag.OrderType = model.OrderType;
            return View(model);
        }

        [HasCredential(RoleID = "ORDERCOUNT_STATISTIC")]
        public ActionResult OrderQuantityData(DateTime? startDate, DateTime? endDate, int? orderType)
        {
            List<string> dates = new List<string>();
            List<int> orderCounts = new List<int>();

            var query = db.Orders.Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate);

            if (orderType != null)
            {
                query = query.Where(x => x.Status == orderType);
            }
            var orderquantity = query
                            .GroupBy(x => DbFunctions.TruncateTime(x.CreateDate))
                            .Select(g => new { Date = g.Key, OrderCount = g.Count() })
                            .OrderBy(o => o.Date)
                            .ToList();

            foreach (var item in orderquantity)
            {
                dates.Add(item.Date.Value.Date.ToString("yyyy-MM-dd"));
                orderCounts.Add(item.OrderCount);
            }
            return Json(new { dates = dates, orderCounts = orderCounts }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OrderDetailsData(DateTime? startDate, DateTime? endDate, int? orderType)
        {
            try
            {
                var orders = (from o in db.Orders
                              join c in db.Customers on o.CustomerID equals c.ID
                              where o.CreateDate >= startDate && o.CreateDate <= endDate && (orderType == null || o.Status == orderType)
                              select new
                              {
                                  OrderCode = o.Code,
                                  CustomerName = c.Name,
                                  CustomerPhone = c.Phone,
                                  ProductCount = (from od in db.OrderDetails where od.OrderID == o.ID select od.Quantity).Sum(),
                                  ShippingFee = o.ShippingFee,
                                  OrderDate = o.CreateDate,
                                  Revenue = (from od in db.OrderDetails
                                             join p in db.Products on od.ProductID equals p.ID
                                             where od.OrderID == o.ID
                                             select od.Quantity * p.Price).Sum() + o.ShippingFee
                              }).ToList();
                return Json(new { orders }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportToExcel(DateTime? startDate, DateTime? endDate, int? orderType)
        {
            try
            {
                string orderTypeName = "";
                if (orderType != null)
                {
                    switch (orderType)
                    {
                        case 1:
                            orderTypeName = "Đơn hàng mới";
                            break;
                        case 2:
                            orderTypeName = "Đã xác nhận";
                            break;
                        case 3:
                            orderTypeName = "Đang vận chuyển";
                            break;
                        case 4:
                            orderTypeName = "Hoàn thành";
                            break;
                        case 0:
                            orderTypeName = "Đơn hàng bị hủy";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    orderTypeName = "Tất cả loại đơn hàng";
                }

                var orders = (from o in db.Orders
                              join c in db.Customers on o.CustomerID equals c.ID
                              where o.CreateDate >= startDate && o.CreateDate <= endDate && (orderType == null || o.Status == orderType)
                              select new
                              {
                                  OrderCode = o.Code,
                                  CustomerName = c.Name,
                                  CustomerPhone = c.Phone,
                                  ProductCount = (from od in db.OrderDetails where od.OrderID == o.ID select od.Quantity).Sum(),
                                  ShippingFee = o.ShippingFee,
                                  OrderDate = o.CreateDate,
                                  Revenue = (from od in db.OrderDetails
                                             join p in db.Products on od.ProductID equals p.ID
                                             where od.OrderID == o.ID
                                             select od.Quantity * p.Price).Sum() + o.ShippingFee
                              }).ToList();

                var ordersWithSTT = orders.Select((order, index) => new
                {
                    STT = index + 1,
                    order.OrderCode,
                    order.CustomerName,
                    order.CustomerPhone,
                    order.ProductCount,
                    order.ShippingFee,
                    OrderDate = order.OrderDate.ToString("yyyy-MM-dd"),
                    Revenue = string.Format("{0:N0} VNĐ", order.Revenue),
                }).ToList();

                ExcelPackage excel = new ExcelPackage();
                var worksheet = excel.Workbook.Worksheets.Add("Orders");

                worksheet.Cells["A1"].Value = "Loại đơn hàng: " + orderTypeName;
                worksheet.Cells["A2"].Value = "Từ ngày: " + startDate?.ToString("yyyy-MM-dd") ?? "Không xác định";
                worksheet.Cells["A3"].Value = "Đến ngày: " + endDate?.ToString("yyyy-MM-dd") ?? "Không xác định";
                worksheet.Cells["A5"].Value = "STT";
                worksheet.Cells["B5"].Value = "Mã đơn hàng";
                worksheet.Cells["C5"].Value = "Tên khách hàng";
                worksheet.Cells["D5"].Value = "Số điện thoại";
                worksheet.Cells["E5"].Value = "Số sản phẩm";
                worksheet.Cells["F5"].Value = "Phí vận chuyển";
                worksheet.Cells["G5"].Value = "Ngày đặt hàng";
                worksheet.Cells["H5"].Value = "Doanh thu";

                int row = 6;
                foreach (var order in ordersWithSTT)
                {
                    worksheet.Cells[row, 1].Value = order.STT;
                    worksheet.Cells[row, 2].Value = order.OrderCode;
                    worksheet.Cells[row, 3].Value = order.CustomerName;
                    worksheet.Cells[row, 4].Value = order.CustomerPhone;
                    worksheet.Cells[row, 5].Value = order.ProductCount;
                    worksheet.Cells[row, 6].Value = order.ShippingFee;
                    worksheet.Cells[row, 7].Value = order.OrderDate;
                    worksheet.Cells[row, 8].Value = order.Revenue;
                    row++;
                }

                worksheet.Cells["A:H"].AutoFitColumns();

                byte[] excelBytes = excel.GetAsByteArray();
                string fileName = "OrderDetails_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }

        [HasCredential(RoleID = "REVENUE_STATISTIC")]
        public ActionResult OrderRevenue(OrderStatisticsViewModel model)
        {
            ViewBag.StartDate = model.StartDate;
            ViewBag.EndDate = model.EndDate;
            return View(model);
        }

        [HasCredential(RoleID = "REVENUE_STATISTIC")]
        public ActionResult OrderRevenueData(DateTime? startDate, DateTime? endDate)
        {
            List<string> dates = new List<string>();
            List<decimal> orderRevenues = new List<decimal>();
            var orders = (from o in db.Orders
                          join c in db.Customers on o.CustomerID equals c.ID
                          where o.CreateDate >= startDate && o.CreateDate <= endDate && o.Status == 4
                          select new
                          {
                              OrderCode = o.Code,
                              CustomerName = c.Name,
                              CustomerPhone = c.Phone,
                              ProductCount = (from od in db.OrderDetails where od.OrderID == o.ID select od.Quantity).Sum(),
                              ShippingFee = o.ShippingFee,
                              OrderDate = o.CreateDate,
                              Revenue = (from od in db.OrderDetails
                                         join p in db.Products on od.ProductID equals p.ID
                                         where od.OrderID == o.ID
                                         select od.Quantity * p.Price).Sum() + o.ShippingFee
                          }).ToList();

            var orderData = orders
                            .GroupBy(x => x.OrderDate)
                            .Select(g => new { Date = g.Key, OrderRevenues = g.Sum(o => o.Revenue) })
                            .OrderBy(o => o.Date)
                            .ToList();

            foreach (var item in orderData)
            {
                dates.Add(item.Date.ToString("yyyy-MM-dd"));
                orderRevenues.Add(item.OrderRevenues);
            }
            return Json(new { dates = dates, orderRevenues = orderRevenues }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderDetailsRevenueData(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var orders = (from o in db.Orders
                              join c in db.Customers on o.CustomerID equals c.ID
                              where o.CreateDate >= startDate && o.CreateDate <= endDate && o.Status == 4
                              select new
                              {
                                  OrderCode = o.Code,
                                  CustomerName = c.Name,
                                  CustomerPhone = c.Phone,
                                  ProductCount = (from od in db.OrderDetails where od.OrderID == o.ID select od.Quantity).Sum(),
                                  ShippingFee = o.ShippingFee,
                                  OrderDate = o.CreateDate,
                                  Revenue = (from od in db.OrderDetails
                                             join p in db.Products on od.ProductID equals p.ID
                                             where od.OrderID == o.ID
                                             select od.Quantity * p.Price).Sum() + o.ShippingFee
                              }).ToList();
                return Json(new { orders }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HasCredential(RoleID = "PRODUCTCOUNT_STATISTIC")]
        public ActionResult ProductQuantity(OrderStatisticsViewModel model)
        {
            ViewBag.StartDate = model.StartDate;
            ViewBag.EndDate = model.EndDate;
            return View(model);
        }
        public ActionResult SalesChart(DateTime startDate, DateTime endDate)
        {
            var topProducts = (from od in db.OrderDetails
                               join o in db.Orders on od.OrderID equals o.ID
                               where o.CreateDate >= startDate && o.CreateDate <= endDate
                               group od by new { od.ProductID, o.CreateDate.Date } into g
                               select new
                               {
                                   ProductID = g.Key.ProductID,
                                   TotalQuantity = g.Sum(od => od.Quantity),
                                   ProductName = db.Products.FirstOrDefault(p => p.ID == g.Key.ProductID).Name,
                                   SalesByDate = g.Select(gr => new { Date = g.Key.Date, Quantity = g.Sum(ood => ood.Quantity) })
                               })
                   .OrderByDescending(g => g.TotalQuantity)
                   .Take(10)
                   .ToList();

            var chartData = new
            {
                ProductNames = topProducts.Select(p => p.ProductName).ToArray(),
                SalesByDate = topProducts.Select(p => p.SalesByDate.Select(s => s.Quantity).ToArray()).ToList()
            };

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

    }
}