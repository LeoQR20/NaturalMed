using AppCore.Util;
using Infraestructura.Models;
using Infraestructura.Repository;
using System;
using System.Collections.Generic;

namespace AppCore.Services
{
    public class ServiceUsuario : IUsuario
    {
        public void DeleteUsuario(int id)
        {
            throw new NotImplementedException();
        }

        public Usuario GetUsuario(int Id, string password)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            string crytpPasswd = Cryptography.EncrypthAES(password);
            return repository.GetUsuario(Id, password);
        }

        public Usuario GetUsuarioByID(int id)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            Usuario oUsuario = repository.GetUsuarioByID(id);
            oUsuario.Password = Cryptography.DecrypthAES(oUsuario.Password);
            return oUsuario;
        }

        public IEnumerable<Usuario> GetUsuarios()
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.GetUsuarios();
        }

        public Usuario Save(Usuario usuario)
        {
            RepositoryUsuario repositoryUsuario = new RepositoryUsuario();
            Usuario auxUsuario  = repositoryUsuario.GetUsuarioByID(usuario.ID);
            if (auxUsuario !=null)
            {
                if (auxUsuario.Password.Equals(usuario.Password))
                {
                    //Desencripta la contraseña para prevenir errores durante el proceso de recuperación
                    usuario.Password = Cryptography.DecrypthAES(usuario.Password);
                }
            }

            //Encripta la contraseña y la envía a la base de datos
            usuario.Password = Cryptography.EncrypthAES(usuario.Password);

            return repositoryUsuario.Save(usuario);

            //IRepositoryUsuario repository = new RepositoryUsuario();
            //usuario.Password = Cryptography.DecrypthAES(usuario.Password);
            //return repository.Save(usuario);
        }

        public Usuario VerificarUsuario(string email)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.VerificarUsuario(email);
        }

        public Usuario GetUsuarioByToken(string token)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.GetEmpleadoByToken(token);
        }
    }
}
