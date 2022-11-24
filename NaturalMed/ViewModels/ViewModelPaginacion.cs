using Infraestructura.Models;
using NaturalMed.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NaturalMed.ViewModels
{
    public class ViewModelPaginacion : Paginacion
    {
        public List<Factura> Facturas { get; set; }
    }
}