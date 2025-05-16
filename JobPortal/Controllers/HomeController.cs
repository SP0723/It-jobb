using JobPortal.Models;
using JobPortal.Repository;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JobPortal.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        
        public ActionResult JobSeekerRegister()
        {
            ModelState.Clear();
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JobSeekerRegister(JobSeekerModel jobSeeker, HttpPostedFileBase imageUpload, HttpPostedFileBase resumeUpload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                    if (jobSeekerRepository.JobSeekerRegister(jobSeeker, imageUpload, resumeUpload))
                    {
                        TempData["Message"] = "Registration successful ";
                        return RedirectToAction("Login");
                    }
                }
                return View();  
            }catch(Exception )
            {
                TempData["Message"] = "Email alredy registred ";
                return View(jobSeeker);
            }
           
        }

        
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login obj)
        {
            try {
                PublicRepository publicRepository = new PublicRepository();
                string result = publicRepository.Login(obj);
                if (result == "JobSeeker")
                {
                    JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                    var details = jobSeekerRepository.JobSeekers().Find(model => model.Username == obj.Username);
                    Session["SeekerId"] = details.SeekerId;
                    Session["SeekerImage"] = Convert.ToBase64String(details.Image);
                    Session["SeekerUsername"] = details.Username;
                    return RedirectToAction("Index", "JobSeeker");
                }
                else if (result == "Employer")
                {
                    EmployerRepository employerRepository = new EmployerRepository();
                    var details = employerRepository.Employers().Find(model => model.Username == obj.Username);
                    if (details.Status == "Pending" || details.Status == "Rejected")
                    {
                        TempData["Message"] = "You are not verified ";
                        return RedirectToAction("Index", "Home");
                    }
                    Session["EmployerId"] = details.EmployerID;
                    Session["CompanyLogo"] = Convert.ToBase64String(details.CompanyLogo);
                    Session["EmployerUsername"] = details.Username;
                    return RedirectToAction("Index", "Employer");
                }
                else if (result == "Admin")
                {
                    Session["Admin"] = obj.Username;
                    
                    FormsAuthentication.SetAuthCookie(obj.Username, false);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    TempData["Message"] = result;
                    return View();
                }
            }catch(Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
       
        public ActionResult About()
        {
            return View();
        }
        
        public ActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactMessage contactMessage)
        {
            try
            {
                PublicRepository publicRepository = new PublicRepository();
                if(publicRepository.CreateContactMessage(contactMessage))
                {
                    TempData["Message"] = "Message sent";
                }
                return RedirectToAction("Index", "Home");
            }catch(Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View(ex.Message);
            }
        }

        
        public ActionResult EmployerRegister()
        {
            ModelState.Clear();
            return View();
        }

       
        [HttpPost]
        public ActionResult EmployerRegister(EmployerModel emp, HttpPostedFileBase logoUpload)
        {
            try
            {
                EmployerRepository employerRepository = new EmployerRepository();
                if (employerRepository.EmployerRegister(emp, logoUpload)){
                    TempData["Message"] = "Registred Successfully";
                    return RedirectToAction("Login");
                }
                return View();
            }catch(Exception)
            {
                TempData["Message"] = "Email is alredy registred";
                return View();
            }
        }

        public ActionResult Jobs()
        {
            try
            {
                PublicRepository publicRepository = new PublicRepository();
                DateTime currentDate = DateTime.Now;
                var jobs = publicRepository.GetJobDetails().Where(job => job.ApplicationDeadline >= currentDate && job.IsPublished).ToList();
                return View(jobs);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }

        }
        
        [HttpGet]
        public ActionResult Jobs(string search)
        {
            try
            {
                PublicRepository publicRepository = new PublicRepository();
                var jobs = publicRepository.GetJobDetails();
                if (!string.IsNullOrEmpty(search))
                {
                    jobs = jobs.Where(job => job.JobTitle.Contains(search) || job.CategoryName.Contains(search) || job.Location.Contains(search) && job.ApplicationDeadline > DateTime.Now && job.IsPublished).ToList();
                }

                return View(jobs);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
        
        [HttpGet]
        public ActionResult CheckUsername(string username)
        {
            try
            {
                PublicRepository publicRepository = new PublicRepository();

                if (!publicRepository.CheckUsername(username))
                {
                    return new HttpStatusCodeResult(200);
                }
                return new HttpStatusCodeResult(202);
            }catch(Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
    }
}