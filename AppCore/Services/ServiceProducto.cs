using Infraestructura.Models;
using Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class ServiceProducto : IProducto
    {
        public void DeleteProducto(int id)
        {
            throw new NotImplementedException();
        }

        public Producto GetProductoByID(int id)
        {
            IRepositoryProducto repository = new RepositoryProducto();
            return repository.GetProductoByID(id);
        }

        public IEnumerable<Producto> GetProductos()
        {
            IRepositoryProducto repository = new RepositoryProducto();
            return repository.GetProductos();
        }

        public Producto Save(Producto producto)
        {
            IRepositoryProducto repository = new RepositoryProducto();
            return repository.Save(producto);
        }
        public IEnumerable<Producto> GetProductosByNombre(string nombre)
        {
            IRepositoryProducto repository = new RepositoryProducto();
            return repository.GetProductosByNombre(nombre);
        }
    }
}
