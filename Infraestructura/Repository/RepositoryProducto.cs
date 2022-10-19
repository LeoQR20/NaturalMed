using Infraestructura.Models;
using Infraestructura.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public class RepositoryProducto : IRepositoryProducto
    {
        public void DeleteProducto(int id)
        {
            throw new NotImplementedException();
        }

        public Producto GetProductoByID(int id)
        {
            Producto producto = null;
            try
            {
                using(MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    producto = ctx.Productos.Find(id);
                }
                return producto;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw;
            }
        }

        public IEnumerable<Producto> GetProductos()
        {
            IEnumerable<Producto> lista = null;
            using(MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Productos.ToList();
            }
            return lista;
        }

        public Producto Save(Producto producto)
        {
            int retorno = 0;
            Producto producto1 = null;
            using(MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                producto1 = GetProductoByID((int)producto.ID);
                if (producto1 == null)
                {
                    ctx.Productos.Add(producto);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.Productos.Add(producto);
                    ctx.Entry(producto).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }
            if (retorno >= 0 )            
                producto1 = GetProductoByID(producto.ID);
            return producto1;            
        }
        public IEnumerable<Producto> GetProductosByNombre(string nombre)
        {
            IEnumerable<Producto> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Productos.ToList().FindAll(p => p.Nombre.ToLower().Contains(nombre.ToLower()));

            }
            return lista;
        }
    }
}
