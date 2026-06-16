<%-- PantallaADMIN.aspx --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaADMIN.aspx.cs" Inherits="Proyecto_BDII.PantallaADMIN" %>
<%@ Register Src="~/_Header.ascx" TagName="Header" TagPrefix="uc" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Panel Admin - iStore</title>
    <style>
        /* ... estilos existentes sin cambios ... */
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; padding: 0 20px 20px; }
        .contenedor { max-width: 1300px; margin: 0 auto; }
        .header-bar { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
        h1 { font-size: 26px; color: #cc0000; border-bottom: 2px solid #cc0000; padding-bottom: 8px; }
        h2 { font-size: 20px; color: #663300; text-decoration: underline; margin: 18px 0 10px; }
        hr { border: none; border-top: 1px solid #ddd; margin: 20px 0; }
        .form-grid { display: flex; flex-wrap: wrap; gap: 10px; margin-bottom: 12px; }
        .form-campo { display: flex; flex-direction: column; min-width: 200px; }
        .form-campo label { font-size: 13px; font-weight: bold; margin-bottom: 3px; }
        .form-campo input, .form-campo select, .form-campo textarea { padding: 7px; border: 1px solid #ccc; border-radius: 4px; font-size: 13px; }
        .form-campo input.input-error { border-color: #dc3545; }
        .campo-err { font-size: 11px; color: #dc3545; }
        .btn-crear { padding: 9px 20px; background-color: #28a745; color: white; border: none; border-radius: 4px; font-size: 14px; font-weight: bold; cursor: pointer; margin-top: 10px; }
        .btn-crear:hover { background-color: #218838; }
        .btn-logout { padding: 8px 14px; background-color: #6c757d; color: white; border: none; border-radius: 4px; font-size: 13px; font-weight: bold; cursor: pointer; }
        .btn-logout:hover { background-color: #5a6268; }
        .btn-pdf { background-color: #6f42c1; color: white; border: none; border-radius: 4px; padding: 5px 10px; cursor: pointer; font-size: 12px; font-weight: bold; }
        .btn-pdf:hover { background-color: #563d7c; }
        .filtros { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; margin-bottom: 12px; }
        .filtros label { font-size: 13px; }
        .filtros input { padding: 6px; border: 1px solid #ccc; border-radius: 4px; font-size: 13px; }
        .btn-filtro { padding: 6px 12px; background-color: #ffc107; color: #333; border: none; border-radius: 4px; font-size: 13px; font-weight: bold; cursor: pointer; }
        .btn-ord { padding: 6px 10px; background-color: #6c757d; color: white; border: none; border-radius: 4px; font-size: 12px; cursor: pointer; }
        .btn-limpiar { padding: 6px 10px; background-color: #999; color: white; border: none; border-radius: 4px; font-size: 12px; cursor: pointer; }
        .msg { font-size: 13px; padding: 8px; border-radius: 4px; display: block; margin-bottom: 10px; }
        .msg-ok { color: green; } .msg-err { color: red; }
        table { border-collapse: collapse; width: 100%; font-size: 13px; }
        th, td { padding: 8px 10px; border: 1px solid #ddd; text-align: left; }
        th { background-color: #f0f0f0; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <uc:Header ID="Header1" runat="server" />
    <div class="contenedor">
        <div class="header-bar">
            <h1>PANEL ADMINISTRADOR</h1>
            <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" CssClass="btn-logout"
                OnClientClick="return confirm('¿Seguro que deseas cerrar sesión?');"
                OnClick="btnLogout_Click" />
        </div>

        <asp:Label ID="lblMsg" runat="server" CssClass="msg" Visible="false"></asp:Label>

        <%-- ===== CELULARES ===== --%>
        <a name="celulares"></a>
        <h2>📱 LISTA DE CELULARES</h2>
        <%-- ... GridView1 sin cambios ... --%>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="id_celular"
            OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating"
            OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting">
            <Columns>
                <asp:BoundField DataField="id_celular" HeaderText="ID" ReadOnly="True" />
                <asp:BoundField DataField="marca" HeaderText="Marca" />
                <asp:BoundField DataField="modelo" HeaderText="Modelo" />
                <asp:BoundField DataField="imei" HeaderText="IMEI"/>
                <asp:BoundField DataField="capacidad_almacenamiento" HeaderText="Almac."/>
                <asp:BoundField DataField="memoria_ram" HeaderText="RAM"/>
                <asp:BoundField DataField="ano_fabricacion" HeaderText="Año"/>
                <asp:BoundField DataField="version_so" HeaderText="SO"/>
                <asp:BoundField DataField="numero_banda" HeaderText="Banda"/>
                <asp:BoundField DataField="descripcion" HeaderText="Descripción"/>
                <asp:TemplateField HeaderText="Categoría">
                    <ItemTemplate><asp:Label ID="LblCategoria" runat="server" Text='<%# Eval("nombre_categoria") %>'></asp:Label></ItemTemplate>
                    <EditItemTemplate><asp:DropDownList ID="DdlCategoriaEdit" runat="server" DataTextField="nombre_categoria" DataValueField="id_categoria"></asp:DropDownList></EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Proveedor">
                    <ItemTemplate><asp:Label ID="LblProveedor" runat="server" Text='<%# Eval("nombre_empresa") %>'></asp:Label></ItemTemplate>
                    <EditItemTemplate><asp:DropDownList ID="DdlProveedorEdit" runat="server" DataTextField="nombre_empresa" DataValueField="id_proveedor"></asp:DropDownList></EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="precio" HeaderText="Precio"/>
                <asp:BoundField DataField="stock" HeaderText="Stock"/>
                <asp:TemplateField HeaderText="Imagen">
                    <ItemTemplate>
                        <asp:Image ID="ImgCelular" runat="server"
                            ImageUrl='<%# "~/" + Eval("url_imagen") %>'
                            Width="70px" Height="70px"
                            Visible='<%# Eval("url_imagen") != DBNull.Value && Eval("url_imagen").ToString() != "" %>'/>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:FileUpload ID="FuImagenEdit" runat="server" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True"/>
            </Columns>
        </asp:GridView>

        <hr/>
        <h2>➕ CREAR NUEVO CELULAR</h2>
        <div class="form-grid">
            <div class="form-campo">
                <label>Marca: *</label>
                <asp:TextBox ID="TxtMarca" runat="server"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Modelo: *</label>
                <asp:TextBox ID="TxtModelo" runat="server"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>IMEI:</label>
                <asp:TextBox ID="TxtIMEI" runat="server" MaxLength="15"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Almacenamiento:</label>
                <asp:TextBox ID="TxtAlmac" runat="server" placeholder="128GB"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>RAM:</label>
                <asp:TextBox ID="TxtRAM" runat="server" placeholder="8GB"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Año Fabricación:</label>
                <asp:TextBox ID="TxtAno" runat="server" placeholder="2024"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Versión SO:</label>
                <asp:TextBox ID="TxtSO" runat="server" placeholder="Android 14"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Banda:</label>
                <asp:TextBox ID="TxtBanda" runat="server" placeholder="5G / 4G LTE"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Descripción:</label>
                <asp:TextBox ID="TxtDescripcion" runat="server"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Categoría:</label>
                <asp:DropDownList ID="DdlCategoria" runat="server" DataTextField="nombre_categoria" DataValueField="id_categoria"></asp:DropDownList>
            </div>
            <div class="form-campo">
                <label>Proveedor:</label>
                <asp:DropDownList ID="DdlProveedor" runat="server" DataTextField="nombre_empresa" DataValueField="id_proveedor"></asp:DropDownList>
            </div>
            <div class="form-campo">
                <label>Precio: *</label>
                <asp:TextBox ID="TxtPrecio" runat="server" placeholder="0.00"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Stock Inicial: *</label>
                <asp:TextBox ID="TxtStock" runat="server" placeholder="0"></asp:TextBox>
            </div>
            <div class="form-campo">
                <label>Imagen:</label>
                <asp:FileUpload ID="FuImagen" runat="server"/>
            </div>
        </div>
        <asp:Button ID="BtnCrearCel" runat="server" Text="CREAR CELULAR" CssClass="btn-crear"
            OnClientClick="return validarFormCelular();" OnClick="BtnCrearCel_Click"/>

        <hr/>
        <%-- ===== CATEGORIAS ===== --%>
        <a name="categorias"></a>
        <h2>🏷️ LISTA DE CATEGORÍAS</h2>
        <%-- GridView2 sin cambios --%>
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataKeyNames="id_categoria"
            OnRowEditing="GridView2_RowEditing" OnRowUpdating="GridView2_RowUpdating"
            OnRowCancelingEdit="GridView2_RowCancelingEdit" OnRowDeleting="GridView2_RowDeleting">
            <Columns>
                <asp:BoundField DataField="id_categoria" HeaderText="ID" ReadOnly="True"/>
                <asp:BoundField DataField="nombre_categoria" HeaderText="Nombre"/>
                <asp:BoundField DataField="descripcion" HeaderText="Descripción"/>
                <asp:BoundField DataField="icono" HeaderText="Icono"/>
                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True"/>
            </Columns>
        </asp:GridView>
        <hr/>
        <h2>➕ CREAR NUEVA CATEGORÍA</h2>
        <div class="form-grid">
            <div class="form-campo"><label>Nombre: *</label><asp:TextBox ID="TxtCatNombre" runat="server"></asp:TextBox></div>
            <div class="form-campo"><label>Descripción:</label><asp:TextBox ID="TxtCatDescripcion" runat="server"></asp:TextBox></div>
            <div class="form-campo"><label>Icono (emoji):</label><asp:TextBox ID="TxtCatIcono" runat="server"></asp:TextBox></div>
        </div>
        <asp:Button ID="BtnCrearCateg" runat="server" Text="CREAR CATEGORÍA" CssClass="btn-crear" OnClick="BtnCrearCateg_Click"/>

        <hr/>
        <%-- ===== PROVEEDORES ===== --%>
        <a name="proveedores"></a>
        <h2>🏭 LISTA DE PROVEEDORES</h2>
        <%-- GridView4 sin cambios --%>
        <asp:GridView ID="GridView4" runat="server" AutoGenerateColumns="False" DataKeyNames="id_proveedor"
            OnRowEditing="GridView4_RowEditing" OnRowUpdating="GridView4_RowUpdating"
            OnRowCancelingEdit="GridView4_RowCancelingEdit" OnRowDeleting="GridView4_RowDeleting">
            <Columns>
                <asp:BoundField DataField="id_proveedor" HeaderText="ID" ReadOnly="True"/>
                <asp:BoundField DataField="nombre_empresa" HeaderText="Empresa"/>
                <asp:BoundField DataField="nit_rut" HeaderText="NIT/RUT"/>
                <asp:BoundField DataField="contacto_nombre" HeaderText="Contacto"/>
                <asp:BoundField DataField="telefono" HeaderText="Teléfono"/>
                <asp:BoundField DataField="correo" HeaderText="Correo"/>
                <asp:BoundField DataField="direccion" HeaderText="Dirección"/>
                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True"/>
            </Columns>
        </asp:GridView>
        <hr/>
        <h2>➕ REGISTRAR PROVEEDOR</h2>
        <div class="form-grid">
            <div class="form-campo"><label>Empresa: *</label><asp:TextBox ID="TxtProvEmpresa" runat="server"></asp:TextBox></div>
            <div class="form-campo"><label>NIT/RUT: *</label><asp:TextBox ID="TxtProvNIT" runat="server"></asp:TextBox></div>
            <div class="form-campo"><label>Contacto:</label><asp:TextBox ID="TxtProvContacto" runat="server"></asp:TextBox></div>
            <div class="form-campo"><label>Teléfono:</label><asp:TextBox ID="TxtProvTel" runat="server"></asp:TextBox></div>
            <div class="form-campo">
                <label>Correo:</label>
                <asp:TextBox ID="TxtProvCorreo" runat="server" TextMode="Email"></asp:TextBox>
            </div>
            <div class="form-campo"><label>Dirección:</label><asp:TextBox ID="TxtProvDir" runat="server"></asp:TextBox></div>
        </div>
        <asp:Button ID="BtnCrearProv" runat="server" Text="REGISTRAR PROVEEDOR" CssClass="btn-crear"
            OnClientClick="return validarFormProveedor();" OnClick="BtnCrearProv_Click"/>

        <hr/>
        <%-- ===== MOVIMIENTO INVENTARIO ===== --%>
        <a name="inventario"></a>
        <h2>📦 MOVIMIENTOS DE INVENTARIO</h2>
        <div class="form-grid">
            <div class="form-campo"><label>Celular:</label><asp:DropDownList ID="DdlCelularMov" runat="server" DataTextField="nombre_celular" DataValueField="id_celular"></asp:DropDownList></div>
            <div class="form-campo"><label>Tipo:</label>
                <asp:DropDownList ID="DdlTipoMov" runat="server">
                    <asp:ListItem Value="entrada">Entrada</asp:ListItem>
                    <asp:ListItem Value="salida">Salida</asp:ListItem>
                    <asp:ListItem Value="ajuste">Ajuste</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-campo"><label>Cantidad: *</label><asp:TextBox ID="TxtMovCantidad" runat="server" placeholder="1"></asp:TextBox></div>
            <div class="form-campo"><label>Motivo:</label><asp:TextBox ID="TxtMovMotivo" runat="server"></asp:TextBox></div>
        </div>
        <asp:Button ID="BtnRegistrarMov" runat="server" Text="REGISTRAR MOVIMIENTO" CssClass="btn-crear"
            OnClientClick="return validarMovimiento();" OnClick="BtnRegistrarMov_Click"/>
        <br/><br/>
        <asp:GridView ID="GridView5" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="id_movimiento" HeaderText="ID"/>
                <asp:BoundField DataField="nombre_celular" HeaderText="Celular"/>
                <asp:BoundField DataField="tipo_movimiento" HeaderText="Tipo"/>
                <asp:BoundField DataField="cantidad" HeaderText="Cantidad"/>
                <asp:BoundField DataField="motivo" HeaderText="Motivo"/>
                <asp:BoundField DataField="fecha_movimiento" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}"/>
                <asp:BoundField DataField="responsable" HeaderText="Responsable"/>
            </Columns>
        </asp:GridView>

        <hr/>
        <%-- ===== HISTORIAL VENTAS ===== --%>
        <a name="ventas"></a>
        <h2>📊 HISTORIAL DE VENTAS</h2>
        <div class="filtros">
            <label>Usuario:</label><asp:TextBox ID="TxtBuscarUsuario" runat="server" style="width:140px"></asp:TextBox>
            <label>Desde:</label><asp:TextBox ID="TxtFechaDesde" runat="server" TextMode="Date"></asp:TextBox>
            <label>Hasta:</label><asp:TextBox ID="TxtFechaHasta" runat="server" TextMode="Date"></asp:TextBox>
            <asp:Button ID="BtnOrdenAsc"      runat="server" Text="Fecha ASC"  CssClass="btn-ord"    OnClick="BtnOrdenAsc_Click"/>
            <asp:Button ID="BtnOrdenDesc"     runat="server" Text="Fecha DESC" CssClass="btn-ord"    OnClick="BtnOrdenDesc_Click"/>
            <asp:Button ID="BtnFiltrarVentas" runat="server" Text="FILTRAR"    CssClass="btn-filtro" OnClick="BtnFiltrarVentas_Click"/>
            <asp:Button ID="BtnLimpiarFiltros" runat="server" Text="LIMPIAR"   CssClass="btn-limpiar" OnClick="BtnLimpiarFiltros_Click"/>
        </div>
        <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False"
            OnRowEditing="GridView3_RowEditing" OnRowUpdating="GridView3_RowUpdating"
            OnRowCancelingEdit="GridView3_RowCancelingEdit" DataKeyNames="id_venta">
            <Columns>
                <asp:BoundField DataField="id_venta" HeaderText="ID" ReadOnly="True"/>
                <asp:BoundField DataField="nombre" HeaderText="Usuario"/>
                <asp:BoundField DataField="correo" HeaderText="Correo"/>
                <asp:BoundField DataField="fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" ReadOnly="True"/>
                <asp:BoundField DataField="total" HeaderText="Total" DataFormatString="{0:N2}" ReadOnly="True"/>
                <asp:BoundField DataField="estado_venta" HeaderText="Estado" ReadOnly="True"/>
                <asp:BoundField DataField="direccion_envio" HeaderText="Dirección Envío"/>
                <asp:TemplateField HeaderText="Proveedor Envío">
                    <ItemTemplate><asp:Label ID="LblProv" runat="server" Text='<%# Eval("nombre_proveedor") %>'></asp:Label></ItemTemplate>
                    <EditItemTemplate><asp:DropDownList ID="DdlProvVenta" runat="server" DataTextField="nombre_empresa" DataValueField="id_proveedor"></asp:DropDownList></EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Factura PDF">
                    <ItemTemplate>
                        <asp:Button ID="btnPDFVenta" runat="server" Text="📄 PDF" CssClass="btn-pdf"
                            CommandArgument='<%# Eval("id_venta") %>' OnClick="BtnDescargarPDFAdmin_Click"
                            Visible='<%# Eval("tiene_factura").ToString() == "1" %>'/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowEditButton="True"/>
            </Columns>
        </asp:GridView>
    </div>
</form>
<script>
function validarFormCelular() {
    var marca  = document.getElementById('<%= TxtMarca.ClientID %>').value.trim();
    var modelo = document.getElementById('<%= TxtModelo.ClientID %>').value.trim();
    var precio = document.getElementById('<%= TxtPrecio.ClientID %>').value.trim();
    var stock  = document.getElementById('<%= TxtStock.ClientID %>').value.trim();
    var ano    = document.getElementById('<%= TxtAno.ClientID %>').value.trim();
    if (!marca || !modelo) { alert('Marca y modelo son obligatorios.'); return false; }
    if (!precio || isNaN(parseFloat(precio)) || parseFloat(precio) <= 0)
        { alert('El precio debe ser un número mayor a 0.'); return false; }
    if (!stock || isNaN(parseInt(stock)) || parseInt(stock) < 0)
        { alert('El stock debe ser un número no negativo.'); return false; }
    if (ano && (isNaN(parseInt(ano)) || parseInt(ano) < 2000 || parseInt(ano) > 2100))
        { alert('Año de fabricación inválido.'); return false; }
    return true;
}
function validarFormProveedor() {
    var emp = document.getElementById('<%= TxtProvEmpresa.ClientID %>').value.trim();
    var nit = document.getElementById('<%= TxtProvNIT.ClientID %>').value.trim();
    var correo = document.getElementById('<%= TxtProvCorreo.ClientID %>').value.trim();
    var reCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emp || !nit) { alert('Empresa y NIT son obligatorios.'); return false; }
    if (correo && !reCorreo.test(correo)) { alert('El formato del correo del proveedor no es válido.'); return false; }
    return true;
}
function validarMovimiento() {
    var cant = document.getElementById('<%= TxtMovCantidad.ClientID %>').value.trim();
    if (!cant || isNaN(parseInt(cant)) || parseInt(cant) <= 0)
        { alert('La cantidad debe ser un número positivo.'); return false; }
    return true;
}
</script>
</body>
</html>