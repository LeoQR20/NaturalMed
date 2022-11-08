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
    public class RepositoryFactura : IRepositoryFactura
    {
        public IEnumerable<Factura> GetAll()
        {
            List<Factura> facturaList = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    facturaList = ctx.Facturas.Include("Cliente").
                        Include("CondicionFactura").Include("Producto_Factura").
                        OrderByDescending(x => x.IdFactura).ToList<Factura>();
                }
                return facturaList;
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
                throw new Exception(mensaje);
            }
        }

        public IEnumerable<Factura> GetFacturaByCondicion(int estado)
        {
            List<Factura> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Facturas.Include("EstadoFactura").Include("Producto_Factura").Where(x => x.IdCondicion == estado).ToList();
            }
            return lista;
        }

        public Factura GetFacturaById(int id)
        {
            Factura factura = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    factura = ctx.Facturas.Include("Cliente").
                        Include("Producto_Factura.Producto").
                        Include("CondicionFactura").
                        Where(p => p.IdFactura == id).FirstOrDefault<Factura>();
                }
                return factura;
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
                throw new Exception(mensaje);
            }
        }

        public IEnumerable<Factura> GetFacturaByFecha(DateTime fecha)
        {
            try
            {
                IEnumerable<Factura> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;

                    //Para definir el primer día del mes
                    DateTime fecha1 = fecha;
                    while (fecha1.Day > 1)
                    {
                        fecha1 = fecha1.AddDays(-1);
                    }

                    //Para definir el último día del mes
                    DateTime fecha2 = fecha;
                    while (fecha2.Day < DateTime.DaysInMonth(fecha.Year, fecha.Month))
                    {
                        fecha2 = fecha2.AddDays(1);
                    }

                    lista = ctx.Facturas.Include("Producto_Factura").
                               Where(p => p.FechaCreacion >= fecha1 && p.FechaCreacion <= fecha2).
                               OrderBy(p => p.FechaCreacion).
                               ToList<Factura>();
                }
                return lista;
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

        public Factura Save(Factura factura)
        {
            int retorno = 0;
            Factura oFactura = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {

                    ctx.Configuration.LazyLoadingEnabled = false;
                    oFactura = GetFacturaById(factura.IdFactura);
                    if (oFactura == null)
                    {
                        ctx.Facturas.Add(factura);
                    }
                    else
                    {
                        ctx.Entry(factura).State = EntityState.Modified;
                    }
                    retorno = ctx.SaveChanges();
                }

                if (retorno >= 0)
                    oFactura = GetFacturaById(factura.IdFactura);

                return oFactura;
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
    }
}
