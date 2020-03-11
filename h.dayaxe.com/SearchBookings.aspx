<%@ Page Title="Search Page" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="SearchBookings.aspx.cs" Inherits="h.dayaxe.com.SearchBookings" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<asp:Content runat="server" ContentPlaceHolderID="CssContentPlaceHolder">
    <link href="/Content/searchbooking.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="row revenue">
        <div class="col-md-4">
            <div class="form-group">
                <div class="input-group">
                    <asp:TextBox ID="SearchText" class="form-control search-booking" placeholder="Search"  runat="server"/>
                    <span class="input-group-btn">
                        <asp:Button runat="server" ID="SearchButton" CssClass="btn btn-default btn-search" Text="Search" OnClick="SearchButton_OnClick"/>
                    </span>
                </div>
            </div>
        </div>
        <div class="col-md-2 col-md-push-6 text-right">
            <div class="today">
                <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div class="all">
                <asp:HyperLink NavigateUrl="?s=all" ID="AllHyperLink" runat="server"></asp:HyperLink>
            </div>
            <div class="bookings">
                <asp:HyperLink NavigateUrl="?s=bookings" ID="BookingsHyperLink" runat="server"></asp:HyperLink>
            </div>
            <div class="customers">
                <asp:HyperLink NavigateUrl="?s=customers" ID="CustomersHyperLink" runat="server"></asp:HyperLink>
            </div>
        </div>
    </div>
    <div class="row">
            <div class="col-md-12">
                <table class="table table-booking table-booking-search">
                    <asp:Repeater ID="SearchRepeater" runat="server" OnItemDataBound="SearchRepeater_OnItemDataBound">
                        <HeaderTemplate>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr id="rowHistory" runat="server">
                                <td>
                                    <%# Eval("Description") %>
                                </td>
                                <td>
                                    <a href="<%# Eval("EditUrl") %>" class="edit-hotel">
                                        <span class="glyphicon glyphicon-edit"></span>
                                    </a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr>
                                <td>
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
                                    <span class="result">
                                        <asp:Literal ID="TotalPass" runat="server"></asp:Literal>
                                    </span>
                                </td>
                            </tr>
                            </tbody>
                        </FooterTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
</asp:Content>

