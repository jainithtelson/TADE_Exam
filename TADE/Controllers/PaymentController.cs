using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TADE.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            // Please refer for payment http://www.west-wind.com/presentations/aspnetecommerce/aspnetecommerce.asp
            return View();
        }
        public ActionResult DonateUs()
        {
            return View();
        }
    }
}