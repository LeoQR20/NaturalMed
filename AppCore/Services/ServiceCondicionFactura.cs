using Infraestructura.Models;
using Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class ServiceCondicionFactura : ICondicionFactura
    {
        public CondicionFactura GetCondicionFacturaById(int id)
        {
            IRepositoryCondicionFactura repository = new RepositoryCondicionFactura();
            return repository.GetCondicionOrdenById(id);
        }

        public IEnumerable<CondicionFactura> GetCondicionFacturas()
        {
            IRepositoryCondicionFactura repository = new RepositoryCondicionFactura();
            return repository.GetCondicionOrden();
        }
    }
}
