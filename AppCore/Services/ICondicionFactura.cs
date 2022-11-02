using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public interface ICondicionFactura
    {
        IEnumerable<CondicionFactura> GetCondicionFacturas();
        CondicionFactura GetCondicionFacturaById(int id);
    }
}
