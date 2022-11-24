using Infraestructura.Models;
using Infraestructura.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
            foreach (var item in lista)
            {
                
                if (item.Cantidad != null)
                {
                    if (item.Cantidad <= 3)
                    {
                        SendEmail(item);
                    }
                }
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
        private string HtmlBody(Producto prod)
        {
            string cantidad;
            if (prod.Cantidad != null)
            {
                cantidad = prod.Cantidad.ToString();
            }
            else
            {
                cantidad = prod.Cantidad.ToString();
            }
            return "<body style=\"color: black\">" +
                    "<h1 style=\"color: black\">Un producto se encuentra con pocas unidades en el inventario, se recomienda reavasteser el inventario.</h1>" + "<br />" +
                    "<h2 style=\"color: black\">Detalles del producto:</h2>" +
                    "<li style=\"color: black\"> ID: " + prod.ID.ToString() + "</ li >" +
                    "<li style=\"color: black\"> Nombre: " + prod.Nombre + "</ li >" +
                    "<li style=\"color: black\"> Descripcion: " + prod.Descripcion + "</ li >" +                    
                    "<li style=\"color: black\"> Cantidad en inventario: " + cantidad + "</ li >" +
                    "<li style=\"color: black\"> Cantidad mínima en inventario es de 3 </ li >" +
                    "<br />" +
                   "*Este es un mensaje auto-generado, por favor no responder*" + "<br />" +
                    "-Enviado el " + DateTime.Now.ToShortDateString() + "</i>" +
                    "</body>";
        }
        private void SendEmail(Producto prod)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("Prac_Profesional22@outlook.com");
                message.To.Add(new MailAddress("Prac_Profesional22@outlook.com"));
                message.Subject = prod.Nombre + " cuenta con muy pocas unidades!!!";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = HtmlBody(prod);
                smtp.Port = 587;
                smtp.Host = "smtp-mail.outlook.com"; //for outlook host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("Prac_Profesional22@outlook.com", "Lqr200698");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception)
            {

            }
        }

        public Producto updatePositions(int Id, int cantidad)
        {
            int retorno;
            Producto oProposal = null;
            try
            {
                using (NaturalMendEntities ctx = new NaturalMendEntities())
                {
                    /* La carga diferida retrasa la carga de datos relacionados,
                     * hasta que lo solicite específicamente.*/
                    ctx.Configuration.LazyLoadingEnabled = false;
                    oProposal = GetProductoByID(Id);

                    if (oProposal.Cantidad > 0)
                    {
                        oProposal.Cantidad -= cantidad;
                    }

                    ctx.Entry(oProposal).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
                if (retorno >= 0)
                    oProposal = GetProductoByID(oProposal.ID);
                return oProposal;
            }
            catch (DbUpdateException dbEx)
            {
                string msj = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref msj);
                throw new Exception(msj);
            }
            catch (Exception ex)
            {
                string msj = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref msj);
                throw;
            }
        }
    }
}
