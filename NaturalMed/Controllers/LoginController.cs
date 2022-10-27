using AppCore.Services;
using Infraestructura.Models;
using Infraestructura.Utils;
using NaturalMed.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NaturalMed.ViewModel;
using Log = NaturalMed.Utils.Log;
using SweetAlertMessageType = NaturalMed.Utils.SweetAlertMessageType;

namespace NaturalMed.Controllers
{
    public class LoginController : Controller
    {

        private static Usuario aux;
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        //private SelectList listaRol(int idRol = 0)
        //{
        //    IRol rol = new ServiceRol();
        //    IEnumerable<Rol> lista = rol.GetRols();
        //    return new SelectList(lista, "Id", "Descripcion", idRol);

        //}
        public ActionResult Login(Usuario usuario)
        {
            IUsuario usuario1 = new ServiceUsuario();
            Usuario oUsuario = null;
            try
            {
                if (ModelState.IsValid)
                {
                    oUsuario = usuario1.GetUsuario(usuario.ID, usuario.Password);
                    if (oUsuario != null)
                    {
                        Session["User"] = oUsuario;
                        Log.Info($"Accede {oUsuario.Nombre} con el rol {oUsuario.Rol.IDRol}-{oUsuario.Rol.Descripcion}");
                        TempData["mensaje"] = Utils.SweetAlertHelper.Mensaje("Login", "Usario autenticado satisfactoriamente", SweetAlertMessageType.success);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Log.Warn($"{usuario.ID} se intentó conectar  y falló");
                        ViewBag.NotificationMessage = Utils.SweetAlertHelper.Mensaje("Login", "Error al autenticarse", SweetAlertMessageType.warning);

                    }
                } 
                return View("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
           
        }
        public ActionResult Logout()
        {
            try
            {
                Log.Info("Usuario desconectado!");
                Session["User"] = null;
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData["Redirect"] = "Login";
                TempData["Redirect-Action"] = "Index";
                return RedirectToAction("Default", "Error");
            }
        }

        public ActionResult Recuperacion(LoginViewModel usuario)
        {
            try
            {
                IUsuario service = new ServiceUsuario();
                Usuario oUsuario = service.VerificarUsuario(usuario.Email);

                if (oUsuario != null)
                {
                    service.Save(oUsuario);
                }

                return View("NotificacionEmail");

            }
            catch (Exception ex)
            {

                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }

        }
        [HttpGet]
        //Vista para solicitar al cliente la nueva contraseña
        public ActionResult RecuperacionPassword(string token)
        {
            IUsuario service = new ServiceUsuario();
            try
            {
                if (token == null || token.Trim().Equals(""))
                {
                    return View("Index");
                }

                Usuario oUsuario = service.GetUsuarioByToken(token);
                if (oUsuario == null)
                {
                    ViewBag.Error = "Tu token ha expirado";
                    return View("Index");
                }
                aux = oUsuario;
                return View();
            }
            catch (Exception ex)
            {

                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }
        [HttpPost]
        public ActionResult Recuperacion(Usuario usuario)
        {
            aux.Password = usuario.Password;
            IUsuario service = new ServiceUsuario();
            try
            {
                if (aux != null)
                {
                    aux.TokenRecuperacion = null;
                    service.Save(aux);
                }
                return View("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }
        public ActionResult NotificacionCorreo()
        {
            return View();
        }
        [HttpGet]
        public ActionResult IniciarRecuperacion()
        {
            return View();
        }
        public ActionResult UnAuthorized()
        {
            try
            {
                ViewBag.Message = "No autorizado";

                if (Session["User"] != null)
                {
                    Usuario oEmpleado = Session["User"] as Usuario;
                    Log.Warn($"El Empleado {oEmpleado.Nombre} con el rol {oEmpleado.Rol.IDRol}-{oEmpleado.Rol.Descripcion}, intentó acceder una página sin derechos  ");
                }
                return View();
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }
    }
}
