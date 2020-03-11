<%@ Page Title="Credits" Language="C#" MasterPageFile="~/Client.master" AutoEventWireup="true" CodeFile="Credits.aspx.cs" Inherits="dayaxe.com.Credits" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MetaContentPlaceHolder" Runat="Server">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/credit" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ScriptAnalyticsHeaderPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <header class="credits">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <a class="go-back hidden-md hidden-lg" href="javascript:history.go(-1);">
                        <img src="/images/arrow-w.png" alt="arrow-left"/>
                    </a>
                    <h3 class="title title-c">
                        DayAxe Credits
                    </h3>
                </div>
            </div>
        </div>
    </header>
    <div class="container wrapper-body credits">
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <h4 class="title title-total-c">
                    Your Total Credit
                </h4>
                <h4 class="title-p">
                    <asp:Literal runat="server" ID="TotalCreditLit"></asp:Literal>
                </h4>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <hr class="c"/>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <h4 class="title-blue">
                    Enjoy Free #DayCations
                </h4>
                <div class="description">
                    <asp:Literal runat="server" ID="EnjoyFreeDayCationLit"></asp:Literal>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-3 col-sm-7 col-xs-6">
                <h3 class="personal-code">
                    <asp:Literal runat="server" ID="ReferralCodeLit" Text="N/A"></asp:Literal>
                </h3>
            </div>
            <div class="col-md-9 col-sm-5 col-xs-6 text-right-m padding-left-5">
                <asp:Button ID="SendToFriendButton" runat="server" CssClass="btn btn-blue-all" OnClick="SendToFriendButton_OnClick" Text="Send To Friend" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <hr class="c"/>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <div class="description">
                    DayAxe Credit is a form of payment which can be used to purchase products on this site. If you received a Gift or Credit Code from us or a friend you can add it here:
                </div>
            </div>
        </div>
        <div class="row" id="errorRow" runat="server" Visible="False">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:Label runat="server" ID="MessageLabel" CssClass="error-message" Text=""></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-3 col-sm-8 col-xs-7">
                <h3 class="personal-code">
                    <asp:TextBox CssClass="form-control" runat="server" ID="GiftCodeText" placeholder="Enter Gift Code" Text=""></asp:TextBox>
                </h3>
            </div>
            <div class="col-md-9 col-sm-4 col-xs-5 text-right-m padding-left-5">
                <asp:Button ID="AddCreditButton" runat="server" CssClass="btn btn-blue-all" OnClick="AddCreditButton_OnClick" Text="Add Credit" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <hr class="c"/>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <h4 class="title title-credits">
                    Your Credits
                </h4>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12 credit-list">
                <asp:Repeater runat="server" ID="CreditHistoryRpt" OnItemDataBound="CreditHistoryRpt_OnItemDataBound">
                    <ItemTemplate>
                        <div class="row credit-history">
                            <div class="col-md-2 col-sm-2 col-xs-3">
                                <div class="date">
                                    <%# ((DateTime)Eval("CreatedDate")).ToString(Constant.DateFormat) %>
                                </div>
                                <div class="type">
                                    <%# ((Enums.CreditType)Enum.ToObject(typeof(Enums.CreditType) , Eval("CreditType"))).ToDescription() %>
                                </div>
                            </div>
                            <div class="col-md-1 col-sm-2 hidden-xs">
                                <%#(bool)Eval("Status") == false ? "Pending" : string.Empty %>
                            </div>
                            <div class="col-md-7 col-sm-6 col-xs-6 description">
                                <asp:Label runat="server"><%# Eval("Description") %></asp:Label>
                            </div>
                            <div class="col-md-2 col-sm-2 col-xs-3 amount">
                                <asp:Literal runat="server" ID="Amount"></asp:Literal>
                            </div>
                        </div>
                        <div class="row credit-history seperate" id="seperate" runat="server">
                            <div class="col-xs-12">
                                <hr class="c"/>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        <div class="row credit-history not-available" runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count == 0 %>'>
                            <asp:Label ID="lblEmptyData" runat="server" Visible='<%# ((Repeater)Container.NamingContainer).Items.Count == 0 %>' Text="You don't have any credits available" />
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <footer class="product-footer">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <div id="navbar">
                        <ul class="nav navbar-nav">
                            <li class="active">
                                <a href="https://dayaxe.zendesk.com/hc/en-us/requests/new" target="_blank">Contact Us</a>
                            </li>
                            <li>
                                <a href="http://land.dayaxe.com/how-it-works">How it Works</a>
                            </li>
                            <li>
                                <a href="http://land.dayaxe.com/terms">Terms &amp; Conditions</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="copyright">
                        Copyright 2016 - Dayaxe.com
                    </div>
                </div>
            </div>
        </div>
    </footer>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ScriptContentPlaceHolder" Runat="Server">
    <%: Scripts.Render("~/bundles/default") %>
</asp:Content>