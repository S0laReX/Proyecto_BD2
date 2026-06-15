using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_BDII
{
    [Serializable]
    public class CarritoItem
    {
        public int IdCelular { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnit { get; set; }
        public int Cantidad { get; set; }
        public int StockMax { get; set; }
        public decimal Subtotal => PrecioUnit * Cantidad;
    }
}