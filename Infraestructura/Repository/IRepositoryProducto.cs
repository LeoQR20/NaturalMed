using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public interface IRepositoryProducto
    {
        Producto GetProductoByID(int id);
        IEnumerable<Producto> GetProductos();
        Producto Save(Producto producto);
        void DeleteProducto(int id);
        IEnumerable<Producto> GetProductosByNombre(string nombre);

        Producto updatePositions(int Id);
    }
}
