//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infraestructura.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Rol
    {
        public Rol()
        {
            this.Usuarios = new HashSet<Usuario>();
        }
    
        public int IDRol { get; set; }
        public string Descripcion { get; set; }
    
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
