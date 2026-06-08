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
            if (!IsPostBack)
            {
                CargarCatálogo();
            }
        }

        private void CargarCatálogo()
        {
            string conexionString = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;
            string query = "SELECT id_celular, marca, modelo, descripcion, precio, stock FROM celular WHERE stock > 0";

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        DataTable dtCelulares = new DataTable();
                        try
                        {
                            conexion.Open();
                            adaptador.Fill(dtCelulares);
                            repCelulares.DataSource = dtCelulares;
                            repCelulares.DataBind();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
            }
        }

        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string idCelular = btn.CommandArgument;
            Response.Redirect($"DetalleProducto.aspx?id={idCelular}");
        }

        // NUEVO MÉTODO PARA EL BOTÓN DE COMPRAR
        protected void btnComprar_Click(object sender, EventArgs e)
        {
            // 1. Control de Seguridad: Si el usuario no se ha logueado, lo mandamos a identificarse
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // 2. Extraer el ID del celular seleccionado
            LinkButton btn = (LinkButton)sender;
            int idCelular = Convert.ToInt32(btn.CommandArgument);

            // 3. Inicializar la estructura del Carrito de Compras en la Sesión si no existe
            if (Session["Carrito"] == null)
            {
                Session["Carrito"] = new List<int>();
            }

            // 4. Agregar el ID del celular a la lista del carrito
            List<int> carrito = (List<int>)Session["Carrito"];
            carrito.Add(idCelular);
            Session["Carrito"] = carrito;

            // 5. Redireccionar a la página de procesamiento de la orden o pasarela de pago
            Response.Redirect("~/Carrito.aspx");
        }
    }
}