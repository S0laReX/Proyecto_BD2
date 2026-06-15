// CarritoCompleto.aspx.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class CarritoCompleto : System.Web.UI.Page
    {
        string cn = ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null) { Response.Redirect("PantallaLOGIN.aspx"); return; }
            if (!IsPostBack) RenderizarCarrito();
        }

        // Refresca el stock máximo de cada ítem desde la BD antes de renderizar
        private void RenderizarCarrito()
        {
            var carrito = ObtenerCarrito();

            if (carrito.Count == 0)
            {
                pnlCarrito.Visible = false;
                pnlVacio.Visible = true;
                return;
            }

            // Actualizar StockMax desde BD
            foreach (var item in carrito)
                item.StockMax = ConsultarStock(item.IdCelular);

            Session["CarritoItems"] = carrito;

            pnlCarrito.Visible = true;
            pnlVacio.Visible = false;
            repCarrito.DataSource = carrito;
            repCarrito.DataBind();

            decimal total = 0;
            foreach (var item in carrito) total += item.Subtotal;
            litTotal.Text = string.Format("{0:N2}", total);
        }

        // ── Eliminar ítem ─────────────────────────────────────────────────────
        protected void repCarrito_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                var carrito = ObtenerCarrito();
                carrito.RemoveAll(i => i.IdCelular == id);
                Session["CarritoItems"] = carrito;
                RenderizarCarrito();
            }
        }

        // ── Actualizar cantidades ─────────────────────────────────────────────
        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            var carrito = ObtenerCarrito();
            bool hayError = false;

            foreach (RepeaterItem ri in repCarrito.Items)
            {
                if (ri.ItemType != ListItemType.Item && ri.ItemType != ListItemType.AlternatingItem)
                    continue;

                TextBox txtCant = (TextBox)ri.FindControl("txtCant");
                // El CommandArgument del botón Eliminar lleva el id; lo obtenemos por posición
                int id = carrito[ri.ItemIndex].IdCelular;

                int nuevaCant;
                if (!int.TryParse(txtCant.Text, out nuevaCant) || nuevaCant < 1)
                {
                    MostrarMsg($"Cantidad inválida para '{carrito[ri.ItemIndex].Nombre}'.", true);
                    hayError = true; break;
                }

                int stockActual = ConsultarStock(id);
                if (nuevaCant > stockActual)
                {
                    MostrarMsg($"Stock insuficiente para '{carrito[ri.ItemIndex].Nombre}'. Disponible: {stockActual}.", true);
                    hayError = true; break;
                }

                carrito[ri.ItemIndex].Cantidad = nuevaCant;
                carrito[ri.ItemIndex].StockMax = stockActual;
            }

            if (!hayError)
            {
                Session["CarritoItems"] = carrito;
                MostrarMsg("Carrito actualizado.", false);
            }
            RenderizarCarrito();
        }

        // ── Vaciar carrito ────────────────────────────────────────────────────
        protected void btnVaciar_Click(object sender, EventArgs e)
        {
            Session["CarritoItems"] = new List<CarritoItem>();
            RenderizarCarrito();
        }

        // ── Confirmar compra ──────────────────────────────────────────────────
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            var carrito = ObtenerCarrito();
            if (carrito.Count == 0) { MostrarMsg("El carrito está vacío.", true); return; }

            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);

            using (SqlConnection con = new SqlConnection(cn))
            {
                con.Open();

                // Verificar stock de todos los ítems antes de procesar
                foreach (var item in carrito)
                {
                    SqlCommand chk = new SqlCommand("SELECT stock FROM celular WHERE id_celular=@id", con);
                    chk.Parameters.AddWithValue("@id", item.IdCelular);
                    int stockActual = Convert.ToInt32(chk.ExecuteScalar());
                    if (item.Cantidad > stockActual)
                    {
                        MostrarMsg($"Stock insuficiente para '{item.Nombre}'. Disponible: {stockActual} unid.", true);
                        RenderizarCarrito();
                        return;
                    }
                }

                // Calcular total
                decimal total = 0;
                foreach (var item in carrito) total += item.Subtotal;

                // Dirección y proveedor
                string direccion = new SqlCommand(
                    "SELECT ISNULL(direccion,'') FROM usuario WHERE id_usuario=@id", con)
                { Parameters = { new SqlParameter("@id", idUsuario) } }
                    .ExecuteScalar()?.ToString() ?? "";

                object resProv = new SqlCommand(
                    "SELECT TOP 1 id_proveedor FROM proveedor ORDER BY NEWID()", con)
                    .ExecuteScalar();
                int idProveedor = resProv != null ? Convert.ToInt32(resProv) : 1;

                // Insertar venta
                SqlCommand cmdV = new SqlCommand(
                    @"INSERT INTO venta (id_usuario,fecha,total,estado_venta,direccion_envio,id_proveedor_envio)
                      VALUES (@u,GETDATE(),@t,'completado',@dir,@prov); SELECT SCOPE_IDENTITY();", con);
                cmdV.Parameters.AddWithValue("@u", idUsuario);
                cmdV.Parameters.AddWithValue("@t", total);
                cmdV.Parameters.AddWithValue("@dir", direccion);
                cmdV.Parameters.AddWithValue("@prov", idProveedor);
                int idVenta = Convert.ToInt32(cmdV.ExecuteScalar());

                // Insertar detalle, actualizar stock, registrar movimiento
                foreach (var item in carrito)
                {
                    SqlCommand cmdDV = new SqlCommand(
                        @"INSERT INTO detalle_venta (id_venta,id_celular,cantidad,precio_unitario,subtotal)
                          VALUES (@v,@c,@cant,@pu,@sub)", con);
                    cmdDV.Parameters.AddWithValue("@v", idVenta);
                    cmdDV.Parameters.AddWithValue("@c", item.IdCelular);
                    cmdDV.Parameters.AddWithValue("@cant", item.Cantidad);
                    cmdDV.Parameters.AddWithValue("@pu", item.PrecioUnit);
                    cmdDV.Parameters.AddWithValue("@sub", item.Subtotal);
                    cmdDV.ExecuteNonQuery();

                    new SqlCommand(
                        "UPDATE celular SET stock=stock-@cant WHERE id_celular=@c", con)
                    {
                        Parameters = {
                            new SqlParameter("@cant", item.Cantidad),
                            new SqlParameter("@c", item.IdCelular)
                        }
                    }.ExecuteNonQuery();

                    new SqlCommand(
                        @"INSERT INTO movimiento_inventario (id_celular,tipo_movimiento,cantidad,motivo,id_usuario_responsable)
                          VALUES (@c,'salida',@cant,@mot,@u)", con)
                    {
                        Parameters = {
                            new SqlParameter("@c",    item.IdCelular),
                            new SqlParameter("@cant", item.Cantidad),
                            new SqlParameter("@mot",  "Venta #" + idVenta),
                            new SqlParameter("@u",    idUsuario)
                        }
                    }.ExecuteNonQuery();
                }

                // Factura
                string ci = new SqlCommand("SELECT ISNULL(ci,'') FROM usuario WHERE id_usuario=@id", con)
                { Parameters = { new SqlParameter("@id", idUsuario) } }
                    .ExecuteScalar()?.ToString() ?? "";
                string numFact = "FACT-" + DateTime.Now.Year + "-" + idVenta.ToString("D4");
                string nombreCliente = Session["NombreUsuario"]?.ToString() ?? "Cliente";

                SqlCommand cmdFact = new SqlCommand(
                    @"INSERT INTO factura (id_venta,numero_factura,fecha_emision,razon_social,nit_ci_cliente,monto_total,metodo_pago)
                      VALUES (@v,@num,GETDATE(),@rs,@ci,@mt,'efectivo')", con);
                cmdFact.Parameters.AddWithValue("@v", idVenta);
                cmdFact.Parameters.AddWithValue("@num", numFact);
                cmdFact.Parameters.AddWithValue("@rs", nombreCliente);
                cmdFact.Parameters.AddWithValue("@ci", ci);
                cmdFact.Parameters.AddWithValue("@mt", total);
                cmdFact.ExecuteNonQuery();
            }

            // Vaciar carrito de sesión
            Session["CarritoItems"] = new List<CarritoItem>();
            Response.Redirect("PantallaUSER.aspx?compra=ok");
        }

        protected void btnVolverCatalogo_Click(object sender, EventArgs e)
        {
            Response.Redirect("PantallaUSER.aspx");
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private List<CarritoItem> ObtenerCarrito()
            => Session["CarritoItems"] as List<CarritoItem> ?? new List<CarritoItem>();

        private int ConsultarStock(int idCelular)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT stock FROM celular WHERE id_celular=@id",
                new SqlConnection(cn));
            cmd.Parameters.AddWithValue("@id", idCelular);
            cmd.Connection.Open();
            int s = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Connection.Close();
            return s;
        }

        private void MostrarMsg(string texto, bool esError)
        {
            lblMsg.Text = texto;
            lblMsg.CssClass = esError ? "msg-err" : "msg-ok";
            lblMsg.Visible = true;
        }
    }
}