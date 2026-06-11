<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaREGISTRO.aspx.cs" Inherits="Proyecto_BDII.PantallaREGISTRO" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registro de Usuario - Tienda de Celulares</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Crear Cuenta</h2>

            <div>
                <label for="txtNombre">Nombre Completo:</label>
                <asp:TextBox ID="txtNombre" runat="server" Placeholder="Juan Pérez" style="margin-left:27px"></asp:TextBox>
            </div>
            <br />
            <div>
                <label for="txtCorreo">Correo Electrónico:</label>
                <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" Placeholder="ejemplo@correo.com" style="margin-left:20px"></asp:TextBox>
            </div>
            <br />
            <div>
                <label for="txtPassword">Contraseña:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Mínimo 6 caracteres" style="margin-left:85px"></asp:TextBox>
            </div>
            <br />
            <div>
                <label for="txtPassword2">Confirmar Contraseña:</label>
                <asp:TextBox ID="TextBox1" runat="server" TextMode="Password" Placeholder="Mínimo 6 caracteres"></asp:TextBox><br />
            </div>
            <br />
            <asp:Button ID="btnRegistrar" runat="server" Text="Registrarse" CssClass="btn-submit" OnClick="btnRegistrar_Click" />
            <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>
            <br /><br />
            <div>
                ¿Ya tienes una cuenta? <a href="PantallaLOGIN.aspx">Inicia sesión</a>
            </div>
        </div>
    </form>
</body>
</html>