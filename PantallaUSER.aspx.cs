using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaUSER : System.Web.UI.Page
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
                litNombre.Text = Session["NombreUsuario"] != null ? Session["NombreUsuario"].ToString() : "";
                CargarCatalogo();
                CargarFavoritos();
                CargarHistorial();
            }
        }

        private void CargarCatalogo()
        {
            string query = @"SELECT id_celular, marca, modelo, descripcion, precio, stock,
                     (SELECT TOP 1 url_imagen FROM celular_imagen WHERE celular_imagen.id_celular = celular.id_celular) AS url_imagen
                     FROM celular WHERE stock > 0";
            SqlConnection con = new SqlConnection(conexionString);
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            repCelulares.DataSource = dt;
            repCelulares.DataBind();
        }

        private void CargarFavoritos()
        {
            if (Session["UsuarioID"] == null) return;
            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            string query = @"SELECT c.id_celular, c.marca, c.modelo, c.precio
                             FROM favorito f
                             INNER JOIN celular c ON f.id_celular = c.id_celular
                             WHERE f.id_usuario = @id";
            SqlConnection con = new SqlConnection(conexionString);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", idUsuario);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            repFavoritos.DataSource = dt;
            repFavoritos.DataBind();
            if (dt.Rows.Count == 0)
                lblMensajeFavoritos.Text = "No tienes productos en favoritos aún.";
        }

        private void CargarHistorial()
        {
            if (Session["UsuarioID"] == null) return;
            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            string query = @"SELECT v.id_venta, v.fecha, v.total, v.estado_venta, v.direccion_envio,
                             ISNULL(p.nombre_empresa, 'Sin asignar') AS nombre_proveedor,
                             CASE WHEN f.id_factura IS NOT NULL THEN 1 ELSE 0 END AS tiene_factura
                             FROM venta v
                             LEFT JOIN proveedor p ON v.id_proveedor_envio = p.id_proveedor
                             LEFT JOIN factura f ON v.id_venta = f.id_venta
                             WHERE v.id_usuario = @id
                             ORDER BY v.fecha DESC";
            SqlConnection con = new SqlConnection(conexionString);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", idUsuario);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            repHistorial.DataSource = dt;
            repHistorial.DataBind();
            if (dt.Rows.Count == 0)
                lblMensajeHistorial.Text = "No tienes compras registradas aún.";
        }

        protected void btnFavorito_Click(object sender, EventArgs e)
        {
            if (Session["UsuarioID"] == null) { Response.Redirect("PantallaLOGIN.aspx"); return; }
            Button btn = (Button)sender;
            int idCelular = Convert.ToInt32(btn.CommandArgument);
            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            SqlConnection con = new SqlConnection(conexionString);
            string query = @"IF NOT EXISTS (SELECT 1 FROM favorito WHERE id_usuario=@u AND id_celular=@c)
                             INSERT INTO favorito (id_usuario, id_celular) VALUES (@u, @c)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@u", idUsuario);
            cmd.Parameters.AddWithValue("@c", idCelular);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            CargarFavoritos();
        }

        protected void btnEliminarFavorito_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idCelular = Convert.ToInt32(btn.CommandArgument);
            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            SqlConnection con = new SqlConnection(conexionString);
            string query = "DELETE FROM favorito WHERE id_usuario=@u AND id_celular=@c";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@u", idUsuario);
            cmd.Parameters.AddWithValue("@c", idCelular);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            CargarFavoritos();
        }

        protected void btnVerDetalle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Response.Redirect("DetalleProducto.aspx?id=" + btn.CommandArgument);
        }

        protected void btnComprar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Response.Redirect("Carrito.aspx?id=" + btn.CommandArgument);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("PantallaLOGIN.aspx");
        }

        protected void btnDescargarPDF_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idVenta = Convert.ToInt32(btn.CommandArgument);
            GenerarFacturaPDF(idVenta);
        }

        private void GenerarFacturaPDF(int idVenta)
        {
            string queryFactura = @"SELECT f.numero_factura, f.fecha_emision, f.razon_social,
                                    f.nit_ci_cliente, f.monto_total, f.metodo_pago,
                                    v.estado_venta, v.direccion_envio,
                                    ISNULL(p.nombre_empresa,'Sin asignar') AS proveedor_envio
                                    FROM factura f
                                    INNER JOIN venta v ON f.id_venta = v.id_venta
                                    LEFT JOIN proveedor p ON v.id_proveedor_envio = p.id_proveedor
                                    WHERE f.id_venta = @id";

            string queryDetalle = @"SELECT c.marca, c.modelo, dv.cantidad, dv.precio_unitario, dv.subtotal
                                    FROM detalle_venta dv
                                    INNER JOIN celular c ON dv.id_celular = c.id_celular
                                    WHERE dv.id_venta = @id";

            SqlConnection con = new SqlConnection(conexionString);
            SqlCommand cmdF = new SqlCommand(queryFactura, con);
            cmdF.Parameters.AddWithValue("@id", idVenta);
            SqlDataAdapter daF = new SqlDataAdapter(cmdF);
            DataTable dtF = new DataTable();

            SqlCommand cmdD = new SqlCommand(queryDetalle, con);
            cmdD.Parameters.AddWithValue("@id", idVenta);
            SqlDataAdapter daD = new SqlDataAdapter(cmdD);
            DataTable dtD = new DataTable();

            daF.Fill(dtF);
            daD.Fill(dtD);

            if (dtF.Rows.Count == 0) return;
            DataRow f = dtF.Rows[0];

            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html><html><head><meta charset='utf-8'/>");
            html.Append("<style>body{font-family:Arial,sans-serif;padding:30px;color:#333;}");
            html.Append("h2{color:#007bff;border-bottom:2px solid #007bff;padding-bottom:8px;}");
            html.Append("table{width:100%;border-collapse:collapse;margin-top:15px;}");
            html.Append("th,td{padding:9px 12px;border:1px solid #ddd;text-align:left;}");
            html.Append("th{background:#f8f9fa;} .total{font-size:16px;font-weight:bold;text-align:right;margin-top:10px;}");
            html.Append(".info p{margin:4px 0;font-size:14px;}</style></head><body>");
            html.Append("<h2>FACTURA - Tienda de Celulares</h2>");
            html.AppendFormat("<div class='info'><p><b>N° Factura:</b> {0}</p>", f["numero_factura"]);
            html.AppendFormat("<p><b>Fecha:</b> {0:dd/MM/yyyy HH:mm}</p>", f["fecha_emision"]);
            html.AppendFormat("<p><b>Cliente:</b> {0}</p>", f["razon_social"]);
            html.AppendFormat("<p><b>C.I./NIT:</b> {0}</p>", f["nit_ci_cliente"]);
            html.AppendFormat("<p><b>Método de pago:</b> {0}</p>", f["metodo_pago"]);
            html.AppendFormat("<p><b>Dirección de envío:</b> {0}</p>", f["direccion_envio"]);
            html.AppendFormat("<p><b>Proveedor de envío:</b> {0}</p></div>", f["proveedor_envio"]);

            html.Append("<table><tr><th>Producto</th><th>Cantidad</th><th>Precio Unitario</th><th>Subtotal</th></tr>");
            foreach (DataRow d in dtD.Rows)
            {
                html.AppendFormat("<tr><td>{0} {1}</td><td>{2}</td><td>Bs. {3:N2}</td><td>Bs. {4:N2}</td></tr>",
                    d["marca"], d["modelo"], d["cantidad"], d["precio_unitario"], d["subtotal"]);
            }
            html.Append("</table>");
            html.AppendFormat("<p class='total'>TOTAL: Bs. {0:N2}</p>", f["monto_total"]);
            html.Append("</body></html>");

            Response.Clear();
            Response.ContentType = "text/html";
            Response.AddHeader("Content-Disposition", "attachment; filename=Factura_" + f["numero_factura"] + ".html");
            Response.Write(html.ToString());
            Response.End();
        }
    }
}
