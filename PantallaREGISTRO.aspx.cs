using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Net; // Requerido para la petición al servidor de Google
using System.IO;  // Requerido para leer la respuesta de Google

namespace Proyecto_BDII
{
    public partial class PantallaREGISTRO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                lblMensaje.Visible = false;
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

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string ci = txtCI.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string direccion = txtDireccion.Text.Trim();
            string password = txtPassword.Text;
            string password2 = txtPassword2.Text;

            // Validaciones de back-end
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) ||
                string.IsNullOrEmpty(ci) || string.IsNullOrEmpty(password))
            { Msg("Nombre, correo, C.I. y contraseña son obligatorios.", true); return; }

            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { Msg("El formato del correo electrónico no es válido.", true); return; }

            if (!Regex.IsMatch(ci, @"^\d{5,12}$"))
            { Msg("El C.I. debe contener solo números (5-12 dígitos).", true); return; }

            if (!string.IsNullOrEmpty(telefono) && !Regex.IsMatch(telefono, @"^[\+\d\s\-\(\)]{7,15}$"))
            { Msg("El formato del teléfono no es válido.", true); return; }

            if (password != password2)
            { Msg("Las contraseñas no coinciden.", true); return; }

            if (password.Length < 8)
            { Msg("La contraseña debe tener al menos 8 caracteres.", true); return; }

            bool tieneMayuscula = false, tieneNumero = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                if (char.IsDigit(c)) tieneNumero = true;
            }
            if (!tieneMayuscula || !tieneNumero)
            { Msg("La contraseña debe tener al menos 1 mayúscula y 1 número.", true); return; }

            // NUEVA VALIDACIÓN DEL CAPTCHA (BACK-END)
            if (!ValidarCaptcha())
            {
                Msg("Por favor, verifica que no eres un robot (Captcha inválido).", true);
                return;
            }

            string hash = HashSHA256(password);
            string conectar = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conectar))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM usuario WHERE correo = @correo", con))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    if ((int)cmd.ExecuteScalar() > 0) { Msg("Este correo ya está registrado.", true); return; }
                }

                string insert = @"INSERT INTO usuario (nombre, correo, contrasena, ci, telefono, direccion, rol)
                                  VALUES (@nombre, @correo, @pass, @ci, @tel, @dir, 'cliente')";
                using (SqlCommand cmd = new SqlCommand(insert, con))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@pass", hash);
                    cmd.Parameters.AddWithValue("@ci", ci);
                    cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(telefono) ? (object)DBNull.Value : telefono);
                    cmd.Parameters.AddWithValue("@dir", string.IsNullOrEmpty(direccion) ? (object)DBNull.Value : direccion);
                    cmd.ExecuteNonQuery();
                }
            }
            Msg("¡Registro exitoso! Redirigiendo...", false);
            Response.AppendHeader("Refresh", "2;url=PantallaLOGIN.aspx");
        }

        private void Msg(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.Visible = true;
            lblMensaje.CssClass = esError ? "msg-error" : "msg-ok";
        }
    }
}