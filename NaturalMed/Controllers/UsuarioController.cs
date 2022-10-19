using AppCore.Services;
using Infraestructura.Models;
using NaturalMed.Security;
using NaturalMed.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NaturalMed.Controllers
{
    public class UsuarioController : Controller
    {
        private static String Action;

        // Significa  que solo los que tienen el rol de Administrador pueden accederla 
        // ver Enums.cs  
        // public enum Roles { Administrador = 1, Procesos = 2, Reportes = 3}
        [CustomAuthorize((int)Roles.Administrador)]
        // GET: Empleado
        public ActionResult Index()
        {
            try
            {
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                //Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult ListaUsuario()
        {
            IEnumerable<Usuario> lista = null;
            try
            {
                IUsuario _ServiceUsuario = new ServiceUsuario();
                lista = _ServiceUsuario.GetUsuarios();
                ViewBag.title = "Lista Usuarios";
                return View(lista);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;

                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        private SelectList listaUsuarios(int idUsuario = 0)
        {
            IUsuario oUsuario = new ServiceUsuario();
            IEnumerable<Usuario> listaUsuarios = oUsuario.GetUsuarios();
            return new SelectList(listaUsuarios, "IdUsuario", "Nombre", idUsuario);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Usuario usuario)
        {
            string errores = "";
            try
            {
                // Es valido
                if (ModelState.IsValid)
                {
                    ServiceUsuario _ServiceEmpleado = new ServiceUsuario();
                    _ServiceEmpleado.Save(usuario);
                }
                else
                {
                    // Valida Errores si Javascript está deshabilitado
                    Util.ValidateErrors(this);

                    TempData["Message"] = "Error al procesar los datos! " + errores;
                    TempData.Keep();

                    return View("Create", usuario);
                }

                Action = "S";

                // redirigir
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                //Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }


        //GET: Empleado/Details/    
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult AjaxFilterDetails(int? id)
        {
            ServiceUsuario _ServiceEmpleado = new ServiceUsuario();
            Usuario usuario = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("List");
                }

                usuario = _ServiceEmpleado.GetUsuarioByID(id.Value);
                //var detalles = new List<Empleado>();
                //detalles.Add(Empleado);

                return PartialView("_PartialViewDetailsEmpleado", usuario);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                //Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        //public ActionResult AjaxFilterDetails5(int id)
        //{
        //    IServiceEmpleado serviceEmpleado = new ServiceEmpleado();
        //    Empleado empleado = serviceEmpleado.GetEmpleadoByID(id);

        //    var detalles = new List<Empleado>();
        //    detalles.Add(empleado);
        //    return PartialView("_PartialViewDetailsEmpleado", detalles);
        //}

        // GET: Empleado/Edit/5
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            IUsuario _ServiceEmpleado = new ServiceUsuario();
            Usuario usuario = null;

            //ViewBag con los tipos de Empleado
            IRol serviceRol = new ServiceRol();
            ViewBag.ListaTipos = serviceRol.GetRols();

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("ListaUsuario");
                }

                usuario = _ServiceEmpleado.GetUsuarioByID(id.Value);
                // Response.StatusCode = 500;

                Action = "U";

                return PartialView("Edit", usuario);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                //Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }


        // GET: Empleado/Create
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            //ViewBag con los tipos de Empleado
            IRol serviceRol = new ServiceRol();
            ViewBag.ListaTipos = serviceRol.GetRols();
            Usuario usuario = new Usuario();
            return PartialView("Create", usuario);
            //return View();
        }


        // GET: Empleado/Delete/5
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Delete(int? id)
        {
            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("List");
                }

                ServiceUsuario _ServiceEmpleado = new ServiceUsuario();
                Usuario usuario = _ServiceEmpleado.GetUsuarioByID(id.Value);

                Action = "D";

                return PartialView("_DeletePartialView", usuario);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                //Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult DeleteConfirmed(int? id)
        {
            ServiceUsuario _ServiceUsuario = new ServiceUsuario();
            try
            {
                if (id == null)
                {
                    return View();
                }
                _ServiceUsuario.DeleteUsuario(id.Value);

                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                //Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }
    }
}
