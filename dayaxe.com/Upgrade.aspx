<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Upgrade.aspx.cs" Inherits="dayaxe.com.Upgrade" %>
<%@ Register src="Controls/AuthControl.ascx" tagname="AuthControl" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MetaContentPlaceHolder" Runat="Server">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/search" />
    <%: Scripts.Render("~/bundles/jquery") %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
    <uc1:AuthControl runat="server" ID="AuthControl" />
    <header class="search">
        <h1>
            Upgrade & Save Now
        </h1>
    </header>
    <section class="wrapper style5 container-fluid wrap-upgrade">
        <div class="row">
            <div class="tab-content tab-pare col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <section class="upgrade">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-xs-12 col-sm-12 text-center">
                            <span class="description">
                                We've hand picked and negotiated these upgrades for you.
                            </span>
                        </div>
                    </div>
                </section>
                <div id="home" class="tab-pane fade in active">
                    <section class="listings upgrade-listings" id="listings">
                        <asp:UpdatePanel runat="server" ID="HotelList" UpdateMode="Conditional">
                            <ContentTemplate>
                                <ul class="properties_list">
                                    <asp:ListView ID="LvHotelRepeater" GroupItemCount="2" OnItemDataBound="LvHotelRepeater_ItemDataBound" runat="server">
                                        <LayoutTemplate>
                                            <div runat="server" id="groupPlaceholder"></div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12 clearfix">
                                                <a id="HotelItem" runat="server" class="upgrade-item">
                                                    <div class="single_room_wrapper clearfix">
                                                        <figure class="uk-overlay uk-overlay-hover">
                                                            <div class="room_media">
                                                                <asp:Image ID="ProductImage" CssClass="lazy img-style img-responsive" AlternateText="" runat="server" />
                                                                <div class="room_passleft clearfix" runat="server" id="passleftDiv" visible="False">
                                                                    <span class="btn btn-passleft">
                                                                        <asp:Literal runat="server" ID="LitPassleft"></asp:Literal>
                                                                    </span>
                                                                </div>
                                                                <div class="room_infoleft clearfix">
                                                                    <h5 class="heading-style" runat="server" id="ProductHeader"></h5>
                                                                    <p class="max-guest">
                                                                        <asp:Literal runat="server" ID="MaxGuestLit"></asp:Literal>
                                                                    </p>
                                                                </div>
                                                            </div>
                                                            <div class="room_title_bottom clearfix">
                                                                <div class="recommendation">
                                                                    <asp:Literal runat="server" ID="RecommendationLit"></asp:Literal>
                                                                </div>
                                                            </div>
                                                            <div class="room_title_bottom clearfix">
                                                                <div class="left_room_title floatleft">
                                                                    <div class="left">
                                                                        <span id="msrp" runat="server" class="old-price" Visible="False">was
                                                                            <asp:Literal ID="PriceProductOff" runat="server"></asp:Literal>
                                                                        </span>
                                                                    </div>
                                                                    <div class="right">
                                                                        <span class="new-price">
                                                                            <asp:Literal ID="PriceProduct" runat="server"></asp:Literal>
                                                                        </span>
                                                                        <span class="with-daypass">with day pass</span>
                                                                    </div>
                                                                </div>
                                                                <div class="right_room_title floatright">
                                                                    <asp:Button runat="server" CssClass="btn btn-blue-all btn-upgrade" ID="Upgrade" Text="Upgrade Now" OnClick="Upgrade_OnClick"/>
                                                                </div>
                                                            </div>
                                                        </figure>
                                                    </div>
                                                </a>
                                            </div>
                                        </ItemTemplate>
                                        <GroupTemplate>
                                            <div class="row">
                                                <div runat="server" id="itemPlaceholder"></div>
                                            </div>
                                        </GroupTemplate>
                                    </asp:ListView>
                                </ul>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="reject-upgrade">
                            <asp:Button runat="server" CssClass="btn btn-blue-all btn-upgrade" ID="RejectUpgrade" Text="I DON'T WANT VIP UPGRADE" OnClick="RejectUpgrade_OnClick"/>
                        </div>
                    </section>
                </div>
            </div>
        </div>
    </section>
    <footer class="upgrade">
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
                                <a href="http://land.dayaxe.com/terms">Terms & Conditions</a>
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
    <script>
        window.showAuth = '<%=ShowAuth%>';
        window.searchCall = '<%=SearchCall%>';
    </script>
    <%: Scripts.Render("~/bundles/search") %>
    <%: Scripts.Render("~/bundles/shortern") %>
    <script>
        $('.upgrade-item .recommendation').shorten({
            showChars: 160,
            onMore: function (e) {
                $('.allcontent:visible').css({ 'display': 'inline' });
            }
        });
    </script>
</asp:Content>

