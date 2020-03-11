<%@ Page Language="C#" AutoEventWireup="true" CodeFile="bookGiftCard.aspx.cs" EnableViewState="true" Inherits="dayaxe.com.GiftCards.BookGiftCardPage" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Register Src="~/Controls/AuthControl.ascx" TagPrefix="uc1" TagName="AuthControl" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gift Card | DayAxe</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1.0, user-scalable=0" />
    <!--[if lte IE 8]><script src="/assets/js/ie/html5shiv.js"></script><![endif]-->
    <!--[if lte IE 8]><script src="/assets/js/ie/respond.min.js"></script><![endif]-->
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <!--[if lte IE 8]><link rel="stylesheet" href="/assets/css/ie8.css" /><![endif]-->
    <!--[if lte IE 9]><link rel="stylesheet" href="/assets/css/ie9.css" /><![endif]-->
    <!--bootstrap-->
    <webopt:bundlereference runat="server" path="~/Content/bookgiftcard" />
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
            content_ids: ['eGiftCards'],
            content_type: 'giftcard',
            content_name: 'Gift Card'
        });

    </script>
    <noscript>
        <img height="1" width="1" style="display:none" src='<%=String.Format("https://www.facebook.com/tr?id={0}&ev=PageView&noscript=1", AppConfiguration.FacebookKey) %>"' />
    </noscript>

    <!-- End Facebook Pixel Code -->
    
    <!-- Start of dayaxe Zendesk Widget script -->
    <script>/*<![CDATA[*/window.zEmbed || function (e, t) { var n, o, d, i, s, a = [], r = document.createElement("iframe"); window.zEmbed = function () { a.push(arguments) }, window.zE = window.zE || window.zEmbed, r.src = "javascript:false", r.title = "", r.role = "presentation", (r.frameElement || r).style.cssText = "display: none", d = document.getElementsByTagName("script"), d = d[d.length - 1], d.parentNode.insertBefore(r, d), i = r.contentWindow, s = i.document; try { o = s } catch (e) { n = document.domain, r.src = 'javascript:var d=document.open();d.domain="' + n + '";void(0);', o = s } o.open()._l = function () { var o = this.createElement("script"); n && (this.domain = n), o.id = "js-iframe-async", o.src = e, this.t = +new Date, this.zendeskHost = t, this.zEQueue = a, this.body.appendChild(o) }, o.write('<body onload="document._l();">'), o.close() }("https://assets.zendesk.com/embeddable_framework/main.js", "dayaxe.zendesk.com");
/*]]>*/</script>
    <!-- End of dayaxe Zendesk Widget script -->


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
            mixpanel.track("Gift Card Purchase Started", { "referrer": document.referrer });
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
    RESERVING GIFT CARD <br />FOR A LIMITED TIME...
    <div class="imgspin text-center"><img src="/images/loading.gif" alt="" /></div>
</div>
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnableCdn="True"></asp:ScriptManager>
<asp:HiddenField runat="server" ID="IsAllowCheckout" ClientIDMode="Static"/>
<uc1:AuthControl runat="server" ID="AuthControl" />

<div id="page-wrapper" class="book-page book-gift-card-page">
<!-- Main -->
<article id="main">
    <header class="single">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <a class="go-back" href="javascript:history.go(-1);">
                        <img src="/images/arrow.png" alt="arrow-left" /> <span class="hidden-md hidden-sm hidden-xs">Back</span>
                    </a>
                    <div class="title">
                        Checkout
                    </div>
                </div>
            </div>
        </div>
    </header>
    <section class="wrapper style5">
        <div id="owl-demo" class="owl-carousel owl-theme">
            <div class="item">
                <img src="<%=Page.ResolveClientUrl("/images/gift-card.png") %>" alt="Mirror Edge"/>
                <div class="overlay-bg overlay-bg-book"></div>
                <div class="overlay-text">
                    DayAxe.com eGift Cards
                </div>
            </div>
        </div>
    </section>
</article>
<asp:UpdatePanel runat="server" ID="BookProductUpdatePanel" UpdateMode="Conditional">
<ContentTemplate>
    <asp:HiddenField ID="HidUserBookedDate" ClientIDMode="Static" runat="server" />
    <div class="container book">
        <div class="row">
            <div class="col-md-7 col-sm-12 col-xs-12 col-left">
                <div class="book-wrap">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <h4 class="gift-detail-title">Gift Details</h4>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12 center-block">
                            <div class="errorCode text-center" id="errorCodeGift">
                                <asp:Literal runat="server" ID="ErrorMessageGiftCard"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div class="row row-gift-detail">
                        <div class="col-md-12 col-sm-12 col-xs-12 gift-detail-box">
                            <div class="row">
                                <div class="col-md-4 col-sm-12 col-xs-12">
                                    <label>
                                        Amount:
                                    </label>
                                </div>
                                <div class="col-md-4 padding-left-0 col-sm-5 col-xs-5">
                                    <div class="input-group" id="giftValueDiv" runat="server">
                                        <span class="input-group-addon">$</span>
                                        <%--<span class="input-group-addon">0.00</span>--%>
                                        <asp:TextBox ID="ValueText" Text="50" CssClass="form-control value" AutoPostBack="True" OnTextChanged="ValueText_OnTextChanged" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4 col-sm-12 col-xs-12">
                                    <label>
                                        To:
                                    </label>
                                </div>
                                <div class="col-md-8 padding-left-0 col-sm-12 col-xs-12">
                                    <div class="input-group">
                                        <asp:TextBox ID="ToText" placeholder="Enter email of recipient" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4 col-sm-12 col-xs-12">
                                    <label>
                                        Name:
                                    </label>
                                </div>
                                <div class="col-md-8 padding-left-0 col-sm-12 col-xs-12">
                                    <div class="input-group">
                                        <asp:TextBox ID="NameText" placeholder="Enter name of recipient" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4 col-sm-12 col-xs-12">
                                    <label>
                                        Message:
                                    </label>
                                </div>
                                <div class="col-md-8 padding-left-0 col-sm-12 col-xs-12">
                                    <div class="input-group">
                                        <asp:TextBox ID="MessageText" TextMode="MultiLine" ClientIDMode="Static" Rows="5" placeholder="Enjoy the gift of DayAxe from me!" CssClass="form-control message" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4 padding-right-0 col-sm-12 col-xs-12">
                                    <label>
                                        Delivery Date:
                                    </label>
                                </div>
                                <div class="col-md-5 padding-left-0 col-sm-12 col-xs-12">
                                    <div class="btn input-group date" id="checkindate">
                                        <asp:TextBox ID="DeliveryDateText" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                                        <span class="input-group-addon">
                                            <i class="glyphicon glyphicon-triangle-bottom"></i>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-5 col-sm-12 col-xs-12 col-right">
                <div class="wrap-purchase">
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <div id="form-group">
                                <div class="input-group greenborder">
                                    <div class="row">
                                        <div class="col-md-8 col-xs-6">
                                            <span class="monthly-label">eGift Card</span>
                                        </div>
                                        <div class="col-md-4 col-xs-6">
                                            <div class="perpricebox">
                                                <asp:Label class="total-price" runat="server" ID="perMoneyPrice"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row border">
                                        <div class="col-md-4 col-xs-4">
                                            <div class="texttotal">Total</div>
                                        </div>
                                        <div class="col-md-8 col-xs-8">
                                            <div class="pricebox">
                                                <span class="upto-text" id="msrp" runat="server" style="text-decoration: line-through;" Visible="False">
                                                    was <asp:Literal id="msrpPrice" runat="server"></asp:Literal>
                                                </span>
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
                                            <asp:Label CssClass="error-message" ForeColor="Red" runat="server" ID="MessageLabel"></asp:Label>
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
                                    <div class="note" style="margin-left:10px; margin-right: 10px;">
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
        </div>
        <div class="row">
            <div class="col-md-7 col-sm-12 col-xs-12 col-left">
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12 text-center center-block">
                        <div class="bookguestinfo center-block">Payment Details
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
                                                                <asp:CheckBox runat="server" ID="DCreditCheckBox" Checked="True"/>
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
                                <div class="emailplaceholder">Email <span>(receipt will be emailed here)</span></div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <!-- EOF PAYMENT FORM -->
                </div>
            </div>
        </div>
    </div>
</ContentTemplate>
<Triggers>
    <asp:AsyncPostBackTrigger ControlID ="ValueText" EventName ="TextChanged" />
    <asp:AsyncPostBackTrigger ControlID ="AnchorButton" EventName ="Click" />
</Triggers>
</asp:UpdatePanel>
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
    (function () {
        var b = document.createElement('script'); b.type = 'text/javascript'; b.async = true;
        b.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + 'a.klaviyo.com/media/js/analytics/analytics.js';
        var a = document.getElementsByTagName('script')[0]; a.parentNode.insertBefore(b, a);
    })();

    <%if(PublicCustomerInfos != null) { %>
    _learnq.push(['track', 'Checked Out eGiftCards', {
        '$email': '<%=PublicCustomerInfos.EmailAddress %>',
        '$first_name': '<%=PublicCustomerInfos.FirstName %>',
        '$last_name': '<%=PublicCustomerInfos.LastName %>',
        'email': '<%=PublicCustomerInfos.EmailAddress %>',
        'customer_id': '<%=PublicCustomerInfos.CustomerId %>',
        'item_type': 'eGiftCards',
        'cart_url': '<%=Request.Url.AbsoluteUri%>'
    }]);
    <%} %>
</script>
<!-- Scripts -->
<script>
    window.searchPage = "<%= string.IsNullOrEmpty((string)Session["SearchPage"]) ? Constant.SearchPageDefault : Session["SearchPage"].ToString() %>";
    window.showAuth = '<%=ShowAuth%>';
</script>
<script type="text/javascript">
    function BeginRequestHandler(sender, args) {
        var purchaseButton = document.getElementById('AnchorButton');
        if (purchaseButton) {
            purchaseButton.disabled = true;
        }
    }

    function EndRequestHandler(sender, args) {
        var purchaseButton = document.getElementById('AnchorButton');
        if (purchaseButton) {
            purchaseButton.disabled = false;
        }
    }

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
</script>
<%: Scripts.Render("~/bundles/bookgiftcard") %>
</form>
</body>
</html>