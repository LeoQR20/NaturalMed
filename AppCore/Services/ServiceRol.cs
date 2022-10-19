using Infraestructura.Models;
using Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class ServiceRol : IRol
    {
        public IEnumerable<Rol> GetRols()
        {
            IRepositoryRol repository = new RepositoryRol();
            return repository.GetRol();
        }
    }
}
