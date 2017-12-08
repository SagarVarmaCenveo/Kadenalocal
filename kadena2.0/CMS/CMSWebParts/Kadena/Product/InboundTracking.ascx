<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Kadena_Product_InboundTracking" CodeBehind="~/CMSWebParts/Kadena/Product/InboundTracking.ascx.cs" %>

<div class="custom_section">
    <div class="custom_block">
        <div class="custom_select">
            <asp:DropDownList ID="ddlCampaign" runat="server" OnSelectedIndexChanged="ddlCampaign_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
            <asp:DropDownList ID="ddlProgram" runat="server" OnSelectedIndexChanged="ddlProgram_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="custom_btns">
            <button type="button" class="btn-action login__login-button btn--no-shadow">Refresh</button>
            <asp:Button ID="btnExport" runat="server" CssClass="btn-action login__login-button btn--no-shadow" Text="Export" />
        </div>
    </div>
    <div class="Inbound_track">
        <asp:GridView ID="gdvInboundProducts" runat="server" AutoGenerateColumns="false" OnRowEditing="inboundProducts_RowEditing" OnRowUpdating="inboundProducts_RowUpdating" OnRowCancelingEdit="gdvInboundProducts_RowCancelingEdit" class="show-table show-table-right">
            <Columns>
                <asp:TemplateField HeaderText="SKU">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblSkuNumber" Text='<%#Eval("SKUNumber") %>'></asp:Label>
                        <asp:HiddenField runat="server" ID="hfSKUID" Value='<%#Eval("SKUID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ITEM">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblItem" Text='<%#Eval("SKUName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="QTY ORDERED">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblQtyOrdered" Text='<%#Eval("QtyOrdered") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="DEMAND GOAL">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDemandGoal" Text='<%#Eval("DemandGoal") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtDemandGoal" Text='<%#Eval("DemandGoal") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="QTY RECEIVED">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblQtyReceived" Text='<%#Eval("QtyReceived") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtQtyReceived" Text='<%#Eval("QtyReceived") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="QTY PRODUCED">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblQtyProduced" Text='<%#Eval("QtyProduced") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtQtyProduced" Text='<%#Eval("QtyProduced") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="OVERAGE">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblOverage" Text='<%#Eval("Overage") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtOverage" ReadOnly="true" Text='<%#Eval("Overage") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="VENDOR">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblVendor" Text='<%#Eval("Vendor") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtVendor" Text='<%#Eval("Vendor") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="EXP ARRIVAL TO CENVEO">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblExpArrivalToCenveo" Text='<%#Eval("ExpArrivalToCenveo") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtExpArrivalToCenveo" Text='<%#Eval("ExpArrivalToCenveo") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="DELIVERY TO DIST BY">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDeliveryToDistBy" Text='<%#Eval("DeliveryToDistBy") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtDeliveryToDistBy" Text='<%#Eval("DeliveryToDistBy") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SHIPPED TO DIST">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblShippedToDist" Text='<%#Eval("ShippedToDist") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtShippedToDist" Text='<%#Eval("ShippedToDist") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CENVEO COMMENTS">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCenveoComments" Text='<%#Eval("CenveoComments") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtCenveoComments" Text='<%#Eval("CenveoComments") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="TWE COMMENTS">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblTweComments" Text='<%#Eval("TweComments") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtTweComments" Text='<%#Eval("TweComments") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ACTUAL PRICE">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblActualPrice" Text='<%#Eval("ActualPrice") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" ID="txtActualPrice" Text='<%#Eval("ActualPrice") %>'></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="STATUS">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblStatus" Text='<%#Eval("Status") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList runat="server" ID="ddlStatus">
                            <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
                            <asp:ListItem Value="0">De-Active</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="edit" Text="Edit"></asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="update" Text="Update"></asp:LinkButton>
                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="cancel" Text="Cancel"></asp:LinkButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>No data available</EmptyDataTemplate>
        </asp:GridView>
    </div>
</div>
