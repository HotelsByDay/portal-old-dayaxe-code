<%@ Page Title="" Language="C#" MasterPageFile="~/Client.master" AutoEventWireup="true" CodeFile="reviews.aspx.cs" Inherits="dayaxe.com.ReviewsPage" %>
<%@ Register Src="~/Controls/AuthControl.ascx" TagPrefix="uc1" TagName="AuthControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/reviews" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
    <uc1:AuthControl runat="server" ID="AuthControl" />
    <header>
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <a class="go-back hidden-lg hidden-md" href="#" onclick="javascript:history.go(-1);">
                        <img src="/images/arrow-w.png" alt="arrow-left"/>
                    </a>
                    <h3 class="title">
                        <asp:Literal runat="server" ID="ReviewTitleLit"></asp:Literal>
                    </h3>
                    <div class="location">
                        <asp:Literal runat="server" ID="LocationLit"></asp:Literal>
                        <span class="rating">
                            <asp:Literal runat="server" ID="RatingLit"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </header>
    
    <div class="container wrapper-body">
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:Repeater runat="server" ID="MarketRepeater" OnItemDataBound="MarketRepeaterItemDataBound">
                    <HeaderTemplate>
                        <review>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="review-item">
                            <span class="rating"><%# Eval("UserRatingWeb") %></span>
                            <div class="row margin-bottom-10">
                                <div class="col-md-6 col-sm-6 col-xs-6">
                                    <span class="rate-by">by <%# Eval("ByUser") %></span>
                                </div>
                                <div class="col-md-6 col-sm-6 col-xs-6 text-right">
                                    <span class="rate-date">
                                        <asp:Label runat="server" ID="RedeemedDateLabel"></asp:Label>
                                    </span>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-sm-12 col-xs-12">
                                    <span class="description"><%# Eval("RateCommend") %></span>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </review>
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
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContentPlaceHolder" Runat="Server">
    <%: Scripts.Render("~/bundles/reviews") %>
</asp:Content>

