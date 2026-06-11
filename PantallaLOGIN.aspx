<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaLOGIN.aspx.cs" Inherits="Proyecto_BDII.PantallaLOGIN" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Iniciar Sesión - Tienda de Celulares</title>
</head>
<body>
    <form id="form1" runat="server">
            <div>
                 <div>
                        <h2>Acceso al Sistema</h2>
            
                        <div>
                            <label for="txtCorreo">Correo Electrónico:</label>
                            <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" Placeholder="ejemplo@correo.com"></asp:TextBox>
                        </div>
                        <br />
                        <div>
                            <label for="txtPassword">Contraseña:</label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="******" style="margin-left:65px"></asp:TextBox>
                        </div>
                     <br />
                        <asp:Button ID="btnLogin" runat="server" Text="Ingresar" OnClick="btnLogin_Click" />

                        <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>
                     <br /><br />
                        <div>
                            ¿No tienes cuenta? <a href="PantallaREGISTRO.aspx">Regístrate aquí</a>
                        </div>
                </div>
            </div> 
    </form>
</body>
</html>