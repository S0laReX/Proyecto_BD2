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
        protected void Page_Load(object sender, EventArgs e)
        {
            // Solo cargamos los datos la primera vez, no cuando hacemos clic en un botón (PostBack)
            if (!IsPostBack)
            {
                CargarDatos();
            }
        }

        // 2. Método básico para traer los datos de SQL Server
        private void CargarDatos()
        {
            // Jalamos la conexión de tu web.config
            string conexionString = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

            // Consulta directa a tu tabla celular
            string query = "SELECT id_celular, marca, modelo, descripcion, precio, stock FROM celular";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        DataTable tabla = new DataTable();

                        conexion.Open();
                        adaptador.Fill(tabla); // Llenamos la tabla con los datos

                        // Pegamos los datos al Repeater
                        reCelulares.DataSource = tabla;
                        reCelulares.DataBind();
                        

                        
                    }
                }
            }
        }

        // 3. Evento para el botón "Ver Detalles"
        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            // Capturamos el botón exacto al que le hicieron clic
            Button btn = (Button)sender;

            // Sacamos el ID que guardamos en el CommandArgument
            string id = btn.CommandArgument;

            // Lo mandamos a otra página enviando el ID por la URL
            Response.Redirect("DetalleProducto.aspx?id=" + id);
        }

        // 4. Evento para el botón "Comprar"
        protected void btnComprar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.CommandArgument;

            // En su versión más básica, mandamos al usuario directo a una página de carrito o pago
            Response.Redirect("Carrito.aspx?id=" + id);
        }
    }
}