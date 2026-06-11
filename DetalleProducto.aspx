<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetalleProducto.aspx.cs" Inherits="Proyecto_BDII.DetalleProducto" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Detalle del Celular</title>
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

        .contenedor {
            max-width: 900px;
            margin: 0 auto;
        }

        h1 {
            font-size: 28px;
            border-bottom: 2px solid #007bff;
            padding-bottom: 10px;
            margin-bottom: 20px;
            margin-top: 20px;
            color: #222;
        }

        
        .detalle-wrapper {
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 25px;
            background-color: #fff;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            display: flex;
            gap: 30px;
            flex-wrap: wrap;
        }

        
        .seccion-galeria {
            flex: 1;
            min-width: 300px;
            text-align: center;
        }

        .imagen-principal {
            max-width: 100%;
            height: auto;
            max-height: 350px;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 5px;
            background-color: #fff;
        }

        .lista-miniaturas {
            margin-top: 15px;
            display: flex;
            gap: 8px;
            justify-content: center;
            flex-wrap: wrap;
        }

        .img-miniatura {
            width: 60px;
            height: 60px;
            object-fit: cover;
            border: 1px solid #ccc;
            border-radius: 4px;
            padding: 2px;
            background-color: #fff;
        }

        
        .seccion-info {
            flex: 1.2;
            min-width: 320px;
            display: flex;
            flex-direction: column;
        }

        .categoria-badge {
            display: inline-block;
            background-color: #e2f0ff;
            color: #007bff;
            padding: 5px 10px;
            border-radius: 4px;
            font-size: 13px;
            font-weight: bold;
            margin-bottom: 15px;
            align-self: flex-start;
        }

        .modelo-titulo {
            font-size: 26px;
            color: #222;
            margin-bottom: 10px;
        }

        .descripcion-texto {
            font-size: 15px;
            line-height: 1.6;
            color: #666;
            margin-bottom: 20px;
            border-left: 3px solid #007bff;
            padding-left: 12px;
        }

        .precio-contenedor {
            font-size: 28px;
            color: #28a745;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .stock-texto {
            font-size: 14px;
            color: #555;
            margin-bottom: 25px;
        }

        
        .botones-accion {
            margin-top: auto;
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
        }

        button, asp\\:Button {
            padding: 10px 20px;
            font-size: 14px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            color: white;
            font-weight: bold;
            transition: background-color 0.3s ease;
        }

        .btn-volver {
            background-color: #6c757d;
        }

        .btn-volver:hover {
            background-color: #5a6268;
        }

        .btn-comprar {
            background-color: #28a745;
        }

        .btn-comprar:hover {
            background-color: #218838;
        }

        .text-error {
            color: #dc3545;
            font-weight: bold;
            margin-bottom: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor">
            
            <h1>Detalles del Dispositivo</h1>

            <asp:Label ID="lblMensaje" runat="server" CssClass="text-error"></asp:Label>

            <asp:Panel ID="pnlDetalle" runat="server" CssClass="detalle-wrapper" Visible="false">
                
                <div class="seccion-galeria">
                    <asp:Image ID="imgPrincipal" runat="server" CssClass="imagen-principal" ImageUrl="https://via.placeholder.com/350x350?text=Sin+Imagen" />
                    
                    <div class="lista-miniaturas">
                        <asp:Repeater ID="repImagenes" runat="server">
                            <ItemTemplate>
                                <img src='<%# Eval("url_imagen") %>' class="img-miniatura" alt="Miniatura" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="seccion-info">
                    <span class="categoria-badge">
                        <asp:Literal ID="litCategoria" runat="server"></asp:Literal>
                    </span>
                    
                    <h2 class="modelo-titulo">
                        <asp:Literal ID="litMarcaModelo" runat="server"></asp:Literal>
                    </h2>
                    
                    <p class="descripcion-texto">
                        <asp:Literal ID="litDescripcion" runat="server"></asp:Literal>
                    </p>
                    
                    <div class="precio-contenedor">
                        Bs. <asp:Literal ID="litPrecio" runat="server"></asp:Literal>
                    </div>
                    
                    <div class="stock-texto">
                        <b>Disponibilidad inmediata:</b> <asp:Literal ID="litStock" runat="server"></asp:Literal> unidades en tienda.
                    </div>

                    <div class="botones-accion">
                        <asp:Button ID="btnVolver" runat="server" Text="⬅ Volver al Catálogo" CssClass="btn-volver" OnClick="btnVolver_Click" />
                        <asp:Button ID="btnComprar" runat="server" Text="🛒 Adquirir Ahora" CssClass="btn-comprar" OnClick="btnComprar_Click" />
                    </div>
                </div>

            </asp:Panel>

        </div>
    </form>
</body>
</html>