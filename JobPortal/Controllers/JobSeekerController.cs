﻿using JobPortal.Models;
using JobPortal.Repository;
using Microsoft.Ajax.Utilities;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPortal.Controllers
{
    public class JobSeekerController : Controller
    {
        private bool IsValid()
        {
            return Session["SeekerId"] != null;
        }
        
        public ActionResult Index()
        {
            bool isValid = IsValid();
            if (isValid)
            {
                PublicRepository publicRepository = new PublicRepository();
                var jobs = publicRepository.GetJobDetails();
                JobSeekerRepository repo = new JobSeekerRepository();
                int id = Convert.ToInt32(Session["SeekerId"]);
                var applications = repo.GetJobApplications(id);
                return View(new Index { JobApplications = applications, JobDetails = jobs });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        public ActionResult JobSeekerProfile()
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                PublicRepository repo = new PublicRepository();
                int seekerId = (int)Session["SeekerId"];
                var jobSeeker = jobSeekerRepository.JobSeekers().Find(model => model.SeekerId == seekerId);
                var edu = jobSeekerRepository.GetEducationDetails(seekerId);
                var userSkills = repo.JobSeekerSkills(seekerId).ToList();
                var userSkillsId = repo.JobSeekerSkills(seekerId).Select(js => js.SkillId).ToList();

                var skills = repo.DisplaySkills().Where(skil => !userSkillsId.Contains(skil.SkillId)).ToList();
                var viewModel = new JobSeekerProfile
                {
                    JobSeekerDetails = jobSeeker,
                    EducationDetails = edu,
                    Skills = userSkills,
                    AllSkills = skills
                };
                return View(viewModel);
            }catch(Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
        public ActionResult UpdateProfile()
        {
            JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
            var jobSeeker = jobSeekerRepository.JobSeekers().Find(model => model.SeekerId == (int)Session["SeekerId"]);
            return View(jobSeeker);

        }
        
        [HttpPost]
        public ActionResult UpdateProfile(JobSeekerModel jobSeeker, HttpPostedFileBase imageUpload)
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                if (jobSeekerRepository.JobSeekerUpdate(jobSeeker, imageUpload, Convert.ToInt32(Session["SeekerId"])))
                {
                    TempData["Message"] = "Updated";
                }
                return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }

        }

        public ActionResult AddEducationDetails()
        {
            return View();
        }
        [HttpPost]
        
        public ActionResult AddEducationDetails(EducationDetails educationList)
        {
            try
            {
                int id = (int)Session["SeekerId"];
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                if (jobSeekerRepository.AddEducationDetails(educationList, id))
                {
                    TempData["Message"] = "Added successfully";
                    return RedirectToAction("JobSeekerProfile");
                }
                return View();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UpdateResume(HttpPostedFileBase resumeFile)
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                if (jobSeekerRepository.UpdateResume(resumeFile, Convert.ToInt32(Session["SeekerId"])))
                {
                    TempData["Message"] = "Resume Updated";
                }
               return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }

        }
        public ActionResult UpdateEducationDetails(int id)
        {
            JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
            var educationDetails = jobSeekerRepository.GetEducationDetails(Convert.ToInt32(Session["SeekerId"])).Find(ed => ed.EducationId == id);
            return View(educationDetails);
        }
        
        [HttpPost]
        public ActionResult UpdateEducationDetails(EducationDetails educationDetails) {
            JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
            try
            {
                if (jobSeekerRepository.UpdateEducationDetails(educationDetails))
                {
                    TempData["Message"] = "Updated Successfully";
                }
                return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
       
        [HttpGet]
        public ActionResult DeleteEducationDetails(int id)
        {
            JobSeekerRepository jobSeekerRepository=new JobSeekerRepository();
            try {
                if (jobSeekerRepository.DeleteEducationDetails(id))
                {
                    TempData["Message"] = "Deleted Successfully ";
                }
                return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
        
        [HttpGet]
        public ActionResult DeleteJobSeekerSkill(int id)
        {
            try {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                if (jobSeekerRepository.DeleteJobSeekerSkill((int)id))
                {
                    TempData["Message"] = "Deleted Successullly";
                }
                return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
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
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
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
                    jobs = jobs.Where(job =>job.JobTitle.Contains(search) || job.CategoryName.Contains(search) ||job.Location.Contains(search) && job.ApplicationDeadline > DateTime.Now && job.IsPublished).ToList();
                }

                return View(jobs);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }

        [HttpGet]
        
        public ActionResult ApplyJob(int  id) {
            try
            {
                JobApplication application = new JobApplication {
                    SeekerId = Convert.ToInt32(Session["SeekerId"]),
                    JobApplicationID = id,
                    ApplicationDate = DateTime.Now
                    };
                JobSeekerRepository repo = new JobSeekerRepository();
                if (repo.CreateJobApplication(application))
                {
                    TempData["Message"] = "Applied Successfull";
                }
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
       
        [HttpGet]
        public ActionResult ViewJob(int id)
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                ViewJob obj = new ViewJob
                {
                    JobId = id,
                    SeekerId = Convert.ToInt32(Session["SeekerId"]),
                    ViewDate = DateTime.Now,
                };
                if (jobSeekerRepository.VisitJob(obj))
                {
                    return new HttpStatusCodeResult(200);
                }
                return new HttpStatusCodeResult(400);
            }
            catch(Exception ){
                return new HttpStatusCodeResult(400);
            }
        }
       
        [HttpGet]
        public ActionResult Bookmark(int id)
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                Bookmark obj = new Bookmark { 
                    JobId = id,
                    SeekerId = Convert.ToInt32(Session["SeekerId"]),
                };
                if (jobSeekerRepository.Bookmark(obj))
                {
                    return new HttpStatusCodeResult(200);
                }
                return new HttpStatusCodeResult(400);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
       
        public ActionResult JobDetails(int id)
        {
            try
            {
                int seekerId = Convert.ToInt32(Session["SeekerId"]);
                PublicRepository publicRepository = new PublicRepository();
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                var jobDetails = publicRepository.GetJobDetails().Find(model => model.JobID == id);
                if (jobDetails != null)
                {
                    var bookmarks = jobSeekerRepository.GetBookmarks(seekerId);
                    var appliedJobs = jobSeekerRepository.GetJobApplications(seekerId);
                    bool isSaved = bookmarks.Any(jobId => jobId.JobId == id);
                    bool isApplied = appliedJobs.Any(jobId => jobId.JobId == id);
                    ViewBag.isSaved = isSaved;
                    ViewBag.isApplied = isApplied;
                }

                return View(jobDetails);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }

        }
        
        public ActionResult AppliedJobs()
        {
            JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
            int id = Convert.ToInt32(Session["SeekerId"]);
            return View(jobSeekerRepository.GetJobApplications(id));
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                if (jobSeekerRepository.ChangePassword(oldPassword, newPassword, Convert.ToInt32(Session["SeekerId"])))
                {
                    TempData["Message"] = "Password changed";
                }
                else
                {
                    TempData["Message"] = "Wrong password";
                    return View();
                }
                return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
        public ActionResult AddSkill()
        {
            PublicRepository publicRepository = new PublicRepository();
            int seekerId = Convert.ToInt32(Session["SeekerId"]);
            var userSkills = publicRepository.JobSeekerSkills(seekerId).Select(js => js.SkillId).ToList();
            var skills = publicRepository.DisplaySkills().Where(skill => !userSkills.Contains(skill.SkillId)).ToList();
            return View(skills);
        }

        [HttpPost]
        public ActionResult AddSkill(int[] SkillId)
        {
            JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
            try {
                foreach (int skillId in SkillId)
                {
                    if (jobSeekerRepository.AddSkill(skillId, Convert.ToInt32(Session["SeekerId"])))
                    {
                        TempData["Message"] = "Skills added";
                    }
                }                              
                return RedirectToAction("JobSeekerProfile");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
        
        public ActionResult Bookmarks()
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                var bookmarks = jobSeekerRepository.GetBookmarks(Convert.ToInt32(Session["SeekerId"]));
                return View(bookmarks);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
        
        public ActionResult SendMessage(int id)
        {
            try
            {
                PublicRepository publicRepository = new PublicRepository();
                int seekerId = (int)Session["SeekerId"];
                var chats = publicRepository.ReadMessage(seekerId, id);
                if (chats.Count == 0)
                {
                    EmployerRepository repo = new EmployerRepository();
                    var employer = repo.Employers().Find(model => model.EmployerID == id);
                    return View("Send-Message", employer);
                }
                return View(chats);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
       
        [HttpPost]
        public ActionResult SendMessage(int id, string message) {
            try
            {
                PublicRepository publicRepository = new PublicRepository();
                int seekerId = (int)Session["SeekerId"];
                char sender = 'J';
                if (publicRepository.SendMessage(seekerId, id, message, sender))
                {
                    return new HttpStatusCodeResult(200);
                }
                return new HttpStatusCodeResult(400);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }
        
        public ActionResult ChatList()
        {
            try
            {
                JobSeekerRepository jobSeekerRepository = new JobSeekerRepository();
                int seekerId = (int)Session["SeekerId"];
                return View(jobSeekerRepository.ChatList(seekerId));
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }
       
        public ActionResult Logout()
        {
            Session["SeekerId"] = null;
            Session["SeekerImage"] = null;
            Session["SeekerUsername"] = null;
            TempData["Message"] = "Logouted";
            return RedirectToAction("Index", "Home");
        }
    }
}