using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BioForeignKit.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="*İsim kısmı boş bırakılamaz.")]
        [Display(Name ="İsim:")]
        public string Name { get; set; }
    }
}