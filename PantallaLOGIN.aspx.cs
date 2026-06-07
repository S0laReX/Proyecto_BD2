using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaLOGIN : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        
        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string contraseña = txtContraseña.Text;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contraseña))
            {
                lblMensaje.Text = "Por favor, ingrese su correo electrónico y contraseña.";
            }
            else
            {
                if (email == "hola@ejemplo.com" && contraseña == "123456")
                {
                    Response.Redirect("PantallaUSER.aspx");
                }
                else
                {
                    lblMensaje.Text = "Correo electrónico o contraseña incorrectos.";
                }
            }
        }
    }
}