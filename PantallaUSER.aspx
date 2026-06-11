<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaUSER.aspx.cs" Inherits="Proyecto_BDII.PantallaUSER" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Catálogo de Celulares</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            color: #333;
            padding: 20px;
        }

        h1, h2 {
            color: #222;
            margin-bottom: 10px;
            margin-top: 20px;
        }

        h1 {
            font-size: 28px;
            border-bottom: 2px solid #007bff;
            padding-bottom: 10px;
        }

        h2 {
            font-size: 22px;
        }

        hr {
            border: none;
            border-top: 1px solid #ddd;
            margin: 15px 0;
        }

        .producto-card {
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            margin-bottom: 15px;
            margin-right: 15px;
            width: 320px;
            display: inline-block;
            vertical-align: top;
            background-color: #fff;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            transition: box-shadow 0.3s ease;
        }

        .producto-card:hover {
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }

        
        .producto-img {
            width: 100%;
            height: 180px;
            object-fit: contain;
            margin-bottom: 12px;
            border-bottom: 1px solid #eee;
            padding-bottom: 8px;
        }

        .producto-card h3 {
            font-size: 18px;
            margin-bottom: 8px;
            color: #222;
        }

        .producto-card p {
            font-size: 14px;
            margin-bottom: 8px;
            line-height: 1.5;
        }

        .favorito-card {
            border: 1px solid #FFD700;
            border-radius: 5px;
            padding: 15px;
            margin-bottom: 15px;
            margin-right: 15px;
            width: 320px;
            display: inline-block;
            vertical-align: top;
            background-color: #FFFDF0;
            box-shadow: 0 2px 4px rgba(255,215,0,0.2);
            transition: box-shadow 0.3s ease;
        }

        .favorito-card:hover {
            box-shadow: 0 4px 8px rgba(255,215,0,0.3);
        }

        .favorito-card h3 {
            font-size: 18px;
            margin-bottom: 8px;
            color: #FF9800;
        }

        .botones {
            margin-top: 12px;
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        button, asp\\:Button, .btn-base {
            padding: 8px 12px;
            font-size: 13px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            color: white;
            font-weight: bold;
            transition: background-color 0.3s ease;
        }

        
        .btn-detalle {
            background-color: #007bff;
        }
        .btn-detalle:hover {
            background-color: #0056b3;
        }

        .btn-comprar {
            background-color: #28a745;
        }
        .btn-comprar:hover {
            background-color: #218838;
        }

        .btn-favorito {
            background-color: #ff9800;
        }
        .btn-favorito:hover {
            background-color: #e68a00;
        }

        .btn-peligro {
            background-color: #dc3545;
        }

        .btn-peligro:hover {
            background-color: #c82333;
        }

        .mensaje-favoritos {
            font-size: 14px;
            margin-bottom: 15px;
            padding: 10px;
            border-radius: 4px;
        }

        .clearfix {
            clear: both;
        }

        .contenedor {
            max-width: 1200px;
            margin: 0 auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor">
            
            <h1>Catálogo de Dispositivos</h1>
            <hr />

            <asp:Repeater ID="repCelulares" runat="server">
                <ItemTemplate>
                    <div class="producto-card">
                        
                        <img src='<%# string.IsNullOrEmpty(Eval("url_imagen").ToString()) ? "https://via.placeholder.com/300x180?text=Sin+Imagen" : Eval("url_imagen") %>' class="producto-img" alt="Celular" />

                        <h3><%# Eval("marca") %> <%# Eval("modelo") %></h3>
                        <p><%# Eval("descripcion") %></p>
                        <p>
                            <b>Precio:</b> Bs. <%# Eval("precio") %><br />
                            <b>Stock:</b> <%# Eval("stock") %>
                        </p>
                        
                        <div class="botones">
                            <asp:Button ID="btnVerDetalle" runat="server" Text="Ver Detalles" CssClass="btn-base btn-detalle" CommandArgument='<%# Eval("id_celular") %>' OnClick="btnVerDetalle_Click" />
                            <asp:Button ID="btnComprar" runat="server" Text="Comprar" CssClass="btn-base btn-comprar" CommandArgument='<%# Eval("id_celular") %>' OnClick="btnComprar_Click" />
                            <asp:Button ID="btnFavorito" runat="server" Text="⭐ Favorito" CssClass="btn-base btn-favorito" CommandArgument='<%# Eval("id_celular") %>' OnClick="btnFavorito_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="clearfix"></div>
            
            <h2>Mis Productos Favoritos</h2>
            <hr />
            <asp:Label ID="lblMensajeFavoritos" runat="server" CssClass="mensaje-favoritos"></asp:Label>
            
            <asp:Repeater ID="repFavoritos" runat="server">
                <ItemTemplate>
                    <div class="favorito-card">
                        <h3><%# Eval("marca") %> <%# Eval("modelo") %></h3>
                        <p><b>Precio:</b> Bs. <%# Eval("precio") %></p>
                        
                        <div class="botones">
                            <asp:Button ID="btnEliminarFavorito" runat="server" Text="❌ Quitar" CssClass="btn-base btn-peligro" CommandArgument='<%# Eval("id_celular") %>' OnClick="btnEliminarFavorito_Click" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </form>
</body>
</html>