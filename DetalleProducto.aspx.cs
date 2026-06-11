using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Proyecto_BDII
{
    public partial class DetalleProducto : System.Web.UI.Page
    {
        string conexionString = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["Rol"] == null)
            {
                Response.Redirect("PantallaLOGIN.aspx");
                return;
            }

            if (!IsPostBack)
            {
                
                if (Request.QueryString["id"] != null)
                {
                    string idCelular = Request.QueryString["id"];
                    CargarDetalleCelular(idCelular);
                    CargarGaleriaImagenes(idCelular);
                }
                else
                {
                    lblMensaje.Text = "No se ha seleccionado ningún dispositivo del catálogo.";
                }
            }
        }

        private void CargarDetalleCelular(string idCelular)
        {
            
            string query = @"SELECT c.marca, c.modelo, c.descripcion, c.precio, c.stock, cat.nombre_categoria, cat.icono 
                             FROM celular c
                             LEFT JOIN categoria cat ON c.id_categoria = cat.id_categoria
                             WHERE c.id_celular = @IdCelular";

            SqlConnection conexion = new SqlConnection(conexionString);
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@IdCelular", idCelular);

            conexion.Open();
            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read())
            {
                pnlDetalle.Visible = true; 

                
                string icono = reader["icono"] != DBNull.Value ? reader["icono"].ToString() : "📱";
                string catNombre = reader["nombre_categoria"] != DBNull.Value ? reader["nombre_categoria"].ToString() : "General";

                litCategoria.Text = icono + " " + catNombre;
                litMarcaModelo.Text = reader["marca"].ToString() + " " + reader["modelo"].ToString();
                litDescripcion.Text = reader["descripcion"] != DBNull.Value ? reader["descripcion"].ToString() : "Sin descripción disponible.";
                litPrecio.Text = string.Format("{0:N2}", Convert.ToDecimal(reader["precio"]));
                litStock.Text = reader["stock"].ToString();
            }
            else
            {
                lblMensaje.Text = "El producto solicitado no existe en nuestro sistema.";
            }

            conexion.Close(); 
        }

        private void CargarGaleriaImagenes(string idCelular)
        {
            string query = "SELECT url_imagen FROM celular_imagen WHERE id_celular = @IdCelular";

            SqlConnection conexion = new SqlConnection(conexionString);
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@IdCelular", idCelular);

            SqlDataAdapter adaptador = new SqlDataAdapter(comando);
            DataTable tablaImg = new DataTable();

            conexion.Open();
            adaptador.Fill(tablaImg);
            conexion.Close(); 

            if (tablaImg.Rows.Count > 0)
            {
                
                imgPrincipal.ImageUrl = tablaImg.Rows[0]["url_imagen"].ToString();

                
                repImagenes.DataSource = tablaImg;
                repImagenes.DataBind();
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("PantallaUSER.aspx");
        }

        protected void btnComprar_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                string id = Request.QueryString["id"];
                Response.Redirect("Carrito.aspx?id=" + id);
            }
        }
    }
}