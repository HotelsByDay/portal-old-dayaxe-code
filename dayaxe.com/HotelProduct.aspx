<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="HotelProduct.aspx.cs" Inherits="dayaxe.com.HotelProduct" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Register Src="~/Controls/AuthControl.ascx" TagPrefix="uc1" TagName="AuthControl" %>
<%@ Register Src="~/Controls/Survey.ascx" TagPrefix="uc1" TagName="Survey" %>
<%@ Register Src="~/Controls/NewsletterControl.ascx" TagPrefix="uc1" TagName="NewsletterControl" %>


<asp:Content ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="StyleHeader">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/product" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptAnalyticsHeader">
    <!-- Facebook Pixel Code -->
<script>
    !function(f,b,e,v,n,t,s){if(f.fbq)return;n=f.fbq=function(){n.callMethod?
            n.callMethod.apply(n,arguments):n.queue.push(arguments)};if(!f._fbq)f._fbq=n;
        n.push=n;n.loaded=!0;n.version='2.0';n.queue=[];t=b.createElement(e);t.async=!0;
        t.src=v;s=b.getElementsByTagName(e)[0];s.parentNode.insertBefore(t,s)}(window,
        document,'script','//connect.facebook.net/en_US/fbevents.js');

    fbq('init', '<%=AppConfiguration.FacebookKey %>');
    fbq('track', 'PageView');
    fbq('track', 'ViewContent', { 
        content_ids: ['<%=PublicProduct.ProductId %>'],
        content_type: 'product',
        content_name:'<%=string.Format("{0} - {1}", PublicHotel.HotelName, PublicProduct.ProductName) %>'
    });
</script>
<noscript>
    <img height="1" width="1" style="display:none" src='<%=String.Format("https://www.facebook.com/tr?id={0}&ev=PageView&noscript=1", AppConfiguration.FacebookKey) %>"' />
</noscript>
<!-- End Facebook Pixel Code -->   

<!-- Start of dayaxe Zendesk Widget script -->
<script>/*<![CDATA[*/window.zEmbed||function(e,t){var n,o,d,i,s,a=[],r=document.createElement("iframe");window.zEmbed=function(){a.push(arguments)},window.zE=window.zE||window.zEmbed,r.src="javascript:false",r.title="",r.role="presentation",(r.frameElement||r).style.cssText="display: none",d=document.getElementsByTagName("script"),d=d[d.length-1],d.parentNode.insertBefore(r,d),i=r.contentWindow,s=i.document;try{o=s}catch(e){n=document.domain,r.src='javascript:var d=document.open();d.domain="'+n+'";void(0);',o=s}o.open()._l=function(){var o=this.createElement("script");n&&(this.domain=n),o.id="js-iframe-async",o.src=e,this.t=+new Date,this.zendeskHost=t,this.zEQueue=a,this.body.appendChild(o)},o.write('<body onload="document._l();">'),o.close()}("https://assets.zendesk.com/embeddable_framework/main.js","dayaxe.zendesk.com");
/*]]>*/</script>
<!-- End of dayaxe Zendesk Widget script -->

 <!-- start Mixpanel -->
    <style type="text/css">
        .gm-style .default-card {
            display: none !important;
        }
    </style>
    <% if (AppConfiguration.EnableMixpanel) { %>
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
            mixpanel.register({
                "Hotel Name": "<%=string.Format("{0} - {1}", PublicHotel.HotelName, PublicProduct.ProductName) %>",
                "Hotel Type": "<%=((Enums.Hoteltype)(PublicHotel.HoteltypeId ?? 0)).ToString()%>",
                "Hotel Price": "<%=RegularPrice.ToString(RegularPrice%1 == 0.0 ? "C0" : "C", new CultureInfo("en-US"))%>",
                "Book Date": "" + new Date() + "",
                "# of People": "2",
                "Book Date Time": "<%=DateTime.Today.ToLongDateString()%>"
            });
            mixpanel.track("Hotel Selected", { "referrer": document.referrer });
        </script>
    <% } %>
    
    <!-- end Mixpanel -->
    <script>
        (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
                (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
                m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
        })(window,document,'script','//www.google-analytics.com/analytics.js','ga');
        ga('create', '<%=AppConfiguration.GoogleKey %>', 'auto');
        ga('send', 'pageview');
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="LoadingContentPlaceHolder">
    <div class="hoteloverlaywrap" id="hoverlay" style="display: none;"></div>
    <div class="hotelmodal" id="hmodal" style="display: none;">
        LOADING, PLEASE WAIT...
        <div class="imgspin text-center">
            <img src="/images/loading.gif" alt="" />
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
    <uc1:AuthControl runat="server" ID="AuthControl" />
    <uc1:Survey runat="server" ID="Survey" />
    <asp:HiddenField runat="server" ID="HidSelectedDate" ClientIDMode="Static"/>
    <asp:HiddenField runat="server" ID="HidSelectedDateBefore" ClientIDMode="Static"/>

    <div id="requestAccessModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
        <div class="vertical-alignment-h">
            <div class="modal-dialog vertical-align-c">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title text-center">Access Requested</h4>
                    </div>
                    <div class="modal-body text-center">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="location">
                                    Thank You!
                                    <br/>
                                    We will let you know as soon as this hotel starts selling day passes
                                </div>
                            </div>
                        </div>
                        <div class="row button-submit">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="btn btn-save btn-close">DONE</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div id="openDateTicketModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
        <div class="vertical-alignment-h">
            <div class="modal-dialog vertical-align-c">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title text-center">Open Date Ticket</h4>
                    </div>
                    <div class="modal-body text-center">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="location">
                                    <ul>
                                        <li>
                                            Ticket can be used one time any day within <b>30 days of purchase</b>, excluding <span class="blue">blackout days</span>.
                                        </li>
                                        <li>
                                            When ready to go select check-in date prior to arrival, Same day selection is available, subject to daily availability.
                                        </li>
                                        <li>
                                            Change check-in date at no charge anytime <b>48 hours or more</b> before <b>9 am on check-in date</b>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="row button-submit">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="btn btn-blue" data-dismiss="modal">OK</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div id="addToWaitingListModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
        <div class="vertical-alignment-h">
            <div class="modal-dialog vertical-align-c">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title text-center">Create Ticket Alert, Free.</h4>
                    </div>
                    <div class="modal-body text-center">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <asp:UpdatePanel runat="server" ID="AddToWaitListPanel">
                                    <ContentTemplate>
                                        <div class="location">
                                            <div class="description">
                                                Extra spots open up often, you will be first to know!
                                            </div>
                                            <div class="error-message">
                                                <asp:Label runat="server" ID="ErrorMessageLabel" ForeColor="Red"></asp:Label>
                                            </div>
                                            <div>
                                                <asp:TextBox runat="server" CssClass="waitlist-date datepicker" ID="CheckInDateRequestText"></asp:TextBox>
                                            </div>
                                            <div>
                                                <asp:TextBox runat="server" placeholder="Email Address" ID="EmailAddressText"></asp:TextBox>
                                            </div>
                                            <div class="row button-submit">
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    <asp:Button class="btn btn-save btn-close" ID="AddToWaitListButton" runat="server" OnClick="AddToWaitListButtonOnClick" Text="ADD ME TO WAITLIST"></asp:Button>
                                                </div>
                                            </div>
                                            <div class="terms-of-use">
                                                By continuing, you agree to our <a href="http://land.dayaxe.com/terms" class="link-terms" target="_blank">Terms of Use</a>.
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="TotalTicketsText" EventName="TextChanged"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <header class="product">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <a class="go-back" href="#" runat="server" id="goBack">
                        <img src="/images/arrow.png" alt="arrow-left"/> <span class="hidden-md hidden-sm hidden-xs">Back to Search</span>
                    </a>
                    <div class="title">
                        <div class="header-top">
                            <h4>All Day Access To Luxury Amenities</h4>
                        </div> 
                        <asp:Label runat="server" ID="HotelNameLit"></asp:Label>
                    </div>
                    <div class="location">
                        <span class="hotel-type hidden-sm hidden-xs">
                            <%=((Enums.Hoteltype)(PublicHotel.HoteltypeId ?? 0)).ToString() %>
                        </span>
                        <%=string.Format("{0}, {1}", PublicHotel.Neighborhood, PublicHotel.City) %>
                        <asp:UpdatePanel runat="server" ID="HotelRatingPanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <span class="rating">
                                    <%=PublicHotel.CustomerRating > 0 ? PublicHotel.CustomerRatingString : PublicHotel.Rating %>
                                </span>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </header>
    <asp:UpdatePanel runat="server" ID="HotelDetail" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="price-mobile overlay-text hidden-md hidden-lg" id="priceMobile" runat="server">
                <div class="left">
                    <span id="msrp" runat="server" class="msrp upto-guest" Visible="False">
                        <asp:Literal runat="server" ID="PerGuestMsrpLitMobile"></asp:Literal>
                    </span>
                    <asp:Literal runat="server" ID="PerGuestLitMobile"></asp:Literal>
                </div>
                <div class="btn-group tickets dropup">
                    <button type="button" class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label CssClass="current-value" runat="server" ID="CurrentTicket"></asp:Label> 
                        <img src="/images/arrow-down-s.png" class="img-responsive" />
                    </button>
                    <ul class="dropdown-menu dropup ticket-capacity" runat="server" ID="TicketCapacity"></ul>
                </div>
            </div>
            <div class="wrapper-body">
                <div class="row">
                    <div class="tab-content tab-pare col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="tab-pane fade in active booking-content">
                            <section class="wrapper style5">
                                <div id="owl-demo" class="owl-carousel owl-theme">
                                    <% if (PublicImages != null)
                                        { %>
                                    <% for (int i = 0; i < PublicImages.Length; i++)
                                        { %>
                                    <div class="item">
                                        <span class="hotel-type hidden-lg hidden-md">
                                            <%=((Enums.Hoteltype)(PublicHotel.HoteltypeId ?? 0)).ToString() %>
                                        </span>
                                        <img class="lazyOwl" data-src="<%= string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicImages[i]).AbsoluteUri) %>" alt="<%= GetTitleImage(PublicImages[i]) %>" />
                                        <div class="room_passleft clearfix" runat="server" id="passleftDiv" visible="False">
                                            <span class="btn btn-passleft">
                                                <asp:Literal runat="server" ID="LitPassleft"></asp:Literal>
                                            </span>
                                        </div>
                                    </div>
                                    <% } %>
                                    <% } %>
                                </div>
                                <div class="clearfix"></div>
                            </section>
                            <div class="container product-detail">
                                <div class="row">
                                    <div class="col-md-8 col-sm-12 col-xs-12">
                                        <div class="row main-tab-content padding-top-30 " id="wrapWhatYouGet" runat="server" Visible="False">
                                            <div class="col-md-4 col-sm-12 col-xs-12 center-mobile padding-b-mobile-20">
                                                <label>What You Get</label>
                                            </div>
                                            <div class="col-md-8 col-sm-12 col-xs-12 padding-top-5 col-data">
                                                <asp:Literal ID="WhatYouGetLit" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                        <div class="row main-tab-content padding-top-30 " id="wrapYourService" runat="server" Visible="False">
                                            <div class="col-md-4 col-sm-12 col-xs-12 center-mobile padding-b-mobile-20">
                                                <label>Your Service</label>
                                            </div>
                                            <div class="col-md-8 col-sm-12 col-xs-12 padding-top-5 col-data">
                                                <asp:Literal ID="YourServiceLit" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                        <div class="row main-tab-content padding-top-30 ">
                                            <div class="col-md-12 col-sm-12 col-xs-12 center-mobile padding-b-mobile-20">
                                                <label>Hotel Amenities</label>
                                            </div>
                                        </div>
                                        <!-- Nav tabs -->
                                        <ul id="main-tabs" class="nav nav-tabs" role="tablist">
                                            <asp:Literal runat="server" ID="LiMainTab"></asp:Literal>
                                        </ul>

                                        <div id="main-tabs-content" role="tabpanel">
                                            <!-- Tab panes -->
                                            <div class="tab-content">
                                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.PoolActive %>'>
                                                    <div role="tabpanel" class="tab-pane fade in active" id="restaurant">
                                                        <div class="row padding-bottom-20">
                                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                                <ul class="services" id="poolAmenties" runat="server"></ul>
                                                            </div>
                                                        </div>
                                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.PoolHours)) { %>
                                                            <div class="row hour-line">
                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                    <label class="hour margin-right-10">Hours: </label>
                                                                    <span class="value"><%= PublicHotel.AmentiesItem.PoolHours %></span>
                                                                </div>
                                                            </div>
                                                        <% } %>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.GymActive %>'>
                                                    <div role="tabpanel" class="tab-pane fade" id="sports-club">
                                                        <div class="row padding-bottom-20">
                                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                                <ul class="services" id="gymAmenties" runat="server"></ul>
                                                            </div>
                                                        </div>
                                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.GymHours)) { %>
                                                            <div class="row hour-line">
                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                    <label class="hour margin-right-10">Hours: </label>
                                                                    <span class="value"><%= PublicHotel.AmentiesItem.GymHours %></span>
                                                                </div>
                                                            </div>
                                                        <% } %>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.SpaActive %>'>
                                                    <div role="tabpanel" class="tab-pane fade" id="spa-club">
                                                        <div class="row padding-bottom-20">
                                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                                <ul class="services" id="spaAmenties" runat="server"></ul>
                                                            </div>
                                                        </div>
                                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.SpaHours)) { %>
                                                            <div class="row hour-line">
                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                    <label class="hour margin-right-10">Hours: </label>
                                                                    <span class="value"><%= PublicHotel.AmentiesItem.SpaHours %></span>
                                                                </div>
                                                            </div>
                                                        <% } %>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.BusinessActive %>'>
                                                    <div role="tabpanel" class="tab-pane fade" id="pick-up">
                                                        <div class="row padding-bottom-20">
                                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                                <ul class="services" id="officeAmenties" runat="server"></ul>
                                                            </div>
                                                        </div>
                                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.BusinessCenterHours)) { %>
                                                            <div class="row hour-line">
                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                    <label class="hour margin-right-10">Hours: </label>
                                                                    <span class="value"><%= PublicHotel.AmentiesItem.BusinessCenterHours %></span>
                                                                </div>
                                                            </div>
                                                        <% } %>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.DinningActive %>'>
                                                    <div role="tabpanel" class="tab-pane fade" id="dining">
                                                        <div class="row padding-bottom-20">
                                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                                <ul class="services" id="dinningAmenties" runat="server"></ul>
                                                            </div>
                                                        </div>
                                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.DinningHours)) { %>
                                                            <div class="row hour-line">
                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                    <label class="hour margin-right-10">Hours: </label>
                                                                    <span class="value"><%= PublicHotel.AmentiesItem.DinningHours %></span>
                                                                </div>
                                                            </div>
                                                        <% } %>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.EventActive %>'>
                                                    <div role="tabpanel" class="tab-pane fade" id="event">
                                                        <div class="row padding-bottom-20">
                                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                                <ul class="services" id="eventAmenties" runat="server"></ul>
                                                            </div>
                                                        </div>
                                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.EventHours)) { %>
                                                            <div class="row hour-line">
                                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                                    <label class="hour margin-right-10">Hours: </label>
                                                                    <span class="value"><%= PublicHotel.AmentiesItem.EventHours %></span>
                                                                </div>
                                                            </div>
                                                        <% } %>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <hr class="divider" />
                                            </div>
                                        </div>
                                        <div class="row margin-right-0" id="moreAtHotelRow" runat="server" Visible="False">
                                            <div class="col-md-12 col-xs-12 col-sm-12 related-product-list">
                                                <div class="row">
                                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                                        <h4 class="more-at-h">
                                                            <%=PublicHotel.MoreAtHotelString %>
                                                        </h4>
                                                    </div>
                                                </div>
                                                <asp:ListView ID="LvRelatedProductsRepeater" GroupItemCount="3" OnItemDataBound="LvRelatedProductsRepeater_OnItemDataBound" runat="server">
                                                    <LayoutTemplate>
                                                        <div runat="server" id="groupPlaceholder"></div>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <div class="col-lg-4 col-md-4 col-sm-12 col-xs-12 clearfix">
                                                            <a id="HotelItem" runat="server">
                                                                <div class="row">
                                                                    <div class="col-md-12 col-sm-7 col-xs-7">
                                                                        <img src="<%#Eval("ImageUrl") %>" class="img-responsive" />
                                                                    </div>
                                                                    <div class="col-md-12 col-sm-5 col-xs-5">
                                                                        <div class="item-info">
                                                                            <h5>
                                                                                <%#Eval("ProductName") %>
                                                                            </h5>
                                                                            <div class="rating">
                                                                                <%#PublicHotel.CustomerRating > 0 ? PublicHotel.CustomerRatingString : PublicHotel.Rating %>
                                                                                <%#PublicHotel.TotalReviews %>
                                                                            </div>
                                                                            <div class="price">
                                                                                <asp:Literal runat="server" ID="ProductPriceLit"></asp:Literal>
                                                                                <span>
                                                                                    per guest
                                                                                </span>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </a>
                                                        </div>
                                                    </ItemTemplate>
                                                    <GroupTemplate>
                                                        <div class="row item">
                                                            <div runat="server" id="itemPlaceholder"></div>
                                                        </div>
                                                    </GroupTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                        <div class="row" id="afterMoreAtRow" runat="server" Visible="False">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <hr class="divider" />
                                            </div>
                                        </div>
                                        <div class="row row-info">
                                            <div class="col-md-4 col-sm-4 col-xs-4">
                                                <div class="check-in-place">
                                                    <img src="/images/icon_checkin.png" class="img-responsive" alt="" />
                                                    <span class="title">Check-In</span>
                                                    <span class="des">
                                                        <%=PublicHotel.CheckInPlace %>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="col-md-4 col-sm-4 col-xs-4">
                                                <div class="open-ic">
                                                    <img src="/images/icon_hour.png" class="img-responsive" alt="" />
                                                    <span class="title">Hours</span>
                                                    <span class="des">
                                                        <%=PublicHotel.GeneralHours%>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="col-md-4 col-sm-4 col-xs-4">
                                                <div class="baby-ic">
                                                    <img src="/images/icon_children.png" class="img-responsive" alt="" />
                                                    <span class="title">Children</span>
                                                    <asp:Label runat="server" ID="KidAllowLabel" CssClass="des"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <ul id="main-tabs-offer" class="nav nav-tabs" role="tablist">
                                            <li role="presentation" class="col-xs-6 active">
                                                <a href="#offer" aria-controls="offer" role="tab" data-toggle="tab" aria-expanded="true">
                                                    <div>
                                                        <img src="/images/icon_offer.png" class="img-responsive" />
                                                        <span>OFFERS</span>
                                                    </div>
                                                </a>
                                            </li>
                                            <li role="presentation" class="col-xs-6">
                                                <a href="#parking" aria-controls="parking" role="tab" data-toggle="tab" aria-expanded="true">
                                                    <div>
                                                        <img src="/images/icon_parking.png" class="img-responsive" />
                                                        <span>PARKING</span>
                                                    </div>
                                                </a>
                                            </li>
                                        </ul>
                                        <div id="main-tabs-content-offer" role="tabpanel">
                                            <div class="tab-content">
                                                <div role="tabpanel" class="tab-pane fade in active" id="offer">
                                                    <%=PublicHotel.HotelDiscountDisclaimer %>
                                                </div>
                                                <div role="tabpanel" class="tab-pane fade" id="parking">
                                                    <%=PublicHotel.HotelParking %>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <hr class="divider" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-4 col-sm-12 col-xs-12 center-mobile padding-b-mobile-20">
                                                <label>Address</label>
                                            </div>
                                            <div class="col-md-8 col-sm-12 col-xs-12 center-mobile padding-top-5">
                                                <span>
                                                    <asp:Literal ID="streetname" runat="server"></asp:Literal></span>
                                                <br class="hidden-md hidden-lg"/>
                                                <span>
                                                    <asp:Literal ID="CityStateZip" runat="server"></asp:Literal></span>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <hr class="divider" />
                                            </div>
                                        </div>
                                        <div class="row product-redemtion hidden-md hidden-lg">
                                            <div class="col-sm-12 col-xs-12">
                                                <div class="check-in-date-mobile">
                                                    <div class="image">
                                                        <img src="/images/icon_date.png" class="img-responsive" />
                                                    </div>
                                                    <div class="max-guest">
                                                        Check-In Date
                                                        <div class="redemption-note">
                                                            <div class="input-group date" id="checkindated">
                                                                <asp:TextBox ID="CheckInDateTextDesktop" OnTextChanged="CheckInDateTextDesktop_OnTextChanged" placeholder="Select Date" AutoPostBack="True" class="form-control datepicker" runat="server"></asp:TextBox>
                                                                <span class="input-group-addon">
                                                                    <span class="glyphicon glyphicon-menu-right" aria-hidden="true"></span>
                                                                </span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row margin-right-0 hidden-md hidden-lg bell-row">
                                            <div class="col-sm-1 col-xs-1 bell-col-1">
                                                <img src='<%=ResolveClientUrl("/images/icon-bell.png") %>' class="img-responsive img-bell" alt="" />
                                            </div>
                                            <div class="col-sm-7 col-xs-7">
                                                <label class="blue">
                                                    This hotel is extremely popular. Sign up for email alerts when spots open up.
                                                </label>
                                            </div>
                                            <div class="col-sm-3 col-xs-3 text-right bell-col-3">
                                                <asp:HiddenField runat="server" ID="IsWaitingListMHidden" ClientIDMode="Static" Value="false" />
                                                <div id="isWaitingListM" class="toggle toggle-iphone"></div>
                                            </div>
                                        </div>
                                        <div class="row hidden-md hidden-lg">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <hr class="divider" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12 center-mobile">
                                                <label>Our Recommendation</label>
                                            </div>
                                            <div class="col-md-12 padding-top-30 our-recommendation">
                                                <asp:Literal ID="WhyWeLikeIt" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                        <asp:Panel runat="server" ID="ReviewPanel" CssClass="review-panel" Visible="False">
                                            <div class="row">
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    <hr class="divider" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12 col-sm-12 col-xs-12 center-mobile padding-b-mobile-20">
                                                    <label>What DayAxers Think</label>
                                                </div>
                                                <div class="col-md-12 col-sm-12 col-xs-12 padding-top-20">
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
                                                            <div class="row margin-left-0 margin-right-0">
                                                                <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                                                                    <asp:Button runat="server" ID="ReadMoreButton" Visible="False" CssClass="btn btn-blue-new" OnClick="ReadMoreButtonClick"/>
                                                                </div>
                                                            </div>
                                                        </FooterTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div class="col-md-4 col-sm-12 col-xs-12">
                                        <div class="book" id="book" runat="server">
                                            <div class="row">
                                                <div class="col-md-12 col-sm-12 col-xs-12 col-book-top">
                                                    <div class="book-top opacity"></div>
                                                    <div class="book-top">
                                                        <div class="float-left" id="priceDesktop" clientidmode="Static" runat="server">
                                                            <span id="msrp2" runat="server" class="msrp" Visible="False">
                                                                <asp:Literal runat="server" ID="PerGuestMsrpLit2"></asp:Literal>
                                                            </span>
                                                            <asp:Literal runat="server" ID="PerGuestLit2"></asp:Literal>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12 col-sm-12 col-xs-12 padding-left-0 padding-right-0">
                                                    <div class="book-now">
                                                        <div class="row product-redemtion hidden-sm hidden-xs">
                                                            <div class="col-md-6 text-center padding-right-0">
                                                                <div class="image">
                                                                    <img src="/images/icon_date.png" class="img-responsive" />
                                                                </div>
                                                                <div class="max-guest">
                                                                    Check-In
                                                                </div>
                                                                <div class="redemption-note">
                                                                    <div class="input-group date" id="checkindatem">
                                                                        <asp:TextBox ID="CheckInDateTextMobile" OnTextChanged="CheckInDateTextMobile_OnTextChanged" placeholder="Select Date" AutoPostBack="True" class="form-control datepicker" runat="server"></asp:TextBox>
                                                                        <span class="input-group-addon">
                                                                            <img src="/images/arrow-down-s.png" class="img-responsive" />
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-6 text-center padding-left-0">
                                                                <div class="image">
                                                                    <img src="/images/icon_people.png" class="img-responsive" />
                                                                </div>
                                                                <div class="max-guest">
                                                                    <asp:Literal runat="server" ID="GuestLit"></asp:Literal>
                                                                </div>
                                                                <div class="form-group">
                                                                    <div class="btn-group tickets dropup">
                                                                        <button type="button" class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                                            <asp:Label CssClass="current-value" runat="server" ID="CurrentTicketDesktop"></asp:Label> 
                                                                            <img src="/images/arrow-down-s.png" class="img-responsive" />
                                                                        </button>
                                                                        <ul class="dropdown-menu dropup ticket-capacity" runat="server" ID="TicketCapacityDesktop"></ul>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row hidden-sm hidden-xs">
                                                            <div class="col-md-8 col-blue">
                                                                <label class="blue">
                                                                    This hotel is extremely popular. Sign up for email alerts when spots open up.
                                                                </label>
                                                            </div>
                                                            <div class="col-md-4 text-right">
                                                                <asp:HiddenField runat="server" ID="IsWaitingListHidden" ClientIDMode="Static" Value="false" />
                                                                <div id="isWaitingList" class="toggle toggle-iphone"></div>
                                                            </div>
                                                        </div>
                                                        <div class="row hidden-sm hidden-xs">
                                                            <div class="col-md-12">
                                                                <hr class="divider-sm" />
                                                            </div>
                                                        </div>
                                                        <div class="row row-total-price hidden-sm hidden-xs">
                                                            <div class="col-md-6">
                                                                <label>TOTAL</label>
                                                            </div>
                                                            <div class="col-md-6">
                                                                <asp:Literal runat="server" ID="TotalLit"></asp:Literal>
                                                            </div>
                                                        </div>
                                                        <a id="AnchorButton" class="AnchorButton" onserverclick="BookNow_Click" runat="server">BOOK NOW</a>
                                                        <div id="ComingSoonAnchorButton" runat="server" class="row row-coming-soon">
                                                            <div class="col-md-12 col-lg-12 visible-lg visible-md">
                                                                <a class="AnchorButton" href="javascript:RequestAccess()">Join Waitlist</a>
                                                            </div>
                                                            <div class="col-sm-6 col-xs-6 visible-sm visible-xs">
                                                                <span class="coming-soon-text">Coming Soon</span>
                                                            </div>
                                                            <div class="col-sm-6 col-xs-6 visible-sm visible-xs">
                                                                <a class="AnchorButton " href="javascript:RequestAccess()">Join Waitlist</a>
                                                            </div>                                                                                                        
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <asp:Repeater runat="server" ID="RptFaqs">
                                            <HeaderTemplate>
                                                <div class="row hidden-md hidden-lg margin-left-0 margin-right-0">
                                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                                        <hr class="divider">
                                                    </div>
                                                </div>
                                                <div class="row margin-left-0 margin-right-0">
                                                    <div class="col-md-12 col-sm-12 col-xs-12 col-fine-print">
                                                        <h4 class="header-fine-print">FAQs</h4>
                                                        <ul class="faqs">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li>
                                                    <a href="javascript:void(0);" class="faq-collapse collapsed" data-toggle="collapse" data-target="#<%#Eval("Id") %>">
                                                        <%#Eval("Name") %>
                                                    </a>
                                                    <div id="<%#Eval("Id") %>" class="collapse">
                                                        <%#Eval("Body") %>
                                                    </div>
                                                </li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                            <li>
                                                                <a class="faq-collapse collapsed" target="_blank" href="<%=Constant.FaqIndexUrl %>">Show more questions</a>
                                                            </li>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        <asp:Repeater runat="server" ID="RptFinePrint">
                                            <HeaderTemplate>
                                                <div class="row hidden-md hidden-lg margin-left-0 margin-right-0">
                                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                                        <hr class="divider">
                                                    </div>
                                                </div>
                                                <div class="row margin-left-0 margin-right-0">
                                                    <div class="col-md-12 col-sm-12 col-xs-12 col-fine-print">
                                                        <h4 class="header-fine-print">Fine Print</h4>
                                                        <ul class="fine-print">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li>
                                                    <%#Eval("Name") %>
                                                </li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                        <div class="row hidden-md hidden-lg margin-left-0 margin-right-0">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <hr class="divider">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="row row-similar-product">
                                    <div class="col-md-11 col-sm-12 col-xs-12 margin-top-30">
                                        <asp:ListView ID="SimilarProductLv" GroupItemCount="3" OnItemDataBound="SimilarProductLv_OnItemDataBound" runat="server">
                                            <LayoutTemplate>
                                                <h4 class="title-c-bo center-mobile padding-b-mobile-20">Customers Also Bought</h4>
                                                <div id="owl-product" class="owl-carousel owl-theme">
                                                    <div runat="server" id="groupPlaceholder"></div>
                                                </div>
                                            </LayoutTemplate>
                                            <GroupTemplate>
                                                <div class="row item">
                                                    <div runat="server" id="itemPlaceholder"></div>
                                                </div>
                                            </GroupTemplate>
                                            <ItemTemplate>
                                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4">
                                                    <a id="HotelItem" runat="server">
                                                        <div class="product-item">
                                                            <asp:Image ID="ProductImage" CssClass="lazyOwl img-responsive" AlternateText="" runat="server" />
                                                            <div class="product-info">
                                                                <div class="type">
                                                                    <%#Eval("ProductName") %>
                                                                </div>
                                                                <div class="hotel-name">
                                                                    <%#Eval("HotelName") %>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="col-md-8 col-sm-12 col-xs-12">
                                                                        <div class="customer-name">
                                                                            <%#Eval("Hotels.Neighborhood") %>
                                                                        </div>
                                                                        <div class="rating">
                                                                            <%#double.Parse(Eval("Hotels.CustomerRating").ToString()) > 0 ? Eval("Hotels.CustomerRatingString") : Eval("Hotels.Rating") %>
                                                                            <span>
                                                                                <%#Eval("Hotels.TotalReviews") %>
                                                                            </span>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-md-4 col-sm-12 col-xs-12">
                                                                        <asp:Label CssClass="price" runat="server" ID="PriceLit"></asp:Label>
                                                                        <span class="max-guest">
                                                                            per guest
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </a>
                                                </div>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <asp:TextBox type="text" runat="server" ID="TotalTicketsText" CssClass="hidden total-tickets-text" AutoPostBack="True" OnTextChanged="TotalTicketsText_OnTextChanged"></asp:TextBox>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID ="TotalTicketsText" EventName ="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID ="AnchorButton" EventName ="serverclick" />
        </Triggers>
    </asp:UpdatePanel>
    <uc1:NewsletterControl runat="server" ID="NewsletterControl" />
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

<asp:Content runat="server" ContentPlaceHolderID="ScriptContentPlaceHolder">
    <%: Scripts.Render("~/bundles/product") %>
    <script>
        window.showAuth = '<%=ShowAuth%>';
        window.restrictDate = '<%= RestrictDateStr %>';

        var _learnq = _learnq || [];
        
        <%if(PublicCustomerInfos != null) { %>
        _learnq.push(['identify', {
            '$email' : '<%=PublicCustomerInfos.EmailAddress %>',
            '$first_name': '<%=PublicCustomerInfos.FirstName %>',
            '$last_name': '<%=PublicCustomerInfos.LastName %>',
            'email': '<%=PublicCustomerInfos.EmailAddress %>',
            'customer_id': '<%=PublicCustomerInfos.CustomerId %>',
        }]);
        <%} %>
        _learnq.push(['track', 'Viewed Product', {
            'item_type': '<%=ProductTypeTrackString%>',
            'item_name': '<%=PublicProduct.ProductName%>',
            'item_hotel_name': '<%=PublicHotel.HotelName%>',
            'item_id': '<%=PublicProduct.ProductId %>',
            'item_location': '<%=string.Format("{0}, {1}", PublicHotel.Neighborhood, PublicHotel.City)%>',
            'item_kids_allowed': '<%=PublicProduct.ProductKidAllowedString%>',
            'item_guest_count': '<%=(PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest)%>',
            'item_photo': '<%=(string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicProduct.ImageUrl).AbsoluteUri))%>',
            'item_price': '<%=RegularPrice%>',
            'item_discount_price': '<%=DiscountPrice %>',
            'item_url': '<%=Request.Url.AbsoluteUri%>',
            'item_description': '<%=WebUtility.HtmlEncode(PublicHotel.Recommendation.Replace("\r\n", "\\n"))%>',
            'market_id': '<%=PublicMarkets != null ? PublicMarkets.Id : 0 %>',
            'market_name': '<%=PublicMarkets != null ? PublicMarkets.LocationName : string.Empty %>'
        }]);
    </script>
    <script type="text/javascript">
        window.blockoutDate = <%=ProductBlockoutDate%>;
        window.tickets = <%=PublicTickets%>;
        function RequestAccess() {
            $.ajax({
                url: "/Handler/RequestAccess.ashx",
                type: "POST",
                dataType: 'json',
                data: { hotelId: <%=PublicProduct.HotelId %>, 
                    customerId: <%=PublicCustomerInfos != null ? PublicCustomerInfos.CustomerId : 0 %>, 
                    email: '<%=PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : string.Empty %>' },
                success: function (result) {
                    $requestAccessModal = $('#requestAccessModal');
                    $requestAccessModal.modal('show');
                }
            });
        }
        function OnSuccess(response) {
               
        }
        $(document).ready(function () {
            $('#requestAccessModal .btn-close')
                .click(function () {
                    //$('.modal-backdrop').remove();
                    $requestAccessModal = $('#requestAccessModal');
                    $requestAccessModal.modal('hide');
                });

            $('body').on('shown.bs.tab', '#main-tabs a', function (event) {
                <% if (AppConfiguration.EnableMixpanel) { %>
                var type = "Pool";
                if ($(this).text().indexOf('gym') != -1) {
                    type = "Gym/Spa";
                }else if($(this).text().indexOf('business') != -1) {
                    type = "Business Services";
                }
                if (mixpanel) {
                    mixpanel.track("Amenity Viewed",
                        {
                            "Amenity Type": type,
                            "referrer": document.referrer
                        });
                }
                <% } %>
            });
        });

        function BeginRequestHandler(sender, args) {
            var control = document.getElementById('<%=AddToWaitListButton.ClientID %>');
            control.disable = true;
            var bookControl = document.getElementById('<%=AnchorButton.ClientID %>');
            bookControl.disable = true;
            var overlay = document.getElementById('hoverlay');
            var modal = document.getElementById('hmodal');
            overlay.removeAttribute('style');
            modal.removeAttribute('style');
        }

        function EndRequestHandler(sender, args) {
            var control = document.getElementById('<%=AddToWaitListButton.ClientID %>');
            control.disable = false;
            var bookControl = document.getElementById('<%=AnchorButton.ClientID %>');
            bookControl.disable = false;
            var overlay = document.getElementById('hoverlay');
            var modal = document.getElementById('hmodal');
            overlay.setAttribute('style', 'display:none;');
            modal.setAttribute('style', 'display:none;');
        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    </script>
</asp:Content>