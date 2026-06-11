<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Carrito.aspx.cs" Inherits="Proyecto_BDII.Carrito" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Carrito de Compras</title>
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
            max-width: 800px;
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

        .carrito-wrapper {
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 20px;
            background-color: #fff;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .tabla-carrito {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }

        .tabla-carrito th, .tabla-carrito td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        .tabla-carrito th {
            background-color: #f8f9fa;
            color: #555;
            font-size: 14px;
            text-transform: uppercase;
        }

        .tabla-carrito td {
            font-size: 15px;
        }

        .txt-cantidad {
            width: 60px;
            padding: 5px;
            border: 1px solid #ccc;
            border-radius: 4px;
            text-align: center;
        }

        .seccion-total {
            text-align: right;
            margin-top: 15px;
            padding-top: 15px;
            border-top: 2px solid #eee;
        }

        .total-titulo {
            font-size: 16px;
            color: #666;
        }

        .total-monto {
            font-size: 24px;
            color: #28a745;
            font-weight: bold;
            margin-top: 5px;
        }

        .botones-orden {
            margin-top: 25px;
            display: flex;
            justify-content: space-between;
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

        .btn-secundario {
            background-color: #6c757d;
        }

        .btn-secundario:hover {
            background-color: #5a6268;
        }

        .btn-exito {
            background-color: #28a745;
        }

        .btn-exito:hover {
            background-color: #218838;
        }

        .mensaje-alerta {
            font-size: 15px;
            margin-bottom: 15px;
            display: block;
            font-weight: bold;
        }

        /* Clase para dar margen al nuevo botón de regreso post-compra */
        .btn-post-compra {
            margin-top: 10px;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor">
            
            <h1>Resumen de tu Pedido</h1>

            <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje-alerta"></asp:Label>

            <asp:Button ID="btnIrCatalogo" runat="server" Text="⬅ Volver al Catálogo" CssClass="btn-secundario btn-post-compra" Visible="false" OnClick="btnIrCatalogo_Click" />

            <asp:Panel ID="pnlCarrito" runat="server" CssClass="carrito-wrapper" Visible="false">
                
                <table class="tabla-carrito">
                    <thead>
                        <tr>
                            <th>Producto</th>
                            <th>Precio Unitario</th>
                            <th>Cantidad</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <b><asp:Literal ID="litProducto" runat="server"></asp:Literal></b>
                            </td>
                            <td>
                                Bs. <asp:Literal ID="litPrecioUnitario" runat="server"></asp:Literal>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCantidad" runat="server" TextMode="Number" CssClass="txt-cantidad" 
                                    AutoPostBack="true" OnTextChanged="txtCantidad_TextChanged" min="1"></asp:TextBox>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <div class="seccion-total">
                    <div class="total-titulo">Total a Pagar:</div>
                    <div class="total-monto">
                        Bs. <asp:Literal ID="litTotal" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="botones-orden">
                    <asp:Button ID="btnRegresar" runat="server" Text="⬅ Cancelar y Volver" CssClass="btn-secundario" OnClick="btnRegresar_Click" />
                    <asp:Button ID="btnConfirmar" runat="server" Text="✔ Confirmar Compra" CssClass="btn-exito" OnClick="btnConfirmar_Click" />
                </div>

            </asp:Panel>

        </div>
    </form>
</body>
</html>