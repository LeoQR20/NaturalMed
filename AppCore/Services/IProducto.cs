using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public interface IProducto
    {
        Producto GetProductoByID(int id);
        Producto Save(Producto producto);
        void DeleteProducto(int id);
        IEnumerable<Producto> GetProductos();
        IEnumerable<Producto> GetProductosByNombre(string nombre);
    }
}
