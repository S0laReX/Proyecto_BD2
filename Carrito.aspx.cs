using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
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
                    CargarItemCarrito(Request.QueryString["id"]);
                }
                else
                {
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Text = "No hay productos seleccionados.";
                    btnIrCatalogo.Visible = true;
                }
            }
        }

        private void CargarItemCarrito(string idCelular)
        {
            SqlConnection con = new SqlConnection(conexionString);
            SqlCommand cmd = new SqlCommand("SELECT marca, modelo, precio, stock FROM celular WHERE id_celular=@id", con);
            cmd.Parameters.AddWithValue("@id", idCelular);
            con.Open();
            SqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                int stock = Convert.ToInt32(r["stock"]);
                if (stock <= 0)
                {
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    lblMensaje.Text = "Este dispositivo está temporalmente agotado.";
                    btnIrCatalogo.Visible = true;
                }
                else
                {
                    pnlCarrito.Visible = true;
                    litProducto.Text = r["marca"] + " " + r["modelo"];
                    decimal precio = Convert.ToDecimal(r["precio"]);
                    litPrecioUnitario.Text = string.Format("{0:N2}", precio);
                    litTotal.Text = string.Format("{0:N2}", precio);
                    txtCantidad.Attributes.Add("max", stock.ToString());
                }
            }
            else
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = "El artículo no existe.";
                btnIrCatalogo.Visible = true;
            }
            con.Close();
        }

        protected void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            int cant;
            if (!int.TryParse(txtCantidad.Text, out cant) || cant < 1)
                txtCantidad.Text = "1";
            decimal precio = Convert.ToDecimal(litPrecioUnitario.Text);
            litTotal.Text = string.Format("{0:N2}", precio * Convert.ToInt32(txtCantidad.Text));
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
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            decimal total = Convert.ToDecimal(litTotal.Text);
            decimal precioUnit = Convert.ToDecimal(litPrecioUnitario.Text);

            SqlConnection con = new SqlConnection(conexionString);
            con.Open();

            SqlCommand cmdStock = new SqlCommand("SELECT stock FROM celular WHERE id_celular=@id", con);
            cmdStock.Parameters.AddWithValue("@id", idCelular);
            int stockActual = Convert.ToInt32(cmdStock.ExecuteScalar());

            if (cantidad > stockActual)
            {
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                lblMensaje.Text = "Stock disponible: " + stockActual + " unidades.";
                con.Close();
                return;
            }

            SqlCommand cmdDir = new SqlCommand("SELECT ISNULL(direccion,'') FROM usuario WHERE id_usuario=@id", con);
            cmdDir.Parameters.AddWithValue("@id", idUsuario);
            string direccion = cmdDir.ExecuteScalar().ToString();

            SqlCommand cmdProv = new SqlCommand("SELECT TOP 1 id_proveedor FROM proveedor ORDER BY NEWID()", con);
            object resProv = cmdProv.ExecuteScalar();
            int idProveedor = resProv != null ? Convert.ToInt32(resProv) : 1;

            SqlCommand cmdV = new SqlCommand(@"INSERT INTO venta (id_usuario, fecha, total, estado_venta, direccion_envio, id_proveedor_envio)
                VALUES (@u, GETDATE(), @t, 'completado', @dir, @prov); SELECT SCOPE_IDENTITY();", con);
            cmdV.Parameters.AddWithValue("@u", idUsuario);
            cmdV.Parameters.AddWithValue("@t", total);
            cmdV.Parameters.AddWithValue("@dir", direccion);
            cmdV.Parameters.AddWithValue("@prov", idProveedor);
            int idVenta = Convert.ToInt32(cmdV.ExecuteScalar());

            SqlCommand cmdDV = new SqlCommand(@"INSERT INTO detalle_venta (id_venta, id_celular, cantidad, precio_unitario, subtotal)
                VALUES (@v, @c, @cant, @pu, @sub)", con);
            cmdDV.Parameters.AddWithValue("@v", idVenta);
            cmdDV.Parameters.AddWithValue("@c", idCelular);
            cmdDV.Parameters.AddWithValue("@cant", cantidad);
            cmdDV.Parameters.AddWithValue("@pu", precioUnit);
            cmdDV.Parameters.AddWithValue("@sub", total);
            cmdDV.ExecuteNonQuery();

            SqlCommand cmdSt = new SqlCommand("UPDATE celular SET stock=stock-@cant WHERE id_celular=@c", con);
            cmdSt.Parameters.AddWithValue("@cant", cantidad);
            cmdSt.Parameters.AddWithValue("@c", idCelular);
            cmdSt.ExecuteNonQuery();

            SqlCommand cmdMov = new SqlCommand(@"INSERT INTO movimiento_inventario (id_celular, tipo_movimiento, cantidad, motivo, id_usuario_responsable)
                VALUES (@c, 'salida', @cant, @motivo, @u)", con);
            cmdMov.Parameters.AddWithValue("@c", idCelular);
            cmdMov.Parameters.AddWithValue("@cant", cantidad);
            cmdMov.Parameters.AddWithValue("@motivo", "Venta #" + idVenta);
            cmdMov.Parameters.AddWithValue("@u", idUsuario);
            cmdMov.ExecuteNonQuery();

            SqlCommand cmdCI = new SqlCommand("SELECT ISNULL(ci,'') FROM usuario WHERE id_usuario=@id", con);
            cmdCI.Parameters.AddWithValue("@id", idUsuario);
            string ci = cmdCI.ExecuteScalar().ToString();

            string numFact = "FACT-" + DateTime.Now.Year + "-" + idVenta.ToString("D4");
            string nombreCliente = Session["NombreUsuario"] != null ? Session["NombreUsuario"].ToString() : "Cliente";

            SqlCommand cmdFact = new SqlCommand(@"INSERT INTO factura (id_venta, numero_factura, fecha_emision, razon_social, nit_ci_cliente, monto_total, metodo_pago)
                VALUES (@v, @num, GETDATE(), @rs, @ci, @mt, 'efectivo')", con);
            cmdFact.Parameters.AddWithValue("@v", idVenta);
            cmdFact.Parameters.AddWithValue("@num", numFact);
            cmdFact.Parameters.AddWithValue("@rs", nombreCliente);
            cmdFact.Parameters.AddWithValue("@ci", ci);
            cmdFact.Parameters.AddWithValue("@mt", total);
            cmdFact.ExecuteNonQuery();

            con.Close();

            pnlCarrito.Visible = false;
            lblMensaje.ForeColor = System.Drawing.Color.Green;
            lblMensaje.Text = "¡Compra procesada! Factura: " + numFact;
            btnIrCatalogo.Visible = true;
        }
    }
}
