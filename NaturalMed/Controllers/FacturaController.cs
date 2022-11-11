using AppCore.Services;
using DocumentFormat.OpenXml.Bibliography;
using Infraestructura.Models;
using Infraestructura.Repository;
using NaturalMed.Security;
using NaturalMed.Utils;
using NaturalMed.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Xml;

namespace NaturalMed.Controllers
{
    public class FacturaController : Controller
    {
        private static String Action;
        private Factura factura1;
        private Usuario usuario1;
        private DataSet _dsDetalle;


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
            //return RedirectToAction("_DetalleOrden", ViewModelCarrito.Instancia.Items);
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

        public void FacturaElectronicaCR(DataSet dsDetalle)
        {
            _dsDetalle = dsDetalle;

        }
        private void GeneraXML(System.Xml.XmlTextWriter writer) // As System.Xml.XmlTextWriter
        {
            try
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("FacturaElectronica");

                writer.WriteAttributeString("xmlns", "https://tribunet.hacienda.go.cr/docs/esquemas/2017/v4.2/facturaElectronica");
                writer.WriteAttributeString("xmlns:ds", "http://www.w3.org/2000/09/xmldsig#");
                writer.WriteAttributeString("xmlns:vc", "http://www.w3.org/2007/XMLSchema-versioning");
                writer.WriteAttributeString("xmlns:xs", "http://www.w3.org/2001/XMLSchema");

                // La clave se crea con la función CreaClave de la clase Datos
                //writer.WriteElementString("Clave", _numeroClave);

                // 'El numero de secuencia es de 20 caracteres, 
                // 'Se debe de crear con la función CreaNumeroSecuencia de la clase Datos
                //writer.WriteElementString("NumeroConsecutivo", _numeroConsecutivo);

                // 'El formato de la fecha es yyyy-MM-ddTHH:mm:sszzz
                writer.WriteElementString("FechaEmision", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

                writer.WriteStartElement("Emisor");

                writer.WriteElementString("Nombre", usuario1.Nombre);
                writer.WriteStartElement("Identificacion");
                //writer.WriteElementString("Tipo", _emisor.Identificacion_Tipo);
                writer.WriteElementString("Numero", usuario1.ID.ToString());
                writer.WriteEndElement(); // 'Identificacion

                // '-----------------------------------
                // 'Los datos de las ubicaciones los puede tomar de las tablas de datos, 
                // 'Debe ser exacto al que hacienda tiene registrado para su compañia
                //writer.WriteStartElement("Ubicacion");
                //writer.WriteElementString("Provincia", _emisor.Ubicacion_Provincia);
                //writer.WriteElementString("Canton", _emisor.Ubicacion_Canton);
                //writer.WriteElementString("Distrito", _emisor.Ubicacion_Distrito);
                //writer.WriteElementString("Barrio", _emisor.Ubicacion_Barrio);
                //writer.WriteElementString("OtrasSenas", _emisor.Ubicacion_OtrasSenas);
                //writer.WriteEndElement(); // 'Ubicacion

                //writer.WriteStartElement("Telefono");
                ////writer.WriteElementString("CodigoPais", _emisor.Telefono_CodigoPais);
                //writer.WriteElementString("NumTelefono", usuario1.Email);
                //writer.WriteEndElement(); // 'Telefono

                writer.WriteElementString("CorreoElectronico", usuario1.Email);

                writer.WriteEndElement(); // Emisor
                                          // '------------------------------------
                                          // 'Receptor es similar con emisor, los datos opcionales siempre siempre siempre omitirlos.
                                          // 'La ubicacion para el receptor es opcional por ejemplo
                writer.WriteStartElement("Receptor");
                writer.WriteElementString("Nombre", factura1.Cliente.Nombre);
                writer.WriteStartElement("Identificacion");
                //// 'Los tipos de identificacion los puede ver en la tabla de datos
                //writer.WriteElementString("Tipo", _receptor.Identificacion_Tipo);
                writer.WriteElementString("Numero", factura1.Cliente.IdCliente.ToString());
                writer.WriteEndElement(); // 'Identificacion

                //writer.WriteStartElement("Telefono");
                //writer.WriteElementString("CodigoPais", _receptor.Telefono_CodigoPais);
                writer.WriteElementString("NumTelefono", factura1.Cliente.Telefono);
                writer.WriteEndElement(); // 'Telefono

                writer.WriteElementString("CorreoElectronico", factura1.Cliente.email);

                writer.WriteEndElement(); // Receptor
                                          // '------------------------------------

                // 'Loa datos estan en la tabla correspondiente
                //writer.WriteElementString("CondicionVenta", _condicionVenta);
                // '01: Contado
                // '02: Credito
                // '03: Consignación
                // '04: Apartado
                // '05: Arrendamiento con opcion de compra
                // '06: Arrendamiento con función financiera
                // '99: Otros

                // 'Este dato se muestra si la condicion venta es credito
                //writer.WriteElementString("PlazoCredito", _plazoCredito);

                //writer.WriteElementString("MedioPago", _medioPago);
                // '01: Efectivo
                // '02: Tarjeta
                // '03: Cheque
                // '04: Transferecia - deposito bancario
                // '05: Recaudado por terceros            
                // '99: Otros

                writer.WriteStartElement("DetalleServicio");

                // '-------------------------------------
                foreach (DataRow dr in _dsDetalle.Tables["Producto_Factura"].Rows)
                {
                    writer.WriteStartElement("LineaDetalle");

                    writer.WriteElementString("NumeropLinea", dr["numero_linea"].ToString());

                    writer.WriteStartElement("Codigo");
                    writer.WriteElementString("Tipo", dr["articulo_tipo"].ToString());
                    writer.WriteElementString("Codigo", dr["articulo_codigo"].ToString());
                    writer.WriteEndElement(); // 'Codigo

                    writer.WriteElementString("Cantidad", dr["cantidad"].ToString());
                    // 'Para las unidades de medida ver la tabla correspondiente
                    writer.WriteElementString("UnidadMedida", dr["unidad_medida"].ToString());
                    writer.WriteElementString("Detalle", dr["detalle_articulo"].ToString());
                    writer.WriteElementString("PrecioUnitario", String.Format("{0:N3}", dr["precio_unitario"].ToString()));
                    writer.WriteElementString("MontoTotal", String.Format("{0:N3}", dr["monto_total"].ToString()));
                    writer.WriteElementString("MontoDescuento", String.Format("{0:N3}", dr["nonto_descuento"].ToString()));
                    writer.WriteElementString("NaturalezaDescuento", dr["naturaleza_descuento"].ToString());
                    writer.WriteElementString("SubTotal", String.Format("{0:N3}", dr["sub_total"].ToString()));

                    writer.WriteStartElement("Impuesto");
                    writer.WriteElementString("Codigo", dr["impuesto_codigo"].ToString());
                    writer.WriteElementString("Tarifa", dr["impuesto_tarifa"].ToString());
                    writer.WriteElementString("Monto", dr["impuesto_monto"].ToString());
                    writer.WriteEndElement(); // Impuesto

                    writer.WriteElementString("MontoTotalLinea", String.Format("{0:N3}", dr["monto_linea"].ToString()));

                    writer.WriteEndElement(); // LineaDetalle
                }
                // '-------------------------------------

                writer.WriteEndElement(); // DetalleServicio


                writer.WriteStartElement("ResumenFactura");

                // Estos campos son opcionales, solo fin desea facturar en dólares
                //writer.WriteElementString("CodigoMoneda", _codigoMoneda);
                //writer.WriteElementString("TipoCambio", "aqui_tipo_cambio");
                // =================

                // 'En esta parte los totales se pueden ir sumando linea a linea cuando se carga el detalle
                // 'ó se pasa como parametros al inicio
                writer.WriteElementString("TotalServGravados", "");
                writer.WriteElementString("TotalServExentos", "");
                writer.WriteElementString("TotalMercanciasGravadas", "");
                writer.WriteElementString("TotalMercanciasExentas", "");

                writer.WriteElementString("TotalGravado", "");
                writer.WriteElementString("TotalExento", "");

                writer.WriteElementString("TotalVenta", "");
                writer.WriteElementString("TotalDescuentos", "");
                writer.WriteElementString("TotalVentaNeta", "");
                writer.WriteElementString("TotalImpuesto", "");
                writer.WriteElementString("TotalComprobante", "");
                writer.WriteEndElement(); // ResumenFactura

                // 'Estos datos te los tiene que brindar los encargados del area financiera
                writer.WriteStartElement("Normativa");
                writer.WriteElementString("NumeroResolucion", "");
                writer.WriteElementString("FechaResolucion", "");
                writer.WriteEndElement(); // Normativa

                // 'Aqui va la firma, despues la agregamos.

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        //[CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Save(Factura factura)
        {
            IProducto _Serviceproducto = new ServiceProducto();
            Producto producto = null;
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
                        producto = _Serviceproducto.updatePositions(item.Cantidad);
                    }
                }

                
                //Se actualizan los valores de Impuesto y totales a la Orden
                //factura.SubTotal = ViewModelCarrito.Instancia.GetSubTotal();
                //factura.Total = ViewModelCarrito.Instancia.GetImpuesto();
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
            RepositoryFactura repository = new RepositoryFactura();      
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
                //repository.SendEmail(orden);
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
