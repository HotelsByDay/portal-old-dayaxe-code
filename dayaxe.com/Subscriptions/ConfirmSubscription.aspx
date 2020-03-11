<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ConfirmSubscription.aspx.cs" Inherits="dayaxe.com.Subscriptions.ConfirmSubscriptionPage" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MetaContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/confirm" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
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
            value: '<%=PublicSubscriptionCycles.MerchantPrice * PublicSubscriptionCycles.Quantity %>',
            currency: 'USD',
            content_ids: ['<%= PublicSubscriptionBooking.Id %>'],
            content_type: 'product',
            content_name: '<%=PublicSubscriptions.Name %>'
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
            
            mixpanel.track("Subscription Booked");

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
            'id': '<%= PublicSubscriptionBooking.Id %>',
            'affiliation': '',
            'revenue': '<%=PublicSubscriptionCycles.Price %>',
            'shipping': '',
            'tax': ''
        });

        ga('ecommerce:addItem', {
            'id': '<%= PublicSubscriptionBooking.Id %>',
            'name': '<%=PublicSubscriptions.Name %>',
            'sku': '<%=PublicSubscriptions.Id %>',
            'category': '<%=ProductTypeTrackString%>',
            'price': '<%=PublicSubscriptionCycles.Price %>',
            'quantity': '<%=PublicSubscriptionBooking.Quantity%>'
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
        var google_conversion_value = "<%=PublicSubscriptionCycles.Price %>";
        var google_conversion_currency = "USD";
        var google_remarketing_only = false;
        /* ]]> */
    </script>
    <script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
    </script>
    <noscript>
        <div style="display:inline;">
            <img height="1" width="1" style="border-style:none;" alt="" src="//www.googleadservices.com/pagead/conversion/940928114/?value=<%=PublicSubscriptionCycles.Price %>&amp;currency_code=USD&amp;label=KTceCPK4nmMQ8tjVwAM&amp;guid=ON&amp;script=0"/>
        </div>
    </noscript>
		
    <!-- End Google Analytics -->
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
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
<div class="header-ghost"></div>
<section class="wrapper style5">
    <div id="owl-demo" class="owl-carousel owl-theme">
        <div class="item">
            <img id="imageProduct" runat="server" alt="Mirror Edge" />
            <div class="overlay-bg overlay-bg-book"></div>
            <div class="overlay-text">
                <%=PublicSubscriptions.Name %>
                <br />
                <span><%= PublicSubscriptions.ProductHighlight %></span>
            </div>
        </div>
    </div>
</section>
<div class="container container-book">
<div class="book">
<div class="row">
    <div class="col-md-12 col-sm-12 col-xs-12 text-center">
        <div class="thanks">
            <%= PublicSubscriptionBooking.CustomerInfos.FirstName %>!
            <br />
            <br />
            <p>
                Thanks for purchasing with DayAxe! Your <%=PublicSubscriptions.Name %> is now active <asp:Literal runat="server" ID="FirstPassReserveLit"></asp:Literal>.
            </p>
            <asp:Button runat="server" CssClass="btn btn-blue-all btn-view-reservation" Text="Reserve Pass" ID="ViewReservation" Visible="False" OnClick="ViewReservationOnClick"/>
        </div>
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
                    <h4 class="text-center header-access">Access In Future</h4>
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
<div class="row details">
    <div class="col-md-12 col-sm-12 col-xs-12 receipt-details padding-left-0 padding-right-0">
        <h4 class="text-center">Receipt Details</h4>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                <h5 class="product-title">
                    <%=PublicSubscriptions.Name %>
                </h5>
                <%= PublicSubscriptions.ProductHighlight %>
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
                    <%= (string.Format("{0} {1}", PublicSubscriptionBooking.CustomerInfos.FirstName, PublicSubscriptionBooking.CustomerInfos.LastName)) %>
                </div>
            </div>
            <div class="row">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        Purchased:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <%= (PublicSubscriptionBooking.BookedDate.ToLosAngerlesTime().ToString(Constant.FullDateFormat)) %>
                </div>
            </div>
            <div class="row">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        Membership ID:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <%= (PublicDiscounts.Code) %>
                </div>
            </div>
            <div class="row">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        Addl Pass Promo:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <span class="friend-promo">
                        GOLDFRIEND - 10% OFF
                    </span>
                    <br/>
                    <span>
                        10% OFF additional passes for friends &amp; family, limit 2 uses per month.
                    </span>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <hr class="divider margin-top-5 margin-bottom-5" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        Price:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <%=Helper.FormatPrice(PublicSubscriptions.Price) %>
                </div>
            </div>
            <div class="row" id="promoRow" runat="server">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        Promo:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <asp:Label runat="server" ID="PromoPrice"></asp:Label>
                </div>
            </div>
            <div class="row" id="creditRow" runat="server">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        Credit:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <asp:Label runat="server" ID="CreditPrice"></asp:Label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-5 col-sm-5 col-xs-5 text-right">
                    <label>
                        TOTAL:
                    </label>
                </div>
                <div class="col-md-7 col-sm-7 col-xs-7 text-left">
                    <asp:Literal runat="server" ID="TotalPriceLit"></asp:Literal>
                </div>
            </div>
            <div class="row" id="PromoAppliedSeperateRow" runat="server" Visible="False">
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
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <hr class="divider margin-top-5 margin-bottom-5" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <h4>Plan Rules</h4>
                </div>
                <div class="col-md-12 col-sm-12 col-xs-12 fine-print">
                    <%=PublicSubscriptions.WhatYouGet %>
                </div>
            </div>
			<div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <hr class="divider margin-top-5 margin-bottom-5" />
                </div>
            </div>
				<div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <h4>Reserving Day Passes</h4>
                </div>
                <div class="col-md-12 col-sm-12 col-xs-12 fine-print" style="width:100%; margin: 0; padding:10px; padding-left: 25px">
                    <ul style="width: 100%;">
                        <li>Ensure you are logged in to the account with active DayAxe membership</li>
                        <li>Browse day passes of your interest</li>
                        <li>Select the one you’d like to reserve</li>
                        <li>Select your check-in date</li>
                        <li>You should see $0 as your total</li>
                        <li>Proceed to checkout and complete booking</li>
                        <li>You will receive a confirmation of your reservation</li>
                        <li>If you are reserving another day pass for the same date, you will be charged full price for your extra day pass. You are only entitled to 1 free day pass per day.</li>
                        <li>If you’d like to get 2 passes, your free day pass  and one for a friend, select the quantity of tickets you need, the total will adjust for a 1 free day pass</li>
                        <li>To use your friend’s & family discount you need to manually apply your promo code which you can find on your membership page or your gold pass email receipt.</li>
                        <li>For more info please see <a href="https://dayaxe.zendesk.com/hc/en-us/categories/115000289754-Gold-Pass-Membership">Gold Pass FAQ</a></li>
                        <li>If you'd like to contact us, please email us at <a href="mailto:help@dayaxe.com">help@dayaxe.com</a></li>
                    </ul>
                </div>
            </div>
				<div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <hr class="divider margin-top-5 margin-bottom-5" />
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
        <h2>Thanks again for purchasing
        </h2>
        <br />
        <span>Have fun</span>
    </div>
</div>
<div class="row">
    <div class="col-md-12 col-sm-12 col-xs-12">
        <div class="book-now">
            <asp:Button id="AnchorButton" ClientIDMode="Static" OnClick="HtmlAnchor_Click" runat="server" class="AnchorButton" Text="RESERVE YOUR DAY PASS"></asp:Button>
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
<asp:Content ID="Content6" ContentPlaceHolderID="ScriptContentPlaceHolder" Runat="Server">
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

        _learnq.push(['track', 'Subscription Booked Item', {
            'item_type': '<%=ProductTypeTrackString%>',
            'item_name': '<%=PublicSubscriptions.Name%>',
            'item_id': '<%=PublicSubscriptions.Id %>',
            'item_zip_code': '<%=PublicCustomerInfos.ZipCode %>',
            'item_photo': '<%=(string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicSubscriptions.ImageUrl).AbsoluteUri))%>',
            'booking_id': '<%=PublicSubscriptionBooking.Id%>',
            'booking_status': '<%=((Enums.SubscriptionBookingStatus)PublicSubscriptionBooking.Status).ToString()%>'
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

