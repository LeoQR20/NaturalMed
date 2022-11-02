using Infraestructura.Models;
using Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class ServiceFactura : IFactura
    {
        public Factura GetFacturaById(int id)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.GetFacturaById(id);
        }

        public IEnumerable<Factura> GetFacturas()
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.GetAll();
        }

        public IEnumerable<Factura> GetFacturaByCondicion(int estado)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.GetFacturaByCondicion(estado);
        }

        public IEnumerable<Factura> GetFacturaByFecha(DateTime fecha)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.GetFacturaByFecha(fecha);
        }

        public Factura Save(Factura factura)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.Save(factura);
        }
    }
}
