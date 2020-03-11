<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bookProduct.aspx.cs" Inherits="dayaxe.com.BookProduct" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Register Src="~/Controls/AuthControl.ascx" TagPrefix="uc1" TagName="AuthControl" %>
<%@ Register Src="~/Controls/NewsletterControl.ascx" TagPrefix="uc1" TagName="NewsletterControl" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Book Hotel Day Pass, Resort Pass, Cabana, Daybed, Spa Pass | DayAxe</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1.0, user-scalable=0" />
    <%-- klaviyo analytics script --%>
    <script type="text/javascript" src="//static.klaviyo.com/onsite/js/klaviyo.js?company_id=<%=AppConfiguration.KlaviyoApiKey %>"></script>

    <!--[if lte IE 8]><script src="/assets/js/ie/html5shiv.js"></script><![endif]-->
    <!--[if lte IE 8]><script src="/assets/js/ie/respond.min.js"></script><![endif]-->
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <!--[if lte IE 8]><link rel="stylesheet" href="/assets/css/ie8.css" /><![endif]-->
    <!--[if lte IE 9]><link rel="stylesheet" href="/assets/css/ie9.css" /><![endif]-->
    <!--bootstrap-->
    <%--<link href='https://fonts.googleapis.com/css?family=Dosis:400,500,700,300' rel='stylesheet' type='text/css'/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css" />--%>
    <webopt:bundlereference runat="server" path="~/Content/book" />
    <% if (AppConfiguration.EnableTracking) { %>
    <!-- Facebook Pixel Code -->

    <script>
        !function(f,b,e,v,n,t,s){if(f.fbq)return;n=f.fbq=function(){n.callMethod?
                n.callMethod.apply(n,arguments):n.queue.push(arguments)};if(!f._fbq)f._fbq=n;
            n.push=n;n.loaded=!0;n.version='2.0';n.queue=[];t=b.createElement(e);t.async=!0;
            t.src=v;s=b.getElementsByTagName(e)[0];s.parentNode.insertBefore(t,s)}(window,
            document,'script','//connect.facebook.net/en_US/fbevents.js');
        fbq('init', '<%=AppConfiguration.FacebookKey %>');
        fbq('track', 'PageView');   
        fbq('track', 'AddToCart', {
            content_ids: ['<%=PublicProduct!= null ? PublicProduct.ProductId : 0 %>'],
            content_type: 'product',
            content_name: '<%=Helper.GetHotelName(PublicProduct) %>'
        });

    </script>
    <noscript>
        <img height="1" width="1" style="display:none" src='<%=String.Format("https://www.facebook.com/tr?id={0}&ev=PageView&noscript=1", AppConfiguration.FacebookKey) %>"' />
    </noscript>

    <!-- End Facebook Pixel Code -->

     <!-- start Mixpanel -->
    <style type="text/css">
        #datePickAlternate{ display: none; }
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
                "Hotel Name": "<%=Helper.GetHotelName(PublicProduct) %>",
                "Hotel Type": "<%=((Enums.Hoteltype)(PublicHotel.HoteltypeId ?? 0)).ToString()%>",
                "Hotel Price": "$<%=NormalPrice.ToString(NormalPrice%1 == 0.0 ? "C0" : "C", new CultureInfo("en-US"))%>"
            });
            mixpanel.track("Booking Started", { "referrer": document.referrer });
        </script>
    <% } %>

    <!-- end Mixpanel -->

    <!-- Google Analytics Start -->
    <script>
        (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
                (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
                m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
        })(window,document,'script','//www.google-analytics.com/analytics.js','ga');
        ga('create', '<%=AppConfiguration.GoogleKey %>', 'auto');
        ga('send', 'pageview');
    </script>
    <!-- Google Analytics End -->
    <% } %>
   </head>
<body>
	<div class="hoteloverlaywrap"></div>
  	<div class="hotelmodal">
    	RESERVING <%=ProductTypeString %> <br />FOR A LIMITED TIME...
        <div class="imgspin text-center"><img src="/images/loading.gif" alt="" /></div>
    </div>
<form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
    <asp:HiddenField runat="server" ID="IsAllowCheckout" ClientIDMode="Static"/>
        <uc1:AuthControl runat="server" ID="AuthControl" />
        <asp:HiddenField runat="server" ID="NewBookingHidden" ClientIDMode="Static" Value="false"/>
        <asp:HiddenField runat="server" ID="HidSelectedDate" ClientIDMode="Static"/>
        <asp:HiddenField ID="HidTicket" runat="server" ClientIDMode="Static"/>

        <div id="page-wrapper" class="book-page">
            <!-- Main -->
            <article id="main">
                <header class="single">
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <a class="go-back" href="#" runat="server" id="goBack">
                                    <img src="/images/arrow.png" alt="arrow-left" /> <span class="hidden-md hidden-sm hidden-xs">Back</span>
                                </a>
                                <div class="title">
                                    Checkout
                                </div>
                            </div>
                        </div>
                    </div>
                </header>
                <div class="header-ghost"></div>
                <section class="wrapper style5">
                    <div id="owl-demo" class="owl-carousel owl-theme">
                        <div class="item">
                            <img src="<%=PublicProduct != null ? PublicProduct.ImageUrl : string.Empty %>" alt="Mirror Edge"/>
                            <div class="overlay-bg overlay-bg-book"></div>
                            <div class="overlay-text">
                                <asp:Literal ID="hotelname" runat="server"></asp:Literal>
                                <span><asp:Literal runat="server" ID="Neighborhood"></asp:Literal></span>
                            </div>
                        </div>
                  </div>
                </section>
            </article>
            <asp:UpdatePanel runat="server" ID="BookProductUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="freePassModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static" runat="server" Visible="False" clientidmode="Static">
                        <div class="vertical-alignment-h">
                            <div class="modal-dialog vertical-align-c">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <h4 class="modal-title text-center">Member Pass</h4>
                                    </div>
                                    <div class="modal-body text-center">
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="location">
                                                    We are sorry, you cannot apply your free day pass to this reservation:
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox runat="server" Enabled="False" ID="ExixtsFreePassCheckBox"/>
                                                            You already have 1 active free pass <% if (TotalActiveBookingWithSubscription > 0)
                                                                                                   { %>
                                                                on <%= ActiveDateSubscription.ToString("MM/dd") %>
                                                            <% } %>
                                                        </label>
                                                    </div>
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox runat="server" Enabled="False" ID="OutsideCheckbox"/>
                                                            Check-in date is outside of a 24 hour window
                                                        </label>
                                                    </div>
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox runat="server" Enabled="False" ID="NonDayPassCheckbox"/>
                                                            You are reserving a non-day pass product
                                                        </label>
                                                    </div>
                                                    <div class="checkbox">
                                                        <label>
                                                            <asp:CheckBox runat="server" Enabled="False" ID="MaxNumberPassesCheckbox"/>
                                                            You've exceeded your max number of passes per month
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row button-submit">
                                            <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                                                <div class="btn btn-save" data-dismiss="modal">OK</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="newBookingModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static" runat="server" Visible="False" clientidmode="Static">
                        <div class="vertical-alignment-h">
                            <div class="modal-dialog vertical-align-c">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <h4 class="modal-title text-center">Buying an Extra Ticket?</h4>
                                    </div>
                                    <div class="modal-body text-center">
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="location">
                                                    You've just purchased a ticket less than 3 minutes ago.<br class="hidden-sm hidden-xs"/>
                                                    Are you trying to purchase another one?
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row button-submit">
                                            <div class="col-md-6 col-sm-6 col-xs-6 col-left">
                                                <div class="btn btn-save btn-cancel" data-dismiss="modal">No</div>
                                            </div>
                                            <div class="col-md-6 col-sm-6 col-xs-6 col-right">
                                                <div class="btn btn-save btn-close" id="btnYes">Yes</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
        
                    <div id="soldOutModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static" runat="server" Visible="False" clientidmode="Static">
                        <div class="vertical-alignment-h">
                            <div class="modal-dialog vertical-align-c">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <h4 class="modal-title text-center">
                                            <asp:Literal runat="server" ID="ProductAvailableTitleLit"></asp:Literal>
                                        </h4>
                                    </div>
                                    <div class="modal-body text-center">
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="location">
                                                    <asp:Literal runat="server" ID="ProductAvailableLit"></asp:Literal>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row button-submit">
                                            <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                                                <div class="btn btn-save btn-close">OK</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
        
                    <div id="upgradeModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static" runat="server" Visible="True" clientidmode="Static">
                        <div class="vertical-alignment-h">
                            <div class="modal-dialog vertical-align-c">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h4 class="modal-title text-center">
                                            Exclusive Offer
                                        </h4>
                                        <h5 class="modal-sub-title text-center">
                                            FREE Unlimited Monthly Access
                                        </h5>
                                    </div>
                                    <div class="modal-body text-center">
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12 extra-text">
                                                Visit as many hotels as you like!
                                                <br/>
                                                First month is on us!
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-7 col-sm-12 col-xs-12 padding-right-5">
                                                <img src="<%= Page.ResolveClientUrl("/images/subscription.png") %>" class="img-responsive img-sub-upgrade"/>
                                            </div>
                                            <div class="col-md-5 col-sm-12 col-xs-12 padding-left-5 upgrade-subscription-list text-left">
                                                <div class="row">
                                                    <div class="col-xs-6">
                                                        <span class="span-10px-bold-4a">
                                                            Regular:
                                                        </span>
                                                    </div>
                                                    <div class="col-xs-6 text-right">
                                                        <span class="span-18px-500-4a line-through">
                                                            <asp:Literal runat="server" ID="SubscriptionPriceLit"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-6">
                                                        <span class="span-10px-bold-4a line-height-40">
                                                            Your Price Today:
                                                        </span>
                                                    </div>
                                                    <div class="col-xs-6 text-right">
                                                        <span class="span-28px-500-red">
                                                            <asp:Literal runat="server" ID="ChargePriceLit"></asp:Literal>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-12">
                                                        <hr class="sub-upgrade"/>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-xs-12">
                                                        <span class="Up-to-30-passes-per">
                                                            Up to 30 passes per month!
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12 col-sm-12 col-xs-12">
                                                <div class="sub-term">
                                                    By clicking OK, I understand my membership automatically renews at <asp:Literal runat="server" ID="SubscriptionPrice2Lit"></asp:Literal> and I can cancel
                                                     on site at any time. I agree to the <a class="blue" href="http://land.dayaxe.com/terms" target="_blank">Terms & Conditions</a>.
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row button-submit">
                                            <div class="col-md-6 col-md-push-6 col-sm-12 col-xs-12">
                                                <asp:Button CssClass="btn btn-save btn-accept" runat="server" Text="OK" ID="btnAccept" OnClick="PurchaseSubscriptionButton_OnClick"></asp:Button>
                                            </div>
                                            <div class="col-md-6 col-md-pull-6 col-sm-12 col-xs-12">
                                                <asp:Button runat="server" CssClass="view-booking" ID="btnDecline" OnClick="ViewBookingOnClick" Text="No Thanks, I Hate Fun."></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="container book">
            	        <div class="row">
	                        <div class="col-md-5 col-md-push-7 col-sm-12 col-xs-12 col-right">
	                            <div class="wrap-purchase">
	                                <div class="row hidden-lg hidden-md">
	                                    <div class="col-md-12 col-sm-12 col-xs-12">
	                                        <div class="countdownwrap">
	                                            <div id="compactCountdown2"></div>
	                                        </div>
	                                    </div>
	                                </div>
	                                <div class="row">
	                                    <div class="col-md-12 col-sm-12 col-xs-12">
	                                        <div id="form-group">
	                                            <div class="input-group greenborder">
	                                                <div class="row">
	                                                    <div class="col-md-6 col-xs-6">
                                                            <div class="btn-group tickets">
                                                                <button type="button" class="btn dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                                    <asp:Label CssClass="current-value" runat="server" ID="CurrentTicket"></asp:Label> 
                                                                    <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
                                                                </button>
                                                                <ul class="dropdown-menu ticket-capacity" runat="server" ID="TicketCapacity"></ul>
                                                            </div>
	                                                    </div>
	                                                    <div class="col-md-6 col-xs-6">
	                                                        <div class="perpricebox">
	                                                            <span class="upto-text" id="msrp" runat="server" style="text-decoration: line-through;" Visible="False">
	                                                                 was <asp:Literal id="msrpPrice" runat="server"></asp:Literal>
	                                                            </span>
	                                                            <asp:Label class="total-price" runat="server" ID="perMoneyPrice"></asp:Label>
                                                                <%--<asp:Label class="upto-text" runat="server" ID="UpToGuestLit"></asp:Label>--%>
	                                                        </div>
                                                            <%--<div class="rate">
                                                                <asp:Label runat="server" ID="RateLabel"></asp:Label>
                                                            </div>--%>
	                                                    </div>
	                                                </div>
                                                    <div class="row border" id="freePassRow" runat="server">
                                                        <div class="col-md-12 col-xs-12 text-right">
                                                            <div class="checkbox free-pass-checkbox">
                                                                <label>
                                                                    <asp:CheckBox runat="server" ID="FreePassCheckBox"/>
                                                                    1 free day pass
                                                                </label>
                                                            </div>
                                                            <a href="javascript:void(0);" class="why-charge" id="WhyChargeLink" data-toggle="tooltip" data-html="true" data-placement="bottom" runat="server" Visible="False">
                                                                <span class="glyphicon glyphicon-question-sign"></span>
                                                                Why am I being charged?
                                                            </a>
                                                        </div>
                                                    </div>
                                                    <div class="row border" id="rowTicket" runat="server" Visible="False">
                                                        <div class="col-md-6 col-xs-6">
                                                            <asp:Literal runat="server" ID="TicketNumberLit"></asp:Literal>
                                                        </div>
                                                        <div class="col-md-6 col-xs-6 text-right">
                                                            <asp:Literal runat="server" ID="TicketPriceLit"></asp:Literal>
                                                        </div>
                                                    </div>
                                                    <div class="row row-between" id="promoRow" runat="server" Visible="False">
                                                        <div class="col-md-6 col-xs-6">
                                                            Promo
                                                        </div>
                                                        <div class="col-md-6 col-xs-6 text-right">
                                                            <asp:Literal runat="server" ID="PromoLit"></asp:Literal>
                                                        </div>
                                                    </div>
                                                    <div class="row row-between" id="taxRow" runat="server" Visible="False">
                                                        <div class="col-md-6 col-xs-6">
                                                            Tax
                                                        </div>
                                                        <div class="col-md-6 col-xs-6 text-right">
                                                            <asp:Literal runat="server" ID="TaxLit"></asp:Literal>
                                                        </div>
                                                    </div>
                                                    <div class="row row-between" id="creditRow" runat="server" Visible="False">
                                                        <div class="col-md-6 col-xs-6">
                                                            Credit
                                                        </div>
                                                        <div class="col-md-6 col-xs-6 text-right">
                                                            <asp:Literal runat="server" ID="CreditLit"></asp:Literal>
                                                        </div>
                                                    </div>
                                                    <div class="row border margin-top-15">
                                                        <div class="col-md-4 col-xs-4">
                                                            <div class="texttotal">Total</div>
                                                        </div>
                                                        <div class="col-md-8 col-xs-8">
                                                            <div class="pricebox">
	                                                            <span class="total-price">
	                                                                <asp:Literal id="moneyPrice" runat="server"></asp:Literal>
	                                                            </span>
	                                                        </div>
                                                        </div>
                                                    </div>
	                                                <div class="row border">
	                                                    <div class="col-md-4 col-sm-4 col-xs-4 first l-height-40">
	                                                        <asp:LinkButton runat="server" Text="Add Promo" CssClass="add-promo" OnClick="AddPromoClick"></asp:LinkButton>
	                                                    </div>
                                                        <div class="col-md-5 col-sm-5 col-xs-5">
                                                            <div class="margin-top-7">
                                                                <asp:TextBox id="PromoText" Visible="False" class="form-control discount-text" runat="server"/>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-3 col-sm-3 col-xs-3 last l-height-40">
                                                            <asp:Button runat="server" Text="Add" ID="AddPromoButton" Visible="False" OnClick="ApplyPromoClick" CssClass="btn btn-blue-new btn-apply"/>
                                                        </div>
	                                                </div>
	                                                <div class="row row-error " id="rowMessage" runat="server" Visible="False">
	                                                    <div class="col-md-12 col-sm-12 col-xs-12">
	                                                        <asp:Label CssClass="error-message" runat="server" ID="MessageLabel"></asp:Label>
	                                                    </div>
	                                                </div>
	                                                <div class="row" id="discountInfoRow" runat="server" Visible="False">
	                                                    <div class="col-md-12 promo-applied">
	                                                        <h5 class="promo-h5">
	                                                            <asp:Literal runat="server" ID="DiscountOffLit"></asp:Literal>
	                                                        </h5>
	                                                        <span>
	                                                            <%=PublicDiscountUsed != null ? PublicDiscountUsed.FinePrint : string.Empty %>
	                                                        </span>
	                                                    </div>
	                                                </div>
	                                            </div>
	                                        </div>
	                                    </div>
	                                </div>
	                                <div class="purchase">
	                                    <div class="row">
	                                        <div class="col-md-12 col-sm-12 col-xs-12 padding-left-0 padding-right-0">
	                                            <div class="book book-md">
	                                                <div class="note">
	                                                    <asp:Literal runat="server" ID="LitDisclaimer"></asp:Literal>
	                                                    By completing your order you agree with the <a class="blue" href="http://land.dayaxe.com/terms" target="_blank">Terms & Conditions</a>.
	                                                </div>
	                                                <asp:Button id="AnchorButton" OnClick="HtmlAnchor_Click" runat="server" CssClass="AnchorButton btngray" Text="PURCHASE"></asp:Button>
	                                            </div>
	                                        </div>
	                                    </div>
	                                </div>
	                            </div>
                            </div>
	                        <div class="col-md-7 col-md-pull-5 col-sm-12 col-xs-12 col-left">
	                            <div class="book-wrap">
	                                <div class="row hidden-sm hidden-xs">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="countdownwrap">
                	                            <div id="compactCountdown"></div>
                                            </div>
                                        </div>
                                    </div>
	                                <div class="row">
	                                    <div class="col-md-12 col-sm-12 col-xs-12 text-center center-block">
	                                        <div class="bookguestinfo center-block">Guest Check-in Details 
                                                <asp:PlaceHolder runat="server" ID="HasAccount">
                                                    or <a href="#" class="blue" onclick="javascript:login();">Sign In</a>
                                                </asp:PlaceHolder>
	                                        </div>
	                                    </div>
	                                </div>
	                                <div class="row">
	                                    <div class="col-md-12 col-sm-12 col-xs-12 center-block">
	                                        <div class="errorCode text-center" id="errorCode">
	                                            <asp:Literal runat="server" ID="ErrorMessageLit"></asp:Literal>
	                                        </div>
	                                    </div>
	                                </div>
	                                <div id="form-group">
	                                    <!-- BOF PAYMENT FORM -->
                                        <div class="proformwrap">
                                            <div class="row date" id="wrapCheckInDate" runat="server" Visible="False">
                                                <div class="col-md-5 col-xs-5">
                                                    <span class="date-label">Check-In Date:</span>
                                                </div>
                                                <div class="col-md-7 col-xs-7 text-right">
                                                    <div class="select-date-later" id="selectLater" runat="server">
                                                        Select Later
                                                    </div>
                                                    <div class="select-date-now" id="selectNow" runat="server">
                                                        Select Now
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row date">
                                                <div class="col-md-6 col-xs-6">
                                                    <asp:Label CssClass="date-label-2" runat="server" ID="DateLabelLabel">Same day selection is available,<br/>
                                                         subject to daily availability.</asp:Label>
                                                </div>
                                                <div class="col-md-6 col-xs-6">
                                                    <div class="btn input-group date" id="checkindate">
                                                        <asp:TextBox ID="CheckInDateText" class="form-control datepicker" AutoPostBack="True" OnTextChanged="CheckInDateText_OnTextChanged" runat="server"/>
                                                        <span class="input-group-addon">
                                                            <i class="glyphicon glyphicon-triangle-bottom"></i>
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
	                                    <div class="proformwrap">
	                                        <div class="row">
	                                            <div class="col-md-6 col-xs-6">
	                                                <div class="input-group">
	                                                    <asp:TextBox placeholder="First name" id="FirstName" class="form-control firstname" runat="server"/>
	                                                </div>
	                                            </div>
	                                            <div class="col-md-6 col-xs-6">
	                                                <div class="input-group">
	                                                    <asp:TextBox placeholder="Last Name" id="LastName" class="form-control lastname" runat="server"/>
	                                                </div>
	                                            </div>
	                                        </div>
	                                    </div>
                                        <asp:MultiView runat="server" ID="MVCardInfo" ActiveViewIndex="0">
                                            <asp:View runat="server">
                                                <asp:PlaceHolder runat="server" ID="PayByCreditRow" Visible="False">
                                                    <div class="proformwrap">
                                                        <div class="row">
                                                            <div class="col-md-12 col-xs-12">
                                                                <div class="checkbox">
                                                                    <label>
                                                                        <asp:CheckBox runat="server" ID="IsPayByCreditCheckBox"/>
                                                                        <asp:Literal runat="server" ID="PayByCreditLiteral"></asp:Literal>
                                                                    </label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
	                                            <div class="proformwrap">
	                                                <div class="row">
	                                                    <div class="col-md-12 col-xs-12">
	                                                        <div class="input-group">
	                                                            <div class="cardtypewrap"></div>
	                                                            <asp:TextBox TextMode="SingleLine" placeholder="Credit Card Number" MaxLength="19" data-maxlength="" min="0" max="9999999999999999999" type="tel" ID="cctextbox" class="form-control cctextbox" runat="server"></asp:TextBox>
	                                                        </div>
	                                                    </div>
	                                                </div>
	                                            </div>
	                                            <div class="proformwrap">
	                                                <div class="row">
	                                                    <div class="col-md-4 col-xs-4">
	                                                        <div class="input-group">
	                                                            <input name="txtexpdat" runat="server" placeholder="MM / YY" maxlength="7" id="txtexpdat" class="form-control"/>
	                                                        </div>
	                                                    </div>
	                                                    <div class="col-md-3 col-xs-3">
	                                                        <div class="input-group">
	                                                            <input name="txtseccode" runat="server" placeholder="CVV" id="txtseccode" maxlength="5" class="form-control"/>
	                                                        </div>
	                                                    </div>
	                                                    <div class="col-md-5 col-xs-5">
	                                                        <div class="input-group">
	                                                            <asp:TextBox name="txtzipcode" placeholder="Billing Zip Code" type="tel" id="txtzipcode" maxlength="5" runat="server" class="form-control"></asp:TextBox>
	                                                        </div>
	                                                    </div>
	                                                </div>
	                                            </div>
                                            </asp:View>
                                            <asp:View runat="server">
                                                <div class="proformwrap cardtype">
	                                                <div class="row">
	                                                    <div class="col-md-8 col-sm-12 col-xs-12 padding-right-0">
	                                                        <div class="btn-group btn-credit-card">
	                                                            <button type="button" class="btn dropdown-toggle btn-card-type" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
	                                                                <span>
	                                                                    <asp:Literal runat="server" ID="CardInfoLit" Mode="Transform"></asp:Literal>
	                                                                </span>
	                                                                <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
                                                                    <div class="credit-info">
                                                                        <asp:Literal runat="server" ID="DCreditInfoLit"></asp:Literal>
                                                                    </div>
	                                                            </button>
	                                                            <ul class="dropdown-menu charge-type" runat="server" ID="ChargeTypeUl">
	                                                                <li id="DayaxeCreditCardRow" runat="server" class="" Visible="False">
	                                                                    <a href="#">
	                                                                        <div class="checkbox">
	                                                                            <label>
	                                                                                <asp:CheckBox runat="server" ID="DCreditCheckBox" Checked="True" AutoPostBack="True" OnCheckedChanged="DCreditCheckBox_OnCheckedChanged"/>
	                                                                                <asp:Literal runat="server" ID="DCardInfoLiteral"></asp:Literal>
	                                                                            </label>
	                                                                        </div>
	                                                                    </a>
	                                                                </li>
	                                                                <li id="CreditCardRow" runat="server" Visible="False">
	                                                                    <a href="#">
	                                                                        <div class="radio">
	                                                                            <label>
	                                                                                <asp:RadioButton runat="server" ID="CreditCardCheckBox" Checked="True"/>
	                                                                                <span class='card-type'>
	                                                                                    <asp:Literal runat="server" ID="CardTypeLiteral"></asp:Literal>
	                                                                                </span>
                                                                                    <asp:Literal runat="server" ID="CardInfoLiteral"></asp:Literal>
	                                                                            </label>
	                                                                        </div>
	                                                                    </a>
	                                                                </li>
	                                                            </ul>
	                                                        </div>
	                                                    </div>
                                                        <div class="col-md-4 col-sm-12 col-xs-12 col-use-new-card">
                                                            <asp:Button ID="UseNewCard" runat="server" Text="Use New Card" OnClick="UseNewCardClick" CssClass="btn btn-blue-new"></asp:Button>
                                                        </div>
	                                                </div>
	                                            </div>
                                            </asp:View>
                                        </asp:MultiView>
                                        <asp:PlaceHolder runat="server" ID="EmailPlaceHolder">
                                            <div class="proformwrap">
	                                            <div class="row">
	                                                <div class="col-md-12 col-xs-12">
	                                                    <div class="input-group">
	                                                        <asp:TextBox ID="email" runat="server" type="email" class="form-control" name="email" aria-describedby="basic-addon2"></asp:TextBox>
	                                                    </div>
	                                                </div>
	                                                <div class="emailplaceholder">Email <span>(day pass will be emailed here)</span></div>
	                                            </div>
	                                        </div>
                                        </asp:PlaceHolder>
	                                    <!-- EOF PAYMENT FORM -->
	                                </div>
	                            </div>
	                        </div>
	                    </div>
			        </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID ="CheckInDateText" EventName ="TextChanged" />
                    <asp:AsyncPostBackTrigger ControlID ="btnAccept" EventName ="Click" />
                    <asp:AsyncPostBackTrigger ControlID ="btnDecline" EventName ="Click" />
                    <asp:AsyncPostBackTrigger ControlID ="AnchorButton" EventName ="Click" />
                    <asp:AsyncPostBackTrigger ControlID ="DCreditCheckBox" EventName ="CheckedChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <uc1:NewsletterControl runat="server" ID="NewsletterControl" />
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
        </div>
        <script>
              var _learnq = _learnq || [];
              _learnq.push(['account', '<%=AppConfiguration.KlaviyoApiKey %>']);
              //(function () {
              //var b = document.createElement('script'); b.type = 'text/javascript'; b.async = true;
              //b.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'a.klaviyo.com/media/js/analytics/analytics.js';
              //var a = document.getElementsByTagName('script')[0]; a.parentNode.insertBefore(b, a);
              //})();

            <%if(PublicCustomerInfos != null) { %>
            _learnq.push(['track', 'Checked Out Product', {
                '$email': '<%=PublicCustomerInfos.EmailAddress %>',
                '$first_name': '<%=PublicCustomerInfos.FirstName %>',
                '$last_name': '<%=PublicCustomerInfos.LastName %>',
                'email': '<%=PublicCustomerInfos.EmailAddress %>',
                'customer_id': '<%=PublicCustomerInfos.CustomerId %>',
                'item_type': '<%=ProductTypeTrackString%>',
                'item_name': '<%=PublicProduct.ProductName%>',
                'item_hotel_name': '<%=PublicHotel.HotelName%>',
                'item_id': '<%=PublicProduct.ProductId %>',
                'item_location': '<%=string.Format("{0}, {1}", PublicHotel.Neighborhood, PublicHotel.City)%>',
                'item_kids_allowed': '<%=PublicProduct.KidAllowedString%>',
                'item_guest_count': '<%=(PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest)%>',
                'item_photo': '<%=(string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicProduct.ImageUrl).AbsoluteUri))%>',
                'item_price': '<%=NormalPrice%>',
                'item_discount_price': '<%=DiscountPrice%>',
                'item_url': '<%=GetProductUrl()%>',
                'cart_url': '<%=Request.Url.AbsoluteUri%>',
                'item_description': '<%=HttpUtility.HtmlEncode(PublicHotel.Recommendation.Replace("\r\n", "\\n"))%>',
                'market_id': '<%=PublicMarkets != null ? PublicMarkets.Id : 0 %>',
                'market_name': '<%=PublicMarkets != null ? PublicMarkets.LocationName : string.Empty %>'
            }]);
            <%} else { %> 
            _learnq.push(['track', 'Checked Out Product', {
                'item_type': '<%=ProductTypeTrackString%>',
                'item_name': '<%=PublicProduct.ProductName%>',
                'item_hotel_name': '<%=PublicHotel.HotelName%>',
                'item_id': '<%=PublicProduct.ProductId %>',
                'item_location': '<%=string.Format("{0}, {1}", PublicHotel.Neighborhood, PublicHotel.City)%>',
                'item_kids_allowed': '<%=PublicProduct.KidAllowedString%>',
                'item_guest_count': '<%=(PublicProduct.MaxGuest <= 0 ? Constant.DefaultMaxGuest : PublicProduct.MaxGuest)%>',
                'item_photo': '<%=(string.Format("{0}", new Uri(new Uri(AppConfiguration.CdnImageUrlDefault), PublicProduct.ImageUrl).AbsoluteUri))%>',
                'item_price': '<%=NormalPrice%>',
                'item_discount_price': '<%=DiscountPrice%>',
                'item_url': '<%=GetProductUrl()%>',
                'cart_url': '<%=Request.Url.AbsoluteUri%>',
                'item_description': '<%=HttpUtility.HtmlEncode(PublicHotel.Recommendation.Replace("\r\n", "\\n"))%>',
                'market_id': '<%=PublicMarkets != null ? PublicMarkets.Id : 0 %>',
                'market_name': '<%=PublicMarkets != null ? PublicMarkets.LocationName : string.Empty %>'
            }]);
            <%} %>
        </script>
        <!-- Scripts -->
        <script>
            window.searchPage = "<%= string.IsNullOrEmpty((string)Session["SearchPage"]) ? Constant.SearchPageDefault : Session["SearchPage"].ToString() %>";
            window.showAuth = '<%=ShowAuth%>';
            window.tickets = <%=PublicTickets%>;
            window.productType = '<%=ProductTypeString %>';
            window.blockoutDate = <%=ProductBlockoutDate%>;
            window.restrictDate = '<%= RestrictDateStr %>';
        </script>
        <script type="text/javascript">
            function BeginRequestHandler(sender, args) {
                var acceptControl = document.getElementById('btnAccept');
                if (acceptControl) {
                    acceptControl.disabled = true;
                }

                var declineControl = document.getElementById('btnDecline');
                if (declineControl) {
                    declineControl.disabled = true;
                }

                var purchaseButton = document.getElementById('AnchorButton');
                if (purchaseButton) {
                    purchaseButton.disabled = true;
                }
            }

            function EndRequestHandler(sender, args) {
                var acceptControl = document.getElementById('btnAccept');
                if (acceptControl) {
                    acceptControl.disabled = false;
                }

                var declineControl = document.getElementById('btnDecline');
                if (declineControl) {
                    declineControl.disabled = false;
                }

                var purchaseButton = document.getElementById('AnchorButton');
                if (purchaseButton) {
                    purchaseButton.disabled = false;
                }
            }

            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        </script>
        <%: Scripts.Render("~/bundles/book") %>
    </form>
</body>
</html>
