<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Survey.ascx.cs" Inherits="dayaxe.com.Controls.ControlsSurvey" %>
<%@ Import Namespace="DayaxeDal" %>
<link href="/assets/css/star-rating.min.css" rel="stylesheet" />
<link href="/assets/css/bootstrap-slider.min.css" rel="stylesheet" />
<link href="/assets/css/survey.css" rel="stylesheet" />
<!-- Facebook Pixel Code -->
<script>
    !function(f,b,e,v,n,t,s){if(f.fbq)return;n=f.fbq=function(){n.callMethod?
            n.callMethod.apply(n,arguments):n.queue.push(arguments)};if(!f._fbq)f._fbq=n;
        n.push=n;n.loaded=!0;n.version='2.0';n.queue=[];t=b.createElement(e);t.async=!0;
        t.src=v;s=b.getElementsByTagName(e)[0];s.parentNode.insertBefore(t,s)}(window,
        document,'script','//connect.facebook.net/en_US/fbevents.js');

    fbq('init', '<%=AppConfiguration.FacebookKey %>');
    fbq('track', 'PageView');
</script>
<noscript>
    <img height="1" width="1" style="display:none" src='<%=String.Format("https://www.facebook.com/tr?id={0}&ev=PageView&noscript=1", AppConfiguration.FacebookKey) %>"' />
</noscript>
<!-- End Facebook Pixel Code -->   

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
            mixpanel.track("Survey Started", { "referrer": document.referrer });
        </script>
    <% } %>

  <!-- end Mixpanel -->

<!--  Start Google Analytics  -->
 <script>
     (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
             (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
             m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
     })(window,document,'script','//www.google-analytics.com/analytics.js','ga');
     ga('create', '<%=AppConfiguration.GoogleKey %>', 'auto');
     ga('send', 'pageview');
 </script>

<div id="cancelledBookingModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static" clientidmode="Static" runat="server" Visible="False">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                <h4 class="modal-title text-center">Cancelled Booking</h4>
            </div>
            <div class="modal-body text-center">
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div class="location">
                            Your booking has been cancelled/refunded. You cannot submit the feedback on this booking.
                        </div>
                    </div>
                </div>
                <div class="row button-submit">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <div class="btn btn-save btn-close" data-dismiss="modal">OK</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!--  Start Google Analytics  -->
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="surveyModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
            <div id="page-wrapper">
                <article id="main">
                    <div class="container">
                        <header>
                            <div class="row">
                                <div class="col-md-10 col-sm-10 col-xs-10 text-left padding-right-0">
                                    <div class="title" id="sTitle" runat="server">
                                        Your Last Experience
                                    </div>
                                    <div class="date">
                                        <asp:Literal runat="server" ID="RedeemedDate"></asp:Literal>
                                    </div>
                                </div>
                                <div class="col-md-2 col-sm-2 col-xs-2 text-right padding-left-0">
                                    <div class="price">
                                        <asp:Literal runat="server" ID="Price"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </header>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="overlay-bg">
                                    <asp:Image runat="server" ID="ImageProduct" AlternateText="" CssClass="img-responsive"/>
                                    <asp:Label runat="server" ID="HotelInfo"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="survey">
                            <asp:MultiView runat="server" ID="SurveyViews" ActiveViewIndex="0">
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    <asp:Literal runat="server" ClientIDMode="Static" Text="How Was it?" ID="RateTitle"></asp:Literal>
                                                </div>
                                            </div>
                                            <div class="form-group text-center">
                                                <asp:TextBox runat="server" TextMode="SingleLine" ID="Rating" Text="0" ClientIDMode="Static" CssClass="rating rating-loading" title="" data-step="1"></asp:TextBox>
                                            </div>
                                            <div class="form-group rate-commend hidden">
                                                Please tell us how was your experience?
                                            </div>
                                            <div class="form-group rate-commend hidden">
                                                <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="Ratecommend"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    What did you use?
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-center">
                                                    <button class="btn btn-use pool" runat="server" id="UsePool">
                                                        <img src="/images/pool.png" class="img-responsive" alt=""/>
                                                        <span>pool</span>
                                                    </button>
                                                    <asp:HiddenField runat="server" ID="HidUsePool"/>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-center">
                                                    <button class="btn btn-use gym" runat="server" id="UseGym">
                                                        <img src="/images/gym-inactive.png" alt="" class="img-responsive"/>
                                                        <span>gym</span>
                                                    </button>
                                                    <asp:HiddenField runat="server" ID="HidUseGym"/>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-20 text-center">
                                                    <button class="btn btn-use spa" runat="server" id="UseSpa">
                                                        <img src="/images/spa.png" alt="" class="img-responsive"/>
                                                        <span>spa</span>
                                                    </button>
                                                    <asp:HiddenField runat="server" ID="HiduseSpa"/>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-20 text-center">
                                                    <button class="btn btn-use business-center" runat="server" id="UseBusinessCenter">
                                                        <img src="/images/handshake.png" alt="" class="img-responsive"/>
                                                        <span>business<br/>center</span>
                                                    </button>
                                                    <asp:HiddenField runat="server" ID="HidUseBusinessCenter"/>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    Did you buy any <br/>
                                                    food & drink?
                                                </div>
                                            </div>
                                            <div class="form-group use-food-drink">
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-right padding-right-5">
                                                    <button class="btn btn-yes" id="useFoodDrinkButton" runat="server" clientidmode="Static">
                                                        <span>YES</span>
                                                    </button>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-left padding-left-5">
                                                    <asp:LinkButton class="btn btn-no" runat="server" id="NotUseFoodDrinkButton" OnClick="NotUseFoodDrinkButton_OnServerClick">
                                                        <span>NO</span>
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                            <div class="form-group use-food-drink-price hidden" runat="server" id="foodDrinkPrice">
                                                <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-50">
                                                    <asp:TextBox runat="server" ID="UseFoodDrinkText" ClientIDMode="Static" data-slider-min="10" data-slider-tooltip="show" data-slider-max="1000" data-slider-value="98" data-slider-step="5"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    Did you have to <br/>
                                                    pay for parking?
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-right padding-right-5">
                                                    <asp:LinkButton class="btn btn-yes" id="PayForParkingYesButton" runat="server" OnClick="PayForParkingYesButton_OnServerClick">
                                                        <span>YES</span>
                                                    </asp:LinkButton>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-left padding-left-5">
                                                    <asp:LinkButton class="btn btn-no" runat="server" id="PayForParkingNoButton" OnClick="PayForParkingNoButton_OnServerClick">
                                                        <span>NO</span>
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    Did you buy <br/>
                                                    any spa services?
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-right padding-right-5">
                                                    <button class="btn btn-yes" id="UseSpaServiceButton" runat="server" clientidmode="Static">
                                                        <span>YES</span>
                                                    </button>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-left padding-left-5">
                                                    <asp:LinkButton class="btn btn-no" runat="server" id="NotUseSpaServiceButton" OnClick="NotUseSpaServiceButton_OnServerClick">
                                                        <span>NO</span>
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                            <div class="form-group use-spa-service-price hidden" runat="server" id="SpaServicePrice">
                                                <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-50">
                                                    <asp:TextBox runat="server" ID="UseSpaServiceText" ClientIDMode="Static" data-slider-min="10" data-slider-tooltip="show" data-slider-max="1000" data-slider-value="100" data-slider-step="5"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    Did you buy <br/>
                                                    any additional services?
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-right padding-right-5">
                                                    <button class="btn btn-yes" id="UseAdditionalServiceButton" runat="server" clientidmode="Static">
                                                        <span>YES</span>
                                                    </button>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6 margin-top-30 text-left padding-left-5">
                                                    <asp:LinkButton class="btn btn-no" runat="server" id="NotUseAdditionalServiceButton" OnClick="NotUseAdditionalServiceButton_OnServerClick">
                                                        <span>NO</span>
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                            <div class="form-group use-additional-service-price hidden" runat="server" id="AdditionalServicePrice">
                                                <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-50">
                                                    <asp:TextBox runat="server" ID="UseAdditionalServiceText" ClientIDMode="Static" data-slider-min="10" data-slider-tooltip="show" data-slider-max="1000" data-slider-value="20" data-slider-step="5"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group text-center">
                                                <div class="title">
                                                    Thank you!
                                                </div>
                                            </div>
                                            <div class="form-group text-center">
                                                <label class="recored">
                                                    Your feedback has been recored!
                                                </label>
                                            </div>
                                            <div class="form-group margin-top-30 margin-bottom-30">
                                                <hr class="divider"/>
                                            </div>
                                            <div class="form-group text-center padding-top-30">
                                                <label class="thank">
                                                    Thank again for using DayAxe!
                                                </label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <%--<div id="TA_cdswritereviewlg455" class="TA_cdswritereviewlg">
                                                <ul id="lHQ9DBaqHc5" class="TA_links TnPTIAU6jb5">
                                                    <li id="B7qwPAQNaH" class="Au2SHIG7GiL7">
                                                        <a target="_blank" href="https://www.tripadvisor.com/"><img src="https://www.tripadvisor.com/img/cdsi/img2/branding/medium-logo-12097-2.png" alt="TripAdvisor"/></a>
                                                    </li>
                                                </ul>
                                            </div>
                                            <script src="https://www.jscache.com/wejs?wtype=cdswritereviewlg&amp;uniq=455&amp;locationId=638765&amp;lang=en_US&amp;lang=en_US&amp;display_version=2"></script>--%>
                                            <%--<%=PublicHotels.TripAdvisorScript %>--%>
                                        </div>
                                    </div>
                                </asp:View>
                            </asp:MultiView>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 progress hidden">
                                <img src="/images/loading.gif" class="img-responsive loading" alt="Processing..."/>
                                Processing... Please wait
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 next">
                                <asp:Button id="NextButton" OnClick="NextButton_OnServerClick" CssClass="btngray next-button" ClientIDMode="Static" runat="server" class="btngray" Text="NEXT"></asp:Button>
                            </div>
                        </div>
                    </div>
                </article>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="NextButton" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="NotUseFoodDrinkButton" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="PayForParkingYesButton" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="PayForParkingNoButton" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="NotUseSpaServiceButton" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>
<script type="text/javascript">
    //function EndRequestHandler(sender, args) {
    //    var control = document.getElementById('');
    //    control.disabled = false;
    //}
    
    //Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
</script>

<%--<div id="TA_cdswritereviewlg455" class="TA_cdswritereviewlg">
    <ul id="lHQ9DBaqHc5" class="TA_links TnPTIAU6jb5">
        <li id="B7qwPAQNaH" class="Au2SHIG7GiL7">
            <a target="_blank" href="https://www.tripadvisor.com/"><img src="https://www.tripadvisor.com/img/cdsi/img2/branding/medium-logo-12097-2.png" alt="TripAdvisor"/></a>
        </li>
    </ul>
</div>
<script src="https://www.jscache.com/wejs?wtype=cdswritereviewlg&amp;uniq=455&amp;locationId=638765&amp;lang=en_US&amp;lang=en_US&amp;display_version=2"></script>--%>