using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NaturalMed.ViewModels
{
    public class ViewModelParametro
    {
        [Required(ErrorMessage = "El dato es requerido")]
        [Display(Name = "Seleccione el Mes")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        
        public DateTime Fecha { get; set; }
    }
}