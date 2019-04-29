using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TADE.Controllers
{
    public class CandidateRegistrationController : Controller
    {
        // GET: CandidateRegistration
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult RegistrationForm()
        {
            return View();
        }
    }
}