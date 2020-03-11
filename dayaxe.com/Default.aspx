<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="dayaxe.com.Default" %>
<%@ Register Src="~/Controls/AuthControl.ascx" TagPrefix="uc1" TagName="AuthControl" %>
<%@ Register Src="~/Controls/NewsletterControl.ascx" TagPrefix="uc1" TagName="NewsletterControl" %>

<%@ Reference Control="~/Controls/AuthControlWithoutPopup.ascx" %>
<%@ Reference Control="~/Controls/InvalidTicket.ascx" %>
<%@ Reference Control="~/Controls/MyAccount.ascx" %>

<%--<%@ Reference Control="~/LandingTemplate/homepage.ascx" %>--%>

<asp:Content ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MetaContentPlaceHolder">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/default" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ScriptAnalyticsHeaderPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
    <uc1:AuthControl runat="server" ID="AuthControl" />
    <asp:PlaceHolder runat="server" ID="HeaderContentPlaceHolder"></asp:PlaceHolder>
    <div class="content">
        <asp:PlaceHolder runat="server" ID="ContentPlaceHolder"></asp:PlaceHolder>
    </div>
    <uc1:NewsletterControl runat="server" ID="NewsletterControl" />
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
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContentPlaceHolder" Runat="Server">
    <script>
        window.showAuth = '<%=ShowAuth%>';
    </script>
    <%: Scripts.Render("~/bundles/default") %>
</asp:Content>

