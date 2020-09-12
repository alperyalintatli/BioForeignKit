using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BioForeignKit.Models
{
    public class PatientViewModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        [Required(ErrorMessage = "*Tanı boş bırakılamaz.")]
        [Display(Name = "Tanı:")]
        public int DiagnosticId { get; set; }

        [Required(ErrorMessage = "*Uyruk boş bırakılamaz.")]
        [Display(Name = "Uyruk:")]
        public int NationalityId { get; set; }

        [Required(ErrorMessage = "*Cinsiyet boş bırakılamaz.")]
        [Display(Name = "Cinsiyet:")]
        public int GenderId { get; set; }

       


        [Required(ErrorMessage = "*İsim boş bırakılamaz.")]
        [Display(Name = "Hasta Adı:")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Soyisim boş bırakılamaz.")]
        [Display(Name = "Hasta Soyadı:")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "*Klinik Yorum boş bırakılamaz.")]
        [Display(Name = "Klinik Yorum:")]
        public string ClinicComment { get; set; }
        public string DoctorComment { get; set; }
   
        public Nullable<System.DateTime> DoctorCommentDate { get; set; }

        public Nullable<System.DateTime> DiagnosticDate { get; set; }

        public Nullable<System.DateTime> Birthdate { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }



    }
}