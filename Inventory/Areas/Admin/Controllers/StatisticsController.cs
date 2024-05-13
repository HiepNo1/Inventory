using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.EF;
using Model.ViewModels;
using System.Data.Entity;
using Model.Dao;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Inventory.Common;
using System.IO;
using NPOI.SS.Util;

namespace Inventory.Areas.Admin.Controllers
{
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
        public ActionResult OrderQuantityData(DateTime startDate, DateTime endDate, int? orderType)
        {
            List<string> dates = new List<string>();
            List<int> orderCounts = new List<int>();
            var endOfDay = endDate.AddDays(1).AddSeconds(-1);
            var query = db.Orders.Where(x => x.CreateDate >= startDate && x.CreateDate <= endOfDay);

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


        public ActionResult OrderDetailsData(DateTime startDate, DateTime endDate, int? orderType)
        {
            try
            {
                var orders = LstOrder(startDate, endDate, orderType);
                return Json(new { orders }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportToExcel(DateTime startDate, DateTime endDate, int? orderType)
        {
            try
            {
                string orderTypeName = "";
                if (orderType == null)
                {
                    orderTypeName = "Tất cả đơn hàng";
                }
                else
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
                            ViewBag.Error = "Loại đơn hàng không hợp lệ.";
                            return View("Error");
                    }
                }

                var orders = LstOrder(startDate, endDate, orderType);
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Orders");
                int rownum = 0;

                IRow infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("THỐNG KÊ SỐ LƯỢNG ĐƠN HÀNG THEO TỪNG LOẠI");
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Từ ngày:");
                infoRow.CreateCell(1).SetCellValue(startDate.ToString("dd-MM-yyyy"));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Đến ngày:");
                infoRow.CreateCell(1).SetCellValue(endDate.ToString("dd-MM-yyyy"));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Loại đơn hàng:");
                infoRow.CreateCell(1).SetCellValue(orderTypeName);
                infoRow = sheet.CreateRow(rownum++);

                IRow headerRow = sheet.CreateRow(rownum++);
                headerRow.CreateCell(0).SetCellValue("Mã đơn hàng");
                headerRow.CreateCell(1).SetCellValue("Tên khách hàng");
                headerRow.CreateCell(2).SetCellValue("Số điện thoại");
                headerRow.CreateCell(3).SetCellValue("Số sản phẩm");
                headerRow.CreateCell(4).SetCellValue("Phí vận chuyển");
                headerRow.CreateCell(5).SetCellValue("Ngày đặt hàng");
                headerRow.CreateCell(6).SetCellValue("Doanh thu");

                foreach (var order in orders)
                {
                    IRow dataRow = sheet.CreateRow(rownum++);
                    dataRow.CreateCell(0).SetCellValue(order.OrderCode);
                    dataRow.CreateCell(1).SetCellValue(order.CustomerName);
                    dataRow.CreateCell(2).SetCellValue(order.CustomerPhone);
                    dataRow.CreateCell(3).SetCellValue(order.ProductCount);
                    dataRow.CreateCell(4).SetCellValue(order.ShippingFee.ToString("N0") + " đ");
                    dataRow.CreateCell(5).SetCellValue(order.OrderDate.ToString("dd-MM-yyyy"));
                    dataRow.CreateCell(6).SetCellValue(order.Revenue.ToString("N0") + " đ");
                }

                for (int i = 0; i < 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                MemoryStream output = new MemoryStream();
                workbook.Write(output);
                byte[] byteArray = output.ToArray();
                return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Thống kê đơn hàng_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
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
        public ActionResult OrderRevenueData(DateTime startDate, DateTime endDate)
        {
            List<string> dates = new List<string>();
            List<decimal> orderRevenues = new List<decimal>();
            var orders = LstOrder(startDate, endDate, 4);
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

        public ActionResult OrderDetailsRevenueData(DateTime startDate, DateTime endDate)
        {
            try
            {
                var orders = LstOrder(startDate, endDate, 4);
                return Json(new { orders }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportRevenueToExcel(DateTime startDate, DateTime endDate)
        {
            try
            {
                var orders = LstOrder(startDate, endDate, 4);
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Orders");
                int rownum = 0;

                IRow infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("THỐNG KÊ DOANH THU");
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Từ ngày:");
                infoRow.CreateCell(1).SetCellValue(startDate.ToString("dd-MM-yyyy"));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Đến ngày:");
                infoRow.CreateCell(1).SetCellValue(endDate.ToString("dd-MM-yyyy"));
                infoRow = sheet.CreateRow(rownum++);

                IRow headerRow = sheet.CreateRow(rownum++);
                headerRow.CreateCell(0).SetCellValue("Mã đơn hàng");
                headerRow.CreateCell(1).SetCellValue("Tên khách hàng");
                headerRow.CreateCell(2).SetCellValue("Số điện thoại");
                headerRow.CreateCell(3).SetCellValue("Số sản phẩm");
                headerRow.CreateCell(4).SetCellValue("Phí vận chuyển");
                headerRow.CreateCell(5).SetCellValue("Ngày đặt hàng");
                headerRow.CreateCell(6).SetCellValue("Doanh thu");

                foreach (var order in orders)
                {
                    IRow dataRow = sheet.CreateRow(rownum++);
                    dataRow.CreateCell(0).SetCellValue(order.OrderCode);
                    dataRow.CreateCell(1).SetCellValue(order.CustomerName);
                    dataRow.CreateCell(2).SetCellValue(order.CustomerPhone);
                    dataRow.CreateCell(3).SetCellValue(order.ProductCount);
                    dataRow.CreateCell(4).SetCellValue(order.ShippingFee.ToString("N0") + "đ");
                    dataRow.CreateCell(5).SetCellValue(order.OrderDate.ToString("dd-MM-yyyy"));
                    dataRow.CreateCell(6).SetCellValue(order.Revenue.ToString("N0") + "đ");
                }

                for (int i = 0; i < 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                MemoryStream output = new MemoryStream();
                workbook.Write(output);
                byte[] byteArray = output.ToArray();
                return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Thống kê doanh thu_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        [HasCredential(RoleID = "PRODUCTCOUNT_STATISTIC")]
        public ActionResult ProductQuantity(OrderStatisticsViewModel model)
        {
            ViewBag.StartDate = model.StartDate;
            ViewBag.EndDate = model.EndDate;
            return View(model);
        }
        [HasCredential(RoleID = "PRODUCTCOUNT_STATISTIC")]
        public ActionResult ProductQuantityData(DateTime startDate, DateTime endDate)
        {
            try
            {
                var top10Products = LstProduct(startDate, endDate);
                var productNames = top10Products.Select(x => x.ProductName).ToList();
                var quantities = top10Products.Select(x => x.Quantity).ToList();

                return Json(new { productNames = productNames, productQuantities = quantities }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult ProductDetailsData(DateTime startDate, DateTime endDate)
        {
            try
            {
                var top10Products = LstProduct(startDate, endDate);
                return Json(new { top10Products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportProductToExcel(DateTime startDate, DateTime endDate)
        {
            try
            {
                var top10Products = LstProduct(startDate, endDate);
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Products");
                int rownum = 0;

                IRow infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("THỐNG KÊ SỐ LƯỢNG SẢN PHẨM BÁN ĐƯỢC");
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 3));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Từ ngày: " + startDate.ToString("dd-MM-yyyy"));
                infoRow = sheet.CreateRow(rownum++);
                infoRow.CreateCell(0).SetCellValue("Đến ngày: " + endDate.ToString("dd-MM-yyyy"));
                infoRow = sheet.CreateRow(rownum++);

                IRow headerRow = sheet.CreateRow(rownum++);
                headerRow.CreateCell(0).SetCellValue("Tên sản phẩm");
                headerRow.CreateCell(1).SetCellValue("Giá bán");
                headerRow.CreateCell(2).SetCellValue("Số lượng bán ra");
                headerRow.CreateCell(3).SetCellValue("Số lượng còn");

                foreach (var product in top10Products)
                {
                    IRow dataRow = sheet.CreateRow(rownum++);
                    dataRow.CreateCell(0).SetCellValue(product.ProductName);
                    dataRow.CreateCell(1).SetCellValue(product.ProductPrice.ToString("N0") + "đ");
                    dataRow.CreateCell(2).SetCellValue(product.Quantity);
                    dataRow.CreateCell(3).SetCellValue(product.StockQuantity);
                }

                for (int i = 0; i < 4; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                MemoryStream output = new MemoryStream();
                workbook.Write(output);
                byte[] byteArray = output.ToArray();
                return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Thống kê sản phẩm_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
        }
        public List<ProductInStatistics> LstProduct(DateTime startDate, DateTime endDate)
        {
            var endOfDay = endDate.AddDays(1).AddSeconds(-1);
            return (from od in db.OrderDetails
                    join o in db.Orders on od.OrderID equals o.ID
                    join p in db.Products on od.ProductID equals p.ID
                    where o.CreateDate >= startDate && o.CreateDate <= endOfDay
                    group new { od, p } by new { p.ID, p.Name, p.Image, p.Quantity, p.Price } into g
                    orderby g.Sum(x => x.od.Quantity) descending
                    select new ProductInStatistics()
                    {
                        ProductID = g.Key.ID,
                        ProductName = g.Key.Name,
                        ProductImage = g.Key.Image,
                        ProductPrice = g.Key.Price,
                        StockQuantity = g.Key.Quantity,
                        Quantity = g.Sum(x => x.od.Quantity)
                    }).Take(10).ToList();
        }

        public List<OrderInStatistics> LstOrder(DateTime startDate, DateTime endDate, int? orderType)
        {
            var endOfDay = endDate.AddDays(1).AddSeconds(-1);
            return (from o in db.Orders
                    join c in db.Customers on o.CustomerID equals c.ID
                    where o.CreateDate >= startDate && o.CreateDate <= endOfDay && (orderType == null || o.Status == orderType)
                    select new OrderInStatistics()
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
        }
    }
}