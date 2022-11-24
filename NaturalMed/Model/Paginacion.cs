using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NaturalMed.Model
{
    public class Paginacion
    {
        public int PaginaActual { get; set; }
        public int TotalItems { get; set; }
        public int RegistroPorPagina { get; set; }
    }
}