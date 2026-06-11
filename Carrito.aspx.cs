using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Proyecto_BDII
{
    public partial class Carrito : System.Web.UI.Page
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
                    txtCantidad.Text = "1";
                    string idCelular = Request.QueryString["id"];
                    CargarItemCarrito(idCelular);
                }
                else
                {
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Text = "No hay productos seleccionados en el carrito.";

                    
                    btnIrCatalogo.Visible = true;
                }
            }
        }

        private void CargarItemCarrito(string idCelular)
        {
            string query = "SELECT marca, modelo, precio, stock FROM celular WHERE id_celular = @IdCelular";

            SqlConnection conexion = new SqlConnection(conexionString);
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.Parameters.AddWithValue("@IdCelular", idCelular);

            conexion.Open();
            SqlDataReader reader = comando.ExecuteReader();

            if (reader.Read())
            {
                int stockDisponible = Convert.ToInt32(reader["stock"]);

                if (stockDisponible <= 0)
                {
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Text = "Lo sentimos, este dispositivo se encuentra temporalmente agotado.";
                    pnlCarrito.Visible = false;

                    
                    btnIrCatalogo.Visible = true;
                }
                else
                {
                    pnlCarrito.Visible = true;
                    litProducto.Text = reader["marca"].ToString() + " " + reader["modelo"].ToString();

                    decimal precio = Convert.ToDecimal(reader["precio"]);
                    litPrecioUnitario.Text = string.Format("{0:N2}", precio);
                    litTotal.Text = string.Format("{0:N2}", precio);

                    txtCantidad.Attributes.Add("max", stockDisponible.ToString());
                }
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = "El artículo solicitado no existe.";
                btnIrCatalogo.Visible = true;
            }

            conexion.Close();
        }

        protected void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCantidad.Text) || Convert.ToInt32(txtCantidad.Text) < 1)
            {
                txtCantidad.Text = "1";
            }

            decimal precioUnitario = Convert.ToDecimal(litPrecioUnitario.Text);
            int cantidad = Convert.ToInt32(txtCantidad.Text);

            decimal totalCalculado = precioUnitario * cantidad;
            litTotal.Text = string.Format("{0:N2}", totalCalculado);
        }

        
        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("PantallaUSER.aspx");
        }

        
        protected void btnIrCatalogo_Click(object sender, EventArgs e)
        {
            Response.Redirect("PantallaUSER.aspx");
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            int idCelular = Convert.ToInt32(Request.QueryString["id"]);
            int cantidadSolicitada = Convert.ToInt32(txtCantidad.Text);
            decimal totalVenta = Convert.ToDecimal(litTotal.Text);
            decimal precioUnitario = Convert.ToDecimal(litPrecioUnitario.Text);

            SqlConnection conexion = new SqlConnection(conexionString);
            conexion.Open();

            string sqlCheckStock = "SELECT stock FROM celular WHERE id_celular = @IdCelular";
            SqlCommand cmdCheck = new SqlCommand(sqlCheckStock, conexion);
            cmdCheck.Parameters.AddWithValue("@IdCelular", idCelular);
            int stockActual = Convert.ToInt32(cmdCheck.ExecuteScalar());

            if (cantidadSolicitada > stockActual)
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = "Error: No puedes adquirir esa cantidad. El stock disponible real es de " + stockActual + " unidades.";
                conexion.Close();
                return;
            }

            string sqlVenta = @"INSERT INTO venta (id_usuario, fecha, total, estado_venta) 
                                VALUES (@IdUsuario, GETDATE(), @Total, 'completado');
                                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdVenta = new SqlCommand(sqlVenta, conexion);
            cmdVenta.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmdVenta.Parameters.AddWithValue("@Total", totalVenta);

            int idVentaGenerada = Convert.ToInt32(cmdVenta.ExecuteScalar());

            string sqlDetalle = @"INSERT INTO detalle_venta (id_venta, id_celular, cantidad, precio_unitario, subtotal) 
                                  VALUES (@IdVenta, @IdCelular, @Cantidad, @PrecioUnitario, @Subtotal)";

            SqlCommand cmdDetalle = new SqlCommand(sqlDetalle, conexion);
            cmdDetalle.Parameters.AddWithValue("@IdVenta", idVentaGenerada);
            cmdDetalle.Parameters.AddWithValue("@IdCelular", idCelular);
            cmdDetalle.Parameters.AddWithValue("@Cantidad", cantidadSolicitada);
            cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", precioUnitario);
            cmdDetalle.Parameters.AddWithValue("@Subtotal", totalVenta);

            cmdDetalle.ExecuteNonQuery();

            string sqlUpdateStock = "UPDATE celular SET stock = stock - @Cantidad WHERE id_celular = @IdCelular";
            SqlCommand cmdUpdate = new SqlCommand(sqlUpdateStock, conexion);
            cmdUpdate.Parameters.AddWithValue("@Cantidad", cantidadSolicitada);
            cmdUpdate.Parameters.AddWithValue("@IdCelular", idCelular);

            cmdUpdate.ExecuteNonQuery();

            conexion.Close();

            
            pnlCarrito.Visible = false;
            lblMensaje.ForeColor = System.Drawing.Color.Green;
            lblMensaje.Text = "¡Compra procesada con éxito! Tu orden ha sido registrada.";

            
            btnIrCatalogo.Visible = true;
        }
    }
}