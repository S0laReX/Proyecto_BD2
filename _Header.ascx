<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="_Header.ascx.cs" Inherits="Proyecto_BDII._Header" %>

<div class="istore-header">
    <div class="istore-brand">
        <asp:Image ID="imgLogo" runat="server" ImageUrl="~/logo_istore.png" AlternateText="Logo iStore" CssClass="istore-logo" />
        <span class="istore-title">iStore: Tienda de celulares</span>
    </div>
    
    <nav class="istore-nav" id="navAdmin" runat="server" visible="false">
        <a href="PantallaADMIN.aspx#celulares">📱 Celulares</a>
        <a href="PantallaADMIN.aspx#categorias">🏷️ Categorías</a>
        <a href="PantallaADMIN.aspx#proveedores">🏭 Proveedores</a>
        <a href="PantallaADMIN.aspx#inventario">📦 Inventario</a>
        <a href="PantallaADMIN.aspx#ventas">📊 Ventas</a>
    </nav>
    
    <nav class="istore-nav" id="navUser" runat="server" visible="false">
        <a href="PantallaUSER.aspx">🛍️ Catálogo</a>
        <a href="CarritoCompleto.aspx">🛒 Mi Carrito <asp:Literal ID="litContadorCarrito" runat="server"></asp:Literal></a>
        <a href="PantallaUSER.aspx#favoritos">⭐ Favoritos</a>
        <a href="PantallaUSER.aspx#historial">📋 Mis Compras</a>
    </nav>
</div>

<style>
    .istore-header { display:flex; justify-content:space-between; align-items:center; background:#fff; border-bottom:2px solid #cc0000; padding:10px 20px; margin-bottom:20px; flex-wrap:wrap; gap:10px; }
    .istore-brand { display:flex; align-items:center; gap:10px; }
    .istore-logo { width:44px; height:44px; object-fit:contain; border-radius:8px; }
    .istore-title { font-size:20px; font-weight:bold; color:#cc0000; }
    .istore-nav { display:flex; gap:12px; flex-wrap:wrap; align-items:center; }
    .istore-nav a { text-decoration:none; color:#333; font-size:13px; font-weight:bold; padding:6px 10px; border-radius:4px; transition:background 0.2s; }
    .istore-nav a:hover { background:#f0f0f0; color:#cc0000; }
</style>