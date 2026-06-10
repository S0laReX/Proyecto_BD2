using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaUSER : System.Web.UI.Page
    {
        string conexionString = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {   
                CargarCatálogo();
                CargarFavoritos();
                
            }
            if (Session["Rol"] == null)
            {
                Response.Redirect("PantallaLOGIN.aspx");
            }
        }

        private void CargarCatálogo()
        {
            string query = "SELECT id_celular, marca, modelo, descripcion, precio, stock FROM celular";

            // 1. Instanciamos los objetos de forma tradicional
            SqlConnection conexion = new SqlConnection(conexionString);
            SqlCommand comando = new SqlCommand(query, conexion);
            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            DataTable tabla = new DataTable();

            // 2. Abrimos, ejecutamos y CERRAMOS explícitamente
            conexion.Open();
            adaptador.Fill(tabla);
            conexion.Close(); // ¡Muy importante!

            // 3. Asignamos los datos a la interfaz
            repCelulares.DataSource = tabla;
            repCelulares.DataBind();
        }

        private void CargarFavoritos()
        {
            if (Session["UsuarioID"] == null)
            {
                lblMensajeFavoritos.Text = "Inicia sesión para ver tu lista de favoritos.";
                return;
            }

            lblMensajeFavoritos.Text = "";
            string correoUsuario = Session["Email"].ToString();

            // Seleccionamos también c.id_celular para enlazarlo al botón Quitar
            string query = @"SELECT c.id_celular, c.marca, c.modelo, c.precio 
                             FROM favorito f 
                             INNER JOIN celular c ON f.id_celular = c.id_celular
                             INNER JOIN usuario u ON f.id_usuario = u.id_usuario
                             WHERE u.correo = @Correo";

            SqlConnection conexion = new SqlConnection(conexionString);
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Correo", correoUsuario);

            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            DataTable tabla = new DataTable();

            conexion.Open();
            adaptador.Fill(tabla);
            conexion.Close();

            repFavoritos.DataSource = tabla;
            repFavoritos.DataBind();
        }

        private int ObtenerIdUsuario(string correo)
        {
            int idUsuario = 0;
            string query = "SELECT id_usuario FROM usuario WHERE correo = @Correo";

            SqlConnection conexion = new SqlConnection(conexionString);
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@Correo", correo);

            conexion.Open();
            object resultado = comando.ExecuteScalar();
            conexion.Close();

            if (resultado != null && resultado != DBNull.Value)
            {
                idUsuario = Convert.ToInt32(resultado);
            }
            return idUsuario;
        }

        protected void btnFavorito_Click(object sender, EventArgs e)
        {
            

            if (Session["UsuarioID"] == null)
            {
                Response.Redirect("PantallaLOGIN.aspx");
                return;
            }

            Button btn = (Button)sender;
            int idCelular = Convert.ToInt32(btn.CommandArgument);
            string correoUsuario = Session["Email"].ToString();
            int idUsuario = ObtenerIdUsuario(correoUsuario);

            if (idUsuario > 0)
            {
                // CORRECCIÓN: Ahora @IdUsuario se envía directamente desde C# como parámetro completo
                string query = @"IF NOT EXISTS (SELECT 1 FROM favorito WHERE id_usuario = @IdUsuario AND id_celular = @IdCelular)
                                 BEGIN
                                     INSERT INTO favorito (id_usuario, id_celular) VALUES (@IdUsuario, @IdCelular);
                                 END";

                SqlConnection conexion = new SqlConnection(conexionString);
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                comando.Parameters.AddWithValue("@IdCelular", idCelular);

                conexion.Open();
                comando.ExecuteNonQuery();
                conexion.Close();

                CargarFavoritos();
            }
        }

        protected void btnEliminarFavorito_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idCelular = Convert.ToInt32(btn.CommandArgument); // Recibe el id_celular del HTML
            string correoUsuario = Session["Email"].ToString();

            int idUsuario = ObtenerIdUsuario(correoUsuario);

            if (idUsuario > 0)
            {
                // CORRECCIÓN: Borramos buscando por el par de llaves correctas de forma unívoca
                string query = "DELETE FROM favorito WHERE id_usuario = @IdUsuario AND id_celular = @IdCelular";

                SqlConnection conexion = new SqlConnection(conexionString);
                SqlCommand comando = new SqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                comando.Parameters.AddWithValue("@IdCelular", idCelular);

                conexion.Open();
                comando.ExecuteNonQuery();
                conexion.Close();

                CargarFavoritos();
            }
        }

        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.CommandArgument;
            Response.Redirect("DetalleProducto.aspx?id=" + id);
        }

        protected void btnComprar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.CommandArgument;
            Response.Redirect("Carrito.aspx?id=" + id);
        }
    }
}