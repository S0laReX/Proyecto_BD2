using System;
using System.Data.SqlClient;
using System.Web.UI;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Net; // Requerido para la petición al servidor de Google
using System.IO;  // Requerido para leer la respuesta de Google

namespace Proyecto_BDII
{
    public partial class PantallaLOGIN : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Clear();
            }
        }

        private string HashSHA256(string texto)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }

        // Método para verificar la validez del CAPTCHA con Google
        private bool ValidarCaptcha()
        {
            string respuestaCaptcha = Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(respuestaCaptcha)) return false;

            string claveSecreta = "6LeRCSAtAAAAAGSmhUPQRdXSgT2wFp-H2xOTwrk5";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"https://www.google.com/recaptcha/api/siteverify?secret={claveSecreta}&response={respuestaCaptcha}");
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        return jsonResponse.Contains("\"success\": true");
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
            {
                lblMensaje.Text = "Por favor, complete todos los campos.";
                lblMensaje.Visible = true;
                return;
            }

            // NUEVA VALIDACIÓN DEL CAPTCHA (BACK-END)
            if (!ValidarCaptcha())
            {
                lblMensaje.Text = "Por favor, verifica que no eres un robot (Captcha inválido).";
                lblMensaje.Visible = true;
                return;
            }

            string hash = HashSHA256(password);
            string conectar = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conectar))
            {
                string query = "SELECT id_usuario, correo, nombre, rol FROM usuario WHERE correo = @correo AND contrasena = @pass";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@pass", hash);
                    try
                    {
                        con.Open();
                        SqlDataReader leer = cmd.ExecuteReader();
                        if (leer.Read())
                        {
                            Session["UsuarioID"] = leer["id_usuario"].ToString();
                            Session["Email"] = leer["correo"].ToString();
                            Session["NombreUsuario"] = leer["nombre"].ToString();
                            string rol = leer["rol"].ToString().ToLower();
                            Session["Rol"] = rol;

                            if (rol == "admin")
                                Response.Redirect("PantallaADMIN.aspx");
                            else
                                Response.Redirect("PantallaUSER.aspx");
                        }
                        else
                        {
                            lblMensaje.Text = "Correo o contraseña incorrectos.";
                            lblMensaje.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMensaje.Text = "Error: " + ex.Message;
                        lblMensaje.Visible = true;
                    }
                }
            }
        }
    }
}