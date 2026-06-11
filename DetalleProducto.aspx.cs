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
                    string id = Request.QueryString["id"];
                    CargarDetalle(id);
                    CargarGaleria(id);
                }
                else
                {
                    lblMensaje.Text = "No se ha seleccionado ningún dispositivo.";
                }
            }
        }

        private void CargarDetalle(string idCelular)
        {
            string query = @"SELECT c.marca, c.modelo, c.descripcion, c.precio, c.stock,
                             c.imei, c.capacidad_almacenamiento, c.memoria_ram,
                             c.ano_fabricacion, c.version_so, c.numero_banda,
                             cat.nombre_categoria, cat.icono
                             FROM celular c
                             LEFT JOIN categoria cat ON c.id_categoria=cat.id_categoria
                             WHERE c.id_celular=@id";

            SqlConnection con = new SqlConnection(conexionString);
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", idCelular);
            con.Open();
            SqlDataReader r = cmd.ExecuteReader();

            if (r.Read())
            {
                pnlDetalle.Visible = true;
                string icono = r["icono"] != DBNull.Value ? r["icono"].ToString() : "📱";
                string cat = r["nombre_categoria"] != DBNull.Value ? r["nombre_categoria"].ToString() : "General";
                litCategoria.Text = icono + " " + cat;
                litMarcaModelo.Text = r["marca"] + " " + r["modelo"];
                litDescripcion.Text = r["descripcion"] != DBNull.Value ? r["descripcion"].ToString() : "Sin descripción.";
                litPrecio.Text = string.Format("{0:N2}", Convert.ToDecimal(r["precio"]));
                litStock.Text = r["stock"].ToString();
                litAlmac.Text = r["capacidad_almacenamiento"] != DBNull.Value ? r["capacidad_almacenamiento"].ToString() : "-";
                litRAM.Text = r["memoria_ram"] != DBNull.Value ? r["memoria_ram"].ToString() : "-";
                litSO.Text = r["version_so"] != DBNull.Value ? r["version_so"].ToString() : "-";
                litBanda.Text = r["numero_banda"] != DBNull.Value ? r["numero_banda"].ToString() : "-";
                litAno.Text = r["ano_fabricacion"] != DBNull.Value ? r["ano_fabricacion"].ToString() : "-";
                litIMEI.Text = r["imei"] != DBNull.Value ? r["imei"].ToString() : "-";
            }
            else
            {
                lblMensaje.Text = "El producto no existe.";
            }
            con.Close();
        }

        private void CargarGaleria(string idCelular)
        {
            SqlConnection con = new SqlConnection(conexionString);
            SqlDataAdapter da = new SqlDataAdapter("SELECT url_imagen FROM celular_imagen WHERE id_celular=@id", con);
            da.SelectCommand.Parameters.AddWithValue("@id", idCelular);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                imgPrincipal.ImageUrl = dt.Rows[0]["url_imagen"].ToString();
                repImagenes.DataSource = dt;
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
                Response.Redirect("Carrito.aspx?id=" + Request.QueryString["id"]);
        }
    }
}
