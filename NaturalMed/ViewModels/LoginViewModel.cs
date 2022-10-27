using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NaturalMed.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ID { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Password { get; set; }

        [Display(Name ="Correo Electrónico")]
        [EmailAddress(ErrorMessage ="Digite un {0} válido")]
        public string Email { get; set; }
        
    }
}