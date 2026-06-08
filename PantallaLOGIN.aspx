<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaLOGIN.aspx.cs" Inherits="Proyecto_BDII.PantallaLOGIN" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Iniciar Sesión - Tienda de Celulares</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f6f9; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; }
        .login-container { background: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); width: 350px; text-align: center; }
        h2 { color: #333; margin-bottom: 24px; }
        .form-group { margin-bottom: 15px; text-align: left; }
        label { display: block; font-size: 14px; margin-bottom: 5px; color: #666; }
        .form-control { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; }
        .btn-submit { width: 100%; background-color: #007bff; color: white; padding: 10px; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; font-weight: bold; margin-top: 10px; }
        .btn-submit:hover { background-color: #0056b3; }
        .error-msg { color: #dc3545; font-size: 14px; margin-top: 10px; display: block; }
        .register-link { margin-top: 20px; font-size: 14px; }
        .register-link a { color: #007bff; text-decoration: none; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <h2>Acceso al Sistema</h2>
            
            <div class="form-group">
                <label for="txtCorreo">Correo Electrónico:</label>
                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email" Placeholder="ejemplo@correo.com"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="txtPassword">Contraseña:</label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" Placeholder="******"></asp:TextBox>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btn-submit" OnClick="btnLogin_Click" />

            <asp:Label ID="lblMensaje" runat="server" CssClass="error-msg" Visible="false"></asp:Label>

            <div class="register-link">
                ¿No tienes cuenta? <a href="PantallaREGISTRO.aspx">Regístrate aquí</a>
            </div>
        </div>
    </form>
</body>
</html>