using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace Proyecto_BDII
{
    public partial class PantallaLOGIN : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Limpiar sesiones previas al entrar al Login
                Session["UsuarioID"] = null;
                Session["Rol"] = null;
                Session["NombreUsuario"] = null;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
            {
                lblMensaje.Text = "Por favor, llene todos los campos.";
                lblMensaje.Visible = true;
                return;
            }

            string conectar = ConfigurationManager.ConnectionStrings["CadenaTienda"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conectar))
            {
                // Nota: En defensas de BD se recomienda usar parámetros para evitar Inyección SQL
                string query = "SELECT id_usuario, nombre, rol FROM usuario WHERE correo = @correo AND contraseña = @pass";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@pass", password); // Validación directa de la cadena

                    try
                    {
                        con.Open();
                        SqlDataReader leer = cmd.ExecuteReader();

                        if (leer.Read())
                        {
                            // Guardar datos clave en la Sesión
                            Session["UsuarioID"] = leer["id_usuario"].ToString();
                            Session["NombreUsuario"] = leer["nombre"].ToString();
                            string rol = leer["rol"].ToString().ToLower();
                            Session["Rol"] = rol;

                            // Redirección controlada según el Rol de la BD
                            if (rol == "admin")
                            {
                                Response.Redirect("PantallaADMIN.aspx");
                            }
                            else if (rol == "cliente")
                            {
                                Response.Redirect("PantallaUSER.aspx");
                            }
                        }
                        else
                        {
                            lblMensaje.Text = "Correo o contraseña incorrectos.";
                            lblMensaje.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMensaje.Text = "Error en el sistema: " + ex.Message;
                        lblMensaje.Visible = true;
                    }
                }
            }
        }
    }
}