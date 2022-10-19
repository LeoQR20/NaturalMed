using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public interface IUsuario
    {
        Usuario GetUsuario(int Id, string password);
        IEnumerable<Usuario> GetUsuarios();
        Usuario GetUsuarioByID(int id);
        void DeleteUsuario(int id);
        Usuario Save(Usuario usuario);
        Usuario VerificarUsuario(string email);
        Usuario GetUsuarioByToken(string token);


    }
}
