using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repository
{
    public interface IRepositoryUsuario
    {
        Usuario GetUsuario(int id, string password);
        IEnumerable<Usuario> GetUsuarios();
        Usuario GetUsuarioByID(int id);
        void DeleteUsuario(int id);
        Usuario Save(Usuario usuario);
        Usuario GetEmpleadoByToken(string token);
        Usuario VerificarUsuario(string email);
    }
}
