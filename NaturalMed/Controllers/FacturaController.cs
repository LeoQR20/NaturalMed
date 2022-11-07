using AppCore.Services;
using Infraestructura.Models;
using NaturalMed.Security;
using NaturalMed.Utils;
using NaturalMed.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NaturalMed.Controllers
{
    public class FacturaController : Controller
    {
        private static String Action;
        // GET: Factura
        public ActionResult Index()
        {
            ICondicionFactura factura = new ServiceCondicionFactura();
            if (TempData.ContainsKey("NotificationMessage"))
            {
                ViewBag.NotificationMessage = TempData["NotificationMessage"];
            }
            ViewBag.idCliente = listaClientes();
            ViewBag.CondicionFacturaId =factura.GetCondicionFacturas();
            ViewBag.FacturaProducto = ViewModelCarrito.Instancia.Items;

            return View();
        }

        private SelectList listaClientes()
        {
            //Lista de Clientes
            ICliente _ServiceCliente = new ServiceCliente();
            IEnumerable<Cliente> listaClientes = _ServiceCliente.GetClientes();

            return new SelectList(listaClientes, "IdCliente", "IdCliente");
        }

        //Ordenar un producto y agregarlo al carrito
        public ActionResult ordenarProducto(int? ProductoID)
        {
            int cantidadProductos = ViewModelCarrito.Instancia.Items.Count();
            ViewBag.NotiCarrito = ViewModelCarrito.Instancia.AgregarItem((int)ProductoID);
            return PartialView("_OrdenCantidad");

        }

        //Actualizar Vista parcial detalle carrito
        private ActionResult DetalleCarrito()
        {

            return PartialView("_DetalleOrden", ViewModelCarrito.Instancia.Items);
        }

        //Actualizar cantidad de productos en el carrito
        public ActionResult actualizarCantidad(int productoID, int cantidad)
        {
            ViewBag.DetalleOrden = ViewModelCarrito.Instancia.Items;
            TempData["NotiCarrito"] = ViewModelCarrito.Instancia.SetItemCantidad(productoID, cantidad);
            TempData.Keep();
            return PartialView("_DetalleOrden", ViewModelCarrito.Instancia.Items);

        }

        //Actualizar solo la cantidad de productos que se muestra en el menú
        public ActionResult actualizarFacturaCantidad()
        {
            if (TempData.ContainsKey("NotiCarrito"))
            {
                ViewBag.NotiCarrito = TempData["NotiCarrito"];
            }
            int cantidadProductos = ViewModelCarrito.Instancia.Items.Count();
            return PartialView("_OrdenCantidad");

        }

        //Eliminar el producto de la orden
        public ActionResult eliminarProducto(int? productoID)
        {
            ViewBag.NotificationMessage = ViewModelCarrito.Instancia.EliminarItem((int)productoID);
            return PartialView("_DetalleOrden", ViewModelCarrito.Instancia.Items);
        }

        //Listado
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult IndexAdmin()
        {
            IEnumerable<Factura> lista = null;
            if (!String.IsNullOrEmpty(Action))
            {
                ViewBag.Action = Action;
            }
            try
            {
                IFactura _ServiceOrden = new ServiceFactura();
                lista = _ServiceOrden.GetFacturas();
                return View(lista);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Factura";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        // GET: Factura/Details/5
        public ActionResult Details(int? id)
        {
            ServiceFactura _ServiceFactura = new ServiceFactura();
            Factura orden = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("IndexAdmin");
                }

                orden = _ServiceFactura.GetFacturaById(id.Value);
                if (orden == null)
                {
                    TempData["Message"] = "No existe la factura solicitada";
                    TempData["Redirect"] = "Factura";
                    TempData["Redirect-Action"] = "IndexAdmin";
                    //TempData.Keep();
                    return RedirectToAction("Default", "Error");
                }
                return View(orden);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Orden";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [HttpPost]
        //[CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Save(Factura factura)
        {
            Producto_Factura detalle = new Producto_Factura();
            try
            {
                // Si no existe la sesión no hay nada
                if (ViewModelCarrito.Instancia.Items.Count() <= 0)
                {
                    // Validaciones de datos requeridos.
                    TempData["NotificationMessage"] = Utils.SweetAlertHelper.Mensaje("Orden", "Seleccione los productos a ordenar", SweetAlertMessageType.warning);
                    return RedirectToAction("Index");
                }
                else
                {
                    var listaDetalle = ViewModelCarrito.Instancia.Items;
                    String localDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    factura.FechaCreacion = Convert.ToDateTime(localDate);
                    factura.IdCondicion = 1;

                    foreach (var item in listaDetalle)
                    {
                        Producto_Factura facturaDetalle = new Producto_Factura();
                        facturaDetalle.IDFactura = item.OrdenId;
                        facturaDetalle.IDProducto = item.ProductoId;
                        facturaDetalle.Precio = item.Precio;
                        facturaDetalle.Cantidad = item.Cantidad;
                        factura.Producto_Factura.Add(facturaDetalle);
                    }
                }
                //Se actualizan los valores de Impuesto y totales a la Orden
                factura.SubTotal = ViewModelCarrito.Instancia.GetSubTotal();
                factura.Total = ViewModelCarrito.Instancia.GetImpuesto();
                factura.Total = ViewModelCarrito.Instancia.GetTotal();
                //Se pone la orden en condición 1: Pendiente
                factura.IdCondicion = 1;

                IFactura _ServiceOrden = new ServiceFactura();
                Factura ordenSave = _ServiceOrden.Save(factura);

                // Limpia el Carrito de compras
                ViewModelCarrito.Instancia.eliminarViewModelCarrito();
                TempData["NotificationMessage"] = Utils.SweetAlertHelper.Mensaje("Orden", "Orden guardada satisfactoriamente!", SweetAlertMessageType.success);
                // Reporte orden
                return View("OrdenCompleta");
            }
            catch (Exception ex)
            {
                // Salvar el error  
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Orden";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult AprobarOrden(int? id)
        {
            IFactura _ServiceOrden = new ServiceFactura();
            Factura orden = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("List");
                }

                orden = _ServiceOrden.GetFacturaById(id.Value);

                orden.IdCondicion = 2;

                //Se fijan algunos valores nulos para evitar que se dupliquen
                orden.CondicionFactura = null;
                orden.Producto_Factura = null;

                _ServiceOrden.Save(orden);

                Action = "S";

                //Cargo la Lista Actualizada
                IEnumerable<Factura> listaOrdenes = _ServiceOrden.GetFacturas();

                return View("IndexAdmin", listaOrdenes);
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
