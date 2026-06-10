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
    public partial class PantallaREGISTRO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
            {
                MostrarMensaje("Todos los campos son obligatorios.", true);
                return;
            }

            string conectar = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conectar))
            {
                // Validación previa para no duplicar correos (Restricción UNIQUE en la BD)
                string verificarQuery = "SELECT COUNT(*) FROM usuario WHERE correo = @correo";

                using (SqlCommand cmdVerificar = new SqlCommand(verificarQuery, con))
                {
                    cmdVerificar.Parameters.AddWithValue("@correo", correo);

                    try
                    {
                        con.Open();
                        int existe = (int)cmdVerificar.ExecuteScalar();

                        if (existe > 0)
                        {
                            MostrarMensaje("El correo ya se encuentra registrado.", true);
                            return;
                        }

                        // Inserción limpia respetando las propiedades por defecto (rol = 'cliente')
                        string insertQuery = "INSERT INTO usuario (nombre, correo, contraseña, rol, fecha_registro) " +
                                             "VALUES (@nombre, @correo, @pass, 'cliente', GETDATE())";

                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                        {
                            cmdInsert.Parameters.AddWithValue("@nombre", nombre);
                            cmdInsert.Parameters.AddWithValue("@correo", correo);
                            cmdInsert.Parameters.AddWithValue("@pass", password);

                            int filasAfectadas = cmdInsert.ExecuteNonQuery();

                            if (filasAfectadas > 0)
                            {
                                MostrarMensaje("¡Registro exitoso! Redirigiendo...", false);
                                // Breve retraso antes de mandar al Login
                                Response.AppendHeader("Refresh", "2;url=PantallaLOGIN.aspx");
                            }
                            else
                            {
                                MostrarMensaje("No se pudo completar el registro.", true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error al registrar: " + ex.Message, true);
                    }
                }
            }
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.Visible = true;
            if (esError)
            {
                lblMensaje.CssClass = "msg-box error-msg";
            }
            else
            {
                lblMensaje.CssClass = "msg-box success-msg";
            }
        }
    }
}