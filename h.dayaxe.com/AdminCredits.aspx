<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="AdminCredits.aspx.cs" Inherits="h.dayaxe.com.AdminCredits" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="DayaxeDal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
    <link href="/Content/searchbooking.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="row revenue">
        <div class="col-md-10">
            <asp:Label runat="server" ID="CustomerEmailLabel"></asp:Label>
        </div>
        <div class="col-md-2 text-right">
            <div class="today">
                <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <asp:Label ID="ErrorLabel" ForeColor="Red" runat="server" Text="" Visible="False"></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-6">
            <span class="blue">Customer Details</span>
        </div>
        <div class="col-md-6 text-right">
            <asp:Label runat="server" ID="StripeCustomerLabel" CssClass="blue"></asp:Label>
        </div>
    </div>
    <div class="row customer-infos">
        <div class="col-md-12">
            <div class="row margin-t-25">
                <div class="col-md-4">
                    <label>First Name</label>
                </div>
                <div class="col-md-4">
                    <label>Last Name</label>
                </div>
                <div class="col-md-4">
                    <label>Customer ID</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <asp:TextBox ID="FirstNameText" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="LastNameText" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="CustomerIdText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-8">
                    <label>Email</label>
                </div>
                <div class="col-md-4">
                    <label>Zip Code</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox ID="EmailText" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="ZipCodeText" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-4">
                    <label>Password</label>
                </div>
                <div class="col-md-4">
                    <label>Stripe Account</label>
                </div>
                <div class="col-md-4">
                    <label>Date Created</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <asp:TextBox ID="PasswordText" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="StripeCustomerIdText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="CreatedDateText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-12">
            <div class="btn-group tab tab-2" role="group" aria-label="">
                <a type="button" class="btn btn-tab" href="CustomerDetails.aspx?id=<%=Request.Params["id"] %>">BOOKINGS</a>
                <a type="button" class="btn btn-tab active" href="AdminCredits.aspx?id=<%=Request.Params["id"] %>">CREDITS</a>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-3">
            <span class="blue">
                TOTAL CREDITS:
            </span>
            <label>
                <asp:Literal runat="server" ID="TotalCreditLabel"></asp:Literal>
            </label>
        </div>
        <div class="col-md-9">
            <span class="blue">
                ACCOUNT TOTAL:
            </span>
            <label>
                <asp:Literal runat="server" ID="AccountTotalLabel"></asp:Literal>
            </label>
        </div>
    </div>
    <div class="row customer-bookings">
        <div class="col-md-12">
            <table class="table table-booking table-customer-bookings-details">
                <asp:Repeater ID="SearchRepeater" runat="server" OnItemDataBound="SearchRepeater_OnItemDataBound">
                    <HeaderTemplate>
                        <thead>
                        <tr>
                            <th>
                                CREDIT DATE
                            </th>
                            <th>
                                CREDIT TYPE
                            </th>
                            <th>
                                DESCRIPTION
                            </th>
                            <th class="text-right">
                                CREDIT AMOUNT
                            </th>
                            <th>
                                Actions
                            </th>
                        </tr>
                        </thead>
                        <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHistory" runat="server">
                            <td>
                                <%# ((DateTime)Eval("CreatedDate")).ToString(Constant.DateFormat) %>
                            </td>
                            <td>
                                <b>
                                    <%# ((Enums.CreditType)Enum.ToObject(typeof(Enums.CreditType) , Eval("CreditType"))).ToDescription() %>
                                </b>
                            </td>
                            <td>
                                <%# Eval("Description") %>
                            </td>
                            <td class="text-right">
                                <asp:Literal runat="server" ID="Amount"></asp:Literal>
                            </td>
                            <td class="text-center">
                                <a href="#" class="edit-hotel">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="3">
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
                            <td colspan="2" class="text-right">
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
    <div class="row margin-t-50">
        <div class="col-md-3">
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" CausesValidation="False" Text="Cancel" OnClick="CancelClick"/>
        </div>
        <div class="col-md-3 text-center">
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteButton" CausesValidation="False" Text="Delete" OnClick="DeleteClick"/>
        </div>
        <div class="col-md-3 text-center">
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="Deactivebutton" CausesValidation="False" Text="De-Active" OnClick="DeactiveClick"/>
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="ActiveButton" CausesValidation="False" Text="Active" OnClick="ActiveClick"/>
        </div>
        <div class="col-md-3 text-right">
            <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Save" OnClick="SaveCustomerInfosClick"/>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
</asp:Content>

