using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using BioForeignKit.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BioForeignKit.Controllers
{[Authorize(Roles ="Doktor,Koordinatör,Admin")]
    public class PatientsController : Controller
    {
        private BioForeignKİTDbEntities db = new BioForeignKİTDbEntities();
        private UserManager<ApplicationUser> userManager;
        private RoleManager<ApplicationRole> roleManager;
        public PatientsController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        }

     

        // GET: Patients
        public ActionResult Index()
        {
            var patients = db.Patients.Include(p => p.Diagnostics).Include(p => p.Genders).Include(p => p.Nationalities);
            return View(patients.ToList());
        }

        // GET: Patients/Details
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Account");
            }
            Patients patients = db.Patients.FirstOrDefault(x=>x.UserId==id);
            if (patients == null)
            {
                return RedirectToAction("Error", "Account");
            }
            
            var users = db.AspNetUsers.Where(x => x.Email != null).ToList();
            var role= roleManager.FindByName("Doktor");
            List<AspNetUsers> doctors = new List<AspNetUsers>();
            foreach (var user in users)
            {
                if (user.AspNetRoles.Where(x => x.Id == role.Id).Any())
                {
                    doctors.Add(user);
                } 
               
            }
            
            ViewBag.Doctors = new SelectList(doctors, "UserName", "Email");
            return View(patients);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddedDoctorComment(string userId,string doctorComment)
        {
            if (ModelState.IsValid)
            {
                if (userId!=null && doctorComment!=null && doctorComment!="")
                {
                    var patient = db.Patients.FirstOrDefault(x => x.UserId == userId);
                    patient.DoctorComment = doctorComment;
                    patient.DoctorCommentDate = DateTime.Now;
                 db.SaveChanges();
                    TempData["successAddedDoctorComment"] = "*Doktor yorumu başarıyla eklenmiştir.";
                }
                else
                {
                    return RedirectToAction("Error","Account");
                }
            }
            
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMailToDoctor(string userId, string Doctors,string addedEmail)
        {
            
            var patient = db.Patients.FirstOrDefault(x => x.UserId == userId);
            var user=userManager.FindByName(User.Identity.Name);
            if (Doctors != "" && Doctors != null)
            {
                var doctor = userManager.FindByName(Doctors);
                MailMessage mail = new MailMessage();
                mail.To.Add(doctor.Email);
                var fromAddress = new MailAddress("aa4581717@gmail.com");
                mail.From = fromAddress;
                mail.Subject = "Hasta Değerlendirme";
                mail.IsBodyHtml = true;
                mail.CC.Add(user.Email);
                if (addedEmail!=""&& addedEmail!=null)
                {
                    mail.CC.Add(addedEmail);
                }
                string firstParagraph = "Hocam Merhabalar," + "<br/>" + "<br/>" + "Aşağıda ilgili hastanın bilgileri bulunmaktadır;" + "<br/>";
                string nameSurname = "<strong>İsim : </strong>" + patient.Name + " " + patient.Surname + "<br/>";
                string birthdate = "<strong>Doğum Tarihi: </strong>" + patient.Birthdate.Value.ToShortDateString() + "<br/>";
                string diagnostic = "<strong>Tanı: </strong>" + patient.Diagnostics.DiagnosticName + "<br/>";
                string nationality = "<strong>Uyruk: </strong>" + patient.Nationalities.Nationality + "<br/>";
                string gender = "<strong>Cinsiyet: </strong>" + patient.Genders.GenderName + "<br/>";
                string clinicComment = "<strong>Klinik Yorum: </strong>" + patient.ClinicComment + "<br/>";
                string lastParagraph = "Yorumunuzu eklemeniz hususunda yardımlarınızı rica ederim." + "<br/>" + "<br/>"+"Bilgilerinize,"+ "<br/>" + "Saygılarımla" + "<br/>" + "KİT Koordinatörü";
                string body = firstParagraph + nameSurname + birthdate + diagnostic + nationality + gender + clinicComment + lastParagraph;
                mail.Body = body;
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    //DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, "1392ayt*")
                };
                try
                {
                    TempData["successPatientMailToDoctor"] = "*Hasta başarıyla mail ile gönderilmiştir.";
                    smtp.Send(mail);
                    return RedirectToAction("Index");

                }
                catch (Exception)
                {

                    return RedirectToAction("Error","Account");
                }
            }
            TempData["successPatientMailToDoctor"] = "*Mail gönderme işleminde bir hata ile karşılaşılmıştır.Lütfen tekrar deneyiniz.";
            return RedirectToAction("Index");

        }
        // GET: Patients/Create
        public ActionResult Create()
        {
            ViewBag.DiagnosticId = new SelectList(db.Diagnostics, "Id", "DiagnosticName");
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "GenderName");
            ViewBag.NationalityId = new SelectList(db.Nationalities, "Id", "Nationality");
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,RoleId,DiagnosticId,NationalityId,GenderId,MutationId,Name,Surname,ClinicComment,DoctorComment,DoctorCommentDate,DiagnosticDate,Birthdate,RegisterDate")] PatientViewModel patients)
        {
            if (ModelState.IsValid)
            {
                string firstname = patients.Name.Trim();
                string surname = patients.Surname.Trim();
                string fullname = firstname + surname;
                fullname = fullname.ToLower().Replace("ı", "i").Replace("ü", "u").Replace("ö", "o").Replace("ç", "c").Replace("ş", "s").Replace("ğ", "g");

                string guidKey = Guid.NewGuid().ToString();
                string password = fullname + "_" +guidKey;
                var user = new ApplicationUser { UserName = fullname.Trim() };
                var result = userManager.Create(user, password);
                string name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(patients.Name);
                string surName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(patients.Surname);
                if (result.Succeeded)
                {
                  
                    var patient = new Patients
                    {
                        UserId = user.Id,
                        Birthdate = patients.Birthdate,
                        ClinicComment = patients.ClinicComment,
                        DiagnosticDate = DateTime.Now,
                        DiagnosticId = patients.DiagnosticId,
                        DoctorComment = patients.DoctorComment,
                        DoctorCommentDate = patients.DoctorCommentDate,
                        GenderId = patients.GenderId,
                        Name = name,
                        NationalityId = patients.NationalityId,
                        RegisterDate = DateTime.Now,
                        RoleId = "eb7e195d-ba6a-4da0-bb70-3d79b1682562",
                        Surname = surName,
                        IsActive = false
                    };
                    db.Patients.Add(patient);
                    db.SaveChanges();
                    TempData["successPatientRegister"] = "*Hasta kaydı başarıyla tamamlanmıştır.";
                }

            
                return RedirectToAction("Index");
            }

            ViewBag.DiagnosticId = new SelectList(db.Diagnostics, "Id", "DiagnosticName", patients.DiagnosticId);
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "GenderName", patients.GenderId);
            ViewBag.NationalityId = new SelectList(db.Nationalities, "Id", "Nationality", patients.NationalityId);
            return View(patients);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Account");
            }
            Patients patients = db.Patients.FirstOrDefault(x=>x.UserId==id);
            if (patients == null)
            {
                return RedirectToAction("Error","Account");
            }
            ViewBag.DiagnosticId = new SelectList(db.Diagnostics, "Id", "DiagnosticName", patients.DiagnosticId);
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "GenderName", patients.GenderId);
            ViewBag.NationalityId = new SelectList(db.Nationalities, "Id", "Nationality", patients.NationalityId);
            return View(patients);
        }

        // POST: Patients/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,RoleId,DiagnosticId,NationalityId,GenderId,MutationId,Name,Surname,Age,ClinicComment,DoctorComment,DoctorCommentDate,DiagnosticDate,Birthdate,RegisterDate")] Patients patients)
        {
            if (ModelState.IsValid)
            {
                var patient = db.Patients.FirstOrDefault(x => x.UserId == patients.UserId);
                if (patients.Birthdate!=null)
                {
                    patient.Birthdate = patients.Birthdate;
                }
                if (patients.IncomingPatientDate != null)
                {
                    patient.IncomingPatientDate = patients.IncomingPatientDate;
                }

                patient.ClinicComment = patients.ClinicComment;
                patient.DiagnosticId = patients.DiagnosticId;
                patient.GenderId = patients.GenderId;
                patient.Name = patients.Name;
                patient.NationalityId = patients.NationalityId;
                patient.Surname = patients.Surname;
                if (patients.DoctorComment != null && patients.DoctorComment != "")
                {
                    patient.DoctorComment = patients.DoctorComment;
                    patient.DoctorCommentDate = DateTime.Now;
                }

                db.SaveChanges();
                TempData["successPatientEdit"] = "*Hasta kaydı başarıyla güncellenmiştir.";
                return RedirectToAction("Index");
            }
            ViewBag.DiagnosticId = new SelectList(db.Diagnostics, "Id", "DiagnosticName", patients.DiagnosticId);
            ViewBag.GenderId = new SelectList(db.Genders, "Id", "GenderName", patients.GenderId);
            ViewBag.NationalityId = new SelectList(db.Nationalities, "Id", "Nationality", patients.NationalityId);
            return View(patients);
        }

        public ActionResult PatientConfirmationList()
        {
            var patients = db.Patients.Include(p => p.Diagnostics).Include(p => p.Genders).Include(p => p.Nationalities);
            return View(patients.Where(model=>model.IsActive==true).ToList());
        }
       
        [HttpGet]
        public ActionResult PatientConfirmation(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Account");
            }
            Patients patients = db.Patients.FirstOrDefault(x => x.UserId == id);
            if (patients == null)
            {
                return RedirectToAction("Error", "Account");
            }
            return View(patients);
        }

        [HttpPost, ActionName("PatientConfirmation")]
        [ValidateAntiForgeryToken]
        public ActionResult PatientConfirmationConfirmed(string id)
        {
            if (id != null)
            {
                Patients patients = db.Patients.FirstOrDefault(x => x.UserId == id);
                patients.IsActive = true;
                patients.IncomingPatientDate = DateTime.Now;
                db.SaveChanges();
                TempData["successPatientConfirm"] = "*Hasta kaydı başarıyla onaylanmıştır.";
            }
           
            return RedirectToAction("Index");
        }

        // GET: Patients/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Account");
            }
            Patients patients = db.Patients.FirstOrDefault(x=>x.UserId==id);
            if (patients == null)
            {
                return RedirectToAction("Error", "Account");
            }
            return View(patients);
        }

        // POST: Patients/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id!=null)
            {
                Patients patients = db.Patients.FirstOrDefault(x => x.UserId == id);
                var user = userManager.FindById(id);
                userManager.Delete(user);
                db.Patients.Remove(patients);
                db.SaveChanges();
                TempData["successPatientDelete"] = "*Hasta kaydı başarıyla silinmiştir.";
            }
         
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpenWord(string UserId, bool isEnglishWord)
        {
            var patient = db.Patients.FirstOrDefault(x => x.UserId == UserId);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName="";
            if (isEnglishWord != true)
            {
                string guidKey = Guid.NewGuid().ToString();
                fileName = @path + "\\" + patient.Name + patient.Surname + "_DurumBildirir_" + DateTime.Now.ToShortDateString()+"_"+ guidKey + ".docx";
            }
            else if(isEnglishWord == true)
            {
                string guidKey = Guid.NewGuid().ToString();
                fileName = @path + "\\" + patient.Name + patient.Surname + "_SituationReport_" + DateTime.Now.ToShortDateString()+"_"+guidKey + ".docx";
            }
         
            // Create the document in memory:
            var doc = DocX.Create(fileName, DocumentTypes.Document);
            

            var headLineFormat = new Formatting();
            headLineFormat.Size = 18D;
            headLineFormat.Position = 12;

            var bodyLineFormatBold = new Formatting();
            bodyLineFormatBold.Size = 13D;
            bodyLineFormatBold.Position = 12;
            bodyLineFormatBold.Bold = true;


            var bodyLineFormat = new Formatting();
            bodyLineFormat.Size = 13D;
            bodyLineFormat.Position = 12;
            bodyLineFormat.Bold = false;
            if (isEnglishWord!=true)
            {
                string titleDateTime = "Tarih:" + DateTime.Now.ToShortDateString();
                Paragraph zeroTitleDateTime = doc.InsertParagraph(titleDateTime, false, bodyLineFormatBold).Font(new Xceed.Document.NET.Font("Times New Roman"));
                zeroTitleDateTime.Alignment = Alignment.right;

                doc.InsertParagraph("" + Environment.NewLine + "");

                string headlineText = "Antalya Medicalpark Hastaneleri" + Environment.NewLine + "Çocuk Kemik İliği Nakil Üniteleri Durum Bildirir Raporu";
                Paragraph topText = doc.InsertParagraph(headlineText, false, headLineFormat).Font(new Xceed.Document.NET.Font("Times New Roman")).Bold();
                topText.Alignment = Alignment.center;


                doc.InsertParagraph("" + Environment.NewLine + "");

                string titleName = "Hasta Adı           :     ";
                Paragraph firstTitleName = doc.InsertParagraph(titleName, false, bodyLineFormatBold).Font(new Xceed.Document.NET.Font("Times New Roman"));
                firstTitleName.InsertText(patient.Name + " " + patient.Surname, false, bodyLineFormat);

                doc.InsertParagraph("" + Environment.NewLine + "");

                string titleBirthday = "Doğum Tarihi    :     ";
                Paragraph secondTitleBirthday = doc.InsertParagraph(titleBirthday, false, bodyLineFormatBold).Font(new Xceed.Document.NET.Font("Times New Roman"));
                secondTitleBirthday.InsertText(patient.Birthdate.Value.ToShortDateString(), false, bodyLineFormat);

                doc.InsertParagraph("" + Environment.NewLine + "");

                string titleDiagnostic = "Tanı                    :     ";
                Paragraph thirdTitleDiagnostic = doc.InsertParagraph(titleDiagnostic, false, bodyLineFormatBold).Font(new Xceed.Document.NET.Font("Times New Roman"));
                thirdTitleDiagnostic.InsertText(patient.Diagnostics.DiagnosticName, false, bodyLineFormat);
            }
            else
            {
               
                string headlineText = "EXTRACT" + Environment.NewLine + "from medical card of inpatient patient № ......." + Environment.NewLine+ "Bone marrow Transplantation Unit ";
                Paragraph topText = doc.InsertParagraph(headlineText, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);
                topText.Alignment = Alignment.center;


               

                string titleName = "1.Full name: ";
                Paragraph firstTitleName = doc.InsertParagraph(titleName, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);
                firstTitleName.InsertText(patient.Name + " " + patient.Surname, false, bodyLineFormat);

               

                string titleBirthday = "2.Date of birth: ";
                Paragraph secondTitleBirthday = doc.InsertParagraph(titleBirthday, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);
                secondTitleBirthday.InsertText(patient.Birthdate.Value.ToShortDateString(), false, bodyLineFormat);

               

                string placeofResidance = "3.Place of residence:";
                Paragraph thirdTitlePlaceofResidance = doc.InsertParagraph(placeofResidance, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);

            

                string hospitalAdmission = "4.In a hospital admission:" + Environment.NewLine + "Discharge:";
                Paragraph fourthTitleHospitalAdmission = doc.InsertParagraph(hospitalAdmission, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);
               

             

                string fullDiagnosis = "5.Full diagnosis (underlying disease, accompanying diseases and complications):" + Environment.NewLine;
                Paragraph fifthTitleFullDiagnosis = doc.InsertParagraph(fullDiagnosis, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);
                fifthTitleFullDiagnosis.InsertText(patient.Diagnostics.Description, false, bodyLineFormat);

                

                string shortTreatment = "6.Short anamnesis, diagnostic researches, disease progression, conducted treatment, condition at admission, at discharge:" + Environment.NewLine;
                Paragraph sixthTitleShortTreatment = doc.InsertParagraph(shortTreatment, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);

                doc.InsertParagraph(Environment.NewLine);

                string lastParagraph = "Kind regards," + Environment.NewLine+ "Prof. Dr.  ....." + Environment.NewLine +""+ Environment.NewLine+ "Medicalpark Antalya Hospital" + Environment.NewLine 
                    + "Pediatric Stem Cell Transplantation Unit" + Environment.NewLine + "Pediatric Hematology";
                Paragraph seventhTitleLastParagraph = doc.InsertParagraph(lastParagraph, false, bodyLineFormat).Font(new Xceed.Document.NET.Font("Arial")).FontSize(12);
            }
        
            // Save to the output directory:
            doc.Save();

            // Open in Word:
            Process.Start("WINWORD.EXE", fileName);
            TempData["successPatientSituationReports"] = "*Durum bildirir raporu başarıyla oluşturulmuştur.";
            return RedirectToAction("Details", new { id = UserId });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
