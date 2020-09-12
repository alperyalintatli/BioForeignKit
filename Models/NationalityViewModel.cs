using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BioForeignKit.Models
{
    public class NationalityViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*Uyruk ismi boş bırakılamaz.")]
        public string Nationality { get; set; }
    }
}