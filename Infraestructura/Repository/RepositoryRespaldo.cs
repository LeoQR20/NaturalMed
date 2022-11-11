using Infraestructura.Models;
using Infraestructura.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public class RepositoryRespaldo : IRepositoryRespaldo
    {
        public void guardarRespaldo()
        {
            try
            {
                string url = @"'D:\NaturalMend\" + DateTime.Now.ToString("dd-MMMM-yyyy HH-mm") + ".bak'";
                using (MyContext ctx = new MyContext())
                {
                    ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "backup database NaturalMend to disk = " + url);
                }
            }
            catch (Exception dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }
    }
}
