<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaLOGIN.aspx.cs" Inherits="Proyecto_BDII.PantallaLOGIN" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Iniciar Sesión - Tienda de Celulares</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; padding: 20px; }
        .contenedor { max-width: 420px; margin: 60px auto; background: #fff; border: 1px solid #ddd; border-radius: 5px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        h2 { font-size: 22px; border-bottom: 2px solid #007bff; padding-bottom: 10px; margin-bottom: 20px; color: #222; }
        .campo { margin-bottom: 14px; }
        .campo label { display: block; font-size: 14px; margin-bottom: 4px; font-weight: bold; }
        .campo input, .campo asp\:TextBox { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; }
        .btn-submit { padding: 9px 18px; background-color: #007bff; color: white; border: none; border-radius: 4px; font-size: 14px; font-weight: bold; cursor: pointer; }
        .btn-submit:hover { background-color: #0056b3; }
        .msg-error { color: red; font-size: 13px; display: block; margin-top: 10px; }
        .msg-ok { color: green; font-size: 13px; display: block; margin-top: 10px; }
        .link-alt { font-size: 13px; margin-top: 14px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor">
            <h2>Acceso al Sistema</h2>
            <div class="campo">
                <label>Correo Electrónico:</label>
                <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" placeholder="ejemplo@correo.com"></asp:TextBox>
            </div>
            <div class="campo">
                <label>Contraseña:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
            </div>
            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btn-submit" OnClick="btnLogin_Click" />
            <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="msg-error"></asp:Label>
            <div class="link-alt">
                ¿No tienes cuenta? <a href="PantallaREGISTRO.aspx">Regístrate aquí</a>
            </div>
        </div>
    </form>
</body>
</html>
