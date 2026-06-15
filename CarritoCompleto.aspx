<%-- CarritoCompleto.aspx --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CarritoCompleto.aspx.cs" Inherits="Proyecto_BDII.CarritoCompleto" %>
<%@ Register Src="~/_Header.ascx" TagName="Header" TagPrefix="uc" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Mi Carrito - iStore</title>
    <style>
        * { margin:0; padding:0; box-sizing:border-box; }
        body { font-family:Arial,sans-serif; background:#f5f5f5; color:#333; padding:0 20px 30px; }
        .contenedor { max-width:860px; margin:0 auto; }
        h1 { font-size:26px; border-bottom:2px solid #17a2b8; padding-bottom:8px; color:#222; margin-bottom:18px; }
        .carrito-box { background:#fff; border:1px solid #ddd; border-radius:5px; padding:20px; box-shadow:0 2px 4px rgba(0,0,0,0.08); }
        table { width:100%; border-collapse:collapse; }
        th,td { padding:11px 10px; border-bottom:1px solid #eee; text-align:left; font-size:14px; }
        th { background:#f8f9fa; color:#555; text-transform:uppercase; font-size:12px; }
        .txt-cant { width:65px; padding:5px; border:1px solid #ccc; border-radius:4px; text-align:center; font-size:14px; }
        .txt-cant.input-error { border-color:#dc3545; }
        .seccion-total { text-align:right; margin-top:18px; padding-top:15px; border-top:2px solid #eee; }
        .total-label { font-size:15px; color:#666; }
        .total-monto { font-size:26px; font-weight:bold; color:#28a745; margin-top:4px; }
        .botones-pie { margin-top:22px; display:flex; justify-content:space-between; gap:10px; flex-wrap:wrap; }
        .btn { padding:10px 20px; font-size:14px; border:none; border-radius:4px; cursor:pointer; color:#fff; font-weight:bold; }
        .btn-gris    { background:#6c757d; } .btn-gris:hover    { background:#5a6268; }
        .btn-rojo    { background:#dc3545; } .btn-rojo:hover    { background:#c82333; }
        .btn-verde   { background:#28a745; } .btn-verde:hover   { background:#218838; }
        .btn-actualizar { background:#ffc107; color:#333; } .btn-actualizar:hover { background:#e0a800; }
        .msg-err { color:#dc3545; font-weight:bold; font-size:14px; margin-bottom:12px; display:block; }
        .msg-ok  { color:#28a745; font-weight:bold; font-size:14px; margin-bottom:12px; display:block; }
        .carrito-vacio { text-align:center; padding:40px; color:#888; font-size:16px; }
        .stock-warn { font-size:11px; color:#dc3545; font-weight:bold; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <uc:Header ID="Header1" runat="server" />
    <div class="contenedor">
        <h1>🛒 Mi Carrito</h1>

        <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>

        <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="carrito-vacio">
            <p>Tu carrito está vacío.</p><br/>
            <asp:Button ID="btnVolverCat" runat="server" Text="← Ver Catálogo" CssClass="btn btn-gris" OnClick="btnVolverCatalogo_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlCarrito" runat="server" CssClass="carrito-box">
            <table>
                <thead>
                    <tr><th>Producto</th><th>Precio Unit.</th><th>Cantidad</th><th>Subtotal</th><th>Acción</th></tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repCarrito" runat="server" OnItemCommand="repCarrito_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><b><%# Eval("Nombre") %></b>
                                    <%# Convert.ToInt32(Eval("StockMax")) == 0
                                        ? "<br/><span class='stock-warn'>⚠ AGOTADO</span>"
                                        : (Convert.ToInt32(Eval("StockMax")) < 3
                                           ? "<br/><span class='stock-warn'>⚠ Stock bajo</span>"
                                           : "") %>
                                </td>
                                <td>Bs. <%# string.Format("{0:N2}", Eval("PrecioUnit")) %></td>
                                <td>
                                    <asp:TextBox ID="txtCant" runat="server"
                                        Text='<%# Eval("Cantidad") %>'
                                        CssClass="txt-cant" TextMode="Number"
                                        data-id='<%# Eval("IdCelular") %>'
                                        data-max='<%# Eval("StockMax") %>'></asp:TextBox>
                                </td>
                                <td>Bs. <%# string.Format("{0:N2}", Eval("Subtotal")) %></td>
                                <td>
                                    <asp:Button ID="btnEliminar" runat="server" Text="✕" CssClass="btn btn-rojo"
                                        CommandName="Eliminar" CommandArgument='<%# Eval("IdCelular") %>'
                                        OnClientClick="return confirm('¿Quitar este producto del carrito?');" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>

            <asp:Button ID="btnActualizar" runat="server" Text="🔄 Actualizar Cantidades"
                CssClass="btn btn-actualizar" OnClick="btnActualizar_Click" Style="margin-top:12px;" />

            <div class="seccion-total">
                <div class="total-label">Total a Pagar:</div>
                <div class="total-monto">Bs. <asp:Literal ID="litTotal" runat="server"></asp:Literal></div>
            </div>

            <div class="botones-pie">
                <asp:Button ID="btnVolver" runat="server" Text="← Seguir comprando"
                    CssClass="btn btn-gris" OnClick="btnVolverCatalogo_Click" />
                <asp:Button ID="btnVaciar" runat="server" Text="🗑 Vaciar carrito"
                    CssClass="btn btn-rojo"
                    OnClientClick="return confirm('¿Vaciar todo el carrito?');"
                    OnClick="btnVaciar_Click" />
                <asp:Button ID="btnConfirmar" runat="server" Text="✔ Confirmar Compra"
                    CssClass="btn btn-verde"
                    OnClientClick="return confirm('¿Confirmar la compra?');"
                    OnClick="btnConfirmar_Click" />
            </div>
        </asp:Panel>
    </div>
</form>
<script>
// Validación de cantidades en front antes de actualizar
document.addEventListener('DOMContentLoaded', function() {
    var inputs = document.querySelectorAll('.txt-cant');
    inputs.forEach(function(inp) {
        inp.addEventListener('input', function() {
            var max = parseInt(inp.getAttribute('data-max'));
            var val = parseInt(inp.value);
            inp.classList.remove('input-error');
            if (isNaN(val) || val < 1) {
                inp.value = 1;
            } else if (val > max) {
                inp.classList.add('input-error');
                inp.title = 'Stock máximo: ' + max;
            }
        });
    });
});
</script>
</body>
</html>