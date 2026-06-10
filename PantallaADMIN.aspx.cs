using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaADMIN : System.Web.UI.Page
    {
        private string conexion = System.Configuration.ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;
        private void CargarDatosCel()
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = @"
                SELECT c.id_celular, c.marca, c.modelo, c.descripcion, c.id_categoria, cat.nombre_categoria, c.precio, c.stock, (SELECT TOP 1 url_imagen FROM celular_imagen WHERE id_celular = c.id_celular) AS url_imagen
                FROM celular c
                LEFT JOIN categoria cat
                ON c.id_categoria = cat.id_categoria";

            SqlDataAdapter da = new SqlDataAdapter(consulta, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        private void CargarDatosCateg()
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = "SELECT id_categoria, nombre_categoria, descripcion, icono FROM categoria";
            SqlDataAdapter da = new SqlDataAdapter(consulta, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            GridView2.DataSource = dt;
            GridView2.DataBind();
        }

        // llena el DropDownList del formulario de crear celular con todas las categorias
        private void CargarCategoriasEnDropDown()
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = "SELECT id_categoria, nombre_categoria FROM categoria";
            SqlDataAdapter da = new SqlDataAdapter(consulta, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            DdlCategoria.DataSource = dt;
            DdlCategoria.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "admin")
            {
                Response.Redirect("PantallaLOGIN.aspx");
            }
            if (!IsPostBack)
            {
                CargarDatosCel();
                CargarDatosCateg();
                CargarCategoriasEnDropDown();
            }
        }

        protected void BtnCrearCel_Click(object sender, EventArgs e)
        {
            int id_categoria = Convert.ToInt32(DdlCategoria.SelectedValue);

            SqlConnection con = new SqlConnection(conexion);
            string consulta = @"INSERT INTO celular (marca, modelo, descripcion, id_categoria, precio, stock)
                                VALUES (@marca, @modelo, @descripcion, @id_categoria, @precio, @stock);
                                SELECT SCOPE_IDENTITY();"; // SCOPE_IDENTITY devuelve el ID del registro mas reciente en CELULAR en un bloque de codigo

            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@marca", TxtMarca.Text);
            cmd.Parameters.AddWithValue("@modelo", TxtModelo.Text);
            cmd.Parameters.AddWithValue("@descripcion", TxtDescripcion.Text);
            cmd.Parameters.AddWithValue("@id_categoria", id_categoria);
            cmd.Parameters.AddWithValue("@precio", TxtPrecio.Text);
            cmd.Parameters.AddWithValue("@stock", TxtCantidadStock.Text);

            con.Open();
            int nuevoIdCelular = Convert.ToInt32(cmd.ExecuteScalar()); // ExecuteScalar obtiene el ID (que devolvio SCOPE_IDENTITY) del celular recien insertado
            con.Close();

            // si se subio una imagen la guarda en la carpeta uploads y registra en celular_imagen
            if (FuImagen.HasFile)
            {
                string nombreArchivo = nuevoIdCelular + "_" + Path.GetFileName(FuImagen.FileName);
                string rutaFisica    = Server.MapPath("~/uploads/" + nombreArchivo);
                FuImagen.SaveAs(rutaFisica);

                string urlImagen = "uploads/" + nombreArchivo;
                GuardarImagen(nuevoIdCelular, urlImagen);
            }

            TxtMarca.Text = "";
            TxtModelo.Text = "";
            TxtDescripcion.Text = "";
            TxtPrecio.Text = "";
            TxtCantidadStock.Text = "";

            CargarDatosCel();
        }

        //insertar una imagen en celular_imagen
        private void GuardarImagen(int id_celular, string urlImagen)
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = "INSERT INTO celular_imagen (id_celular, url_imagen) VALUES (@id_celular, @url_imagen)";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@id_celular",  id_celular);
            cmd.Parameters.AddWithValue("@url_imagen",  urlImagen);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            CargarDatosCel();

            // llenar el DropDownList de categoria en la fila en edicion
            GridViewRow fila = GridView1.Rows[e.NewEditIndex];
            DropDownList ddl = (DropDownList)fila.FindControl("DdlCategoriaEdit");
            if (ddl != null)
            {
                CargarCategoriasEnDdlEdit(ddl);

                // Pre-selecciona la categoria actual del celular
                // id_categoria viene de DataKeys o de la celda — la guardamos en el DataSource
                // Usamos el valor que se cargó en el DataTable (columna id_categoria)
                Label lblCat = (Label)fila.FindControl("LblCategoria");
                // El id_categoria lo obtenemos del DataKey secundario: usamos el texto del label
                // y buscamos el item correspondiente en el DDL
                // Más robusto: guardamos id_categoria como segundo DataKeyName
                // Como solo tenemos id_celular en DataKeyNames, buscamos por nombre:
                if (lblCat != null)
                {
                    ListItem item = ddl.Items.FindByText(lblCat.Text);
                    if (item != null) item.Selected = true;
                }
            }
        }

        // Llena un DropDownList de edicion con las categorias
        private void CargarCategoriasEnDdlEdit(DropDownList ddl)
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = "SELECT id_categoria, nombre_categoria FROM categoria";
            SqlDataAdapter da = new SqlDataAdapter(consulta, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddl.DataSource    = dt;
            ddl.DataTextField  = "nombre_categoria";
            ddl.DataValueField = "id_categoria";
            ddl.DataBind();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id_celular = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            string marca = ((TextBox)GridView1.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
            string modelo = ((TextBox)GridView1.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
            string descripcion = ((TextBox)GridView1.Rows[e.RowIndex].Cells[3].Controls[0]).Text;

            // Celda 4 = TemplateField de categoria => buscar el DropDownList por nombre
            DropDownList ddlEdit = (DropDownList)GridView1.Rows[e.RowIndex].FindControl("DdlCategoriaEdit");
            int id_categoria = Convert.ToInt32(ddlEdit.SelectedValue);
            double precio = Convert.ToDouble(((TextBox)GridView1.Rows[e.RowIndex].Cells[5].Controls[0]).Text);
            int    stock  = Convert.ToInt32(((TextBox)GridView1.Rows[e.RowIndex].Cells[6].Controls[0]).Text);

            SqlConnection con = new SqlConnection(conexion);
            string consulta = @"UPDATE celular SET marca=@marca, modelo=@modelo, descripcion=@descripcion, id_categoria=@id_categoria, precio=@precio, stock=@stock WHERE id_celular=@id_celular";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@id_celular", id_celular);
            cmd.Parameters.AddWithValue("@marca", marca);
            cmd.Parameters.AddWithValue("@modelo", modelo);
            cmd.Parameters.AddWithValue("@descripcion", descripcion);
            cmd.Parameters.AddWithValue("@id_categoria", id_categoria);
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@stock", stock);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            // si se subio una nueva imagen en la edicion la guarda
            FileUpload fuEdit = (FileUpload)GridView1.Rows[e.RowIndex].FindControl("FuImagenEdit");
            if (fuEdit != null && fuEdit.HasFile)
            {
                string nombreArchivo = id_celular + "_" + Path.GetFileName(fuEdit.FileName);
                string rutaFisica    = Server.MapPath("~/uploads/" + nombreArchivo);
                fuEdit.SaveAs(rutaFisica);

                string urlImagen = "uploads/" + nombreArchivo;

                // Actualiza la primera imagen del celular (o inserta si no tenia)
                SqlConnection con2 = new SqlConnection(conexion);
                string sqlImg = @"IF EXISTS (SELECT 1 FROM celular_imagen WHERE id_celular=@id)
                                      UPDATE celular_imagen SET url_imagen=@url
                                      WHERE id_imagen = (SELECT TOP 1 id_imagen FROM celular_imagen WHERE id_celular=@id)
                                  ELSE
                                      INSERT INTO celular_imagen (id_celular, url_imagen) VALUES (@id, @url)";
                SqlCommand cmdImg = new SqlCommand(sqlImg, con2);
                cmdImg.Parameters.AddWithValue("@id", id_celular);
                cmdImg.Parameters.AddWithValue("@url", urlImagen);
                con2.Open();
                cmdImg.ExecuteNonQuery();
                con2.Close();
            }

            GridView1.EditIndex = -1;
            CargarDatosCel();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            CargarDatosCel();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id_celular = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            SqlConnection con = new SqlConnection(conexion);
            string consulta = "DELETE FROM celular WHERE id_celular=@id_celular";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@id_celular", id_celular);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            CargarDatosCel();
        }


        protected void BtnCrearCateg_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(conexion);
            string consulta = @"INSERT INTO categoria (nombre_categoria, descripcion, icono) VALUES (@nombre, @descripcion, @icono)";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@nombre", TxtCatNombre.Text);
            cmd.Parameters.AddWithValue("@descripcion", TxtCatDescripcion.Text);
            cmd.Parameters.AddWithValue("@icono", TxtCatIcono.Text);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            TxtCatNombre.Text = "";
            TxtCatDescripcion.Text = "";
            TxtCatIcono.Text = "";

            CargarDatosCateg();
            CargarCategoriasEnDropDown();
        }

        protected void GridView2_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView2.EditIndex = e.NewEditIndex;
            CargarDatosCateg();
        }

        protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id_categoria = Convert.ToInt32(GridView2.DataKeys[e.RowIndex].Value);

            string nombre = ((TextBox)GridView2.Rows[e.RowIndex].Cells[1].Controls[0]).Text;
            string descripcion = ((TextBox)GridView2.Rows[e.RowIndex].Cells[2].Controls[0]).Text;
            string icono = ((TextBox)GridView2.Rows[e.RowIndex].Cells[3].Controls[0]).Text;

            SqlConnection con = new SqlConnection(conexion);
            string consulta = @"UPDATE categoria SET nombre_categoria=@nombre, descripcion=@descripcion, icono=@icono WHERE id_categoria=@id_categoria";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@id_categoria", id_categoria);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@descripcion", descripcion);
            cmd.Parameters.AddWithValue("@icono", icono);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            GridView2.EditIndex = -1;
            CargarDatosCateg();
            CargarCategoriasEnDropDown();
        }

        protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView2.EditIndex = -1;
            CargarDatosCateg();
        }

        protected void GridView2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id_categoria = Convert.ToInt32(GridView2.DataKeys[e.RowIndex].Value);

            SqlConnection con = new SqlConnection(conexion);
            string consulta = "DELETE FROM categoria WHERE id_categoria=@id_categoria";
            SqlCommand cmd = new SqlCommand(consulta, con);
            cmd.Parameters.AddWithValue("@id_categoria", id_categoria);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            CargarDatosCateg();
            CargarDatosCel();
            CargarCategoriasEnDropDown();
        }
    }
}
