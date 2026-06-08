<%@ Page Language="C#"  AutoEventWireup="true" CodeBehind="PantallaUSER.aspx.cs" Inherits="Proyecto_BDII.PantallaUSER" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Catálogo de Celulares</title>
</head>
<body>
    <form id="form1" runat="server">
        
        <h1>Catálogo de Dispositivos</h1>
        <hr />

        <asp:Repeater ID="reCelulares" runat="server">
            <ItemTemplate>
                
                <div style="border: 1px solid #000; padding: 10px; margin-bottom: 15px; width: 350px;">
                    
                    <h3><%# Eval("marca") %> <%# Eval("modelo") %></h3>
                    <p><%# Eval("descripcion") %></p>
                    
                    <p>
                        <b>Precio:</b> Bs. <%# Eval("precio") %><br />
                        <b>Stock disponible:</b> <%# Eval("stock") %>
                    </p>
                    
                    <asp:Button ID="btnVerDetalle" runat="server" Text="Ver Detalles" 
                        CommandArgument='<%# Eval("id_celular") %>' OnClick="btnVerDetalle_Click" />
                        
                    <asp:Button ID="btnComprar" runat="server" Text="Comprar" 
                        CommandArgument='<%# Eval("id_celular") %>' OnClick="btnComprar_Click" />
                        
                </div>
                
            </ItemTemplate>
        </asp:Repeater>

    </form>
</body>
</html>
