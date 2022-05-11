using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMNHAC.Models;
namespace AMNHAC.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(String timkiem,FormCollection form)
        {            
            timkiem = form["Id"] ;
            if(timkiem != "")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }
    }

    
}