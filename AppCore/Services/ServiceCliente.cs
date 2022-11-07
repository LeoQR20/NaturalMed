using Infraestructura.Models;
using Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class ServiceCliente : ICliente
    {
        public Cliente GetCliente(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Cliente> GetClientes()
        {
            IRepositoryCliente repository = new RepositoryCliente();
            return repository.GetCliente();
        }
    }
}
