using AppCore.Services;
using Infraestructura.Models;
using NaturalMed.Security;
using NaturalMed.Utils;
using NaturalMed.ViewModels;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace NaturalMed.Controllers
{
    public class ReportesController : Controller
    {
        static IEnumerable<Factura> listaOrdenes = null;
        static string mesOrdenes;
        static IEnumerable<Producto> listaProducts = null;

        // GET: Reportes
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult ReporteOrdenes() //AntesCierreDep
        {
            try
            {
                return View();
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
        }
        public ActionResult AjaxReporteOrdenes(ViewModelParametro parametro)
        {
            IEnumerable<Factura> lista = null;
            try
            {
                IFactura _ServiceOrden = new ServiceFactura();
                lista = _ServiceOrden.GetFacturaByFecha(parametro.Fecha);
                //Llena la lista para el PDF
                listaOrdenes = lista;
                //Convierte en letra el mes consultado para mostrarlo en el pdf
                mesOrdenes = parametro.Fecha.ToString("MMMM yyyy");
                return PartialView("_ReporteOrdenes", lista);
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
        }

        //Creación de PDF de Órdenes con Rotativa
        public ActionResult createPdfOrdenCatalogo()
        {
            ViewBag.mesOrdenes = mesOrdenes;
            return new PartialViewAsPdf("PdfOrdenes", listaOrdenes)
            {
                FileName = "Reporte Órdenes " + mesOrdenes + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(10, 10, 20, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page] --footer-font-size 10"
            };
        }
        public ActionResult ProductoCatalogo()
        {
            IEnumerable<Producto> lista = null;
            try
            {

                IProducto _ServiceLibro = new ServiceProducto();
                lista = _ServiceLibro.GetProductos();
                return View(lista);
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
        }
        public ActionResult AjaxReporteProducto(ViewModelParametro parametro)
        {
            IEnumerable<Producto> lista = null;
            try
            {
                IProducto _ServiceProducto = new ServiceProducto();
                IFactura _ServiceOrden = new ServiceFactura();
                lista = _ServiceProducto.GetProductos();
                //Llena la lista para el PDF
                listaProducts = lista;
                //Convierte en letra el mes consultado para mostrarlo en el pdf

                return PartialView("_ReporteProductos", lista);
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
        }


    }
}
