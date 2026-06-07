<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaLOGIN.aspx.cs" Inherits="Proyecto_BDII.PantallaLOGIN" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <section style="display: flex; justify-content: center;">
            <div>
                <asp:Label ID="Label3" runat="server" Text="INICIO DE SESIÓN" style ="margin-left: 170px"></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" Text="EMAIL" style="margin-left: 100px"></asp:Label>
                <asp:TextBox ID="txtEmail" runat="server" style="margin-left: 100px"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Text="CONTRASEÑA" style="margin-left: 100px"></asp:Label>
                <asp:TextBox ID="txtContraseña" runat="server" style="margin-left: 46px"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="lblMensaje" runat="server" Text="" style="margin-left: 150px"></asp:Label>
                <br />
                <asp:Button ID="btnIngresar" runat="server" Text="INGRESAR" style ="margin-left:123px" Width="112px" OnClick="btnIngresar_Click" />
                <asp:Button ID="btnRegistro" runat="server" Text="REGISTRO" style ="margin-left:68px" Width="112px"/>
    
            </div>
        </section>
        
    </form>
</body>
</html>
