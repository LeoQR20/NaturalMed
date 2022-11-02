using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public interface IFactura
    {
        Factura GetFacturaById(int id);
        IEnumerable<Factura> GetFacturas();
        Factura Save(Factura factura);
        IEnumerable<Factura> GetFacturaByCondicion(int estado);
        IEnumerable<Factura> GetFacturaByFecha(DateTime fecha);
    }
}
