﻿using Infraestructura.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public interface ICliente
    {
        Cliente GetCliente(int Id);
        IEnumerable<Cliente> GetClientes();
        
    }
}
