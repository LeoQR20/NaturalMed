using AppCore.Services;
using NaturalMed.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NaturalMed.Controllers
{
    public class RespaldoController : Controller
    {
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult guardarRespaldo()
        {
            IRespaldo serviceRespaldos = new ServiceRespaldo();
            serviceRespaldos.guardarRespaldo();

            return View("respaldoExitoso");
        }
    }
}
