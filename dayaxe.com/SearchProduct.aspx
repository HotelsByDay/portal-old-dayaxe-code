<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" EnableEventValidation="false" EnableViewState="false" CodeFile="SearchProduct.aspx.cs" Inherits="dayaxe.com.SearchProductPage" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Register src="Controls/AuthControl.ascx" tagname="AuthControl" tagprefix="uc1" %>
<%@ Register Src="~/Controls/NewsletterControl.ascx" TagPrefix="uc1" TagName="NewsletterControl" %>


<asp:Content ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MetaContentPlaceHolder">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="StyleHeader">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/search" />
    <%: Scripts.Render("~/bundles/jquery") %>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptAnalyticsHeader">
    <!-- Start Google Analytics -->
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
                m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', '<%=AppConfiguration.GoogleKey %>', 'auto');
        ga('send', 'pageview');
    </script>

    <!-- End Google Analytics -->

<!-- Start of dayaxe Zendesk Widget script -->
<script>/*<![CDATA[*/window.zEmbed||function(e,t){var n,o,d,i,s,a=[],r=document.createElement("iframe");window.zEmbed=function(){a.push(arguments)},window.zE=window.zE||window.zEmbed,r.src="javascript:false",r.title="",r.role="presentation",(r.frameElement||r).style.cssText="display: none",d=document.getElementsByTagName("script"),d=d[d.length-1],d.parentNode.insertBefore(r,d),i=r.contentWindow,s=i.document;try{o=s}catch(e){n=document.domain,r.src='javascript:var d=document.open();d.domain="'+n+'";void(0);',o=s}o.open()._l=function(){var o=this.createElement("script");n&&(this.domain=n),o.id="js-iframe-async",o.src=e,this.t=+new Date,this.zendeskHost=t,this.zEQueue=a,this.body.appendChild(o)},o.write('<body onload="document._l();">'),o.close()}("https://assets.zendesk.com/embeddable_framework/main.js","dayaxe.zendesk.com");
/*]]>*/</script>
<!-- End of dayaxe Zendesk Widget script -->

    <!-- Facebook Pixel Code -->

    <script>
        !function (f, b, e, v, n, t, s) {
            if (f.fbq) return; n = f.fbq = function () {
                n.callMethod ?
                    n.callMethod.apply(n, arguments) : n.queue.push(arguments)
            }; if (!f._fbq) f._fbq = n;
            n.push = n; n.loaded = !0; n.version = '2.0'; n.queue = []; t = b.createElement(e); t.async = !0;
            t.src = v; s = b.getElementsByTagName(e)[0]; s.parentNode.insertBefore(t, s)
        }(window,
            document, 'script', '//connect.facebook.net/en_US/fbevents.js');

        fbq('init', '<%=AppConfiguration.FacebookKey %>');
        fbq('track', 'PageView');
        fbq('track', "Search");
    </script>
    <noscript>
    <img height="1" width="1" style="display:none" src='<%=String.Format("https://www.facebook.com/tr?id={0}&ev=PageView&noscript=1", AppConfiguration.FacebookKey) %>"' />
</noscript>

    <!-- End Facebook Pixel Code -->

    <!-- start Mixpanel -->
    <% if (AppConfiguration.EnableMixpanel)
        { %>
    <script>
        (function (e, b) {
            if (!b.__SV) {
                var a, f, i, g; window.mixpanel = b; b._i = []; b.init = function (a, e, d) {
                    function f(b, h) { var a = h.split("."); 2 == a.length && (b = b[a[0]], h = a[1]); b[h] = function () { b.push([h].concat(Array.prototype.slice.call(arguments, 0))) } } var c = b; "undefined" !== typeof d ? c = b[d] = [] : d = "mixpanel"; c.people = c.people || []; c.toString = function (b) { var a = "mixpanel"; "mixpanel" !== d && (a += "." + d); b || (a += " (stub)"); return a }; c.people.toString = function () { return c.toString(1) + ".people (stub)" }; i = "disable time_event track track_pageview track_links track_forms register register_once alias unregister identify name_tag set_config people.set people.set_once people.increment people.append people.union people.track_charge people.clear_charges people.delete_user".split(" ");
                    for (g = 0; g < i.length; g++) f(c, i[g]); b._i.push([a, e, d])
                }; b.__SV = 1.2; a = e.createElement("script"); a.type = "text/javascript"; a.async = !0; a.src = "undefined" !== typeof MIXPANEL_CUSTOM_LIB_URL ? MIXPANEL_CUSTOM_LIB_URL : "file:" === e.location.protocol && "//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js".match(/^\/\//) ? "https://cdn.mxpnl.com/libs/mixpanel-2-latest.min.js" : "//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"; f = e.getElementsByTagName("script")[0]; f.parentNode.insertBefore(a, f)
            }
        })(document, window.mixpanel || []);
    </script>
    <script type="text/javascript">
        mixpanel.init("<%= AppConfiguration.MixpanelKey %>");
        <% if (!String.IsNullOrEmpty(PageCta))
        {  %>
        mixpanel.register({ <%=SearchStartedString %> });
        <% } %>
        mixpanel.track("Search Started", { "CTA": "<%=Request.Params["page_cta"] %>" });
    </script>
    <% } %>
    <!-- end Mixpanel -->
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
    <uc1:AuthControl runat="server" ID="AuthControl" />
    <asp:HiddenField ID="hdVisit" ClientIDMode="Static" Value="false" runat="server" />
    <asp:HiddenField ID="hdLat" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdLng" runat="server" ClientIDMode="Static" />
    <asp:TextBox ID="HidGuest" runat="server" CssClass="fotm-control hid-guest hidden" AutoPostBack="False" OnTextChanged="HidGuest_OnTextChanged"/>
    <section class="access-amenities">
        <div class="row row-search">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <ul class="nav-search">
                    <li class="first">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default btn-city dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <img src="<%=Page.ResolveClientUrl("/images/search-location-icon.png") %>" class="img-responsive" />
                                <asp:Label CssClass="product-value" runat="server" ID="CurrentProductLabel"></asp:Label>
                            </button>
                            <asp:HiddenField ID="HidCity" runat="server" ClientIDMode="Static" />
                            <ul class="dropdown-menu city-dropdown" runat="server" ID="CityDropdown">
                                <li class="first">
                                    <span>Destinations</span>
                                </li>
                            </ul>
                        </div>
                    </li>
                    <li>
                        <div class="input-group date" id="checkindate">
                            <span class="input-group-addon">
                                <img src="<%=Page.ResolveClientUrl("/images/calendar-icon.png") %>" class="img-responsive" />
                            </span>
                            <asp:TextBox ID="CheckInDateText" OnTextChanged="CheckInDateText_OnTextChanged" AutoPostBack="False" class="form-control datepicker" runat="server"/>
                        </div>
                    </li>
                    <li>
                        <div class="btn-group guests">
                            <button type="button" class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <img src="<%=Page.ResolveClientUrl("/images/search-people-icon-gray.png") %>" class="img-responsive" alt="" />
                                <asp:Label CssClass="current-value" runat="server" ID="CurrentGuest"></asp:Label> 
                            </button>
                            <ul class="dropdown-menu guest-capacity" runat="server" ID="GuestCapacity"></ul>
                        </div>
                    </li>
                    <li>
                        <asp:LinkButton runat="server" ID="SearchProductButton" CssClass="search-product-b">
                            <img src="<%=Page.ResolveClientUrl("/images/search-icon-white.png") %>" class="img-responsive" />
                        </asp:LinkButton>
                    </li>
                </ul>
            </div>
        </div>
        <div class="row row-select-product">
            <div class="col-lg-1-5 col-md-2 col-xs-12 col-h col-ft-re">
                <div class="row">
                    <div class="col-xs-6">
                        <span class="filter">
                            <a href="javascript:closeFilter();" class="close-filter hidden-md hidden-lg">
                                <img src="<%=Page.ResolveClientUrl("/images/close-icon-gray.png") %>" class="img-responsive" />
                            </a> FILTERS
                        </span>
                    </div>
                    <div class="col-xs-6 text-right-sm">
                        <asp:LinkButton runat="server" CssClass="btn-clear-all" ID="ClearAllLinkButton" OnClick="ClearAllLinkButtonOnClick">CLEAR ALL</asp:LinkButton>
                    </div>
                </div>
                <div class="row row-filter-result">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <span class="filter-result hidden-sm hidden-xs"></span>
                        <button class="btn btn-blue-all btn-view-passes hidden-md hidden-lg"></button>
                    </div>
                </div>
            </div>
            <div class="col-lg-4-5 col-md-2 col-sm-12 col-xs-12 col-h col-h-2">
                <div class="row">
                    <div class="col-md-6 col-sm-6 col-xs-6 margin-bottom-10">
                        <span class="amenities-label">Amenities</span>
                    </div>
                    <div class="col-md-6 col-sm-6 col-xs-6 margin-bottom-10 text-right">
                        <asp:LinkButton runat="server" ID="ClearAmenitiesLink" CssClass="clear-amenities" OnClick="ClearAmenitiesOnClick">CLEAR</asp:LinkButton>
                    </div>
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <asp:LinkButton runat="server" CssClass="btn btn-link-search" ID="DaypassLink" OnClick="DaypassLinkOnClick" Text="Day Passes"></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-link-search" ID="CabanasLink" OnClick="CabanasLinkOnClick" Text="Cabanas"></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn btn-link-search" ID="DaybedsLink" OnClick="DaybedsLinkOnClick" Text="Daybeds"></asp:LinkButton>
                        <asp:LinkButton runat="server" CssClass="btn last btn-link-search" ID="SpapassLink" OnClick="SpapassLinkOnClick" Text="Spa Passes"></asp:LinkButton>
                    </div>
                </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12 col-h col-h-3">
                <div class="row">
                    <div class="col-md-6 col-sm-6 col-xs-6 margin-bottom-10 padding-left-5">
                        <span class="price-label">Price</span>
                    </div>
                    <div class="col-md-6 col-sm-6 col-xs-6 margin-bottom-10 padding-right-0 text-right">
                        <asp:LinkButton runat="server" ID="ClearPriceLink" CssClass="clear-price">CLEAR</asp:LinkButton>
                    </div>
                    <div class="col-md-12 col-sm-12 col-xs-12 col-price">
                        <asp:TextBox runat="server" ID="PriceText" CssClass="hidden" AutoPostBack="False" ClientIDMode="Static" data-slider-tooltip="show" data-slider-step="1"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-12 col-xs-12 col-h col-h-4">
                <div class="row">
                    <div class="col-md-6 col-sm-6 col-xs-6 margin-bottom-10 padding-left-5">
                        <span class="distance-label">Distance</span>
                    </div>
                    <div class="col-md-6 col-sm-6 col-xs-6 margin-bottom-10 padding-right-0 text-right">
                        <asp:LinkButton ID="ClearDistanceLink" runat="server" CssClass="clear-distance">CLEAR</asp:LinkButton>
                    </div>
                    <div class="col-md-12 col-sm-12 col-xs-12 col-distance">
                        <asp:TextBox runat="server" ID="DistanceText" CssClass="hidden" AutoPostBack="False" data-slider-min="0" data-slider-max="400" data-slider-value="[0,400]" ClientIDMode="Static" data-slider-tooltip="show" data-slider-step="1"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
        <div class="row row-filter hidden-md hidden-lg">
            <div class="col-sm-12 col-xs-12">
                <asp:LinkButton ID="FilterLinkButton" runat="server" CssClass="btn btn-blue-all btn-filter">
                    <img src="<%=Page.ResolveClientUrl("~/images/search-filter-icon-white.png") %>" class="img-responsive img-search-filter" alt="" />
                    <span></span>
                    Filters
                </asp:LinkButton>
            </div>
        </div>
    </section>
    <section class="wrapper style5 container-fluid">
        <div class="row">
            <div class="tab-content tab-pare col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div id="overlay">
                    <div id="loading">
                        <span>CHECKING PASS AVAILABILITY...</span>
                        <img src="<%=Page.ResolveClientUrl("/images/loading.gif") %>" class="img-responsive loading" alt="CHECKING PASS AVAILABILITY..."/>
                    </div>
                </div>
                <div id="home" class="tab-pane fade in active">
                    <section class="listings product-list" id="listings">
                        <asp:UpdatePanel runat="server" ID="HotelList" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="properties_list">
                                    <asp:HiddenField runat="server" ID="FilterResultHidden" ClientIDMode="Static"/>
                                    <asp:ListView ID="LvHotelRepeater" GroupItemCount="2" OnItemDataBound="LvHotelRepeater_ItemDataBound" runat="server">
                                        <LayoutTemplate>
                                            <div runat="server" id="groupPlaceholder"></div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <a id="HotelItem" runat="server">
                                                <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12 clearfix">
                                                    <div class="single_room_wrapper clearfix">
                                                        <figure class="uk-overlay uk-overlay-hover">
                                                            <div class="room_media">
                                                                <asp:Image ID="ProductImage" CssClass="lazy img-style img-responsive" AlternateText="" runat="server" />
                                                                <div class="room_passleft clearfix" runat="server" id="passleftDiv" visible="False">
                                                                    <span class="btn btn-passleft">
                                                                        <asp:Literal runat="server" ID="LitPassleft"></asp:Literal>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                            <div class="room_title room_title_top">
                                                                <asp:Literal ID="MAwayLit" runat="server"></asp:Literal>
                                                            </div>
                                                            <div class="room_title border-bottom-whitesmoke clearfix" id="roomTitle" runat="server">
                                                                <div class="left_room_title floatleft">
                                                                    <a class="btn product-type" id="productTypeLink" runat="server">
                                                                        <asp:Literal ID="ProductType" runat="server"></asp:Literal>
                                                                    </a>
                                                                </div>
                                                            </div>
                                                            <div class="room_title_bottom clearfix clear" id="titleBottom" runat="server">
                                                                <div class="left_room_title floatleft col-lg-9 col-md-9 col-sm-9 col-xs-9 padding-left-0 padding-right-0">
                                                                    <h4 class="heading-style" runat="server" id="ProductHeader"></h4>
                                                                    <h5 class="heading-style" runat="server" id="HotelName"></h5>
                                                                    <h6 class="rating" runat="server" id="ProductAddress"></h6>
                                                                </div>
                                                                <div class="right_room_title floatright col-lg-3 col-md-3 col-sm-3 col-xs-3 padding-left-0 padding-right-0">
                                                                    <p>
                                                                        <span id="msrp" runat="server" class="old-price" Visible="False">
                                                                            <b>was</b>
                                                                            <asp:Label ID="PriceProductOff" runat="server"></asp:Label>
                                                                        </span>
                                                                        <asp:Literal ID="PriceProduct" runat="server"></asp:Literal>
                                                                    </p>
                                                                    <p class="max-guest" id="maxGuest" runat="server">
                                                                        per guest
                                                                    </p>
                                                                </div>
                                                            </div>
                                                        </figure>
                                                    </div>
                                                </div>
                                            </a>
                                        </ItemTemplate>
                                        <GroupTemplate>
                                            <div class="row">
                                                <div runat="server" id="itemPlaceholder"></div>
                                            </div>
                                        </GroupTemplate>
                                        <EmptyDataTemplate>
                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                                    <div class="empty-listing">
                                                        Uh oh, looks like there are no tickets matching your search. Please clear your filters and try again.
                                                    </div>
                                                </div>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID ="HidGuest" EventName ="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID ="PriceText" EventName ="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID ="DistanceText" EventName ="TextChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ClearAllLinkButton" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="DaypassLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="CabanasLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="DaybedsLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="SpapassLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="ClearAmenitiesLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="ClearPriceLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="ClearDistanceLink" EventName="Click"/>
                                <asp:AsyncPostBackTrigger ControlID="SearchProductButton" EventName="Click"/>
                            </Triggers>
                        </asp:UpdatePanel>
                    </section>
                </div>
            </div>
        </div>
    </section>
    <uc1:NewsletterControl runat="server" ID="NewsletterControl" />
    <footer>
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

<asp:Content runat="server" ContentPlaceHolderID="ScriptContentPlaceHolder">
    <script>
        window.showAuth = '<%=ShowAuth%>';
        window.hotelList = '<%=HotelList.ClientID %>';
        window.restrictDate = '<%= RestrictDate.ToString("yyyy/MM/dd") %>';
        var overlayControl = document.getElementById('overlay');
        var bodyControl = document.getElementsByTagName('body');
        <%--window.userIp = '<%=UserIp%>';--%>
        window.startDate = '<%=StartDate.ToString("yyyy-MM-dd") %>';
        window.endDate = '<%=EndDate.ToString("yyyy-MM-dd") %>';

        function BeginRequestHandler(sender, args) {
            if (!sender._postBackSettings.panelsToUpdate || sender._postBackSettings.panelsToUpdate[0].indexOf('AuthControl') === -1) {
                overlayControl.className = "";
                bodyControl[0].style.cssText = "overflow: hidden;";
            }
        }

        function EndRequestHandler(sender, args) {
            if (!sender._postBackSettings.panelsToUpdate || sender._postBackSettings.panelsToUpdate[0].indexOf('AuthControl') === -1) {
                overlayControl.className = "hidden";
                bodyControl[0].style.cssText = "";
            }
        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    </script>
    <%: Scripts.Render("~/bundles/search") %>
</asp:Content>