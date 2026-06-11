using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Proyecto_BDII
{
    public partial class PantallaADMIN : System.Web.UI.Page
    {
        private string conexion = System.Configuration.ConfigurationManager.ConnectionStrings["Mi Conexion"].ConnectionString;

        private string OrdenVentas
        {
            get { return ViewState["OrdenVentas"] == null ? "DESC" : ViewState["OrdenVentas"].ToString(); }
            set { ViewState["OrdenVentas"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "admin")
                Response.Redirect("PantallaLOGIN.aspx");

            if (!IsPostBack)
            {
                CargarDatosCel();
                CargarDatosCateg();
                CargarDropdowns();
                CargarDatosProv();
                CargarMovimientos();
                CargarHistorialVentas(orden: OrdenVentas);
            }
        }

        private void CargarDropdowns()
        {
            SqlDataAdapter daC = new SqlDataAdapter("SELECT id_categoria, nombre_categoria FROM categoria", conexion);
            DataTable dtC = new DataTable();
            daC.Fill(dtC);
            DdlCategoria.DataSource = dtC;
            DdlCategoria.DataBind();

            SqlDataAdapter daP = new SqlDataAdapter("SELECT id_proveedor, nombre_empresa FROM proveedor", conexion);
            DataTable dtP = new DataTable();
            daP.Fill(dtP);
            DdlProveedor.DataSource = dtP;
            DdlProveedor.DataBind();

            SqlDataAdapter daM = new SqlDataAdapter("SELECT id_celular, marca + ' ' + modelo AS nombre_celular FROM celular", conexion);
            DataTable dtM = new DataTable();
            daM.Fill(dtM);
            DdlCelularMov.DataSource = dtM;
            DdlCelularMov.DataTextField = "nombre_celular";
            DdlCelularMov.DataValueField = "id_celular";
            DdlCelularMov.DataBind();
        }

        private void CargarDatosCel()
        {
            string q = @"SELECT c.id_celular, c.marca, c.modelo, c.imei, c.capacidad_almacenamiento,
                         c.memoria_ram, c.ano_fabricacion, c.version_so, c.numero_banda,
                         c.descripcion, c.id_categoria, cat.nombre_categoria,
                         c.id_proveedor, p.nombre_empresa, c.precio, c.stock,
                         (SELECT TOP 1 url_imagen FROM celular_imagen WHERE id_celular=c.id_celular) AS url_imagen
                         FROM celular c
                         LEFT JOIN categoria cat ON c.id_categoria=cat.id_categoria
                         LEFT JOIN proveedor p ON c.id_proveedor=p.id_proveedor";
            SqlDataAdapter da = new SqlDataAdapter(q, conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }

        private void CargarDatosCateg()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT id_categoria, nombre_categoria, descripcion, icono FROM categoria", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }

        private void CargarDatosProv()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT id_proveedor, nombre_empresa, nit_rut, contacto_nombre, telefono, correo, direccion FROM proveedor", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            GridView4.DataSource = dt;
            GridView4.DataBind();
        }

        private void CargarMovimientos()
        {
            string q = @"SELECT m.id_movimiento, c.marca + ' ' + c.modelo AS nombre_celular,
                         m.tipo_movimiento, m.cantidad, m.motivo, m.fecha_movimiento,
                         ISNULL(u.nombre, 'Sistema') AS responsable
                         FROM movimiento_inventario m
                         INNER JOIN celular c ON m.id_celular=c.id_celular
                         LEFT JOIN usuario u ON m.id_usuario_responsable=u.id_usuario
                         ORDER BY m.fecha_movimiento DESC";
            SqlDataAdapter da = new SqlDataAdapter(q, conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            GridView5.DataSource = dt;
            GridView5.DataBind();
        }

        private void CargarHistorialVentas(string buscar = "", string desde = "", string hasta = "", string orden = "DESC")
        {
            string q = @"SELECT v.id_venta, u.nombre, u.correo, v.fecha, v.total, v.estado_venta,
                         v.direccion_envio, ISNULL(p.nombre_empresa,'Sin asignar') AS nombre_proveedor,
                         v.id_proveedor_envio,
                         CASE WHEN f.id_factura IS NOT NULL THEN 1 ELSE 0 END AS tiene_factura
                         FROM venta v
                         INNER JOIN usuario u ON v.id_usuario=u.id_usuario
                         LEFT JOIN proveedor p ON v.id_proveedor_envio=p.id_proveedor
                         LEFT JOIN factura f ON v.id_venta=f.id_venta
                         WHERE 1=1";
            if (!string.IsNullOrEmpty(buscar)) q += " AND u.nombre LIKE @nombre";
            if (!string.IsNullOrEmpty(desde)) q += " AND v.fecha >= @desde";
            if (!string.IsNullOrEmpty(hasta)) q += " AND v.fecha < DATEADD(day,1,@hasta)";
            q += " ORDER BY v.fecha " + (orden == "ASC" ? "ASC" : "DESC");

            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand(q, con);
            if (!string.IsNullOrEmpty(buscar)) cmd.Parameters.AddWithValue("@nombre", "%" + buscar + "%");
            if (!string.IsNullOrEmpty(desde)) cmd.Parameters.AddWithValue("@desde", Convert.ToDateTime(desde));
            if (!string.IsNullOrEmpty(hasta)) cmd.Parameters.AddWithValue("@hasta", Convert.ToDateTime(hasta));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            GridView3.DataSource = dt;
            GridView3.DataBind();
        }

        protected void BtnCrearCel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMarca.Text) || string.IsNullOrEmpty(TxtModelo.Text) || string.IsNullOrEmpty(TxtPrecio.Text) || string.IsNullOrEmpty(TxtStock.Text))
            {
                MostrarMsg("Marca, modelo, precio y stock son obligatorios.", true); return;
            }
            decimal precio;
            int stock;
            if (!decimal.TryParse(TxtPrecio.Text, out precio) || !int.TryParse(TxtStock.Text, out stock) || stock < 0 || precio <= 0)
            {
                MostrarMsg("Precio y stock deben ser valores numéricos válidos.", true); return;
            }
            int? ano = null;
            if (!string.IsNullOrEmpty(TxtAno.Text))
            {
                int a;
                if (!int.TryParse(TxtAno.Text, out a) || a < 2000)
                { MostrarMsg("Año de fabricación inválido.", true); return; }
                ano = a;
            }

            SqlConnection con = new SqlConnection(conexion);
            string q = @"INSERT INTO celular (marca, modelo, imei, capacidad_almacenamiento, memoria_ram, ano_fabricacion,
                          version_so, numero_banda, descripcion, id_categoria, id_proveedor, precio, stock)
                          VALUES (@marca,@modelo,@imei,@alm,@ram,@ano,@so,@banda,@desc,@cat,@prov,@precio,@stock);
                          SELECT SCOPE_IDENTITY();";
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@marca", TxtMarca.Text.Trim());
            cmd.Parameters.AddWithValue("@modelo", TxtModelo.Text.Trim());
            cmd.Parameters.AddWithValue("@imei", string.IsNullOrEmpty(TxtIMEI.Text) ? (object)DBNull.Value : TxtIMEI.Text.Trim());
            cmd.Parameters.AddWithValue("@alm", string.IsNullOrEmpty(TxtAlmac.Text) ? (object)DBNull.Value : TxtAlmac.Text.Trim());
            cmd.Parameters.AddWithValue("@ram", string.IsNullOrEmpty(TxtRAM.Text) ? (object)DBNull.Value : TxtRAM.Text.Trim());
            cmd.Parameters.AddWithValue("@ano", ano.HasValue ? (object)ano.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@so", string.IsNullOrEmpty(TxtSO.Text) ? (object)DBNull.Value : TxtSO.Text.Trim());
            cmd.Parameters.AddWithValue("@banda", string.IsNullOrEmpty(TxtBanda.Text) ? (object)DBNull.Value : TxtBanda.Text.Trim());
            cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(TxtDescripcion.Text) ? (object)DBNull.Value : TxtDescripcion.Text.Trim());
            cmd.Parameters.AddWithValue("@cat", Convert.ToInt32(DdlCategoria.SelectedValue));
            cmd.Parameters.AddWithValue("@prov", Convert.ToInt32(DdlProveedor.SelectedValue));
            cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@stock", stock);
            con.Open();
            int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());

            if (stock > 0)
            {
                int idAdmin = Convert.ToInt32(Session["UsuarioID"]);
                SqlCommand cmdMov = new SqlCommand("INSERT INTO movimiento_inventario (id_celular, tipo_movimiento, cantidad, motivo, id_usuario_responsable) VALUES (@c,'entrada',@cant,'Stock inicial',@u)", con);
                cmdMov.Parameters.AddWithValue("@c", nuevoId);
                cmdMov.Parameters.AddWithValue("@cant", stock);
                cmdMov.Parameters.AddWithValue("@u", idAdmin);
                cmdMov.ExecuteNonQuery();
            }
            con.Close();

            if (FuImagen.HasFile)
            {
                string nombre = nuevoId + "_" + Path.GetFileName(FuImagen.FileName);
                FuImagen.SaveAs(Server.MapPath("~/uploads/" + nombre));
                GuardarImagen(nuevoId, "uploads/" + nombre);
            }

            TxtMarca.Text = TxtModelo.Text = TxtIMEI.Text = TxtAlmac.Text = TxtRAM.Text = "";
            TxtAno.Text = TxtSO.Text = TxtBanda.Text = TxtDescripcion.Text = TxtPrecio.Text = TxtStock.Text = "";
            MostrarMsg("Celular creado exitosamente.", false);
            CargarDatosCel(); CargarDropdowns(); CargarMovimientos();
        }

        private void GuardarImagen(int id, string url)
        {
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("INSERT INTO celular_imagen (id_celular, url_imagen) VALUES (@id,@url)", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@url", url);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            CargarDatosCel();
            GridViewRow fila = GridView1.Rows[e.NewEditIndex];
            DropDownList ddlC = (DropDownList)fila.FindControl("DdlCategoriaEdit");
            if (ddlC != null)
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT id_categoria, nombre_categoria FROM categoria", conexion);
                DataTable dt = new DataTable(); da.Fill(dt);
                ddlC.DataSource = dt; ddlC.DataTextField = "nombre_categoria"; ddlC.DataValueField = "id_categoria"; ddlC.DataBind();
                Label lbl = (Label)fila.FindControl("LblCategoria");
                if (lbl != null) { ListItem it = ddlC.Items.FindByText(lbl.Text); if (it != null) it.Selected = true; }
            }
            DropDownList ddlP = (DropDownList)fila.FindControl("DdlProveedorEdit");
            if (ddlP != null)
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT id_proveedor, nombre_empresa FROM proveedor", conexion);
                DataTable dt = new DataTable(); da.Fill(dt);
                ddlP.DataSource = dt; ddlP.DataTextField = "nombre_empresa"; ddlP.DataValueField = "id_proveedor"; ddlP.DataBind();
                Label lbl = (Label)fila.FindControl("LblProveedor");
                if (lbl != null) { ListItem it = ddlP.Items.FindByText(lbl.Text); if (it != null) it.Selected = true; }
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            GridViewRow r = GridView1.Rows[e.RowIndex];
            string marca = ((TextBox)r.Cells[1].Controls[0]).Text;
            string modelo = ((TextBox)r.Cells[2].Controls[0]).Text;
            string imei = ((TextBox)r.Cells[3].Controls[0]).Text;
            string alm = ((TextBox)r.Cells[4].Controls[0]).Text;
            string ram = ((TextBox)r.Cells[5].Controls[0]).Text;
            string anoTxt = ((TextBox)r.Cells[6].Controls[0]).Text;
            string so = ((TextBox)r.Cells[7].Controls[0]).Text;
            string banda = ((TextBox)r.Cells[8].Controls[0]).Text;
            string desc = ((TextBox)r.Cells[9].Controls[0]).Text;
            int idCat = Convert.ToInt32(((DropDownList)r.FindControl("DdlCategoriaEdit")).SelectedValue);
            int idProv = Convert.ToInt32(((DropDownList)r.FindControl("DdlProveedorEdit")).SelectedValue);
            decimal precio = Convert.ToDecimal(((TextBox)r.Cells[12].Controls[0]).Text);
            int stock = Convert.ToInt32(((TextBox)r.Cells[13].Controls[0]).Text);

            int? ano = null;
            int anoVal; if (int.TryParse(anoTxt, out anoVal)) ano = anoVal;

            SqlConnection con = new SqlConnection(conexion);
            string q = @"UPDATE celular SET marca=@m, modelo=@mo, imei=@imei, capacidad_almacenamiento=@alm,
                         memoria_ram=@ram, ano_fabricacion=@ano, version_so=@so, numero_banda=@banda,
                         descripcion=@desc, id_categoria=@cat, id_proveedor=@prov, precio=@precio, stock=@stock
                         WHERE id_celular=@id";
            SqlCommand cmd = new SqlCommand(q, con);
            cmd.Parameters.AddWithValue("@m", marca); cmd.Parameters.AddWithValue("@mo", modelo);
            cmd.Parameters.AddWithValue("@imei", string.IsNullOrEmpty(imei) ? (object)DBNull.Value : imei);
            cmd.Parameters.AddWithValue("@alm", alm); cmd.Parameters.AddWithValue("@ram", ram);
            cmd.Parameters.AddWithValue("@ano", ano.HasValue ? (object)ano.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@so", so); cmd.Parameters.AddWithValue("@banda", banda);
            cmd.Parameters.AddWithValue("@desc", desc); cmd.Parameters.AddWithValue("@cat", idCat);
            cmd.Parameters.AddWithValue("@prov", idProv); cmd.Parameters.AddWithValue("@precio", precio);
            cmd.Parameters.AddWithValue("@stock", stock); cmd.Parameters.AddWithValue("@id", id);
            con.Open(); cmd.ExecuteNonQuery();

            FileUpload fu = (FileUpload)r.FindControl("FuImagenEdit");
            if (fu != null && fu.HasFile)
            {
                string nombre = id + "_" + Path.GetFileName(fu.FileName);
                fu.SaveAs(Server.MapPath("~/uploads/" + nombre));
                string url = "uploads/" + nombre;
                SqlCommand ci = new SqlCommand(@"IF EXISTS (SELECT 1 FROM celular_imagen WHERE id_celular=@id)
                    UPDATE celular_imagen SET url_imagen=@url WHERE id_imagen=(SELECT TOP 1 id_imagen FROM celular_imagen WHERE id_celular=@id)
                    ELSE INSERT INTO celular_imagen (id_celular, url_imagen) VALUES (@id,@url)", con);
                ci.Parameters.AddWithValue("@id", id); ci.Parameters.AddWithValue("@url", url);
                ci.ExecuteNonQuery();
            }
            con.Close();
            GridView1.EditIndex = -1;
            CargarDatosCel();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) 
        {
            GridView1.EditIndex = -1; CargarDatosCel(); 
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("DELETE FROM celular WHERE id_celular=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            CargarDatosCel();
        }

        protected void BtnCrearCateg_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtCatNombre.Text)) { MostrarMsg("El nombre de categoría es obligatorio.", true); return; }
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("INSERT INTO categoria (nombre_categoria, descripcion, icono) VALUES (@n,@d,@i)", con);
            cmd.Parameters.AddWithValue("@n", TxtCatNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@d", TxtCatDescripcion.Text.Trim());
            cmd.Parameters.AddWithValue("@i", string.IsNullOrEmpty(TxtCatIcono.Text) ? "📱" : TxtCatIcono.Text.Trim());
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            TxtCatNombre.Text = TxtCatDescripcion.Text = TxtCatIcono.Text = "";
            MostrarMsg("Categoría creada.", false);
            CargarDatosCateg(); CargarDropdowns();
        }

        protected void GridView2_RowEditing(object sender, GridViewEditEventArgs e)
        { 
            GridView2.EditIndex = e.NewEditIndex; CargarDatosCateg(); 
        }
        protected void GridView2_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) 
        { 
            GridView2.EditIndex = -1; CargarDatosCateg(); 
        }

        protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(GridView2.DataKeys[e.RowIndex].Value);
            GridViewRow r = GridView2.Rows[e.RowIndex];
            string nombre = ((TextBox)r.Cells[1].Controls[0]).Text;
            string desc = ((TextBox)r.Cells[2].Controls[0]).Text;
            string icono = ((TextBox)r.Cells[3].Controls[0]).Text;
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("UPDATE categoria SET nombre_categoria=@n,descripcion=@d,icono=@i WHERE id_categoria=@id", con);
            cmd.Parameters.AddWithValue("@n", nombre); cmd.Parameters.AddWithValue("@d", desc);
            cmd.Parameters.AddWithValue("@i", icono); cmd.Parameters.AddWithValue("@id", id);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            GridView2.EditIndex = -1; CargarDatosCateg(); CargarDropdowns();
        }

        protected void GridView2_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView2.DataKeys[e.RowIndex].Value);
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("DELETE FROM categoria WHERE id_categoria=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            CargarDatosCateg(); CargarDatosCel(); CargarDropdowns();
        }

        protected void BtnCrearProv_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtProvEmpresa.Text) || string.IsNullOrEmpty(TxtProvNIT.Text))
            { MostrarMsg("Empresa y NIT son obligatorios.", true); return; }
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("INSERT INTO proveedor (nombre_empresa,nit_rut,contacto_nombre,telefono,correo,direccion) VALUES (@e,@n,@c,@t,@co,@d)", con);
            cmd.Parameters.AddWithValue("@e", TxtProvEmpresa.Text.Trim());
            cmd.Parameters.AddWithValue("@n", TxtProvNIT.Text.Trim());
            cmd.Parameters.AddWithValue("@c", TxtProvContacto.Text.Trim());
            cmd.Parameters.AddWithValue("@t", TxtProvTel.Text.Trim());
            cmd.Parameters.AddWithValue("@co", TxtProvCorreo.Text.Trim());
            cmd.Parameters.AddWithValue("@d", TxtProvDir.Text.Trim());
            con.Open();
            try { cmd.ExecuteNonQuery(); MostrarMsg("Proveedor registrado.", false); }
            catch { MostrarMsg("El NIT ya existe.", true); }
            con.Close();
            TxtProvEmpresa.Text = TxtProvNIT.Text = TxtProvContacto.Text = TxtProvTel.Text = TxtProvCorreo.Text = TxtProvDir.Text = "";
            CargarDatosProv(); CargarDropdowns();
        }

        protected void GridView4_RowEditing(object sender, GridViewEditEventArgs e)
        { 
            GridView4.EditIndex = e.NewEditIndex; CargarDatosProv(); 
        }
        protected void GridView4_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) 
        { 
            GridView4.EditIndex = -1; CargarDatosProv(); 
        }

        protected void GridView4_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(GridView4.DataKeys[e.RowIndex].Value);
            GridViewRow r = GridView4.Rows[e.RowIndex];
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("UPDATE proveedor SET nombre_empresa=@e,nit_rut=@n,contacto_nombre=@c,telefono=@t,correo=@co,direccion=@d WHERE id_proveedor=@id", con);
            cmd.Parameters.AddWithValue("@e", ((TextBox)r.Cells[1].Controls[0]).Text);
            cmd.Parameters.AddWithValue("@n", ((TextBox)r.Cells[2].Controls[0]).Text);
            cmd.Parameters.AddWithValue("@c", ((TextBox)r.Cells[3].Controls[0]).Text);
            cmd.Parameters.AddWithValue("@t", ((TextBox)r.Cells[4].Controls[0]).Text);
            cmd.Parameters.AddWithValue("@co", ((TextBox)r.Cells[5].Controls[0]).Text);
            cmd.Parameters.AddWithValue("@d", ((TextBox)r.Cells[6].Controls[0]).Text);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            GridView4.EditIndex = -1; CargarDatosProv(); CargarDropdowns();
        }

        protected void GridView4_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView4.DataKeys[e.RowIndex].Value);
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("DELETE FROM proveedor WHERE id_proveedor=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            CargarDatosProv(); CargarDropdowns();
        }

        protected void BtnRegistrarMov_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtMovCantidad.Text)) { MostrarMsg("Ingrese una cantidad.", true); return; }
            int cant;
            if (!int.TryParse(TxtMovCantidad.Text, out cant) || cant <= 0) { MostrarMsg("La cantidad debe ser un número positivo.", true); return; }

            int idCel = Convert.ToInt32(DdlCelularMov.SelectedValue);
            string tipo = DdlTipoMov.SelectedValue;
            int idAdmin = Convert.ToInt32(Session["UsuarioID"]);

            SqlConnection con = new SqlConnection(conexion);
            con.Open();

            if (tipo == "salida")
            {
                SqlCommand chk = new SqlCommand("SELECT stock FROM celular WHERE id_celular=@id", con);
                chk.Parameters.AddWithValue("@id", idCel);
                int stockActual = Convert.ToInt32(chk.ExecuteScalar());
                if (cant > stockActual) { MostrarMsg("Stock insuficiente para la salida. Disponible: " + stockActual, true); con.Close(); return; }
            }

            SqlCommand cmdMov = new SqlCommand("INSERT INTO movimiento_inventario (id_celular,tipo_movimiento,cantidad,motivo,id_usuario_responsable) VALUES (@c,@t,@cant,@m,@u)", con);
            cmdMov.Parameters.AddWithValue("@c", idCel); cmdMov.Parameters.AddWithValue("@t", tipo);
            cmdMov.Parameters.AddWithValue("@cant", cant);
            cmdMov.Parameters.AddWithValue("@m", string.IsNullOrEmpty(TxtMovMotivo.Text) ? "Ajuste manual" : TxtMovMotivo.Text.Trim());
            cmdMov.Parameters.AddWithValue("@u", idAdmin);
            cmdMov.ExecuteNonQuery();

            string updateStock = tipo == "entrada" ? "UPDATE celular SET stock=stock+@cant WHERE id_celular=@id"
                               : tipo == "salida" ? "UPDATE celular SET stock=stock-@cant WHERE id_celular=@id"
                               : null;
            if (updateStock != null)
            {
                SqlCommand cmdSt = new SqlCommand(updateStock, con);
                cmdSt.Parameters.AddWithValue("@cant", cant); cmdSt.Parameters.AddWithValue("@id", idCel);
                cmdSt.ExecuteNonQuery();
            }
            con.Close();
            TxtMovCantidad.Text = TxtMovMotivo.Text = "";
            MostrarMsg("Movimiento registrado.", false);
            CargarMovimientos(); CargarDatosCel();
        }

        protected void BtnFiltrarVentas_Click(object sender, EventArgs e)
        { 
            CargarHistorialVentas(TxtBuscarUsuario.Text.Trim(), TxtFechaDesde.Text, TxtFechaHasta.Text, OrdenVentas);
        }

        protected void BtnOrdenAsc_Click(object sender, EventArgs e)
        { 
            OrdenVentas = "ASC"; CargarHistorialVentas(TxtBuscarUsuario.Text.Trim(), TxtFechaDesde.Text, TxtFechaHasta.Text, "ASC"); 
        }

        protected void BtnOrdenDesc_Click(object sender, EventArgs e)
        { 
            OrdenVentas = "DESC"; CargarHistorialVentas(TxtBuscarUsuario.Text.Trim(), TxtFechaDesde.Text, TxtFechaHasta.Text, "DESC"); 
        }

        protected void BtnLimpiarFiltros_Click(object sender, EventArgs e)
        { 
            TxtBuscarUsuario.Text = TxtFechaDesde.Text = TxtFechaHasta.Text = ""; OrdenVentas = "DESC"; CargarHistorialVentas(orden: "DESC"); 
        }

        protected void GridView3_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView3.EditIndex = e.NewEditIndex;
            CargarHistorialVentas(orden: OrdenVentas);
            GridViewRow r = GridView3.Rows[e.NewEditIndex];
            DropDownList ddl = (DropDownList)r.FindControl("DdlProvVenta");
            if (ddl != null)
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT id_proveedor, nombre_empresa FROM proveedor", conexion);
                DataTable dt = new DataTable(); da.Fill(dt);
                ddl.DataSource = dt; ddl.DataTextField = "nombre_empresa"; ddl.DataValueField = "id_proveedor"; ddl.DataBind();
                Label lbl = (Label)r.FindControl("LblProv");
                if (lbl != null) { ListItem it = ddl.Items.FindByText(lbl.Text); if (it != null) it.Selected = true; }
            }
        }

        protected void GridView3_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        { GridView3.EditIndex = -1; CargarHistorialVentas(orden: OrdenVentas); }

        protected void GridView3_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int idVenta = Convert.ToInt32(GridView3.DataKeys[e.RowIndex].Value);
            GridViewRow r = GridView3.Rows[e.RowIndex];
            string dir = ((TextBox)r.Cells[6].Controls[0]).Text;
            int idProv = Convert.ToInt32(((DropDownList)r.FindControl("DdlProvVenta")).SelectedValue);
            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmd = new SqlCommand("UPDATE venta SET direccion_envio=@dir, id_proveedor_envio=@prov WHERE id_venta=@id", con);
            cmd.Parameters.AddWithValue("@dir", dir); cmd.Parameters.AddWithValue("@prov", idProv); cmd.Parameters.AddWithValue("@id", idVenta);
            con.Open(); cmd.ExecuteNonQuery(); con.Close();
            GridView3.EditIndex = -1; CargarHistorialVentas(orden: OrdenVentas);
        }

        protected void BtnDescargarPDFAdmin_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idVenta = Convert.ToInt32(btn.CommandArgument);
            GenerarFacturaPDF(idVenta);
        }

        private void GenerarFacturaPDF(int idVenta)
        {
            string qF = @"SELECT f.numero_factura, f.fecha_emision, f.razon_social, f.nit_ci_cliente,
                          f.monto_total, f.metodo_pago, v.estado_venta, v.direccion_envio,
                          ISNULL(p.nombre_empresa,'Sin asignar') AS proveedor_envio
                          FROM factura f INNER JOIN venta v ON f.id_venta=v.id_venta
                          LEFT JOIN proveedor p ON v.id_proveedor_envio=p.id_proveedor
                          WHERE f.id_venta=@id";
            string qD = @"SELECT c.marca, c.modelo, dv.cantidad, dv.precio_unitario, dv.subtotal
                          FROM detalle_venta dv INNER JOIN celular c ON dv.id_celular=c.id_celular
                          WHERE dv.id_venta=@id";

            SqlConnection con = new SqlConnection(conexion);
            SqlCommand cmdF = new SqlCommand(qF, con); cmdF.Parameters.AddWithValue("@id", idVenta);
            SqlCommand cmdD = new SqlCommand(qD, con); cmdD.Parameters.AddWithValue("@id", idVenta);
            SqlDataAdapter daF = new SqlDataAdapter(cmdF), daD = new SqlDataAdapter(cmdD);
            DataTable dtF = new DataTable(), dtD = new DataTable();
            daF.Fill(dtF); daD.Fill(dtD);
            if (dtF.Rows.Count == 0) return;
            DataRow f = dtF.Rows[0];

            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html><html><head><meta charset='utf-8'/>");
            html.Append("<style>body{font-family:Arial,sans-serif;padding:30px;color:#333;}");
            html.Append("h2{color:#007bff;border-bottom:2px solid #007bff;padding-bottom:8px;}");
            html.Append("table{width:100%;border-collapse:collapse;margin-top:15px;}");
            html.Append("th,td{padding:9px 12px;border:1px solid #ddd;}th{background:#f8f9fa;}");
            html.Append(".total{font-size:16px;font-weight:bold;text-align:right;margin-top:10px;}");
            html.Append(".info p{margin:4px 0;font-size:14px;}</style></head><body>");
            html.Append("<h2>FACTURA - Tienda de Celulares</h2><div class='info'>");
            html.AppendFormat("<p><b>N° Factura:</b> {0}</p>", f["numero_factura"]);
            html.AppendFormat("<p><b>Fecha:</b> {0:dd/MM/yyyy HH:mm}</p>", f["fecha_emision"]);
            html.AppendFormat("<p><b>Cliente:</b> {0}</p>", f["razon_social"]);
            html.AppendFormat("<p><b>C.I./NIT:</b> {0}</p>", f["nit_ci_cliente"]);
            html.AppendFormat("<p><b>Método de pago:</b> {0}</p>", f["metodo_pago"]);
            html.AppendFormat("<p><b>Dirección de envío:</b> {0}</p>", f["direccion_envio"]);
            html.AppendFormat("<p><b>Proveedor de envío:</b> {0}</p></div>", f["proveedor_envio"]);
            html.Append("<table><tr><th>Producto</th><th>Cantidad</th><th>Precio Unit.</th><th>Subtotal</th></tr>");
            foreach (DataRow d in dtD.Rows)
                html.AppendFormat("<tr><td>{0} {1}</td><td>{2}</td><td>Bs.{3:N2}</td><td>Bs.{4:N2}</td></tr>",
                    d["marca"], d["modelo"], d["cantidad"], d["precio_unitario"], d["subtotal"]);
            html.Append("</table>");
            html.AppendFormat("<p class='total'>TOTAL: Bs. {0:N2}</p></body></html>", f["monto_total"]);

            Response.Clear();
            Response.ContentType = "text/html";
            Response.AddHeader("Content-Disposition", "attachment; filename=Factura_" + f["numero_factura"] + ".html");
            Response.Write(html.ToString());
            Response.End();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("PantallaLOGIN.aspx");
        }

        private void MostrarMsg(string texto, bool esError)
        {
            lblMsg.Text = texto;
            lblMsg.CssClass = esError ? "msg msg-err" : "msg msg-ok";
            lblMsg.Visible = true;
        }
    }
}
