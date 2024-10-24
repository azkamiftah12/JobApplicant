using JobApplicant.Models;
using JobApplicant.Services;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace JobApplicant.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            this._context = context;
            this._environment = environment;
        }


        //APPLICANT CONTROLLER START

        public IActionResult Index()
        {
            var applicants = _context.applicants.ToList();
            return View(applicants);
        }

        public IActionResult Detail(int id)
        {
            var applicant = _context.applicants.Find(id);
            if (applicant == null)
            {
                return NotFound();
            }
            return View(applicant);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Applicant applicant)
        {

            if (!ModelState.IsValid)
            {
                return View(applicant);
            }

            //check if resume file is inserted
            if(applicant.ResumeFile != null)
            {
                // save the resume file 
                string newFileName = SaveResumeFile(applicant.ResumeFile);

                // Save the file name to the database
                applicant.ResumeFileName = newFileName;
            }
            
            applicant.CreatedAt = DateTime.Now;
            _context.applicants.Add(applicant);
            _context.SaveChanges();

            return RedirectToAction("Index", "home");
        }

        public IActionResult Edit(int id)
        {
            var applicant = _context.applicants.Find(id);

            if (applicant == null)
            {
                return NotFound();
            }

            ViewData["ApplicantId"] = applicant.Id;

            return View(applicant);
        }

        [HttpPost]
        public IActionResult Edit(int id, Applicant applicant, IFormFile? resumeFile)
        {
            if (ModelState.IsValid)
            {
                var existingApplicant = _context.applicants.Find(id);

                if (existingApplicant == null)
                {
                    return NotFound();
                }

                if (resumeFile != null)
                {
                    // delete old file if exists
                    if (!string.IsNullOrEmpty(existingApplicant.ResumeFileName))
                    {
                        CheckAndDeleteResumeFile(existingApplicant.ResumeFileName);
                    }

                    // save the resume file 
                    string newFileName = SaveResumeFile(resumeFile);

                    // update resume file name in the database
                    existingApplicant.ResumeFileName = newFileName;
                }

                // update applicant
                existingApplicant.Name = applicant.Name;
                existingApplicant.Email = applicant.Email;
                existingApplicant.Phone = applicant.Phone;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(applicant);
        }

        public IActionResult Delete(int id)
        {
            var applicant = _context.applicants.Find(id);

            if (applicant == null)
            {
                return RedirectToAction("Index", "Home");
            }

            CheckAndDeleteResumeFile(applicant.ResumeFileName);

            _context.applicants.Remove(applicant);
            _context.SaveChanges(true);

            return RedirectToAction("Index", "Home");
        }

        private void CheckAndDeleteResumeFile(string fileName)
        {
            string resumeFilePath = Path.Combine(_environment.WebRootPath, "resumeFile", fileName);

            if (System.IO.File.Exists(resumeFilePath))
            {
                System.IO.File.Delete(resumeFilePath);
            }
        }

        private string SaveResumeFile(IFormFile resumeFile)
        {
            if (resumeFile == null) return "";

            // generate unique file name
            string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(resumeFile.FileName);

            // define resume file path
            string filePath = Path.Combine(_environment.WebRootPath, "resumeFile", uniqueFileName);

            // save resume
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                resumeFile.CopyTo(stream);
            }

            // return the saved file name
            return uniqueFileName;
        }



        //APPLICANT CONTROLLER End


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
