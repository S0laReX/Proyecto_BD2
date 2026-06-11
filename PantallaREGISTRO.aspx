<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaREGISTRO.aspx.cs" Inherits="Proyecto_BDII.PantallaREGISTRO" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Registro - Tienda de Celulares</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; padding: 20px; }
        .contenedor { max-width: 460px; margin: 40px auto; background: #fff; border: 1px solid #ddd; border-radius: 5px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        h2 { font-size: 22px; border-bottom: 2px solid #007bff; padding-bottom: 10px; margin-bottom: 20px; color: #222; }
        .campo { margin-bottom: 13px; }
        .campo label { display: block; font-size: 14px; margin-bottom: 4px; font-weight: bold; }
        .campo input { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; }
        .hint { font-size: 12px; color: #888; margin-top: 3px; }
        .btn-submit { padding: 9px 18px; background-color: #28a745; color: white; border: none; border-radius: 4px; font-size: 14px; font-weight: bold; cursor: pointer; }
        .btn-submit:hover { background-color: #218838; }
        .msg-error { color: red; font-size: 13px; display: block; margin-top: 10px; }
        .msg-ok { color: green; font-size: 13px; display: block; margin-top: 10px; }
        .link-alt { font-size: 13px; margin-top: 14px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor">
            <h2>Crear Cuenta</h2>
            <div class="campo">
                <label>Nombre Completo:</label>
                <asp:TextBox ID="txtNombre" runat="server" placeholder="Juan Pérez"></asp:TextBox>
            </div>
            <div class="campo">
                <label>Correo Electrónico:</label>
                <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" placeholder="ejemplo@correo.com"></asp:TextBox>
            </div>
            <div class="campo">
                <label>C.I. (Carnet de Identidad):</label>
                <asp:TextBox ID="txtCI" runat="server" placeholder="12345678"></asp:TextBox>
            </div>
            <div class="campo">
                <label>Teléfono:</label>
                <asp:TextBox ID="txtTelefono" runat="server" placeholder="+59170000000"></asp:TextBox>
            </div>
            <div class="campo">
                <label>Dirección:</label>
                <asp:TextBox ID="txtDireccion" runat="server" placeholder="Calle, Ciudad"></asp:TextBox>
            </div>
            <div class="campo">
                <label>Contraseña:</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                <span class="hint">Mínimo 8 caracteres, 1 mayúscula y 1 número.</span>
            </div>
            <div class="campo">
                <label>Confirmar Contraseña:</label>
                <asp:TextBox ID="txtPassword2" runat="server" TextMode="Password" placeholder="Repite la contraseña"></asp:TextBox>
            </div>
            <asp:Button ID="btnRegistrar" runat="server" Text="Registrarse" CssClass="btn-submit" OnClick="btnRegistrar_Click" />
            <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>
            <div class="link-alt">
                ¿Ya tienes cuenta? <a href="PantallaLOGIN.aspx">Inicia sesión</a>
            </div>
        </div>
    </form>
</body>
</html>
