using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaLOGIN : System.Web.UI.Page
    {
        private string conexion = System.Configuration.ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string obtenerRol()
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = "SELECT rol FROM usuario WHERE correo = @correo AND contraseña = @pass";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@correo", txtEmail.Text);
            cmd.Parameters.AddWithValue("@pass", txtContraseña.Text);
            con.Open();
            string rol = cmd.ExecuteScalar().ToString();
            con.Close();
            return rol;
        }

        public bool EsValido()
        {
            
            SqlConnection con = new SqlConnection(conexion);
            string consulta = "SELECT COUNT(1) FROM usuario WHERE correo = @correo AND contraseña = @pass";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@correo", txtEmail.Text);
            cmd.Parameters.AddWithValue("@pass", txtContraseña.Text);
            con.Open();
            int num = Convert.ToInt32(cmd.ExecuteScalar());
            Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            if (num == 1)
            {
                
              
                return true;
            }
            else
            {
                return false;
            }
            

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
                if (EsValido())
                {
                    if (obtenerRol() == "admin")
                    {
                        Response.Redirect("PantallaADMIN.aspx");
                    }
                    else
                    {
                        Response.Redirect("PantallaUSER.aspx");
                    }
                    
                }
                else
                {
                    lblMensaje.Text = "Correo electrónico o contraseña incorrectos.";
                }
            }
        }

        protected void btnRegistro_Click(object sender, EventArgs e)
        {
            Response.Redirect("PantallaREGISTRO.aspx");
        }
    }
}