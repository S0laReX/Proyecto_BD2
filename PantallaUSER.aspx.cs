// PantallaUSER.aspx.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaUSER : System.Web.UI.Page
    {
        string conexionString = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

        // ── Helpers para CSS de stock (llamados desde el .aspx) ──────────────
        protected string GetCardCss(object stockObj)
        {
            int s = Convert.ToInt32(stockObj);
            if (s == 0) return "producto-card card-agotado";
            if (s < 3) return "producto-card card-critico";
            return "producto-card";
        }
        protected string GetStockCss(object stockObj)
        {
            int s = Convert.ToInt32(stockObj);
            if (s == 0) return "stock-agotado";
            if (s < 3) return "stock-critico";
            return "";
        }
        protected string GetStockBadge(object stockObj)
        {
            int s = Convert.ToInt32(stockObj);
            if (s == 0) return "<span class='badge-agotado'>AGOTADO</span>";
            if (s < 3) return "<span class='badge-critico'>¡Últimas unidades!</span>";
            return "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null) { Response.Redirect("PantallaLOGIN.aspx"); return; }
            if (!IsPostBack)
            {
                litNombre.Text = Session["NombreUsuario"]?.ToString() ?? "";
                CargarCatalogo();
                CargarFavoritos();
                CargarHistorial();
            }
        }

        private void CargarCatalogo()
        {
            // Mostrar todos los productos (incluyendo agotados para mostrar alertas)
            string query = @"SELECT id_celular, marca, modelo, descripcion, precio, stock,
                     (SELECT TOP 1 url_imagen FROM celular_imagen WHERE celular_imagen.id_celular = celular.id_celular) AS url_imagen
                     FROM celular ORDER BY stock DESC, marca";
            SqlDataAdapter da = new SqlDataAdapter(query, conexionString);
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
                             FROM favorito f INNER JOIN celular c ON f.id_celular = c.id_celular
                             WHERE f.id_usuario = @id";
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(conexionString));
            cmd.Parameters.AddWithValue("@id", idUsuario);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            repFavoritos.DataSource = dt;
            repFavoritos.DataBind();
            if (dt.Rows.Count == 0) lblMensajeFavoritos.Text = "No tienes productos en favoritos aún.";
        }

        private void CargarHistorial()
        {
            if (Session["UsuarioID"] == null) return;
            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            string query = @"SELECT v.id_venta, v.fecha, v.total, v.estado_venta, v.direccion_envio,
                             ISNULL(p.nombre_empresa,'Sin asignar') AS nombre_proveedor,
                             CASE WHEN f.id_factura IS NOT NULL THEN 1 ELSE 0 END AS tiene_factura
                             FROM venta v
                             LEFT JOIN proveedor p ON v.id_proveedor_envio = p.id_proveedor
                             LEFT JOIN factura f ON v.id_venta = f.id_venta
                             WHERE v.id_usuario = @id ORDER BY v.fecha DESC";
            SqlCommand cmd = new SqlCommand(query, new SqlConnection(conexionString));
            cmd.Parameters.AddWithValue("@id", idUsuario);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            repHistorial.DataSource = dt;
            repHistorial.DataBind();
            if (dt.Rows.Count == 0) lblMensajeHistorial.Text = "No tienes compras registradas aún.";
        }

        // ── Catálogo: manejo unificado de comandos ────────────────────────────
        protected void repCelulares_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int idCelular = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Detalle")
            {
                Response.Redirect("DetalleProducto.aspx?id=" + idCelular);
            }
            else if (e.CommandName == "AgregarCarrito")
            {
                // Verificar stock antes de agregar
                int stock = ObtenerStock(idCelular);
                if (stock <= 0)
                {
                    MostrarMsgCatalogo("El producto está agotado.", true);
                    return;
                }

                var carrito = Session["CarritoItems"] as List<CarritoItem> ?? new List<CarritoItem>();
                var item = carrito.Find(i => i.IdCelular == idCelular);

                if (item != null)
                {
                    if (item.Cantidad + 1 > stock)
                    {
                        MostrarMsgCatalogo($"Stock máximo disponible: {stock} unidades.", true);
                        return;
                    }
                    item.Cantidad++;
                    item.StockMax = stock;
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT marca, modelo, precio, stock FROM celular WHERE id_celular=@id",
                        new SqlConnection(conexionString));
                    cmd.Parameters.AddWithValue("@id", idCelular);
                    cmd.Connection.Open();
                    SqlDataReader r = cmd.ExecuteReader();
                    if (r.Read())
                        carrito.Add(new CarritoItem
                        {
                            IdCelular = idCelular,
                            Nombre = r["marca"] + " " + r["modelo"],
                            PrecioUnit = Convert.ToDecimal(r["precio"]),
                            Cantidad = 1,
                            StockMax = Convert.ToInt32(r["stock"])
                        });
                    cmd.Connection.Close();
                }
                Session["CarritoItems"] = carrito;
                MostrarMsgCatalogo("Producto agregado al carrito. <a href='CarritoCompleto.aspx'>Ver carrito →</a>", false);
            }
            else if (e.CommandName == "Favorito")
            {
                if (Session["UsuarioID"] == null) { Response.Redirect("PantallaLOGIN.aspx"); return; }
                int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
                SqlConnection con = new SqlConnection(conexionString);
                SqlCommand cmd = new SqlCommand(
                    @"IF NOT EXISTS (SELECT 1 FROM favorito WHERE id_usuario=@u AND id_celular=@c)
                      INSERT INTO favorito (id_usuario, id_celular) VALUES (@u, @c)", con);
                cmd.Parameters.AddWithValue("@u", idUsuario);
                cmd.Parameters.AddWithValue("@c", idCelular);
                con.Open(); cmd.ExecuteNonQuery(); con.Close();
                CargarFavoritos();
            }
        }

        protected void repFavoritos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EliminarFavorito")
            {
                int idCelular = Convert.ToInt32(e.CommandArgument);
                int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
                SqlConnection con = new SqlConnection(conexionString);
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM favorito WHERE id_usuario=@u AND id_celular=@c", con);
                cmd.Parameters.AddWithValue("@u", idUsuario);
                cmd.Parameters.AddWithValue("@c", idCelular);
                con.Open(); cmd.ExecuteNonQuery(); con.Close();
                CargarFavoritos();
            }
        }

        protected void repHistorial_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DescargarPDF")
            {
                int idVenta = Convert.ToInt32(e.CommandArgument);
                GenerarFacturaPDF(idVenta);
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("PantallaLOGIN.aspx");
        }

        // ── PDF nativo con iTextSharp ─────────────────────────────────────────
        private void GenerarFacturaPDF(int idVenta)
        {
            string qF = @"SELECT f.numero_factura, f.fecha_emision, f.razon_social,
                          f.nit_ci_cliente, f.monto_total, f.metodo_pago,
                          v.estado_venta, v.direccion_envio,
                          ISNULL(p.nombre_empresa,'Sin asignar') AS proveedor_envio
                          FROM factura f INNER JOIN venta v ON f.id_venta=v.id_venta
                          LEFT JOIN proveedor p ON v.id_proveedor_envio=p.id_proveedor
                          WHERE f.id_venta=@id";
            string qD = @"SELECT c.marca, c.modelo, dv.cantidad, dv.precio_unitario, dv.subtotal
                          FROM detalle_venta dv INNER JOIN celular c ON dv.id_celular=c.id_celular
                          WHERE dv.id_venta=@id";

            SqlConnection con = new SqlConnection(conexionString);
            SqlCommand cmdF = new SqlCommand(qF, con);
            cmdF.Parameters.AddWithValue("@id", idVenta);
            SqlCommand cmdD = new SqlCommand(qD, con);
            cmdD.Parameters.AddWithValue("@id", idVenta);
            DataTable dtF = new DataTable(), dtD = new DataTable();
            new SqlDataAdapter(cmdF).Fill(dtF);
            new SqlDataAdapter(cmdD).Fill(dtD);
            if (dtF.Rows.Count == 0) return;

            byte[] pdfBytes = PdfHelper.GenerarFacturaPdf(dtF.Rows[0], dtD);
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition",
                "attachment; filename=Factura_" + dtF.Rows[0]["numero_factura"] + ".pdf");
            Response.BinaryWrite(pdfBytes);
            Response.End();
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private int ObtenerStock(int idCelular)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT stock FROM celular WHERE id_celular=@id",
                new SqlConnection(conexionString));
            cmd.Parameters.AddWithValue("@id", idCelular);
            cmd.Connection.Open();
            int s = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Connection.Close();
            return s;
        }

        private void MostrarMsgCatalogo(string texto, bool esError)
        {
            lblMsgCatalogo.Text = texto;
            lblMsgCatalogo.CssClass = esError ? "msg" : "msg";
            lblMsgCatalogo.ForeColor = esError
                ? System.Drawing.Color.Red
                : System.Drawing.Color.Green;
            lblMsgCatalogo.Visible = true;
        }
    }
}