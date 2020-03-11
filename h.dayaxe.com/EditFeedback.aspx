<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EditFeedback.aspx.cs" Inherits="h.dayaxe.com.EditFeedback" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="DayaxeDal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="edit-feedback">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
    <div class="row">
        <div class="col-md-6">
            <span class="blue">Details</span>
        </div>
        <div class="col-md-6">
                
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-4">
            <label>Full Name</label>
        </div>
        <div class="col-md-4">
            <label>Tickets purchased</label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-4">
            <%=PublicSurveys.CustomerFullName %>
        </div>
        <div class="col-md-4">
            <%=string.Format("{0} of {1}", PublicSurveys.Quantity, PublicSurveys.TicketPurchased) %>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-4">
            <label>Hotel Name</label>
        </div>
        <div class="col-md-4">
            <label>Hotel City</label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-4">
            <%=PublicSurveys.HotelName %>
        </div>
        <div class="col-md-4">
            <%=PublicSurveys.City %>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-4">
            <label>Product Name</label>
        </div>
        <div class="col-md-4">
            <label>Check-In Date</label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-4">
            <%=PublicSurveys.ProductName %>
        </div>
        <div class="col-md-4">
            <%=PublicSurveys.CheckInDate.HasValue ? PublicSurveys.CheckInDate.Value.ToString(Constant.DateFormat) : string.Empty %>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-4">
            <label>Per Ticket Price</label>
        </div>
        <div class="col-md-4">
            <label>Total Paid</label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-4">
            <%=Helper.FormatPrice(PublicSurveys.PerTicketPrice) %>
        </div>
        <div class="col-md-4">
            <%=Helper.FormatPrice(PublicSurveys.TotalPrice) %>
        </div>
    </div>
    <div class="row">
        <div class="col-md-10">
            <asp:Label runat="server" ID="ErrorMessage" ForeColor="Red"></asp:Label>
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-10">
            <label>
                Rate
            </label>
        </div>
    </div>
        <div class="row">
            <div class="col-md-10">
                <asp:TextBox runat="server" TextMode="SingleLine" ID="SurveyRating" Text="4" ClientIDMode="Static" CssClass="rating rating-loading" title=""></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-10">
                <label>
                    Comment
                </label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-10">
                <asp:TextBox runat="server" TextMode="MultiLine" CssClass="form-control" ID="CommentText" Rows="10" Text="" title=""></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-3 checkbox">
                <label>
                    <asp:CheckBox runat="server" ID="UsePoolCheckBox"/>
                    Use Pool
                </label>
            </div>
            <div class="col-md-2 checkbox">
                <label>
                    <asp:CheckBox runat="server" ID="UseGymCheckBox"/>
                    Use Gym
                </label>
            </div>
            <div class="col-md-3 checkbox">
                <label>
                    <asp:CheckBox runat="server" ID="UseSpaCheckBox"/>
                    Use Spa
                </label>
            </div>
            <div class="col-md-2 checkbox">
                <label>
                    <asp:CheckBox runat="server" ID="UseBusinessCenterCheckBox"/>
                    Use Business Center
                </label>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-3 checkbox">
                <label>
                    <asp:CheckBox runat="server" CssClass="buy-any" ID="IsBuyFoodAndDrinkCheckBox"/>
                    Buy Food & Drink?
                </label>
            </div>
            <div class="col-md-7">
                <div class="input-group">
                    <span class="input-group-addon">
                        $
                    </span>
                    <asp:TextBox runat="server" data-value="98" ID="FoodAndDrinkPriceTextBox" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-10 checkbox">
                <label>
                    <asp:CheckBox runat="server" ID="IsPayForParkingCheckBox"/>
                    Is Pay For Parking
                </label>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-3 checkbox">
                <label>
                    <asp:CheckBox runat="server" CssClass="buy-any" ID="IsBuySpaServiceCheckBox"/>
                    Buy Spa Service?
                </label>
            </div>
            <div class="col-md-7">
                <div class="input-group">
                    <span class="input-group-addon">
                        $
                    </span>
                    <asp:TextBox runat="server" data-value="100" ID="BuySpaServicePriceText" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-3 checkbox">
                <label>
                    <asp:CheckBox runat="server" CssClass="buy-any" ID="IsBuyAdditionalServiceCheckBox"/>
                    Buy Additional Service?
                </label>
            </div>
            <div class="col-md-7">
                <div class="input-group">
                    <span class="input-group-addon">
                        $
                    </span>
                    <asp:TextBox runat="server" data-value="20" ID="BuyAdditionalServicePriceText" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="row margin-t-50">
            <div class="col-md-3">
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" CausesValidation="False" Text="Cancel" OnClick="CancelClick"/>
            </div>
            <div class="col-md-3 text-center">
                <asp:Button runat="server" CssClass="btn btn-cancel delete-survey" ID="DeleteButton" CausesValidation="False" Text="Delete" OnClick="DeleteClick"/>
            </div>
            <div class="col-md-4 text-right">
                <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Save" OnClick="SaveClick"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/star-rating.min.js"></script>
    <script src="/Scripts/plugins/toggles/toggles.min.js"></script>
    <script src="/Scripts/editfeedback.js" type="text/javascript"></script>
</asp:Content>

