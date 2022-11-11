using AppCore.Services;
using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NaturalMed.ViewModels
{
    public class ViewModelFacturaDetalle
    {
        [Display(Name = "Número de Orden")]
        public int OrdenId { get; set; }
        public int ProductoId { get; set; }
        public byte[] Imagen { get; set; }
        public int Cantidad { get; set; }

        public decimal Precio { get { return (decimal)Producto.Precio; } }
        public virtual Producto Producto { get; set; }
        public virtual Factura Orden { get; set; }

        public decimal SubTotal
        {
            get
            {
                return calculoSubtotal();
            }
        }
        private decimal calculoSubtotal()
        {
            return this.Precio * this.Cantidad;
        }

        //public decimal Total
        //{
        //    get
        //    {
        //        return calculoSubtotal();
        //    }
        //}

        //private decimal total()
        //{

        //    decimal sub = calculoSubtotal();
        //    decimal total;

        //    total = (decimal)((sub * Producto.Precio) + sub);
        //    return total;
        //}


        public ViewModelFacturaDetalle(int ProductoId)
        {
            IProducto _ServiceProducto = new ServiceProducto();
            this.ProductoId = ProductoId;
            this.Producto = _ServiceProducto.GetProductoByID(ProductoId);
        }

        public int getUltimaOrdenId()
        {
            IFactura serviceOrden = new ServiceFactura();
            if (serviceOrden.GetFacturas().Count<Factura>() == 0)
            {
                return 1;
            }
            else
                return serviceOrden.GetFacturas().First<Factura>().IdFactura + 1;
        }

        //Fin
    }
}