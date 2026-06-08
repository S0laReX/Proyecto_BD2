<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PantallaADMIN.aspx.cs" Inherits="Proyecto_BDII.PantallaADMIN" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <section>
            <div style="text-align:center">
                <asp:Label ID="Label8" runat="server" Text="PANEL ADMIN." Font-Bold="true" Font-Size="XX-Large" ForeColor="#cc0000"></asp:Label>
                <br /><br />
            </div>
        </section>

        <section>
            <div style="text-align:center">
                <asp:Label ID="Label7" runat="server" Text="LISTA DE CELULARES" Font-Bold="true" Font-Size="Large" Font-Underline="true" ForeColor="#663300"></asp:Label>
                <br /><br />
                <asp:GridView ID="GridView1" runat="server" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowDeleting="GridView1_RowDeleting" AutoGenerateColumns="False" DataKeyNames="id_celular" HorizontalAlign="Center">
                    <Columns>
                        <asp:BoundField DataField="id_celular" HeaderText="ID" ReadOnly="True"/>
                        <asp:BoundField DataField="marca" HeaderText="MARCA"/>
                        <asp:BoundField DataField="modelo" HeaderText="MODELO"/>
                        <asp:BoundField DataField="descripcion" HeaderText="DESCRIPCION"/>

                        <%-- Categoria: TemplateField para mostrar nombre en vista normal
                             y DropDownList al editar --%>
                        <asp:TemplateField HeaderText="CATEGORIA">
                            <ItemTemplate>
                                <%-- Muestra el nombre de la categoria (viene del JOIN) --%>
                                <asp:Label ID="LblCategoria" runat="server" Text='<%# Eval("nombre_categoria") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <%-- Al editar muestra un DropDownList con todas las categorias --%>
                                <asp:DropDownList ID="DdlCategoriaEdit" runat="server" DataTextField="nombre_categoria" DataValueField="id_categoria">
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="precio" HeaderText="PRECIO"/>
                        <asp:BoundField DataField="stock" HeaderText="CANTIDAD STOCK"/>

                        <%-- Imagen: TemplateField para mostrar la primera imagen del celular --%>
                        <asp:TemplateField HeaderText="IMAGEN">
                            <ItemTemplate>
                                <asp:Image ID="ImgCelular" runat="server"
                                    ImageUrl='<%# "~/" + Eval("url_imagen") %>'
                                    Width="80px" Height="80px"
                                    Visible='<%# Eval("url_imagen") != DBNull.Value && Eval("url_imagen").ToString() != "" %>'/>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <%-- Al editar se puede subir una nueva imagen --%>
                                <asp:FileUpload ID="FuImagenEdit" runat="server" />
                                <br />
                                <asp:Image ID="ImgCelularEdit" runat="server"
                                    ImageUrl='<%# "~/" + Eval("url_imagen") %>'
                                    Width="60px" Height="60px"
                                    Visible='<%# Eval("url_imagen") != DBNull.Value && Eval("url_imagen").ToString() != "" %>'/>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True"/>
                    </Columns>
                </asp:GridView>
            </div>
        </section>

        <hr />

        <section>
            <br />
            <div style="text-align:center">
                <asp:Label ID="Label9" runat="server" Text="CREAR/REGISTRAR NUEVO CELULAR" Font-Bold="true" Font-Size="Large" Font-Underline="true" ForeColor="#663300"></asp:Label>
            </div>
            <div style="margin-left:41%">
                <br /><br />
                <asp:Label ID="Label1" runat="server" Text="Marca:"></asp:Label>
                <asp:TextBox ID="TxtMarca" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="Label2" runat="server" Text="Modelo:"></asp:Label>
                <asp:TextBox ID="TxtModelo" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="Label3" runat="server" Text="Descripción:"></asp:Label>
                <asp:TextBox ID="TxtDescripcion" runat="server"></asp:TextBox>
                <br />

                <asp:Label ID="Label4" runat="server" Text="Categoría:"></asp:Label>
                <asp:DropDownList ID="DdlCategoria" runat="server" DataTextField="nombre_categoria" DataValueField="id_categoria"></asp:DropDownList>

                <br />
                <asp:Label ID="Label5" runat="server" Text="Precio:"></asp:Label>
                <asp:TextBox ID="TxtPrecio" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="Label6" runat="server" Text="Cantidad en stock:"></asp:Label>
                <asp:TextBox ID="TxtCantidadStock" runat="server"></asp:TextBox>
                <br />

                <asp:Label ID="LblImagen" runat="server" Text="Imagen:"></asp:Label>
                <asp:FileUpload ID="FuImagen" runat="server" />

                <br /><br />
            </div>
            <div>
                <asp:Button ID="BtnCrearCel" runat="server" Text="CREAR" OnClick="BtnCrearCel_Click" BorderStyle="Double" Font-Bold="true" Font-Size="Large" BackColor="#00ff00" style="display:block; margin: 0 auto;" />
            </div>
        </section>

        <br />
        <hr />
        <br />

        <section>
            <div style="text-align:center">
                <asp:Label ID="Label10" runat="server" Text="LISTA DE CATEGORÍAS" Font-Bold="true" Font-Size="Large" Font-Underline="true" ForeColor="#663300"></asp:Label>
                <br /><br />
                <asp:GridView ID="GridView2" runat="server" OnRowCancelingEdit="GridView2_RowCancelingEdit" OnRowEditing="GridView2_RowEditing" OnRowUpdating="GridView2_RowUpdating" OnRowDeleting="GridView2_RowDeleting" AutoGenerateColumns="False" DataKeyNames="id_categoria" HorizontalAlign="Center">
                    <Columns>
                        <asp:BoundField DataField="id_categoria" HeaderText="ID" ReadOnly="True"/>
                        <asp:BoundField DataField="nombre_categoria" HeaderText="NOMBRE"/>
                        <asp:BoundField DataField="descripcion" HeaderText="DESCRIPCION"/>
                        <asp:BoundField DataField="icono" HeaderText="ICONO"/>
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True"/>
                    </Columns>
                </asp:GridView>
            </div>
        </section>

        <hr />

        <section>
            <br />
            <div style="text-align:center">
                <asp:Label ID="Label11" runat="server" Text="CREAR/REGISTRAR NUEVA CATEGORIA" Font-Bold="true" Font-Size="Large" Font-Underline="true" ForeColor="#663300"></asp:Label>
            </div>
            <div style="margin-left:41%">
                <br /><br />
                <asp:Label ID="Label12" runat="server" Text="Nombre:"></asp:Label>
                <asp:TextBox ID="TxtCatNombre" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="Label14" runat="server" Text="Descripción:"></asp:Label>
                <asp:TextBox ID="TxtCatDescripcion" runat="server"></asp:TextBox>
                <br />
                <asp:Label ID="Label15" runat="server" Text="Icono (emoji):"></asp:Label>
                <asp:TextBox ID="TxtCatIcono" runat="server"></asp:TextBox>
                <br /><br />
            </div>
            <div>
                <asp:Button ID="BtnCrearCateg" runat="server" Text="CREAR" OnClick="BtnCrearCateg_Click"
                    BorderStyle="Double" Font-Bold="true" Font-Size="Large" BackColor="#00ff00"
                    style="display:block; margin: 0 auto;" />
            </div>
        </section>

    </form>
</body>
</html>
