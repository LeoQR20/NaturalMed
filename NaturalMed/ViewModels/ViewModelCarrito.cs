using NaturalMed.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NaturalMed.ViewModels
{
    public class ViewModelCarrito
    {
        public List<ViewModelFacturaDetalle> Items { get; private set; }

        //Implementación Singleton

        // Las propiedades de solo lectura solo se pueden establecer en la inicialización o en un constructor
        public static readonly ViewModelCarrito Instancia;

        // Se llama al constructor estático tan pronto como la clase se carga en la memoria
        static ViewModelCarrito()
        {
            // Si el ViewModelCarrito no está en la sesión, cree uno y guarde los items.
            if (HttpContext.Current.Session["ViewModelCarrito"] == null)
            {
                Instancia = new ViewModelCarrito();
                Instancia.Items = new List<ViewModelFacturaDetalle>();
                HttpContext.Current.Session["ViewModelCarrito"] = Instancia;
            }
            else
            {
                // De lo contrario, obténgalo de la sesión.
                Instancia = (ViewModelCarrito)HttpContext.Current.Session["ViewModelCarrito"];
            }
        }

        // Un constructor protegido asegura que un objeto no se puede crear desde el exterior
        protected ViewModelCarrito() { }

        /**
         * AgregarItem (): agrega un artículo a la compra
         */
        public String AgregarItem(int ProductoId)
        {
            String mensaje = "";
            // Crear un nuevo artículo para agregar al ViewModelCarrito
            ViewModelFacturaDetalle nuevoItem = new ViewModelFacturaDetalle(ProductoId);
            // Si este artículo ya existe en lista de productos, aumente la Cantidad
            // De lo contrario, agregue el nuevo elemento a la lista
            if (nuevoItem != null)
            {
                if (Items.Exists(x => x.ProductoId == ProductoId))
                {
                    ViewModelFacturaDetalle item = Items.Find(x => x.ProductoId == ProductoId);
                    item.Cantidad++;
                }
                else
                {
                    nuevoItem.Cantidad = 1;
                    Items.Add(nuevoItem);
                }
                mensaje = SweetAlertHelper.Mensaje("Información", "¡Agregado con éxito!", SweetAlertMessageType.success);

            }
            else
            {
                mensaje = SweetAlertHelper.Mensaje("Orden", "El producto solicitado no existe", SweetAlertMessageType.warning);
            }
            return mensaje;
        }


        /**
         * SetItemCantidad(): cambia la Cantidad de un artículo en el ViewModelCarrito
         */
        public String SetItemCantidad(int ProductoId, int Cantidad)
        {
            String mensaje = "";
            // Si estamos configurando la Cantidad a 0, elimine el artículo por completo
            if (Cantidad == 0)
            {
                EliminarItem(ProductoId);
                mensaje = SweetAlertHelper.Mensaje("Orden", "Producto Eliminado", SweetAlertMessageType.success);

            }
            else
            {
                // Encuentra el artículo y actualiza la Cantidad
                ViewModelFacturaDetalle actualizarItem = new ViewModelFacturaDetalle(ProductoId);
                if (Items.Exists(x => x.ProductoId == ProductoId))
                {
                    ViewModelFacturaDetalle item = Items.Find(x => x.ProductoId == ProductoId);
                    item.Cantidad = Cantidad;
                    mensaje = SweetAlertHelper.Mensaje("Orden", "Cantidad Actualizada", SweetAlertMessageType.success);

                }
            }
            return mensaje;

        }

        /**
         * EliminarItem (): elimina un artículo del ViewModelCarrito de compras
         */
        public String EliminarItem(int ProductoId)
        {
            String mensaje = "El producto no existe";
            if (Items.Exists(x => x.ProductoId == ProductoId))
            {
                var itemEliminar = Items.Single(x => x.ProductoId == ProductoId);
                Items.Remove(itemEliminar);
                mensaje = SweetAlertHelper.Mensaje("Orden", "Producto Eliminado", SweetAlertMessageType.success);
            }
            return mensaje;

        }
        //public decimal GetSubTotal()
        //{
        //    decimal subtotal = 0;
        //    subtotal = Items.Sum(x => x.SubTotal);

        //    return subtotal;
        //}


        /**
         * GetTotal() - Devuelve el precio total de todos los productos.
         */
        public decimal GetTotal()
        {
            decimal total = 0;
            total = Items.Sum(x => x.SubTotal);

            return total;
        }
        public int GetCountItems()
        {
            int total = 0;
            total = Items.Sum(x => x.Cantidad);

            return total;
        }


        public void eliminarViewModelCarrito()
        {
            Items.Clear();

        }

        //public decimal GetImpuesto()
        //{
        //    decimal impuesto = 0;
        //    impuesto = GetSubTotal() * 0.13M;
        //    return impuesto;
        //    //return (decimal)(Items.Sum(x => x.Producto.Precio));
        //}
    }
}