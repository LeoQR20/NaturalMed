using Infraestructura.Models;
using Infraestructura.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        public void DeleteUsuario(int id)
        {
            int returno;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    Usuario usuario = GetUsuarioByID(id);
                    usuario.Estado = false;
                    ctx.Entry(usuario).State = EntityState.Modified;
                    returno = ctx.SaveChanges();
                }
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

        public Usuario GetUsuario(int id, string password)
        {
            Usuario oUsuario = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    oUsuario = ctx.Usuarios.Include("Rol").
                        Where(p => p.ID.Equals(id) && p.Password == password).
                        FirstOrDefault<Usuario>();
                }
                if (oUsuario != null)
                    oUsuario = GetUsuarioByID(oUsuario.ID);
                return oUsuario;
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

        public Usuario GetUsuarioByID(int id)
        {
            Usuario usuario = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    usuario = ctx.Usuarios.Where(p => p.ID == id).Where(p => p.Estado == true).
                        Include("Rol").FirstOrDefault();
                }
                return usuario;
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

        public IEnumerable<Usuario> GetUsuarios()
        {
            try
            {
                IEnumerable<Usuario> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Usuarios.Include("Rol").ToList<Usuario>();
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

        public Usuario Save(Usuario usuario)
        {
            int retorno = 0;
            Usuario oUsuario = null;
            try
            {
                usuario.Estado = true;
                using (MyContext ctx = new MyContext())
                {
                    //oUsuario.Estado = true;

                    ctx.Configuration.LazyLoadingEnabled = false;
                    oUsuario = GetUsuarioByID(usuario.ID);
                    if (oUsuario == null)
                    {                        
                        ctx.Usuarios.Add(usuario);
                    }
                    else
                    {
                        ctx.Entry(usuario).State = EntityState.Modified;
                    }
                    retorno = ctx.SaveChanges();
                }
                if (retorno >= 0)
                    oUsuario = GetUsuarioByID(usuario.ID);
                return oUsuario;
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

        public Usuario GetEmpleadoByToken(string token)
        {
            try
            {
                Usuario empleado = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    empleado = ctx.Usuarios.Where(p => p.TokenRecuperacion == token).Include("Rol").FirstOrDefault();
                }
                return empleado;

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
        private void SendEmail(string EmailDestino, string token)
        {
            string urlDomain = "https://localhost:44345/";
            string EmailOrigen = "Prac_Profesional22@outlook.com";
            string Contraseña = "Lqr200698";
            string url = urlDomain + "/Login/Recuperacion/?token=" + token;
            MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Recuperación de Contraseña",
                "<p>Estimado usuario,</br></br>Ha iniciado el proceso para recuperación de contraseña del sistema NaturalMend Inventory, a continuación se le presentará el link para que proceda a realizar el cambio de su respectiva contraseña.</p><br>" +
                "<a href='" + url + "'>Click para recuperar la contraseña</a></br>" +
                "<p>De no haber sido usted quién inició el proceso de recuperación, por favor comuníquelo a su administrador.</p>");

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient("smtp-mail.outlook.com");
            oSmtpClient.EnableSsl = true;
            oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Port = 587;
            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

            oSmtpClient.Send(oMailMessage);

            oSmtpClient.Dispose();
        }
        private string GetSha256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public Usuario VerificarUsuario(string email)
        {
            try
            {
                Usuario empleado = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    empleado = ctx.Usuarios.Where(p => p.Email == email).Include("Rol").FirstOrDefault();
                }
                if (empleado != null)
                {
                    empleado.TokenRecuperacion = GetSha256(Guid.NewGuid().ToString());
                    SendEmail(email, empleado.TokenRecuperacion);

                    return empleado;
                }
                else
                    return null;

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

        public void DeshabilitarUsuario(int id)
        {
            int retorno;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    Usuario usuario = GetUsuarioByID(id);
                    usuario.Estado = false;
                    ctx.Entry(usuario).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Utils.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Utils.Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw;
            }
        }
    }
}
