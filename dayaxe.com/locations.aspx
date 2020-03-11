<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="locations.aspx.cs" Inherits="dayaxe.com.locations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MetaContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/search" />
    <%: Scripts.Render("~/bundles/jquery") %>
    <style>
        .wrapper.style5 {
            margin-top: 150px;
        }
        .wrapper.style5 img {
            margin-bottom: 20px;
        }
        .margin-bottom-25 {
            margin-bottom: 25px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="LoadingContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <section class="container-fluid">
        <div class="row">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <asp:Repeater runat="server" ID="LocationRpt" OnItemDataBound="LocationRpt_OnItemDataBound">
                    <HeaderTemplate>
                        <location>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-md-8 margin-bottom-25">
                                <img id="LocationImage" class="cld-responsive" runat="server" />
                            </div>
                            <div class="col-md-4">
                                <%#Eval("LocationName") %>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </location>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ScriptContentPlaceHolder" Runat="Server">
    <%--<script src="/assets/js/cloudinary/cloudinary-core-shrinkwrap.js" type="text/javascript"></script>--%>
    <script>
        function resize() {
            if ($(window).width() > 1200) {
                $("img[img-r]").each(function () {
                    $(this).attr('src', $(this).attr('img-r'));
                });
            }
            else if ($(window).width() > 992) {
                $("img[img-d]").each(function () {
                    $(this).attr('src', $(this).attr('img-d'));
                });
            } else {
                $("img[img-m]").each(function () {
                    $(this).attr('src', $(this).attr('img-m'));
                });
            }
        }
        resize();
        $(window).on('resize', resize);
        //var cl = cloudinary.Cloudinary.new({ cloud_name: "vietluyen" });
        //cl.responsive();
    </script>
</asp:Content>

