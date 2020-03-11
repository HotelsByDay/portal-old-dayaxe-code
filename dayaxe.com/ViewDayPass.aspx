<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ViewDayPass.aspx.cs" Inherits="dayaxe.com.ViewDayPass" %>

<%@ Import Namespace="DayaxeDal" %>

<asp:Content runat="server" ContentPlaceHolderID="StyleHeader">
    <webopt:BundleReference runat="server" Path="~/Content/main" />
    <webopt:BundleReference runat="server" Path="~/Content/daypass" />
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
        mixpanel.track("Ticket Viewed", { "referrer": document.referrer });
    </script>
    <% } %>

    <!-- end Mixpanel -->

    <!--  Start Google Analytics  -->
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

    <!--  Start Google Analytics  -->
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True" EnablePartialRendering="False"></asp:ScriptManager>
    <div class="row main-row" id="ContainerMobile" runat="server" visible="False">
        <div class="col-md-12 col-sm-12 col-xs-12">
            <div class="row" style="margin-left: 0; margin-right: 0;">
                <div class="content-area content-area-fixed">
                    <div class="content-top full"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="main-content">
                        <div class="page-title">
                            <div class="row" style="padding: 11px 0">
                                <div class="col-md-8 col-sm-8 col-xs-8">
                                    <a href="/" class="navbar-brand">
                                        <img src="/Images/logo.png" class="img-responsive" alt="Logo" />
                                    </a>
                                    <div class="sub-title"><%=PublicProduct.ProductName %></div>
                                </div>
                                <div class="col-md-4 col-sm-4 col-xs-4 text-right">
                                    <a href="#" id="switcher1" style="display: none" onclick="onSwitcher1Click()">CHECK-IN</a>
                                    <a href="#" id="switcher2" onclick="onSwitcher2Click()">DETAILS
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="page1" style="display: none;">
                <% if (PublicBooking.ProductId != 0)
                    { %>
                <% if (!string.IsNullOrEmpty(PublicProduct.WhatYouGet))
                    { %>
                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="content-container terms">
                                <h6 class="title">
                                    <img src="/images/present-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">WHAT YOU GET</span>
                                </h6>
                                <div class="body">
                                    <%= PublicProduct.WhatYouGet %>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <% } %>
                <% if (!string.IsNullOrEmpty(PublicProduct.Service))
                    { %>
                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="content-container terms">
                                <h6 class="title">
                                    <img src="/images/service-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">YOUR SERVICE</span>
                                </h6>
                                <div class="body">
                                    <%=PublicProduct.Service %>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <% } %>
                <% } %>
                <div class="content-area">
                    <div class="content-top line-normal"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="content-container amenities">
                                <h6 class="title">
                                    <img src="/images/amenities-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">AMENITIES</span>
                                </h6>
                            </div>
                        </div>
                    </div>
                    <div class="main-content">
                        <ul id="main-tabs" class="nav nav-tabs" role="tablist">
                            <asp:Literal runat="server" ID="LiMainTab"></asp:Literal>
                        </ul>
                        <div id="main-tabs-content" role="tabpanel">
                            <!-- Tab panes -->
                            <div class="tab-content padding-t-30">
                                <asp:PlaceHolder runat="server" Visible='<%# PublicHotel.AmentiesItem.PoolActive %>'>
                                    <div role="tabpanel" class="tab-pane fade in active" id="restaurant">
                                        <div class="row padding-bottom-15">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <ul class="services">
                                                    <% if (PublicHotel.PoolFeatures != null)
                                                        { %>
                                                    <% for (int i = 0; i < PublicHotel.PoolFeatures.Length; i++)
                                                        { %>
                                                    <li>
                                                        <span><%= PublicHotel.PoolFeatures[i] %></span></li>
                                                    <% } %>
                                                    <% } %>
                                                </ul>
                                            </div>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.PoolHours))
                                            { %>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="content-container padding-top-0">
                                                    <h6 class="title">
                                                        <img src="/images/amenities-open-icon-white.png" alt="" class="img-responsive" />
                                                        <span class="text">HOURS</span>
                                                    </h6>
                                                    <span class="value"><%= PublicHotel.AmentiesItem.PoolHours %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.GymActive %>'>
                                    <div role="tabpanel" class="tab-pane fade" id="sports-club">
                                        <div class="row padding-bottom-15">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <ul class="services">
                                                    <% if (PublicHotel.GymFeatures != null)
                                                        { %>
                                                    <% for (int i = 0; i < PublicHotel.GymFeatures.Length; i++)
                                                        { %>
                                                    <li>
                                                        <span><%= PublicHotel.GymFeatures[i] %></span></li>
                                                    <% } %>
                                                    <% } %>
                                                </ul>
                                            </div>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.GymHours))
                                            { %>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="content-container padding-top-0">
                                                    <h6 class="title">
                                                        <img src="/images/amenities-open-icon-white.png" alt="" class="img-responsive" />
                                                        <span class="text">HOURS</span>
                                                    </h6>
                                                    <span class="value"><%= PublicHotel.AmentiesItem.GymHours %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.SpaActive %>'>
                                    <div role="tabpanel" class="tab-pane fade" id="spa-club">
                                        <div class="row padding-bottom-15">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <ul class="services">
                                                    <% if (PublicHotel.SpaFeatures != null)
                                                        { %>
                                                    <% for (int i = 0; i < PublicHotel.SpaFeatures.Length; i++)
                                                        { %>
                                                    <li>
                                                        <span><%= PublicHotel.SpaFeatures[i] %></span></li>
                                                    <% } %>
                                                    <% } %>
                                                </ul>
                                            </div>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.SpaHours))
                                            { %>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="content-container padding-top-0">
                                                    <h6 class="title">
                                                        <img src="/images/amenities-open-icon-white.png" alt="" class="img-responsive" />
                                                        <span class="text">HOURS</span>
                                                    </h6>
                                                    <span class="value"><%= PublicHotel.AmentiesItem.SpaHours %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.BusinessActive %>'>
                                    <div role="tabpanel" class="tab-pane fade" id="pick-up">
                                        <div class="row padding-bottom-15">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <ul class="services">
                                                    <% if (PublicHotel.OfficeFeatures != null)
                                                        { %>
                                                    <% for (int i = 0; i < PublicHotel.OfficeFeatures.Length; i++)
                                                        { %>
                                                    <li>
                                                        <span><%= PublicHotel.OfficeFeatures[i] %></span></li>
                                                    <% } %>
                                                    <% } %>
                                                </ul>
                                            </div>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.BusinessCenterHours))
                                            { %>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="content-container padding-top-0">
                                                    <h6 class="title">
                                                        <img src="/images/amenities-open-icon-white.png" alt="" class="img-responsive" />
                                                        <span class="text">HOURS</span>
                                                    </h6>
                                                    <span class="value"><%= PublicHotel.AmentiesItem.BusinessCenterHours %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.DinningActive %>'>
                                    <div role="tabpanel" class="tab-pane fade" id="dining">
                                        <div class="row padding-bottom-15">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <ul class="services">
                                                    <% if (PublicHotel.DinningFeatures != null)
                                                        { %>
                                                    <% for (int i = 0; i < PublicHotel.DinningFeatures.Length; i++)
                                                        { %>
                                                    <li>
                                                        <span><%= PublicHotel.DinningFeatures[i] %></span></li>
                                                    <% } %>
                                                    <% } %>
                                                </ul>
                                            </div>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.DinningHours))
                                            { %>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="content-container padding-top-0">
                                                    <h6 class="title">
                                                        <img src="/images/amenities-open-icon-white.png" alt="" class="img-responsive" />
                                                        <span class="text">HOURS</span>
                                                    </h6>
                                                    <span class="value"><%= PublicHotel.AmentiesItem.DinningHours %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible='<%#PublicHotel.AmentiesItem.EventActive %>'>
                                    <div role="tabpanel" class="tab-pane fade" id="event">
                                        <div class="row padding-bottom-15">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <ul class="services">
                                                    <% if (PublicHotel.EventFeatures != null)
                                                        { %>
                                                    <% for (int i = 0; i < PublicHotel.EventFeatures.Length; i++)
                                                        { %>
                                                    <li>
                                                        <span><%= PublicHotel.EventFeatures[i] %></span></li>
                                                    <% } %>
                                                    <% } %>
                                                </ul>
                                            </div>
                                        </div>
                                        <% if (!string.IsNullOrEmpty(PublicHotel.AmentiesItem.EventHours))
                                            { %>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="content-container padding-top-0">
                                                    <h6 class="title">
                                                        <img src="/images/amenities-open-icon-white.png" alt="" class="img-responsive" />
                                                        <span class="text">HOURS</span>
                                                    </h6>
                                                    <span class="value"><%= PublicHotel.AmentiesItem.EventHours %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <% } %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="BlackoutDaysPlaceHolder" runat="server" Visible="False">
                                    <div class="row margin-r-0">
                                        <div class="col-md-4 col-sm-12 col-xs-12 center-mobile padding-b-mobile-20">
                                            <label>Ticket Valid</label>
                                        </div>
                                        <div class="col-md-8 col-sm-12 col-xs-12 center-mobile padding-top-10">
                                            <div class="margin-bottom-20">One time use only, valid for 3 days, excluding blackout days:</div>
                                            <ul class="blackout-days">
                                                <asp:PlaceHolder runat="server" ID="LitBlackoutDays"></asp:PlaceHolder>
                                            </ul>
                                        </div>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row">
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <div class="content-container padding-right-0">
                                <h6 class="title">
                                    <img src="/images/parking-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">PARKING INFO</span>
                                </h6>
                                <div class="body">
                                    <%=PublicHotel.HotelParking %>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <div class="content-container padding-left-0">
                                <h6 class="title">
                                    <img src="/images/wifi-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">WIFI</span>
                                </h6>
                                <div class="body">
                                    Inquire at check-in
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="content-area" runat="server" id="DiscountRow" visible="False">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row">
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <div class="content-container padding-right-0">
                                <h6 class="title">
                                    <img src="/images/price-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">DISCOUNT</span>
                                </h6>
                                <div class="body">
                                    <%=(!string.IsNullOrEmpty(PublicHotel.HotelDiscountCode) ? string.Format("{0}% OFF", PublicHotel.HotelDiscountPercent) : string.Empty) %>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <div class="content-container padding-left-0">
                                <h6 class="title no-icon">
                                    <span class="text">DISCOUNT CODE</span>
                                </h6>
                                <div class="body">
                                    <%=PublicHotel.HotelDiscountCode %>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <div class="content-container padding-right-0">
                                <h6 class="title no-icon">
                                    <span class="text">APPLICABLE FOR</span>
                                </h6>
                                <div class="body">
                                    <%=PublicHotel.HotelDiscountDisclaimer %>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <div class="content-container padding-left-0">
                                <h6 class="title no-icon">
                                    <span class="text">FINE PRINT</span>
                                </h6>
                                <div class="body">
                                    Show ticket and the above discount code at checkout. Discounts are not automatically applied to purchases.
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom full"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="main-content">
                        <div class="content-container terms">
                            <h6 class="title">
                                <img src="/images/waiver-icon-white.png" alt="" class="img-responsive" />
                                <span class="text">TERMS & CONDITIONS</span>
                            </h6>
                            <div class="body">
                                Ticket holder voluntarily assumes all risks and danger incidental to the day stay for which the ticket is issued,
                    whether occurring prior to, during or later the day stay. Ticket holder voluntarily agrees that the property management,
                    facility, participants, participating clubs, Dayaxe, and all of their respective parents, affiliated entities, agents, officers, directors, owners and employees are expressly
                    released by ticket holder from any claims arising from such causes. By continuing, you agree to the following:
                    <ul>
                        <% if (!string.IsNullOrEmpty(PublicHotel.TermsAndConditions))
                            { %>
                        <li>
                            <%= (string.Format("{0} {1}, {2}", PublicHotel.HotelName, PublicHotel.Neighborhood, PublicHotel.City)) %> <a href='<%= PublicHotel.TermsAndConditions %>' class="blue" target="blank">Terms &amp; Conditions</a>
                        </li>
                        <% } %>
                        <li>DayAxe <a href="http://land.dayaxe.com/terms" class="blue" target="blank">Terms &amp; Conditions</a></li>
                    </ul>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
            <div id="page2">
                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row check-in">
                        <div class="col-md-6 col-sm-6 col-xs-6 padding-right-0 padding-left-30">
                            <div class="content-container padding-right-0">
                                <h6 class="title">
                                    <img src="/images/mobile-phone-icon-white.png" alt="" class="img-responsive" />
                                    <span class="text">CHECK-IN</span>
                                </h6>
                                <div class="body check-in-place">
                                    <asp:Literal runat="server" ID="CheckInPlaceLit"></asp:Literal>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6 padding-left-0">
                            <div class="content-container padding-left-0 padding-right-5">
                                <div class="body gray">
                                    Show this mobile ticket and photo ID at check-in
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="content-bottom-left"></div>
                    <div class="content-bottom-right"></div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 padding-right-0">
                            <div class="content-container padding-left-20 padding-bottom-0">
                                <h6 class="title">
                                    <span class="text">CHECK-IN NAME</span>
                                </h6>
                                <div class="body gray">
                                    <span class="confirm-name"><%= (string.Format("{0} {1}", PublicBooking.CustomerInfos.FirstName, PublicBooking.CustomerInfos.LastName)) %></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6 col-sm-6 col-xs-6 padding-right-0">
                            <div class="row">
                                <div class="col-xs-7 padding-left-0">
                                    <div class="content-container padding-left-20 padding-bottom-0">
                                        <h6 class="title">
                                            <span class="text">TICKETS</span>
                                        </h6>
                                        <div class="body gray">
                                            <label>
                                                <%= PublicBooking.Quantity %>
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xs-5 padding-left-0 padding-right-0">
                                    <div class="content-container padding-bottom-0">
                                        <h6 class="title">
                                            <span class="text nowrap">EACH TICKET</span>
                                        </h6>
                                        <div class="body gray">
                                            <label>
                                                <%= string.Format("{0} adult{1}", PublicProduct.MaxGuest, PublicProduct.MaxGuest == 1 ? string.Empty : "s") %>
                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 col-xs-12 col-sm-12">
                                    <div class="content-container padding-top-0 padding-right-0">
                                        <div class="body gray">
                                            <asp:Label runat="server" ID="KidAllowLabel"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="content-container padding-top-0">
                                <h6 class="title">
                                    <span class="text">Hotel</span>
                                </h6>
                                <div class="body">
                                    <%=PublicHotel.HotelName %><br />
                                    <%= PublicHotel.StreetAddress %><br />
                                    <%= (string.Format("{0}, {1} {2}", PublicHotel.City, PublicHotel.State, PublicHotel.ZipCode)) %><br />
                                    <%= PublicHotel.PhoneNumber %>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6 padding-right-5 padding-left-0">
                            <div class="content-container padding-right-0">
                                <div class="body text-center gray">
                                    <img class="img" src="/images/qr_code.png" alt="Ticket QR"></img>
                                    <div style="margin-top: 10px; font-size: 10px;">
                                        <span><%= (PublicBooking.BookingIdString) %></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="content-area">
                    <div class="content-top line-dash"></div>
                    <div class="content-left"></div>
                    <div class="content-right"></div>
                    <div class="content-bottom full"></div>
                    <div class="content-top-left"></div>
                    <div class="content-top-right"></div>
                    <div class="main-content">
                        <div class="row">
                            <div class="col-md-6 col-sm-6 col-xs-6 padding-right-0">
                                <div class="content-container status text-center">
                                    <div class="title">
                                        <img src="/images/calendar-icon-white.png" alt="" class="img-responsive line-break calendar" />
                                        <div class="text line-break">CHECK-IN DATE</div>

                                    </div>
                                    <div class="value line-break date check-in-date-d">
                                        <asp:TextBox runat="server" ID="CheckInDateText" AutoPostBack="True" OnTextChanged="CheckInDateText_OnTextChanged" CssClass="datepicker hidden" ClientIDMode="Static"></asp:TextBox>
                                        <asp:Label runat="server" ID="CheckInDateLabel" CssClass="input-group-addon not-selected"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6 col-sm-6 col-xs-6 padding-left-0">
                                <div class="content-container status text-center content-container-footer">
                                    <div class="title">
                                        <img src="/images/barcode-icon-white.png" alt="" class="img-responsive line-break barcode" />
                                        <div class="text line-break padding-top-0">STATUS</div>

                                    </div>
                                    <div class="value line-break" id="StatusLabel" runat="server"><%= ((Enums.BookingStatus)PublicBooking.PassStatus).ToString() %></div>
                                    <div class="date-value line-break">
                                        <asp:Label runat="server" CssClass="date-label-daypass gray" ID="DateLabel"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row footer-row">
                <div class="col-md-4 col-sm-4 col-xs-4" style="padding-right: 0; padding-left: 7px;">
                    <div class="daypassbtn btn-save" id="CheckInDateDiv" runat="server">
                        <asp:LinkButton CssClass="select-checkin-date" OnClick="CheckInDateLink_OnClick" runat="server" ID="CheckInDateLink">SELECT
                    <br/>
                    CHECK-IN DATE
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="col-md-4 col-sm-4 col-xs-4" style="padding: 0 10px;">
                    <div class="daypassbtn btn-calendar">
                        <span class="addtocalendar atc-style-blue">
                            <a class="atcb-link">ADD TO CALENDAR</a>
                            <var class="atc_event">
                                <var class="atc_date_start"><%=PublicBooking.BookedDate.ToString("yyyy-MM-dd hh:mm:ss") %></var>
                                <var class="atc_date_end"><%=PublicBooking.ExpiredDate.Value.ToString("yyyy-MM-dd hh:mm:ss") %></var>
                                <var class="atc_timezone">America/Los_Angeles</var>
                                <var class="atc_title"><%= string.Format("{0} at {1}", PublicProduct.ProductName, PublicHotel.HotelName) %></var>
                                <var class="atc_description"></var>
                                <var class="atc_location"><%= PublicHotel.City %></var>
                                <var class="atc_organizer">DayAxe</var>
                                <var class="atc_organizer_email">help@dayaxe.com</var>
                            </var>
                        </span>
                    </div>
                </div>
                <div class="col-md-4 col-sm-4 col-xs-4" style="padding-left: 0; padding-right: 7px;">
                    <div class="daypassbtn btn-validate">
                        <a id="StaffValidate">STAFF
                    <br class="hidden-md hidden-lg" />
                            VALIDATE</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="book" id="containerDesktop" runat="server" visible="True">
        <div class="row" style="padding: 20px;">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <h2 class="sub-header" style="text-transform: capitalize;">DayAxe Tickets are
                    <br />
                    not available to view on desktop.</h2>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <p class="padding-t-20">
                    Our Tickets are fully electronic and MOBILE ONLY. No printing needed. Just show up at the hotel front desk with your ticket on your mobile phone and they will validate your ticket in your mobile browser. See <a href="https://vimeo.com/180120208" target="_blank">how DayAxe works here</a>.
                </p>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <h4 class="sub-header padding-t-20">There are two ways to locate your MOBILE ONLY TICKET:</h4>
                <p class="padding-l-20 padding-t-20">
                    (1) Locate your purchase confirmation email on your mobile phone. In the confirmation email, locate the button "VIEW TICKET". Tap on it and you will get to your ticket in your mobile browser.
                </p>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">

                <p class="padding-l-20 padding-t-20">
                    (2) Go to your mobile phone. Access <a href="/">www.dayaxe.com</a> in any mobile browser. Once you are on the website, tap menu (hamburger icon) on the top right corner. On the menu, tap "Login" item. Once you are logged in, you will see "My Tickets" menu item. Click on that and you will get to the page that lists all your Tickets, active, expired and used.
                </p>
            </div>
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <p style="padding-top: 20px;">See other ticket related <a target="_blank" href="https://dayaxe.zendesk.com/hc/en-us/sections/207335247-FAQ">DayAxe FAQs here</a></p>
            </div>
        </div>
    </div>

    <div id="validateModal" class="modal fade" role="dialog" data-keyboard="false">
        <div class="vertical-alignment-h">
            <div class="modal-dialog vertical-align-c">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Ticket Validation</h4>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-10 margin-bottom-10">
                                <p>
                                    Staff, please enter validation code
                                    <br />
                                    provided to you by DayAxe.
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <input type="password" id="pinCode" maxlength="4" value="" pattern="[0-9]*" inputmode="numeric" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-15 margin-bottom-15 text-center height-35">
                                <span id="errorMessage" class="alert hidden" role="alert"></span>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row no-gutter ml-none mr-none">
                            <div class="col-xs-12 progress hidden">
                                <img src="/images/loading.gif" class="img-responsive loading" alt="Processing..." />
                                Processing... Please wait
                            </div>
                            <div class="col-xs-6">
                                <button type="button" class="close btn " data-dismiss="modal" aria-label="Close">Cancel</button>
                            </div>
                            <div class="col-xs-6">
                                <button type="button" id="validate" class="btn btn-save">Staff Validate</button>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div id="updateNotPossible" class="modal fade" role="dialog" data-keyboard="false">
        <div class="vertical-alignment-h">
            <div class="modal-dialog vertical-align-c">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Update Not Possible</h4>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-10 margin-bottom-10">
                                <p>
                                    We are sorry. You cannot change your check-in date 48 hours before the arrival. Note, "no shows" are charged at full price of the ticket.
                                    <br />
                                    <br />
                                    If you need further assistance please contact our customer support.
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-save" data-dismiss="modal">OK</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel runat="server" ID="CheckInDateChangePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="updateFail" class="modal fade" role="dialog" data-keyboard="false">
                <div class="vertical-alignment-h">
                    <div class="modal-dialog vertical-align-c">
                        <!-- Modal content-->
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <h4 class="modal-title">We Are Sorry</h4>
                            </div>
                            <div class="modal-body">
                                <div class="row">
                                    <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-10 margin-bottom-10">
                                        <p>
                                            <asp:Literal runat="server" ID="UpdateNotPossibleLit"></asp:Literal>
                                            <br />
                                            <br />
                                            Please try again later or contact our customer service for further assistance.
                                        </p>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-save" data-dismiss="modal">OK</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="confirmUpdate" class="modal fade" role="dialog" data-keyboard="false">
                <div class="vertical-alignment-h">
                    <div class="modal-dialog vertical-align-c">
                        <!-- Modal content-->
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                <h4 class="modal-title">Check-In Date Change</h4>
                            </div>
                            <div class="modal-body">
                                <div class="row">
                                    <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-10 margin-bottom-10">
                                        <p>
                                            <b>Check-In Date:
                                                <asp:Literal runat="server" ID="CheckInDateLit"></asp:Literal></b>
                                            <br />
                                            <b>Price Difference:
                                                <asp:Label CssClass="price" runat="server" ID="PriceDiffLabel"></asp:Label></b>
                                            <br />
                                            <br />
                                            <asp:Literal runat="server" ID="ConfirmLit"></asp:Literal>
                                            <br />
                                            <br />
                                            Note: Changes within 48 hours of a check-in are final.
                                        </p>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-cancel" data-dismiss="modal">CANCEL</button>
                                <button type="button" class="btn btn-save">SUBMIT</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="CheckInDateText" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="CheckInDateLink" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <div id="updateSuccess" class="modal fade" role="dialog" data-keyboard="false" clientidmode="Static" runat="server">
        <div class="vertical-alignment-h">
            <div class="modal-dialog vertical-align-c">
                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Update Success</h4>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-10 margin-bottom-10">
                                <p>
                                    Your check-in date has been updated to <b><%= PublicBooking.CheckinDate.HasValue ? PublicBooking.CheckinDate.Value.ToString(Constant.DateFormat) : string.Empty %></b>.
                                    <br />
                                    <br />
                                </p>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-save" data-dismiss="modal">OK</button>
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

        _learnq.push(['track', 'Booking Status Changed', {
            '$email': '<%=PublicCustomerInfos.EmailAddress %>',
            'email': '<%=PublicCustomerInfos.EmailAddress %>',
            'customer_id': '<%=PublicCustomerInfos.CustomerId %>',
            'item_type': '<%=ProductTypeTrackString%>',
            'item_name': '<%=PublicProduct.ProductName%>',
            'item_hotel_name': '<%=PublicHotel.HotelName%>',
            'item_id': '<%=PublicProduct.ProductId %>',
            'item_location': '<%=string.Format("{0}, {1}", PublicHotel.Neighborhood, PublicHotel.City)%>',
            'item_photo': '<%=(string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicProduct.ImageUrl).AbsoluteUri))%>',
            'booking_id': '<%=PublicBooking.BookingId%>',
            'booking_code': '<%=PublicBooking.BookingIdString%>',
            'booking_url': '<%=Request.Url.AbsoluteUri%>',
            'booking_status': '<%=((Enums.BookingStatus)PublicBooking.PassStatus).ToString()%>',
            'market_id': '<%=PublicMarkets != null ? PublicMarkets.Id : 0 %>',
            'market_name': '<%=PublicMarkets != null ? PublicMarkets.LocationName : string.Empty %>'
        }]);
    </script>
    <script>
        window.bookId = '<%= Page.RouteData.Values["bookingId"] %>';
        window.tickets = <%=PublicTickets%>;
        window.blockoutDate = <%=ProductBlockoutDate%>;
    </script>
    <%: Scripts.Render("~/bundles/daypass") %>
</asp:Content>
