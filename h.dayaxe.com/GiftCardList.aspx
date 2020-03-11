<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="GiftCardList.aspx.cs" Inherits="h.dayaxe.com.GiftCardList" %>
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
        <div class="row" runat="server" id="AddNewRow">
            <div class="col-md-12 text-right margin-t-15">
                <a href="/GiftCardDetails.aspx?id=0" class="btn btn-cancel">Add New</a>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 margin-t-15">
                <asp:Repeater runat="server" ID="RptGiftCardListing" OnItemDataBound="RptGiftCardListing_OnItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                        <thead>
                        <th>Name</th>
                        <th>Code</th>
                        <th>Amount</th>
                        <th>Status</th>
                        <th>Actions</th>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHotel" runat="server">
                            <td>
                                <%# Eval("Name") %>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td>
                                <%# Helper.FormatPrice((double)Eval("Amount")) %>
                            </td>
                            <td>
                                <%# (Eval("Status")).ToString() == "0" ? "Available" : "Used" %>
                            </td>
                            <td>
                                <a href="/GiftCardDetails.aspx?id=<%# Eval("Id") %>" class="edit-hotel">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                                <a href="#" class="delete-hotel">
                                    <span class="glyphicon glyphicon-trash"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="4">
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
</asp:Content>

