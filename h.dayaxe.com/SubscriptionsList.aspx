<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="SubscriptionsList.aspx.cs" Inherits="h.dayaxe.com.SubscriptionsList" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="DayaxeDal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="hotel-listings">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
        <div class="row" runat="server" id="AddNewRow" Visible="False">
            <div class="col-md-12 text-right margin-t-15">
                <a href="/EditSubscriptions.aspx?id=0" class="btn btn-cancel">Add New</a>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 margin-t-15">
                <asp:Repeater runat="server" ID="RptSubscriptionsListing" OnItemDataBound="RptSubscriptionsListing_OnItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                        <thead>
                        <th style="text-align: center;">Thumbnail</th>
                        <th>Name</th>
                        <th>SubTitle</th>
                        <th>Type</th>
                        <th>Price</th>
                        <th>Active</th>
                        <th>Actions</th>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHotel" class="subscription-item" runat="server">
                            <td>
                                <asp:Image runat="server" ID="ThumbnailImage" Width="140px" ImageUrl='<%# Eval("AdminImageUrl") %>' />
                            </td>
                            <td>
                                <%# Eval("Name") %>
                            </td>
                            <td>
                                <%# Eval("ProductHighlight") %>
                            </td>
                            <td>
                                <%# ((Enums.SubscriptionType)Eval("SubscriptionType")).ToString() %>
                            </td>
                            <td>
                                <%# String.Format("${0}", Eval("Price")) %>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("IsActive")) %>' />
                            </td>
                            <td data-id='<%# Eval("Id") %>'>
                                <a href="/EditSubscriptions.aspx?id=<%# Eval("Id") %>" class="edit-subscription">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                                <asp:LinkButton ID="DeleteSubscriptionsLinkButton" OnCommand="DeleteSubscriptionsLinkButtonOnCommand" CssClass="delete-subscription" runat="server">
                                    <span class="glyphicon glyphicon-trash"></span>
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="6">
                                <div class="btn-group">
                                    <button runat="server" id="Previous" clientidmode="Static" class="btn btn-prev" onserverclick="Previous_OnClick">
                                        <img src="/images/arrow-left.png" alt="" class="img-responsive" />
                                    </button>
                                    <button runat="server" id="Next" clientidmode="Static" class="btn btn-next" onserverclick="Next_OnClick">
                                        <img src="/images/arrow-right.png" alt="" class="img-responsive" />
                                    </button>
                                </div>
                                <b>
                                    <asp:Literal runat="server" ID="LitPage"></asp:Literal>
                                </b>
                            </td>
                            <td>
                                <span class="result-hotel">
                                    <asp:Literal runat="server" ID="LitTotal"></asp:Literal>
                                </span>
                            </td>
                        </tr>
                        </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/subscriptionlist.js" type="text/javascript"></script>
</asp:Content>

