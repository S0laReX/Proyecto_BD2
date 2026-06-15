<%-- PantallaLOGIN.aspx --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaLOGIN.aspx.cs" Inherits="Proyecto_BDII.PantallaLOGIN" %>
<%@ Register Src="~/_Header.ascx" TagName="Header" TagPrefix="uc" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Iniciar Sesión - iStore</title>
    <script src="https://www.google.com/recaptcha/api.js" async defer></script>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; }
        .contenedor { max-width: 420px; margin: 30px auto; background: #fff; border: 1px solid #ddd; border-radius: 5px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        h2 { font-size: 22px; border-bottom: 2px solid #007bff; padding-bottom: 10px; margin-bottom: 20px; color: #222; }
        .campo { margin-bottom: 14px; }
        .campo label { display: block; font-size: 14px; margin-bottom: 4px; font-weight: bold; }
        .campo input { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 14px; }
        .campo input.input-error { border-color: #dc3545; }
        .btn-submit { padding: 9px 18px; background-color: #007bff; color: white; border: none; border-radius: 4px; font-size: 14px; font-weight: bold; cursor: pointer; width: 100%; }
        .btn-submit:hover { background-color: #0056b3; }
        .msg-error { color: red; font-size: 13px; display: block; margin-top: 10px; }
        .msg-ok { color: green; font-size: 13px; display: block; margin-top: 10px; }
        .link-alt { font-size: 13px; margin-top: 14px; text-align: center; }
        .campo-err { font-size: 12px; color: #dc3545; margin-top: 3px; display: none; }
        .captcha-container { margin-bottom: 15px; display: flex; flex-direction: column; align-items: center; }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <uc:Header ID="Header1" runat="server" />
    <div class="contenedor">
        <h2>Acceso al Sistema</h2>
        <div class="campo">
            <label>Correo Electrónico:</label>
            <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" placeholder="ejemplo@correo.com"></asp:TextBox>
            <span id="errCorreo" class="campo-err">Ingresa un correo válido.</span>
        </div>
        <div class="campo">
            <label>Contraseña:</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
            <span id="errPass" class="campo-err">La contraseña es obligatoria.</span>
        </div>
        
        <div class="captcha-container">
            <div class="g-recaptcha" data-sitekey="6LeRCSAtAAAAAKXsiIYdfoq5Tg-n_dhpf1mavzMX"></div>
            <span id="errCaptcha" class="campo-err">Por favor, verifica que no eres un robot.</span>
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btn-submit"
            OnClientClick="return validarLogin();" OnClick="btnLogin_Click" />
        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="msg-error"></asp:Label>
        
        <div class="link-alt">
            ¿No tienes cuenta? <a href="PantallaREGISTRO.aspx">Regístrate aquí</a>
        </div>
    </div>
</form>

<script>
    function validarLogin() {
        var ok = true;
        var correo = document.getElementById('<%= txtCorreo.ClientID %>');
    var pass   = document.getElementById('<%= txtPassword.ClientID %>');
        var reCorreo = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        document.getElementById('errCorreo').style.display = 'none';
        document.getElementById('errPass').style.display = 'none';
        document.getElementById('errCaptcha').style.display = 'none';
        correo.classList.remove('input-error');
        pass.classList.remove('input-error');

        if (!reCorreo.test(correo.value.trim())) {
            correo.classList.add('input-error');
            document.getElementById('errCorreo').style.display = 'block';
            ok = false;
        }
        if (pass.value.trim() === '') {
            pass.classList.add('input-error');
            document.getElementById('errPass').style.display = 'block';
            ok = false;
        }

        // Validación del CAPTCHA
        var response = grecaptcha.getResponse();
        if (response.length === 0) {
            document.getElementById('errCaptcha').style.display = 'block';
            ok = false;
        }

        return ok;
    }
</script>
</body>
</html>