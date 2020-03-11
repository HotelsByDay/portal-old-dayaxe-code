<%@ Page Title="Confirm" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Confirm.aspx.cs" Inherits="dayaxe.com.Confirm" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="Dayaxe.SendEmail" %>

<asp:Content runat="server" ContentPlaceHolderID="StyleHeader">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/confirm" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptAnalyticsHeader">
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
        <% if(Session["BookingSuccess"] != null) { %>
            fbq('track', 'Purchase', {
                value: '<%=PublicBooking.MerchantPrice * PublicBooking.Quantity %>',
                currency: 'USD',
                content_ids: ['<%= PublicBooking.ProductId %>'],
                content_type: 'product',
                content_name: '<%=string.Format("{0} - {1} - {2}", ProductTypeTrackString, PublicHotels.HotelName, PublicProduct.ProductName) %>'
            });
        <% Session.Remove("BookingSuccess"); } %>
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
            
            mixpanel.track("Hotel Booked");

            <% if (Session["IsRegister"] != null) { %>
                
            //<!-- first time sign up during checkout -->
            window.mixpanel.alias("<%=PublicCustomerInfos.EmailAddress %>");

            window.mixpanel.track("Signed Up", { "referrer": document.referrer });
												   
            mixpanel.people.set({
                '$first_name': '<%=PublicCustomerInfos.FirstName %>',
                '$last_name': '<%=PublicCustomerInfos.LastName %>',
                'Zip Code': '<%=PublicCustomerInfos.ZipCode %>',
                '$email': '<%=PublicCustomerInfos.EmailAddress %>'
            });
            <% } else { %> 
            mixpanel.people.set({
                '$first_name': '<%=PublicCustomerInfos.FirstName %>',
                '$last_name': '<%=PublicCustomerInfos.LastName %>',
                'Zip Code': '<%=PublicCustomerInfos.ZipCode %>'
            });
			window.mixpanel.identify("<%=PublicCustomerInfos.EmailAddress %>"); 
            <% } %>
            
        </script>
    <% } %>
    <!-- end Mixpanel -->

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

        //<!-- GA eCommerce Tracking -->
        ga('require', 'ecommerce');

        ga('ecommerce:addTransaction', {
            'id': '<%= PublicBooking.BookingId %>',
            'affiliation': '',
            'revenue': '<%=PublicBooking.HotelPrice %>',
            'shipping': '',
            'tax': ''
        });

        ga('ecommerce:addItem', {
            'id': '<%= PublicBooking.BookingId %>',
            'name': '<%=string.Format("{0} - {1} - {2}", ProductTypeTrackString, PublicHotels.HotelName, PublicProduct.ProductName) %>',
            'sku': '<%=PublicProduct.ProductId %>',
            'category': '<%=ProductTypeTrackString%>',
            'price': '<%=PublicBooking.HotelPrice %>',
            'quantity': '<%=PublicBooking.Quantity%>'
        });

        ga('ecommerce:send');

    </script>

    <!-- Google Code for Conversion Conversion Page -->
    <script type="text/javascript">
        /* <![CDATA[ */
        var google_conversion_id = 940928114;
        var google_conversion_language = "en";
        var google_conversion_format = "3";
        var google_conversion_color = "ffffff";
        var google_conversion_label = "KTceCPK4nmMQ8tjVwAM";
        var google_conversion_value = "<%=PublicBooking.HotelPrice %>";
        var google_conversion_currency = "USD";
        var google_remarketing_only = false;
        /* ]]> */
    </script>
    <script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
    </script>
    <noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="//www.googleadservices.com/pagead/conversion/940928114/?value=<%=PublicBooking.HotelPrice %>&amp;currency_code=USD&amp;label=KTceCPK4nmMQ8tjVwAM&amp;guid=ON&amp;script=0"/>
</div>
</noscript>
		
    <!-- End Google Analytics -->
		
		
		
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
    <header class="single">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="title">
                        Confirmation
                    </div>
                </div>
            </div>
        </div>
    </header>
    <section class="wrapper style5">
        <div id="owl-demo" class="owl-carousel owl-theme">
            <div class="item">
                <img id="imageProduct" runat="server" alt="Mirror Edge" />
                <div class="overlay-bg overlay-bg-book"></div>
                <div class="overlay-text">
                    <asp:Literal runat="server" ID="ProductNameLit"></asp:Literal>
                            <br />
                    <span><%= string.Format("{0}, {1}", PublicHotels.Neighborhood, PublicHotels.City) %></span>
                    <span class="rating">
                        <%=PublicHotels.CustomerRating > 0 ? PublicHotels.CustomerRatingString : PublicHotels.Rating %>
                    </span>
                </div>
            </div>
        </div>
    </section>
    <div class="container container-book">
        <div class="book">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                    <div class="thanks">
                        <%= PublicBooking.CustomerInfos.FirstName %>!
                        <br />
                        <br />
                        <p>
                            Thanks for booking with DayAxe!<br />
                            Here is a receipt for your purchase. Check your email for your ticket and mobile check-in instructions.
                        </p>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="book-now">
                        <asp:Button ID="btnViewSubscriptionTop" OnClick="btnViewSubscription_Click" runat="server" class="btn btn-blue-all btn-view-reservation" Text="VIEW SUBSCRIPTION" Visible="false"/>
                    </div>
                </div>
            </div>
            <div class="row" id="reminderRow" runat="server" Visible="False">
                <div class="col-md-12 col-sm-12 col-xs-12 col-pass-reminder">
                    <b>REMINDER:</b> You'll be charged $20 if you don't show up.
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <hr class="divider" />
                </div>
            </div>
            <asp:MultiView runat="server" ID="CustomerMV">
                <asp:View runat="server">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                            <div class="book-wrap">
                                <h4 class="text-center header-access">Access <%=ProductTypeString %> In Future</h4>
                                <div class="row">
                                    <div class="col-md-12 col-sm-12 col-xs-12 margin-top-25 margin-bottom-10">
                                        Create a Password:
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Label ID="PasswordErrorMessage" runat="server" Text="" ForeColor="red"></asp:Label>
                                    </div>
                                </div>
                                <div id="form-group">
                                    <div class="proformwrap">
                                        <div class="row">
                                            <div class="col-md-12 col-xs-12">
                                                <div class="input-group">
                                                    <asp:TextBox ID="Password" TextMode="Password" CssClass="form-control" runat="server" placeholder="Enter Password"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="proformwrap">
                                        <div class="row">
                                            <div class="col-md-12 col-xs-12">
                                                <div class="input-group">
                                                    <asp:TextBox ID="PasswordConfirm" TextMode="Password" CssClass="form-control" runat="server" placeholder="Confirm Password"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <asp:Button ID="ConfirmButton" runat="server" Text="Confirm" CssClass="btn btn-blue-all" OnClick="ConfirmButtonClick" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row margin-r-0">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <hr class="divider" />
                        </div>
                    </div>
                </asp:View>
                <asp:View runat="server" runat="server">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                            <span class="confirm-success">Success! Your account has been created.
                                                    <br />
                                You can now access your tickets<br />
                                through your account.
                            </span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-25">
                            <asp:Button ID="MyAccountButton" runat="server" Text="My Account" CssClass="btn btn-blue-new" OnClientClick="window.location = '/my-day-passes'; return false;" />
                        </div>
                    </div>
                    <div class="row margin-r-0">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <hr class="divider" />
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>
            <asp:PlaceHolder ID="AddOnPanel" runat="server" Visible="False">
                <div class="row adds-on">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <asp:ListView ID="AddOnsListView" GroupItemCount="4" OnItemDataBound="AddOnsListView_OnItemDataBound" runat="server">
                            <LayoutTemplate>
                                <h4 class="title-a-t">Add to Your Trip</h4>
                                <div runat="server" id="groupPlaceholder"></div>
                            </LayoutTemplate>
                            <GroupTemplate>
                                <div class="row item">
                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                        <div class="row">
                                            <div runat="server" id="itemPlaceholder"></div>
                                        </div>
                                    </div>
                                </div>
                            </GroupTemplate>
                            <ItemTemplate>
                                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-6">
                                    <a id="HotelItem" runat="server">
                                        <div class="product-item">
                                            <asp:Image ID="ProductImage" CssClass="lazy img-responsive" AlternateText="" runat="server"/>
                                            <div class="product-info">
                                                <div class="row">
                                                    <div class="col-md-8 col-sm-7 col-xs-7">
                                                        <div class="type">
                                                            <%#Eval("ProductName") %>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-4 col-sm-5 col-xs-5">
                                                        <asp:Label CssClass="price" runat="server" ID="PriceLit"></asp:Label>
                                                        <asp:Label runat="server" CssClass="max-guest" id="MaxGuest"></asp:Label>
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
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <hr class="divider"/>
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row details">
                <div class="col-md-12 col-sm-12 col-xs-12 receipt-details padding-left-0 padding-right-0">
                    <h4 class="text-center">Receipt Details</h4>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                            <h5 class="product-title">
                                <asp:Literal runat="server" ID="ProductInfoLit"></asp:Literal>
                            </h5>
                            <%= PublicHotels.StreetAddress %>
                            <br /><%= (string.Format("{0}, {1} {2}", PublicHotels.City, PublicHotels.State, PublicHotels.ZipCode)) %>
                            <br /><%= PublicHotels.PhoneNumber %>
                        </div>
                    </div>
                    <div class="guest-info">
                        <div class="row">
                            <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                                <label>
                                    Guest Name:
                                </label>
                            </div>
                            <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                                <%= (string.Format("{0} {1}", PublicBooking.CustomerInfos.FirstName, PublicBooking.CustomerInfos.LastName)) %>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                                <label>
                                    Booked:
                                </label>
                            </div>
                            <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                                <%= (PublicBooking.BookedDate.ToLosAngerlesTimeWithTimeZone(PublicHotels.TimeZoneId).ToString(Constant.FullDateFormat)) %>
                            </div>
                        </div>
                        <%--<div class="row">
                            <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                                <label>
                                    Amenities:
                                </label>
                            </div>
                            <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                                <%= (PublicBooking.Access) %> <a id="moreInfoLink" runat="server" class="more-detail" target="_blank">More Info</a>
                            </div>
                        </div>--%>
                        <div class="row">
                            <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                                <label>
                                    Booking Id:
                                </label>
                            </div>
                            <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                                <%= (PublicBooking.BookingIdString) %>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-5 col-sm-5 col-xs-5 padding-top-5 text-right">
                                <label>
                                    Check-In Date:
                                </label>
                            </div>
                            <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                                <div id="checkindate">
                                    <asp:Label runat="server" ID="CheckInDateLabel" Text="Not Selected" CssClass="check-in-date"></asp:Label>
                                    <a href="<%=string.Format("/{0}/ViewDayPass.aspx", Page.RouteData.Values["bookingId"]) %>" class="input-group-addon">
                                        <asp:Literal runat="server" ID="CheckInDateButtonLabel" Text="Select"></asp:Literal>
                                    </a>
                                </div>
                                <div class="check-in-description">
                                    Must be selected prior to arrival. <br/>
                                    Same day check-in is available, <br/>
                                    subject to daily availability.
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                                <label>
                                    Tickets:
                                </label>
                            </div>
                            <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                                <asp:Label runat="server" ID="TicketFirstLabel"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                                <span class="promo-fine-print">
                                    <asp:Label runat="server" ID="MaxGuestLabel"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="row row-price-confirm">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="divider margin-top-5 margin-bottom-5" />
                            </div>
                        </div>
                        <div class="row row-price-confirm">
                            <div class="col-md-12 col-sm-12 col-xs-12 col-price-confirm">
                                <div class="row">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        Tickets
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <asp:Label runat="server" ID="TicketLabel"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        Ticket Price
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <asp:Label runat="server" ID="PerPriceLabel"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        <asp:Literal runat="server" ID="TicketNumberLit"></asp:Literal>
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <asp:Literal runat="server" ID="TicketPriceLit"></asp:Literal>
                                    </div>
                                </div>
                                <div class="row" id="promoRow" runat="server">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        Promo
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <asp:Label runat="server" ID="PromoPrice"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" id="taxRow" runat="server" Visible="False">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        Tax
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <asp:Label runat="server" ID="TaxLit"></asp:Label>
                                    </div>
                                </div>
                                <div class="row" id="creditRow" runat="server">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        Credit
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <asp:Label runat="server" ID="CreditPrice"></asp:Label>
                                    </div>
                                </div>
                                <div class="row margin-top-15">
                                    <div class="col-md-12 col-sm-12 col-xs-12">
                                        <hr class="divider margin-top-5 margin-bottom-5" />
                                    </div>
                                </div>
                                <div class="row row-total-confirm">
                                    <div class="col-md-5 col-sm-5 col-xs-5 text-left">
                                        <label class="total-label">
                                            Total
                                        </label>
                                    </div>
                                    <div class="col-md-7 col-sm-7 col-xs-7 text-right">
                                        <span class="confirm-total">
                                            <asp:Literal runat="server" ID="TotalPriceLit"></asp:Literal>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row row-price-confirm" id="PromoAppliedSeperateRow" runat="server" Visible="False">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="divider margin-top-5 margin-bottom-5" />
                            </div>
                        </div>
                        <div class="row" id="PromoAppliedRow" runat="server" Visible="False">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <h4 class="promo-applied-title">Promo Applied</h4>
                            </div>
                            <div class="col-md-12 col-sm-12 col-xs-12 promo-applied">
                                <div>
                                    <h5 class="promo-h5">
                                        <%= PublicDiscountUsed.DiscountName %>
                                    </h5>
                                    <span class="promo-code">
                                        <%=PublicDiscountUsed.Code %>
                                    </span> - <asp:Literal runat="server" ID="DiscountCodeLit"></asp:Literal><br/>
                                    <span class="promo-fine-print">
                                        <%=PublicDiscountUsed.FinePrint %>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <div class="row" id="FinePrintSeperateRow" runat="server">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="divider margin-top-5 margin-bottom-5" />
                            </div>
                        </div>
                        <div class="row" id="FinePrintRow" runat="server">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="divider margin-top-5 margin-bottom-5" />
                            </div>
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <h4>FINE PRINT</h4>
                            </div>
                            <div class="col-md-12 col-sm-12 col-xs-12 fine-print">
                                <ul>
                                    <li>
                                        Valid for <b>30 days from purchase</b>; one time use only
                                    </li>
                                    <li>
                                        Check-in date must be selected prior to arrival (same day selection is available, subject to daily availability)
                                    </li>
                                    <li>
                                        Check-in date can be changed at no charge anytime 48 hours or more before check-in
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                                For more information please see <a class="blue" href="http://land.dayaxe.com/terms">Terms &amp; Conditions</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12 text-center thanks-again">
                    <h2>Thanks again for booking
                    </h2>
                    <br />
                    <span>Have fun</span>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="book-now">
                        <asp:Button id="AnchorButton" ClientIDMode="Static" OnClick="HtmlAnchor_Click" runat="server" class="AnchorButton" Text="VIEW TICKET"></asp:Button>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <div class="book-now">
                        <asp:Button id="btnViewSubscriptionBottom" ClientIDMode="Static" OnClick="btnViewSubscription_Click" runat="server" class="AnchorButton" Text="VIEW SUBSCRIPTION" Visible="false"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <footer class="footer-book">
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
    <script>
        var _learnq = _learnq || [];
        
        _learnq.push(['identify', {
            '$email': '<%=PublicCustomerInfos.EmailAddress %>',
            '$first_name': '<%=PublicCustomerInfos.FirstName %>',
            '$last_name': '<%=PublicCustomerInfos.LastName %>',
            'email': '<%=PublicCustomerInfos.EmailAddress %>',
            'customer_id': '<%=PublicCustomerInfos.CustomerId %>',
            'zip_code': '<%=PublicCustomerInfos.ZipCode %>',
            "referral_code": '<%=Session["ReferralCode"]%>'
        }]);

        _learnq.push(['track', 'Booked Item', {
            'item_type': '<%=ProductTypeTrackString%>',
            'item_name': '<%=PublicProduct.ProductName%>',
            'item_hotel_name': '<%=PublicHotels.HotelName%>',
            'item_id': '<%=PublicProduct.ProductId %>',
            'item_validation_location': '<%=PublicHotels.CheckInPlace%>',
            'item_location': '<%=string.Format("{0}, {1}", PublicHotels.Neighborhood, PublicHotels.City)%>',
            'item_zip_code': '<%=PublicCustomerInfos.ZipCode %>',
            'item_kids_allowed': '<%=PublicProduct.KidAllowedString%>',
            'item_guest_count': '<%=(PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest)%>',
            'item_photo': '<%=(string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicProduct.ImageUrl).AbsoluteUri))%>',
            'booking_expiration_date': '<%=(PublicBooking.ExpiredDate.HasValue ? PublicBooking.ExpiredDate.Value.ToLosAngerlesTimeWithTimeZone(PublicHotels.TimeZoneId).ToString(Constant.DateFormat) : string.Empty)%>',
            'booking_redemption_period': '<%=(PublicProduct.RedemptionPeriod <= 0 ? Constant.DefaultRedemptionPeriod : PublicProduct.RedemptionPeriod)%>',
            'booking_id': '<%=PublicBooking.BookingId%>',
            'booking_url': '<%=string.Format("{0}/{1}/ViewDayPass.aspx", EmailConfig.DefaultImageUrlSendEmail, Page.RouteData.Values["bookingId"])%>',
            'booking_status': '<%=((Enums.BookingStatus)PublicBooking.PassStatus).ToString()%>',
            'booking_checkin_date': '<%=PublicBooking.CheckinDate.HasValue ? PublicBooking.CheckinDate.Value.ToString(Constant.DateFormat) : string.Empty%>',
            'market_id': '<%=PublicMarkets != null ? PublicMarkets.Id : 0 %>',
            'market_name': '<%=PublicMarkets != null ? PublicMarkets.LocationName : string.Empty %>'
        }])

        <% if (Session["IsRegister"] != null) { %>
            _learnq.push(['track', 'Signed Up', {
                '$first_name': '<%=PublicCustomerInfos.FirstName %>',
                '$last_name': '<%=PublicCustomerInfos.LastName %>',
                "referrer": document.referrer,
                "referral_code": '<%=Session["ReferralCode"]%>'
            }]);
        <% } %>
    </script>
    <%: Scripts.Render("~/bundles/confirm") %>
</asp:Content>