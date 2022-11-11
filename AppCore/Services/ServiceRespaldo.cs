using Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class ServiceRespaldo : IRespaldo
    {
        public void guardarRespaldo()
        {
            IRepositoryRespaldo repositoryRespaldos = new RepositoryRespaldo();
            repositoryRespaldos.guardarRespaldo();
        }
    }
}
