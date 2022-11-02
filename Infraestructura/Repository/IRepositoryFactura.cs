using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public interface IRepositoryFactura
    {
        Factura GetFacturaById(int id);

        IEnumerable<Factura> GetAll();

        Factura Save(Factura factura);

        IEnumerable<Factura> GetFacturaByCondicion(int estado);
        IEnumerable<Factura> GetFacturaByFecha(DateTime fecha);

    }
}
