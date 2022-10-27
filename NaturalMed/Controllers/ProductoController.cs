using AppCore.Services;
using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using NaturalMed.Security;
using NaturalMed.Utils;

namespace NaturalMed.Controllers
{
    public class ProductoController : Controller
    {
        private static String Action;

        // Significa  que solo los que tienen el rol de Administrador pueden accederla 
        // ver Enums.cs  
        // public enum Roles { Administrador = 1, Cliente = 2}
        //[CustomAuthorize((int)Roles.Administrador)]
        // GET: Producto
        
        public ActionResult AdminProducto()
        {
            IEnumerable<Producto> lista = null;
            try
            {
                IProducto producto = new ServiceProducto();
                lista = producto.GetProductos();
                ViewBag.title = "Lista Productos";
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
        public ActionResult ClienteProducto()
        {
            IEnumerable<Producto> lista = null;
            try
            {
                IProducto producto = new ServiceProducto();
                lista = producto.GetProductos();
                ViewBag.title = "Lista Productos";
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

        // GET: Producto/Details/5
        public ActionResult Details(int? id)
        {
            ServiceProducto serviceproducto = new ServiceProducto();
            Producto producto = null;
            try
            {
                if (id == null)
                {
                    return RedirectToAction("AdminProducto");
                }
                producto = serviceproducto.GetProductoByID(id.Value);
                if (producto==null)
                {
                    TempData["Message"] = "No existe el Producto solicitado";
                    TempData["Redirect"] = "Producto";
                    TempData["Redirect-Action"] = "AdminProducto";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Producto";
                TempData["Redirect-Action"] = "AdminProducto";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        //GET: Producto/Create
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            ViewBag.idProducto = listaProductos();
            return View();
        }
        private SelectList listaProductos(int idProducto = 0)
        {
            IProducto sproducto = new ServiceProducto();
            IEnumerable<Producto> listaProductos = sproducto.GetProductos();
            return new SelectList(listaProductos, "IdProducto","Nombre", idProducto);

        }
        public ActionResult Catalogo()
        {
            IEnumerable<Producto> lista = null;
            try
            {
                Log.Info("Visita");

                if (!String.IsNullOrEmpty(Action))
                {
                    ViewBag.Action = Action;
                }

                IProducto _ServiceProducto = new ServiceProducto();
                lista = _ServiceProducto.GetProductos();
                Action = "";
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());

                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData.Keep();
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
            return View(lista);
        }

        // POST: Producto/Create
        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("AdminProducto");
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Producto producto, HttpPostedFileBase ImageFile)
        {
            string errores = "";
            MemoryStream target = new MemoryStream();
            IProducto _producto = new ServiceProducto();
            try
            {
                if (producto.Foto == null)
                {
                    if (ImageFile != null)
                    {
                        ImageFile.InputStream.CopyTo(target);
                        producto.Foto = target.ToArray();
                        ModelState.Remove("Foto");
                    }
                }
                 //Es Valiado
                if (ModelState.IsValid)
                {
                    Producto product = _producto.Save(producto);
                }
                else
                {
                    // Valida Errores si Javascript está deshabilitado
                    Util.ValidateErrors(this);
                    TempData["Message"] = "Error al procesar los datos! " + errores;
                    TempData.Keep();

                    return View("Create", producto);
                }
                return RedirectToAction("AdminProducto");
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos!" + ex.Message;
                TempData["Redirect"] = "Producto";
                TempData["Redirect-Action"] = "AdminProducto";
                //Redireccion a la capruta de Error
                return RedirectToAction("AdminProducto", "Producto");
            }
        }
        // GET: Producto/Edit/5
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            ServiceProducto service = new ServiceProducto();
            Producto producto = null;
            try
            {
                if (id == null)
                {
                    return RedirectToAction("AdminProducto");
                }
                producto = service.GetProductoByID(id.Value);
                if (producto == null)
                {
                    TempData["Message"] = "No existe el Producto";
                    TempData["Redirect"] = "Producto";
                    TempData["Redirect-Action"] = "Adminproducto";
                    //Redireccion a la capruta de Error
                    return RedirectToAction("Default", "Error");
                }
                ViewBag.IdProducto = listaProductos();
                return View(producto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos!" + ex.Message;
                TempData["Redirect"] = "Producto";
                TempData["Redirect-Action"] = "AdminProducto";
                //Redireccion a la capruta de Error
                return RedirectToAction("Default", "AdminProducto");
            }            
        }
        // POST: Producto/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("AdminProducto");
            }
            catch
            {
                return View();
            }
        }
        //public ActionResult buscarProducto(string filtro)
        //{
        //    IEnumerable<Producto> lista = null;
        //    IProducto producto = new ServiceProducto();

        //    // Error porque viene en blanco 
        //    if (string.IsNullOrEmpty(filtro))
        //    {
        //        lista = producto.GetProductos();
        //    }
        //    else
        //    {
        //        lista = producto.GetProductosByNombre(filtro);
        //    }
        //    // Retorna un Partial View
        //    return PartialView("BuscarProducto", lista);

        //}
    }
}
