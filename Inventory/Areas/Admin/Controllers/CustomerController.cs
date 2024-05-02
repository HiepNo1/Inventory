﻿using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory.Common;

namespace Inventory.Areas.Admin.Controllers
{
    [ClearSessions("SelectedProducts", "SelectedReceipts")]
    public class CustomerController : BaseController
    {
        // GET: Admin/Customer
        [HasCredential(RoleID ="VIEW_CUSTOMER")]
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10) 
        {            
            var dao = new CustomerDao();
            var model = dao.ListAllString(searchString, page, pageSize);
            ViewBag.searchString = searchString;
            return View(model);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_CUSTOMER")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [HasCredential(RoleID = "ADD_CUSTOMER")]
        public ActionResult Create(Customer customer)
        {
            if(ModelState.IsValid)
            {
                var dao = new CustomerDao();
                long id = dao.Insert(customer);
                if(id == 0)
                {
                    ModelState.AddModelError("", "Khách hàng này đã tồn tại");
                    return View(customer);
                }
                if(id > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công");
                }
            }
            return View(customer);
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_CUSTOMER")]
        public ActionResult Edit(long id)
        {
            var cus = new CustomerDao().GetById(id);
            return View(cus);
        }
        [HttpPost]
        [HasCredential(RoleID = "EDIT_CUSTOMER")]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var dao = new CustomerDao();
                bool result = dao.Update(customer);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Sửa không thành công");
                }
            }
            return View(customer);
        }

        [HasCredential(RoleID = "DELETE_CUSTOMER")]
        public ActionResult Delete(long Id)
        {
            new CustomerDao().Delete(Id);
            return RedirectToAction("Index");
        }
    }
}