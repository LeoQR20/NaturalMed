using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models
{

    //Se crea un internal partial class para cada clase con su nombre "nombreClaseMetadata",
    //aqui se hacen los DataAnotations de cada clase
    internal partial class ProductoData
    {
        [Display(Name = "Producto")]
        public int ID { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Nullable<decimal> Precio { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Estado { get; set; }

        [Display(Name = "Foto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public byte[] Foto { get; set; }    

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Cantidad { get; set; }
    }
    internal partial class UsuarioMetaData
    {
        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Favor ingrese únicamente números")]
        public int ID { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Por favor digite una {0}")]
        [RegularExpression(@"^(?=.*[A-z])(?=.*[A-Z])(?=.*\D)[a-zA-Z\d\w\W]{9,}$", ErrorMessage = "La contraseña debe tener al menos 9 caracteres con mayúsculas, minúsculas, números y caracteres especiales")]
        public string Password { get; set; }   
        
        [Display(Name = "Rol")]
        public int IDRole { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Nombre { get; set; }
        public virtual Rol Rol { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor revise el formato de su {0}")]
        public string Email { get; set; }

        public Nullable<bool> Estado { get; set; }
    }

    internal partial class FacturaMetadata
    {
        [Display(Name ="Numero de Factura")]
        public int IdFactura { get; set; }

        [Display(Name = "Sub Total")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public Nullable<decimal> Subtotal { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public Nullable<decimal> Total { get; set; }

        [Display(Name = "Impuesto de Venta")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public Nullable<decimal> IVA { get; set; }

        public Nullable<int > IdCondicion { get; set; }

        [Display(Name = "Fecha de Creación")]
        public Nullable<System.DateTime> FechaCreacion { get; set; }

        [Display(Name = "Cliente")]
        public virtual Cliente Cliente { get; set; }

        [Display(Name = "Detalle Orden")]
        public virtual ICollection<Producto_Factura> FacturaDetalle { get; set; }
    }
    internal partial class ClienteMetaData
    {
        [Display(Name = "Cliente")]
        public string IdCliente { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Por favar ingrese su {0}")]
        public string Nombre { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "Por favar ingrese su {0}")]
        public string Apellidos{ get; set; }

        public string Sexo { get; set; }
        public string Telefono { get; set; }

        [Display(Name = "Correo Electronico")]
        public string email { get; set; }

        public virtual ICollection<Factura> Facturas { get; set; }
    }

}

