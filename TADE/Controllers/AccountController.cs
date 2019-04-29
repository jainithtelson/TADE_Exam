using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TADE.Models;
using CaptchaMvc.HtmlHelpers;
using System.IO;
using System.Web.UI;
using System.Text;

namespace TADE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private TADEDBEntities TD = new TADEDBEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private TADEDBEntities db = new TADEDBEntities();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session["CandidateId"] = null;
            Session["ExaminerId"] = null;
            Session["Administrator"] = null;
            // ViewBag.ReturnUrl = returnUrl;
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                returnUrl = Server.UrlEncode(Request.UrlReferrer.PathAndQuery);

            //if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            //{

            //}
            ViewBag.ReturnURL = returnUrl;
            return View();
                       
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            string decodedUrl = "";
            if (!string.IsNullOrEmpty(returnUrl))
                decodedUrl = Server.UrlDecode(returnUrl);

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var userdt = db.AspNetUsers.Where(a => a.UserName == model.Email).Select(u =>
                      new { userID = u.Id }).FirstOrDefault();
                    
                    var role = UserManager.GetRoles(userdt.userID).FirstOrDefault();

                    if (role == "Candidate")
                    {
                        var candDetails = db.CandidateDetails.Where(a => a.UserId == userdt.userID).Select(x => new
                        {
                            CandidateId = x.CandidateId
                        }).FirstOrDefault();
                        if (candDetails != null)
                        {
                            Session["CandidateId"] = candDetails.CandidateId;
                            if (Url.IsLocalUrl(decodedUrl) && decodedUrl != "/")
                            {
                                return Redirect(decodedUrl);
                            }
                            else
                            {
                                return RedirectToAction("StartExam", "DrivingTest");
                            }
                            
                        }
                    }
                    if (role == "Administrator")
                    {
                        Session["Administrator"] = userdt.userID;
                        if (Url.IsLocalUrl(decodedUrl) && decodedUrl != "/")
                        {
                            return Redirect(decodedUrl);
                        }
                        else
                        {
                            return RedirectToAction("DistributeCandidatesToExaminers", "Administrator");
                        }
                        
                    }
                    if (role == "Examiner")
                    {
                        var examinerDetails = db.ExaminerDetails.Where(a => a.UserId == userdt.userID).Select(x => new
                        {
                            ExaminerId = x.ExaminerId
                        }).FirstOrDefault();
                        if (examinerDetails != null)
                        {
                            Session["ExaminerId"] = examinerDetails.ExaminerId;
                            if (Url.IsLocalUrl(decodedUrl) && decodedUrl != "/")
                            {
                                return Redirect(decodedUrl);
                            }
                            else
                            {
                                return RedirectToAction("AllocatedCandidateDetails", "Examiner");
                            }
                            
                        }
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    var addroles = await UserManager.AddToRoleAsync(user.Id, model.Role);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    if (model.Role == "Administrator")
                    {
                        return RedirectToAction("DistributeCandidatesToExaminers", "Administrator");
                    }
                    if (model.Role == "Examiner")
                    {
                        ExaminerDetail examinerDetail = new Models.ExaminerDetail();
                        examinerDetail.UserId = user.Id;
                        examinerDetail.FirstName = model.FirstName;
                        examinerDetail.LastName = model.LastName;
                        examinerDetail.Phone = model.Phone;
                        examinerDetail.Status = true;
                        db.ExaminerDetails.Add(examinerDetail);
                        db.SaveChanges();
                        Session["ExaminerId"] = examinerDetail.ExaminerId;
                        return RedirectToAction("AllocatedCandidateDetails", "Examiner");
                    }
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult CandidateRegister()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CandidateRegister([Bind(Include = "CandidateId,Email,Password,FirstName,MiddleName,LastName,Year,Month,Day,Phone,AddressLine1,AddressLine2,AddressLine3,PostCode,DrivingLicenseNumber")] CandidateDetail candidateDetail)
        {
            if (this.IsCaptchaValid("Captcha is not valid"))
            {

                if (ModelState.IsValid)
                {

                    RegisterViewModel rModel = new RegisterViewModel();
                    rModel.Email = candidateDetail.Email;
                    rModel.Password = candidateDetail.Password;
                    rModel.ConfirmPassword = candidateDetail.Password;
                    rModel.Role = "Candidate";
                    var user = new ApplicationUser { UserName = rModel.Email, Email = rModel.Email };
                    var result = await UserManager.CreateAsync(user, rModel.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        var addroles = await UserManager.AddToRoleAsync(user.Id, rModel.Role);

                        DateTime dob = new DateTime(candidateDetail.Year, candidateDetail.Month, candidateDetail.Day);
                        string ip = Request.UserHostAddress;
                        Random rand = new Random();


                        candidateDetail.EmailApproved = true;
                        candidateDetail.Status = false;
                        candidateDetail.DateOfBirth = dob.ToString();
                        candidateDetail.IPAddress = ip;
                        candidateDetail.ExamId = rand.Next().ToString();
                        candidateDetail.UserId = user.Id;
                        candidateDetail.ExamAttended = false;
                        db.CandidateDetails.Add(candidateDetail);
                        db.SaveChanges();
                        Session["CandidateId"] = Convert.ToInt32(candidateDetail.CandidateId);
                        SendMail(candidateDetail);
                        //
                        return RedirectToAction("RegistrationConfirmation", "CandidateDetails");

                    }
                    AddErrors(result);

                    //   return RedirectToAction("Register", "Account", rModel);
                    //  return RedirectToAction("StartExam", "DrivingTest");
                }
            }

            ViewBag.ErrMessage = "Error: captcha is not valid.";

            return View(candidateDetail);
        }

       
        [AllowAnonymous]
        public ActionResult CompanyRegister()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CompanyRegister([Bind(Include = "CompanyId,CompanyName,AddressLIne1,	AddressLIne2,AddressLIne3,PostCode,Email,Password,PhoneNumber,ContactPerson,Position,CompanyLogo,NatureofBusiness,CompanySize,RoleType")] CompanyDetail companyDetail)
        {
            if (this.IsCaptchaValid("Captcha is not valid"))
            {

                if (ModelState.IsValid)
                {

                    RegisterViewModel rModel = new RegisterViewModel();
                    rModel.Email = companyDetail.Email;
                    rModel.Password = companyDetail.Password;
                    rModel.ConfirmPassword = companyDetail.Password;
                    rModel.Role = companyDetail.RoleType;
                    var user = new ApplicationUser { UserName = rModel.Email, Email = rModel.Email };
                    var result = await UserManager.CreateAsync(user, rModel.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        var addroles = await UserManager.AddToRoleAsync(user.Id, rModel.Role);

                        //DateTime dob = new DateTime(candidateDetail.Year, candidateDetail.Month, candidateDetail.Day);
                        string ip = Request.UserHostAddress;
                        Random rand = new Random();



                        companyDetail.Status = true;

                        companyDetail.UserId = user.Id;

                        db.CompanyDetails.Add(companyDetail);
                        db.SaveChanges();
                        //Session["CandidateId"] = Convert.ToInt32(candidateDetail.CandidateId);
                        // SendMail(candidateDetail);
                        //
                        return RedirectToAction("RegistrationConfirmation", "EmployerDetails");

                    }
                    AddErrors(result);

                    //   return RedirectToAction("Register", "Account", rModel);
                    //  return RedirectToAction("StartExam", "DrivingTest");
                }
            }

            ViewBag.ErrMessage = "Error: captcha is not valid.";

            return View(companyDetail);
        }










        public bool SendMail(CandidateDetail candidateDetail)
        {
            string message = "";
            //Here we will save data to the database
            
            //check username available
            byte[] bytes = null;
            MailModel mm = new Models.MailModel();
            mm.To = candidateDetail.Email;
            mm.From = "jainith@gmail.com";
            mm.Body = "Dear " + candidateDetail.FirstName + ", " + MailBody(candidateDetail.ExamId.ToString());
            mm.Subject = "TADE registration confirmation";
            message = "Confirmation";
            SendMailBLL sm = new SendMailBLL();
            sm.SendMail(mm, bytes);
            return true;
        }
        public string MailBody(string registrationNumber)
        {
            string mailBody = " <div>Thanks for registering with TADE.&nbsp;</div> " +
" <div>&nbsp;</div> " +
" <div>Your registration number is <b>" + registrationNumber  + "</b>. You would get a booking reference number after you have booked for the exam. But please read the information given below carefully before booking for the exam.&nbsp;</div> " +
" <div>&nbsp;</div> " +
" <div>We don't take responsibility of any loss occurring to you or anyone in any form if you try the TADE exam. The loss includes &nbsp;but not limited to the items like money, health, time, mobile data etc. You or any person representing you are not authorized to ask any claim from us under any circumstances. TADE exam team are authorized to change the rules or regulations or statements or exam system at anytime with out proper notice or not.&nbsp;</div> " +
" <div>&nbsp;</div> " +
" <span style='text - decoration: underline; '>TO ATTEND THE EXAM &nbsp;- TICK ITEMS</span> " +
   " <div>Please check your lap top/computer camera, microphone are working. Please heck if the charger is available.</div> " +
   " <div>&nbsp;</div> " +
   " <div>Please check your mobile camera and microphone are working also if the charger is available.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Please install Google chrome or Firefox browser on your computer to write the exam. Our exam video would not work with internet explorer or any other browser. Apologies for that.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>On your mobile which you intend to use for the exam as a back camera, please install the video sharing app ' TADEexammobileapp' from our website. Open the app before the exam and enter your booking reference number. Before the candidate approval time(before the 30 minutes of exam start time), you would be able to enter the exam booking reference number in to the app and you would be able to see the video from your mobile on your computer to adjust the mobile phone. You can uninstall the app after finishing your exam.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Plan a place where nobody(including kids, housemates, pets etc.) do not interfere with you while you attend the exam.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>The exam room or location shall not have any other sound from TV or any other mechanical or electrical appliances.</div> " +
   " <div>&nbsp;</div> " +
   " <div>The exam room/location &nbsp;shall be quiet.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>There shall be plenty of light in the exam room for the examiners to see you properly. But the lights shall not fall directly on the laptop camera or the mobile camera.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Think how you can fix your mobile behind you and how you can adjust it so that the examiner can see the full area.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>If you are sick please don't take the exam until you are fit to do so. You have the choice to postpone the exam date or time and cancel the exam if you feel unwell. We do not take responsibility for your health under any circumstances &nbsp;if anything happens to you as you are sitting at a seat till the exam would be finished.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Keep your computer and mobile full charge, and if it is possible keep these items charging depending up on the battery capacity. It may be good to connect the mobile and laptop to charger as the total exam duration may be 2.5 hours including candidate approval.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Please have somebody at your place to face any unexpected visitor or attend phone call etc. (eg. somebody knocks at door, mail, milkman, phone calls, kitchen, washing machine etc. )You are not allowed to leave your seat once the candidate approval has been done. If you leave, you would be out of the exam, and you may need to book for exam for a different date.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Before the candidate approval begins(in the 30 minutes before the exam time), please adjust all items ready and be comfortable and be ready for the exam and if you wish please use loo before the candidate approval begins. &nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>We suggest you to use a mouse instead of touch pad for the exam. You are allowed to use touch pad also. But touch pad may be more time consuming than mouse to click and scroll during the exam. It is your choice to select the suitable item.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Keep the driving licence card ready to show the examiner.</div> " +
   " <div>&nbsp;</div> " +
   " <div>We do not allow anyone to cover your head with anything. This is very strict. We cannot allow you to wear anything above your ear level(This includes cap, turban, burkha etc. ). It is very easy to wear a cap and have a cordless microphone and attend the exam. Hence we don't allow you to wear anything above ear level. In case you cannot attend the exam with out any head cover, you may not try for the exam. We respect religions, beliefs but we want to conduct the exam in a way that there shall not by any chance for cheating the examiner. We need to keep our quality level for the exam to keep the ISO certification. Only those who can show your head with out any cover need to appear for the exam.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>You are not allowed to eat anything or drink during the candidate approval time which may last longer than 30 minutes in some cases. So please be prepared.&nbsp;</div> " +
   " <div>You shall not be using chewing gum.</div> " +
   " <div>&nbsp;</div> " +
   " <div>Once the candidate approval has been done, you can eat or drink as you wish.&nbsp;</div> " +
   " <div>We do not allow loo breaks. You can leave your seat and book for exam once again.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>Please wear proper dress. We do not allow nudity. Please respect the examiner. The examiner has full right to exit you from the exam on the basis of improper dressing.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div>If you remove your microphone, switch off your laptop camera, mobile camera, or leave your seat, you would be exit from the exam. If you feel that you have to stop the exam, please inform the examiner by clicking the button indicated as ' please let me stop the exam' . Examiner would allow you to leave the exam. If you leave the exam with out examiner's approval, you may be barred to book the exam for next 10 or more days depending up on the examiners discretion. We expect you to respect the examiner and the exam system.&nbsp;</div> " +
   " <div>&nbsp;</div> " +
   " <div><span style='text - decoration: underline; '>Candidate Approval</span></div> " +
      " <div>Candidate approval begins 30 minutes before the exam time. You should be logged in before 30 minutes of the exam time.&nbsp;</div> " +
      " <div>Before the candidate approval begins please make sure your mobile and laptop are connected to charger so that you don't have to spend time to leave your seat to connect charger. Also before the candidate approval begins, please make sure you have logged in to the video app on your mobile phone.&nbsp;</div> " +
      " <div>&nbsp;</div> " +
      " <div>Candidate approval tick items</div> " +
      " <div>&nbsp;</div> " +
      " <div>You would see instructions from examiner in the below given sequence for your approval.&nbsp;</div> " +
      " <div> " +
      " <ol> " +
      " <li>Checking your front camera. Examiner would ask you to rotate your head so that the examiner can see properly your ear area to make sure you are not using any equipment &nbsp;to communicate with anyone during the exam. We do not give voice commands during the exam. So we don't expect you to wear any hearing aid also. In case if you have any situation to hear anything during the exam, please ask the help of a friend or relative to be at your place to meet such circumstances(not to tell you answers for the exam). Examiner would tell you if you need to adjust the room light. Concentrated light shall not fall on the laptop camera or the mobile camera. Examiner may ask you to rotate your front camera left or right, up or down depending up on circumstances.&nbsp;</li> " +
      " <li>Examiner may ask you to rotate your laptop camera or computer camera to see your surroundings to make sure there is nobody to assist you or &nbsp;help you with answers for the exam.&nbsp;</li> " +
      " <li>Examiner checks the back camera and may ask you to rotate it upward or downward, left or right.</li> " +
      " <li>Examiner would ask you to show your face clearly on the front camera. You would have to show the the driving licence card photo page also. The driving licence card should be held steady so that the examiner can verify the photo on the driving licence card with your face.&nbsp;</li> " +
      " <li>You would see a number(1-9) on your screen. Examiner releases the number. You should read that number loud as soon as you see it on your front screen. Examiner needs to check if your microphones are working.</li> " +
      " </ol> " +
      " <div>After all these checks has been done, examiner would confirm that your identity has been verified.&nbsp;</div> " +
      " </div> " +
      " <div>&nbsp;</div> " +
      " <div>Then examiner releases the exam for you. and you have the choice of starting the exam with in 10 minutes of the release of the exam. If you do not start the exam with in 10 minutes of release, you would be out of the exam automatically and you can book again a different date.&nbsp;</div> " +
      " <div>&nbsp;</div> " +
      " <div><span style='text - decoration: underline; '>Question sample</span></div> " +
         " <div>Please refer sample questions and videos given in the TADE home page.&nbsp;</div> " +
         " <div>&nbsp;</div> " +
         " <div><span style='text - decoration: underline; '>Exam results</span></div> " +
            " <div>Once you have finished your exam, we would send your score (if you have one), to your email address given.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Please be prepared and kindly respect our examiners and software engineers who have developed this site.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Our aim is to refresh your knowledge in motorway driving and reduce the accidents and help economy.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>We are conducting this exam with out charging any fees. You can attend the exam with out paying any money and get result also. However there is a dedicated examiner sitting at the other side spending time and observing you to make sure the exam meets the quality standards. Because of the quality standards, the insurance agents reduce your premium and you get discounts from various parties.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Time is money and the examiners are spending time to help you to write the exam. We would be very grateful if you can kindly donate any amount to us to meet our expenses. We accept donations of any amount. You can donate 10 pence, 50 pence, &pound;1, &pound;2, &pound;5, &pound;10, &pound;20, &pound;50, &pound;100, &pound;200 or any amount you wish.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>You spend hundreds for MOT, hundreds for insurance, hundreds for road tax, hundreds for annual service, and your car costs thousands and your life's value cannot be measured. We help all to refresh the knowledge and our aim would be achieved if we can save one life on the motorway in a year. Our exam may make other drivers more responsible and you may get less traffic block if all drivers are following rules correctly.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>We request you to kindly donate any amount you can to achieve this aim. We would publish your donating amount if you tick the relevant box while donating.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Thanks for registering for the exam.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>If you accept the information given above you may proceed to book the exam.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Please be prepared, read this email twice or more &nbsp;to understand everything.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Good luck from the TADE team.&nbsp;</div> " +
            " <div>&nbsp;</div> " +
            " <div>Telson Augustine B Tech MSc CEng MICE</div> ";
            return mailBody;
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}