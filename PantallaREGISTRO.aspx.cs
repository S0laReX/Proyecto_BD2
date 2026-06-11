using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaREGISTRO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Opcional: limpiar mensajes previos en recargas de página
            if (!IsPostBack)
            {
                lblMensaje.Visible = false;
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmarPassword = TextBox1.Text.Trim(); // Mapeado al ID="TextBox1" de tu ASPX

            // 1. Validación de campos vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmarPassword))
            {
                MostrarMensaje("Todos los campos son obligatorios.", true);
                return;
            }

            // 2. Validación de coincidencia de contraseñas
            if (password != confirmarPassword)
            {
                MostrarMensaje("Las contraseñas ingresadas no coinciden.", true);
                return;
            }

            // 3. Validación de complejidad de la contraseña (Mínimo 8 caracteres)
            if (password.Length < 8)
            {
                MostrarMensaje("La contraseña debe tener una longitud mínima de 8 caracteres.", true);
                return;
            }

            // 4. Validación de caracteres especiales (Mínimo 3)
            // Esta expresión regular cuenta cuántos caracteres que NO son letras ni números existen
            int cantidadEspeciales = Regex.Matches(password, @"[^a-zA-Z0-9]").Count;
            if (cantidadEspeciales < 3)
            {
                MostrarMensaje("La contraseña debe contener al menos 3 caracteres especiales (Ej: @, #, $, %, *, !, ?).", true);
                return;
            }

            string conectar = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conectar))
            {
                // Validación previa para evitar la duplicidad de correos electrónicos en el sistema
                string verificarQuery = "SELECT COUNT(*) FROM usuario WHERE correo = @correo";

                using (SqlCommand cmdVerificar = new SqlCommand(verificarQuery, con))
                {
                    cmdVerificar.Parameters.AddWithValue("@correo", correo);

                    try
                    {
                        con.Open();
                        int usuarioExistente = (int)cmdVerificar.ExecuteScalar();

                        if (usuarioExistente > 0)
                        {
                            MostrarMensaje("Este correo electrónico ya se encuentra registrado.", true);
                            return;
                        }

                        // Inserción limpia del nuevo usuario usando parámetros para evitar Inyección SQL
                        string insertQuery = "INSERT INTO usuario (nombre, correo, contraseña, rol, estado_2fa) VALUES (@nombre, @correo, @pass, 'cliente', 0)";

                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                        {
                            cmdInsert.Parameters.AddWithValue("@nombre", nombre);
                            cmdInsert.Parameters.AddWithValue("@correo", correo);
                            cmdInsert.Parameters.AddWithValue("@pass", password); // En entornos de producción real se recomienda encriptar (Hashing)

                            int filasAfectadas = cmdInsert.ExecuteNonQuery();

                            if (filasAfectadas > 0)
                            {
                                MostrarMensaje("¡Registro exitoso! Redirigiendo al inicio de sesión...", false);
                                // Genera un retraso controlado de 2 segundos antes de redirigir a la pantalla de Login
                                Response.AppendHeader("Refresh", "2;url=PantallaLOGIN.aspx");
                            }
                            else
                            {
                                MostrarMensaje("No se pudo completar el proceso de registro en el servidor.", true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarMensaje("Error crítico al registrar: " + ex.Message, true);
                    }
                    finally
                    {
                        if (con.State == System.Data.ConnectionState.Open)
                        {
                            con.Close();
                        }
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
                // Cambia el estilo visual dinámicamente si cuentas con clases de Bootstrap o CSS personalizado
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.CssClass = "msg-box error-msg";
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Green;
                lblMensaje.CssClass = "msg-box success-msg";
            }
        }
    }
}