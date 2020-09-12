using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BioForeignKit.Models
{
    public class DiagnosticViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="*Tanı ismi boş bırakılamaz.")]
        public string DiagnosticName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
    }
}