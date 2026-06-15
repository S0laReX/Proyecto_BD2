// _Header.ascx.cs
using System;
using System.Web.UI;

namespace Proyecto_BDII
{
    public partial class _Header : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] != null)
            {
                string rol = Session["Rol"].ToString();
                navAdmin.Visible = (rol == "admin");
                navUser.Visible = (rol != "admin");

                if (rol != "admin" && Session["UsuarioID"] != null)
                {
                    // Contar ítems en CarritoSesion
                    var carrito = Session["CarritoItems"] as System.Collections.Generic.List<CarritoItem>;
                    int total = 0;
                    if (carrito != null) foreach (var i in carrito) total += i.Cantidad;
                    litContadorCarrito.Text = total > 0 ? $" ({total})" : "";
                }
            }
        }
    }
}