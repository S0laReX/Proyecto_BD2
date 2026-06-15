<%-- PantallaREGISTRO.aspx --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaREGISTRO.aspx.cs" Inherits="Proyecto_BDII.PantallaREGISTRO" %>
<%@ Register Src="~/_Header.ascx" TagName="Header" TagPrefix="uc" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Registro - iStore</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; }
        .contenedor { max-width: 460px; margin: 20px auto; background: #fff; border: 1px solid #ddd; border-radius: 5px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        h2 { font-size: 22px; border-bottom: 2px solid #007bff; padding-bottom: 10px; margin-bottom: 20px; color: #222; }
        .campo { margin-bottom: 13px; }
        .campo label { display: block; font-size: 14px; margin-bottom: 4px; font-weight: bold; }
        .campo input { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; }
        .campo input.input-error { border-color: #dc3545; }
        .campo-err { font-size: 12px; color: #dc3545; margin-top: 3px; display: none; }
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
    <uc:Header ID="Header1" runat="server" />
    <div class="contenedor">
        <h2>Crear Cuenta</h2>
        <div class="campo">
            <label>Nombre Completo: *</label>
            <asp:TextBox ID="txtNombre" runat="server" placeholder="Juan Pérez"></asp:TextBox>
            <span id="errNombre" class="campo-err">El nombre es obligatorio.</span>
        </div>
        <div class="campo">
            <label>Correo Electrónico: *</label>
            <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" placeholder="ejemplo@correo.com"></asp:TextBox>
            <span id="errCorreo" class="campo-err">Ingresa un correo válido.</span>
        </div>
        <div class="campo">
            <label>C.I. (Carnet de Identidad): *</label>
            <asp:TextBox ID="txtCI" runat="server" placeholder="12345678"></asp:TextBox>
            <span id="errCI" class="campo-err">El C.I. es obligatorio y solo números.</span>
        </div>
        <div class="campo">
            <label>Teléfono:</label>
            <asp:TextBox ID="txtTelefono" runat="server" placeholder="+59170000000"></asp:TextBox>
            <span id="errTel" class="campo-err">Teléfono inválido.</span>
        </div>
        <div class="campo">
            <label>Dirección:</label>
            <asp:TextBox ID="txtDireccion" runat="server" placeholder="Calle, Ciudad"></asp:TextBox>
        </div>
        <div class="campo">
            <label>Contraseña: *</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
            <span class="hint">Mínimo 8 caracteres, 1 mayúscula y 1 número.</span>
            <span id="errPass" class="campo-err">Mínimo 8 caracteres, 1 mayúscula y 1 número.</span>
        </div>
        <div class="campo">
            <label>Confirmar Contraseña: *</label>
            <asp:TextBox ID="txtPassword2" runat="server" TextMode="Password" placeholder="Repite la contraseña"></asp:TextBox>
            <span id="errPass2" class="campo-err">Las contraseñas no coinciden.</span>
        </div>
        <asp:Button ID="btnRegistrar" runat="server" Text="Registrarse" CssClass="btn-submit"
            OnClientClick="return validarRegistro();" OnClick="btnRegistrar_Click" />
        <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>
        <div class="link-alt">
            ¿Ya tienes cuenta? <a href="PantallaLOGIN.aspx">Inicia sesión</a>
        </div>
    </div>
</form>
<script>
function validarRegistro() {
    var ok = true;
    var ids = ['errNombre','errCorreo','errCI','errTel','errPass','errPass2'];
    ids.forEach(function(id){ document.getElementById(id).style.display='none'; });

    var nombre = document.getElementById('<%= txtNombre.ClientID %>').value.trim();
    var correo = document.getElementById('<%= txtCorreo.ClientID %>').value.trim();
    var ci     = document.getElementById('<%= txtCI.ClientID %>').value.trim();
    var tel    = document.getElementById('<%= txtTelefono.ClientID %>').value.trim();
    var pass   = document.getElementById('<%= txtPassword.ClientID %>').value;
    var pass2  = document.getElementById('<%= txtPassword2.ClientID %>').value;
    var reCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    var reCI     = /^\d+$/;
    var reTel    = /^[\+\d\s\-\(\)]{7,15}$/;
    var rePass   = /^(?=.*[A-Z])(?=.*\d).{8,}$/;

    if (!nombre) { document.getElementById('errNombre').style.display='block'; ok=false; }
    if (!reCorreo.test(correo)) { document.getElementById('errCorreo').style.display='block'; ok=false; }
    if (!reCI.test(ci) || ci.length < 5) { document.getElementById('errCI').style.display='block'; ok=false; }
    if (tel && !reTel.test(tel)) { document.getElementById('errTel').style.display='block'; ok=false; }
    if (!rePass.test(pass)) { document.getElementById('errPass').style.display='block'; ok=false; }
    if (pass !== pass2) { document.getElementById('errPass2').style.display='block'; ok=false; }
    return ok;
}
</script>
</body>
</html>