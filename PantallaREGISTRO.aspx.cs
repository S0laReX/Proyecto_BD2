using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

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

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string ci = txtCI.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string direccion = txtDireccion.Text.Trim();
            string password = txtPassword.Text;
            string password2 = txtPassword2.Text;

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(ci) || string.IsNullOrEmpty(password))
            {
                Msg("Nombre, correo, C.I. y contraseña son obligatorios.", true);
                return;
            }

            if (password != password2)
            {
                Msg("Las contraseñas no coinciden.", true);
                return;
            }

            if (password.Length < 8)
            {
                Msg("La contraseña debe tener al menos 8 caracteres.", true);
                return;
            }

            bool tieneMayuscula = false;
            bool tieneNumero = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                if (char.IsDigit(c)) tieneNumero = true;
            }

            if (!tieneMayuscula || !tieneNumero)
            {
                Msg("La contraseña debe tener al menos 1 mayúscula y 1 número.", true);
                return;
            }

            string hash = HashSHA256(password);
            string conectar = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(conectar))
            {
                con.Open();
                string verificar = "SELECT COUNT(*) FROM usuario WHERE correo = @correo";
                using (SqlCommand cmd = new SqlCommand(verificar, con))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);
                    int existe = (int)cmd.ExecuteScalar();
                    if (existe > 0)
                    {
                        Msg("Este correo ya está registrado.", true);
                        return;
                    }
                }

                string insert = "INSERT INTO usuario (nombre, correo, contrasena, ci, telefono, direccion, rol) VALUES (@nombre, @correo, @pass, @ci, @tel, @dir, 'cliente')";
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
