<%-- PantallaUSER.aspx --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaUSER.aspx.cs" Inherits="Proyecto_BDII.PantallaUSER" %>
<%@ Register Src="~/_Header.ascx" TagName="Header" TagPrefix="uc" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Catálogo - iStore</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; padding: 0 20px 20px; }
        .contenedor { max-width: 1200px; margin: 0 auto; }
        .seccion-header { display:flex; justify-content:space-between; align-items:center; margin-bottom:10px; }
        h1 { font-size: 26px; border-bottom: 2px solid #007bff; padding-bottom: 8px; color: #222; }
        h2 { font-size: 20px; color: #222; margin-bottom: 10px; margin-top: 20px; }
        hr { border: none; border-top: 1px solid #ddd; margin: 15px 0; }
        .producto-card {
            border: 1px solid #ddd; border-radius: 5px; padding: 15px;
            margin-bottom: 15px; margin-right: 15px; width: 310px;
            display: inline-block; vertical-align: top; background-color: #fff;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .producto-img { width: 100%; height: 180px; object-fit: contain; margin-bottom: 12px; border-bottom: 1px solid #eee; padding-bottom: 8px; }
        .producto-card h3 { font-size: 17px; margin-bottom: 8px; color: #222; }
        .producto-card p { font-size: 14px; margin-bottom: 8px; line-height: 1.5; }
        /* Alertas de stock */
        .stock-agotado  { color: #dc3545; font-weight: bold; }
        .stock-critico  { color: #e67e00; font-weight: bold; }
        .badge-agotado  { display:inline-block; background:#dc3545; color:#fff; font-size:11px; padding:2px 8px; border-radius:10px; margin-left:6px; }
        .badge-critico  { display:inline-block; background:#ffc107; color:#333; font-size:11px; padding:2px 8px; border-radius:10px; margin-left:6px; }
        .card-agotado   { border-color: #dc3545; opacity: 0.7; }
        .card-critico   { border-color: #ffc107; }
        .favorito-card { border:1px solid #FFD700; border-radius:5px; padding:15px; margin-bottom:15px; margin-right:15px; width:300px; display:inline-block; vertical-align:top; background:#FFFDF0; }
        .favorito-card h3 { font-size:17px; margin-bottom:8px; color:#FF9800; }
        .botones { margin-top: 12px; display: flex; gap: 8px; flex-wrap: wrap; }
        .btn-base { padding:8px 12px; font-size:13px; border:none; border-radius:4px; cursor:pointer; color:white; font-weight:bold; }
        .btn-detalle  { background-color:#007bff; } .btn-detalle:hover  { background-color:#0056b3; }
        .btn-carrito  { background-color:#17a2b8; } .btn-carrito:hover  { background-color:#117a8b; }
        .btn-comprar  { background-color:#28a745; } .btn-comprar:hover  { background-color:#218838; }
        .btn-favorito { background-color:#ff9800; } .btn-favorito:hover { background-color:#e68a00; }
        .btn-peligro  { background-color:#dc3545; } .btn-peligro:hover  { background-color:#c82333; }
        .btn-logout   { background-color:#6c757d; padding:8px 14px; font-size:13px; border:none; border-radius:4px; cursor:pointer; color:white; font-weight:bold; }
        .btn-logout:hover { background-color:#5a6268; }
        .btn-pdf { background-color:#6f42c1; } .btn-pdf:hover { background-color:#563d7c; }
        .clearfix { clear:both; }
        .historial-tabla { width:100%; border-collapse:collapse; font-size:14px; }
        .historial-tabla th, .historial-tabla td { padding:10px 12px; border-bottom:1px solid #ddd; text-align:left; }
        .historial-tabla th { background-color:#f8f9fa; color:#555; }
        .msg { font-size:14px; padding:10px; border-radius:4px; }
        .bienvenida { font-size:15px; color:#555; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <uc:Header ID="Header1" runat="server" />
    <div class="contenedor">
        <div class="seccion-header">
            <h1>🛍️ Catálogo de Dispositivos</h1>
            <div>
                <span class="bienvenida">Hola, <asp:Literal ID="litNombre" runat="server"></asp:Literal></span>&nbsp;&nbsp;
                <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" CssClass="btn-logout"
                    OnClientClick="return confirm('¿Seguro que deseas cerrar sesión?');"
                    OnClick="btnLogout_Click" />
            </div>
        </div>
        <hr />

        <asp:Label ID="lblMsgCatalogo" runat="server" CssClass="msg" Visible="false"></asp:Label>

        <asp:Repeater ID="repCelulares" runat="server" OnItemCommand="repCelulares_ItemCommand">
            <ItemTemplate>
                <div class='<%# GetCardCss(Eval("stock")) %>'>
                    <img src='<%# string.IsNullOrEmpty(Eval("url_imagen").ToString()) ? "https://via.placeholder.com/300x180?text=Sin+Imagen" : Eval("url_imagen") %>'
                         class="producto-img" alt="Celular" />
                    <h3><%# Eval("marca") %> <%# Eval("modelo") %></h3>
                    <p><%# Eval("descripcion") %></p>
                    <p>
                        <b>Precio:</b> Bs. <%# Eval("precio", "{0:N2}") %><br />
                        <b>Stock:</b> <span class='<%# GetStockCss(Eval("stock")) %>'><%# Eval("stock") %></span>
                        <%# GetStockBadge(Eval("stock")) %>
                    </p>
                    <div class="botones">
                        <asp:Button ID="btnVerDetalle" runat="server" Text="Ver Detalles" CssClass="btn-base btn-detalle"
                            CommandName="Detalle" CommandArgument='<%# Eval("id_celular") %>' />
                        <asp:Button ID="btnAgregarCarrito" runat="server" Text="🛒 Agregar" CssClass="btn-base btn-carrito"
                            CommandName="AgregarCarrito" CommandArgument='<%# Eval("id_celular") %>'
                            Enabled='<%# Convert.ToInt32(Eval("stock")) > 0 %>' />
                        <asp:Button ID="btnFavorito" runat="server" Text="⭐ Fav" CssClass="btn-base btn-favorito"
                            CommandName="Favorito" CommandArgument='<%# Eval("id_celular") %>' />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="clearfix"></div>
        <a name="favoritos"></a>
        <h2>⭐ Mis Favoritos</h2>
        <hr />
        <asp:Label ID="lblMensajeFavoritos" runat="server" CssClass="msg"></asp:Label>
        <asp:Repeater ID="repFavoritos" runat="server" OnItemCommand="repFavoritos_ItemCommand">
            <ItemTemplate>
                <div class="favorito-card">
                    <h3><%# Eval("marca") %> <%# Eval("modelo") %></h3>
                    <p><b>Precio:</b> Bs. <%# Eval("precio", "{0:N2}") %></p>
                    <div class="botones">
                        <asp:Button ID="btnEliminarFavorito" runat="server" Text="❌ Quitar" CssClass="btn-base btn-peligro"
                            CommandName="EliminarFavorito" CommandArgument='<%# Eval("id_celular") %>' />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="clearfix"></div>
        <a name="historial"></a>
        <h2>📋 Historial de Compras</h2>
        <hr />
        <asp:Label ID="lblMensajeHistorial" runat="server" CssClass="msg"></asp:Label>
        <asp:Repeater ID="repHistorial" runat="server" OnItemCommand="repHistorial_ItemCommand">
            <HeaderTemplate>
                <table class="historial-tabla">
                <tr><th>ID Venta</th><th>Fecha</th><th>Total</th><th>Estado</th><th>Proveedor Envío</th><th>Dirección</th><th>Factura</th></tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("id_venta") %></td>
                    <td><%# Eval("fecha", "{0:dd/MM/yyyy HH:mm}") %></td>
                    <td>Bs. <%# Eval("total", "{0:N2}") %></td>
                    <td><%# Eval("estado_venta") %></td>
                    <td><%# Eval("nombre_proveedor") %></td>
                    <td><%# Eval("direccion_envio") %></td>
                    <td>
                        <asp:Button ID="btnDescargarPDF" runat="server" Text="📄 PDF"
                            CssClass="btn-base btn-pdf"
                            CommandName="DescargarPDF"
                            CommandArgument='<%# Eval("id_venta") %>'
                            Visible='<%# Eval("tiene_factura").ToString() == "1" %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></table></FooterTemplate>
        </asp:Repeater>
    </div>
</form>
</body>
</html>