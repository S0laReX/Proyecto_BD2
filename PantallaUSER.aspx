<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PantallaUSER.aspx.cs" Inherits="Proyecto_BDII.PantallaUSER" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="mb-4 text-center font-weight-bold text-dark">Catálogo de Dispositivos</h2>
        
        <div class="row">
            <asp:Repeater ID="repCelulares" runat="server">
                <ItemTemplate>
                    <div class="col-md-4 mb-4">
                        <div class="card h-100 shadow-sm border-0 animate-fade-in">
                            <img src="https://via.placeholder.com/300x200" class="card-img-top" alt="Celular">
                            
                            <div class="card-body d-flex flex-column">
                                <h5 class="card-title font-weight-bold text-dark mb-2">
                                    <%# Eval("marca") %> <%# Eval("modelo") %>
                                </h5>
                                <p class="card-text text-muted flex-grow-1" style="font-size: 0.9rem;">
                                    <%# Eval("descripcion") %>
                                </p>
                                
                                <div class="d-flex justify-content-between align-items-center mt-3">
                                    <span class="h4 text-primary font-weight-bold mb-0">
                                        Bs. <%# string.Format("{0:N2}", Eval("precio")) %>
                                    </span>
                                    <span class='badge <%# Convert.ToInt32(Eval("stock")) <= 3 ? "badge-danger" : "badge-light text-secondary" %>'>
                                        Stock: <%# Eval("stock") %>
                                    </span>
                                </div>
                            </div>
                            
                            <div class="card-footer bg-white border-top-0 pt-0 pb-3">
                                <div class="row gx-2">
                                    <div class="col-6">
                                        <asp:LinkButton ID="btnVerDetalle" runat="server" CssClass="btn btn-outline-secondary btn-block btn-sm" 
                                            CommandArgument='<%# Eval("id_celular") %>' OnClick="btnVerDetalle_Click">
                                            Detalles
                                        </asp:LinkButton>
                                    </div>
                                    <div class="col-6">
                                        <asp:LinkButton ID="btnComprar" runat="server" CssClass="btn btn-success btn-block btn-sm font-weight-bold" 
                                            CommandArgument='<%# Eval("id_celular") %>' OnClick="btnComprar_Click">
                                            🛒 Comprar
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
