<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="BookingDetails.aspx.cs" Inherits="h.dayaxe.com.BookingDetails" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="DayaxeDal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
    <link href="Content/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    <link href="/Content/searchbooking.css" rel="stylesheet" />
    <link href="/Content/custom-datepicker.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row revenue">
        <div class="col-md-10">
            <asp:Label runat="server" ID="CustomerEmailLabel"></asp:Label>
        </div>
        <div class="col-md-2 text-right">
            <div class="today">
                <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-6">
            <span class="blue">Customer Details</span>
        </div>
        <div class="col-md-6 text-right">
            <asp:Label runat="server" ID="StripeCustomerLabel" CssClass="blue"></asp:Label>
        </div>
    </div>
    <div class="row customer-infos margin-t-15">
        <div class="col-md-12">
            <div class="row margin-t-25">
                <div class="col-md-4">
                    <label>First Name</label>
                </div>
                <div class="col-md-4">
                    <label>Last Name</label>
                </div>
                <div class="col-md-4">
                    <label>Customer ID</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <asp:TextBox ID="FirstNameText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="LastNameText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="CustomerIdText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-8">
                    <label>Email</label>
                </div>
                <div class="col-md-4">
                    <label>Zip Code</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox ID="EmailText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="ZipCodeText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-4">
                    <label>Password</label>
                </div>
                <div class="col-md-4">
                    <label>Stripe Account</label>
                </div>
                <div class="col-md-4">
                    <label>Date Created</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-4">
                    <%--<asp:TextBox ID="PasswordText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>--%>
                </div>
                <div class="col-md-4">
                    
                </div>
                <div class="col-md-4">
                    <asp:TextBox ID="CreatedDateText" CssClass="form-control" disabled="disabled" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-6">
            <span class="blue">Bookings Details</span>
        </div>
        <div class="col-md-6 text-right">
            <asp:Label runat="server" ID="StripeTransactionLabel" CssClass="blue"></asp:Label>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-12">
            <asp:Label runat="server" ID="ChangeCheckInDateMessage" CssClass="message-change" ForeColor="Red"></asp:Label>
        </div>
    </div>
    <div class="row bookings-details margin-t-15">
        <div class="col-md-12">
            <div class="row">
                <div class="col-md-3">
                    <label>Booking ID</label>
                </div>
                <div class="col-md-3">
                    <label>Booking Code</label>
                </div>
                <div class="col-md-3">
                    <label>Booking Date</label>
                </div>
                <div class="col-md-3">
                    <label>Status</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-3">
                    <asp:TextBox ID="BookingIdText" disabled="disabled" CssClass="form-control booking-id" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="BookingCodeText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="BookingDateText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="RefundPanel">
                        <ContentTemplate>
                            <asp:DropDownList runat="server" ID="BookingStatusDdl" AutoPostBack="True" OnSelectedIndexChanged="BookingStatusDdl_OnSelectedIndexChanged" CssClass="form-control"/>
                            <div id="refundModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
                                <div class="modal-dialog">
                                    <!-- Modal content-->
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                            <h4 class="modal-title">Refund Payment</h4>
                                        </div>
                                        <div class="modal-body text-center">
                                            <div class="row row-message">
                                                <div class="col-md-12">
                                                    <asp:Label runat="server" ID="RefundMessageLabel" ForeColor="Red"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="radio">
                                                        <label>
                                                        <asp:RadioButton id="FullRefundRadio" AutoPostBack="True" OnCheckedChanged="FullRefundRadio_OnCheckedChanged" CssClass="refund-r" Checked="True" runat="server" GroupName="Refund"></asp:RadioButton>
                                                            Full Refund
                                                            <span>
                                                                Refund the full amount (<asp:Literal runat="server" ID="FullAmountPrice"></asp:Literal>)
                                                            </span>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <div class="radio">
                                                        <label>
                                                            <asp:RadioButton id="PartialRefundRadio" AutoPostBack="True" OnCheckedChanged="PartialRefundRadio_OnCheckedChanged" CssClass="refund-r" runat="server" GroupName="Refund"></asp:RadioButton>
                                                            Partial Refund
                                                            <span>
                                                                Refund a partial amount
                                                            </span>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row row-refund-amount" ID="RefundPartialRow" runat="server" Visible="False">
                                                <div class="col-md-6">
                                                    <label class="refund-l">
                                                        Refund Amount:
                                                    </label>
                                                </div>
                                                <div class="col-md-6">
                                                    <div class="input-group">
                                                         <span class="input-group-addon">$</span> 
                                                        <asp:TextBox runat="server" ID="RefundAmount" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row row-refund-method">
                                                <div class="col-md-6">
                                                    <label class="label-p">
                                                        Refund Payment Medthod:
                                                    </label>
                                                </div>
                                                <div class="col-md-6">
                                                    <asp:DropDownList runat="server" CssClass="form-control" ID="RefundPaymentMethodDdl"/>
                                                </div>
                                            </div>
                                            <div class="row row-button">
                                                <div class="col-md-6">
                                                    <div class="btn btn-cancel" data-dismiss="modal">Cancel</div>
                                                </div>
                                                <div class="col-md-6 text-right">
                                                    <asp:Button runat="server" CssClass="btn btn-save" ID="RefundButton" Text="Refund" OnClick="RefundButton_OnClick"/>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID ="BookingStatusDdl" EventName ="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-2">
                    <label>Product Type</label>
                </div>
                <div class="col-md-7">
                    Hotel
                </div>
                <div class="col-md-3">
                    Product Name
                </div>
            </div>
            <div class="row">
                <div class="col-md-2">
                    <asp:TextBox ID="ProductTypeText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-7">
                    <asp:TextBox ID="HotelText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <asp:TextBox ID="ProductNameText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-2-5">
                    <label>Ticket Qty</label>
                </div>
                <div class="col-md-2-5">
                    <label>MSRP Item Price</label>
                </div>
                <div class="col-md-2-5">
                    <label>Paid Item Price</label>
                </div>
                <div class="col-md-2-5">
                    <label>Discount Used</label>
                </div>
                <div class="col-md-2-5">
                    <label>Booking Total</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-2-5">
                    <asp:TextBox ID="TicketQtyText" CssClass="form-control" runat="server" AutoPostBack="True" OnTextChanged="TicketQtyText_OnTextChanged"></asp:TextBox>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="MerchantPriceText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="PaidItemPriceText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="DiscountUsedText" CssClass="form-control" runat="server" AutoPostBack="True" OnTextChanged="DiscountUsedText_OnTextChanged"></asp:TextBox>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="BookingTotalText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-2-5">
                    <label>Check-In Date</label>
                </div>
                <div class="col-md-2-5">
                    <label>Expiration Date</label>
                </div>
                <div class="col-md-2-5">
                    <label>Redemption Date</label>
                </div>
                <div class="col-md-2-5">
                    <label>Refund Date</label>
                </div>
                <div class="col-md-2-5">
                    <label>
                        <asp:Literal runat="server" ID="RefundTotalLit"></asp:Literal>
                    </label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-2-5">
                    <asp:TextBox ID="CheckInDateText" CssClass="form-control check-in-date" runat="server" AutoPostBack="True" OnTextChanged="CheckInDateText_OnTextChanged"></asp:TextBox>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="ExpiredDateText" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                    <span class="alternative-tt hidden" runat="server" id="alternativeTimezone"></span>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="RedemptionDateText" ClientIDMode="Static" CssClass="form-control" runat="server"></asp:TextBox>
                    <span class="alternative-tt hidden" runat="server" id="alternativeRedemptionTimezone"></span>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="RefundDateText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-2-5">
                    <asp:TextBox ID="RefundTotalText" disabled="disabled" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <ul class="booking-histories">
                        <asp:Repeater ID="BookingHistoriesRpt" runat="server" OnItemDataBound="BookingHistoriesRpt_OnItemDataBound">
                            <ItemTemplate>
                                <li class="item">
                                    <span>
                                        <%#Eval("Description") %> - <%#((DateTime)Eval("UpdatedDate")).ToLosAngerlesTimeWithTimeZone(CurrentHotel.TimeZoneId).ToString(Constant.DiscountDateTimeFormat) %>&nbsp; <%#CurrentHotel.TimeZoneId.GetTimeZoneInfo() %>
                                    </span>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>
            </div>
        </div>
    </div>
    <asp:UpdatePanel runat="server" ID="ConfirmPanel" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row margin-t-50">
                <div class="col-md-6">
                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" CausesValidation="False" Text="Cancel" OnClick="CancelClick"/>
                </div>
                <div class="col-md-4 text-right">
                    <asp:Button runat="server" CssClass="btn btn-save" ID="ReSendSurveyButton" ClientIDMode="Static" CausesValidation="False" Text="Resend Survey" OnClick="ReSendSurveyClick"/>
                </div>
                <div class="col-md-2 text-right">
                    <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Save" OnClick="SaveBookingsClick"/>
                </div>
            </div>
            <div id="confirmModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
                <div class="modal-dialog">
                    <!-- Modal content-->
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">Booking Change</h4>
                        </div>
                        <div class="modal-body text-center">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Label runat="server" ID="ErrorMessageLabel" CssClass="message-change" ForeColor="Red"></asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div>
                                        
                                    </div>
                                    <div>
                                        <label>
                                            Check-In Date:
                                        </label>
                                    </div>
                                    <div>
                                        <label>Ticket Qty:</label>
                                    </div>
                                    <div>
                                        <label>
                                            MSRP Item Price:
                                        </label>
                                    </div>
                                    <div>
                                        <label>
                                            Paid Item Price:
                                        </label>
                                    </div>
                                    <div>
                                        <label>
                                            Discount:
                                        </label>
                                    </div>
                                    <div>
                                        <label>
                                            Promo Code:
                                        </label>
                                    </div>
                                    <div>
                                        <label>
                                            <asp:Literal runat="server" ID="BookingChangeLabel"></asp:Literal>
                                        </label>
                                    </div>
                                </div>
                                <div class="col-md-4 text-right">
                                    <div>
                                        <span class="header">
                                            Current
                                        </span>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="CurrentCheckInDateLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="CurrentTicketQuantityLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="CurrentMsrpPriceLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="CurrentPaidItemPriceLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="CurrentDiscountLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="CurrentPromoCodeLabel"></asp:Label>
                                    </div>
                                    <div>
                                        &nbsp;
                                    </div>
                                </div>
                                <div class="col-md-4 text-right">
                                    <div>
                                        <span class="header">
                                            New
                                        </span>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewCheckInDateLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewTicketQuantityLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewMsrpPriceLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewPaidItemPriceLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewDiscountLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewPromoCodeLabel"></asp:Label>
                                    </div>
                                    <div>
                                        <asp:Label runat="server" ID="NewBookingChangePriceLabel"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="row row-total">
                                <div class="col-md-4">
                                    <label>Order Total:</label>
                                </div>
                                <div class="col-md-4 text-right">
                                    <asp:Label runat="server" ID="CurrentTotalPriceLabel"></asp:Label>
                                </div>
                                <div class="col-md-4 text-right">
                                    <asp:Label runat="server" ID="NewTotalPriceLabel"></asp:Label>
                                </div>
                            </div>
                            <div class="row row-payment-method">
                                <div class="col-md-5">
                                    <label>
                                        Payment Medthod:
                                    </label>
                                </div>
                                <div class="col-md-7">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="PaymentMethodDdl"/>
                                </div>
                            </div>
                            <div class="row row-button">
                                <div class="col-md-6">
                                    <div class="btn btn-cancel" data-dismiss="modal">Cancel</div>
                                </div>
                                <div class="col-md-6 text-right">
                                    <asp:Button runat="server" CssClass="btn btn-save" ID="UpdateBooking" ClientIDMode="Static" Text="Save" OnClick="UpdateBooking_OnClick"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
         </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID ="CheckInDateText" EventName ="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID ="TicketQtyText" EventName ="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID ="DiscountUsedText" EventName ="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script>
        window.tickets = <%=PublicTickets%>;
        window.blockoutDate = <%=ProductBlockoutDate%>;
    </script>
    
    <%: Scripts.Render("~/bundles/bookingdetail") %>
    <script type="text/javascript">
        function BeginRequestHandler(sender, args) {
            var control = document.getElementById('SaveButton');
            control.disabled = true;
        }

        function EndRequestHandler(sender, args) {
            var control = document.getElementById('SaveButton');
            control.disabled = false;
        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    </script>
</asp:Content>

