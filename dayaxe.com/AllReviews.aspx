<%@ Page Title="Reviews" Language="C#" MasterPageFile="~/Client.master" AutoEventWireup="true" CodeFile="AllReviews.aspx.cs" Inherits="dayaxe.com.AllReviews" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MetaContentPlaceHolder" Runat="Server">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/reviews" />
    <style>
        .reviews-page .navbar-default {
            background-color: #13c1f4;
        }
        #NavHeader {
            display: block !important;
        }
        @media (max-width: 992px) {
            .navbar-brand {
                display: none;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ScriptAnalyticsHeaderPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <header class="reviews">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <a class="go-back hidden-lg hidden-md" href="#" onclick="javascript:history.go(-1);">
                        <img src="/images/arrow-w.png" alt="arrow-left"/>
                    </a>
                    <h3 class="title title-reviews">
                        DayAxe Reviews
                    </h3>
                </div>
            </div>
        </div>
    </header>
    
    <div class="container wrapper-body reviews-d">
        <div class="row">
            <div class="container reviews-detail">
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default btn-reviews dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <asp:Label CssClass="reviews-value" runat="server" ID="HotelLabel"></asp:Label> 
                                <span class="glyphicon glyphicon-menu-down"></span>
                            </button>
                            <ul class="dropdown-menu hotel-reviews" runat="server" id="HotelDropdown"></ul>
                        </div>
                        <asp:HyperLink id="FilterButton" OnClick="FilterButton_OnClick" runat="server" CssClass="filter-button btn btn-blue-all" Text="FILTER"></asp:HyperLink>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12 margin-t-20">
                        <asp:Label runat="server" ID="TotalReviewsLabel" CssClass="total-reviews"></asp:Label>
                    </div>
                </div>
                <asp:Repeater runat="server" ID="ReviewsRpt" OnItemDataBound="ReviewsRpt_OnItemDataBound">
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

    <footer class="product-footer" id="footer" runat="server">
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
    <%: Scripts.Render("~/bundles/reviews") %>
</asp:Content>

