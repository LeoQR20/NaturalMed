using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NaturalMed.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly int[] allowedroles;
        public CustomAuthorizeAttribute(params int[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //Cambié manualmente a Empleado, en el ejemplo se trabaja con Usuario
            bool authorize = false;
            var oEmpleado = (Usuario)httpContext.Session["User"];

            if (oEmpleado != null)
            {

                foreach (var rol in allowedroles)
                {
                    if (rol == oEmpleado.IDRole)
                        return true;
                }
            }
            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Si hubo un error redireccione a el siguiente Controller y View
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Login" },
                    { "action", "UnAuthorized" }
               });
        }
    }
}