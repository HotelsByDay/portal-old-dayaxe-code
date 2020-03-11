<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="RedemptionSettings.aspx.cs" Inherits="h.dayaxe.com.RedemptionSettings" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="Content">
    <div class="settings">
        <div class="row">
            <div class="col-md-12">
                <hr/>
            </div>
            <div class="col-md-12">
                <label>
                    Core Products
                </label>
            </div>
            <div class="col-md-6">
                <div class="row">
                    <div class="col-md-8">
                        <h4>Day Pass Daily Redemption Max</h4>
                    </div>
                    <div class="col-md-4">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <span>
                                    <img src="Images/iconCapacity.png" alt="" />
                                </span> 
                                <asp:Label CssClass="daily-current-value" runat="server" ID="CurrentPass"></asp:Label> 
                                <span class="caret"></span>
                            </button>
                            <asp:HiddenField ID="HidPass" runat="server" ClientIDMode="Static" />
                            <ul class="dropdown-menu" runat="server" ID="DailyCapacity" ClientIDMode="Static"></ul>
                        </div>
                    </div>
                </div>
                <div class="row margin-t-25">
                    <div class="col-md-8">
                        <h4>Cabana Daily Limit</h4>
                    </div>
                    <div class="col-md-4">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <span>
                                    <img src="Images/iconCapacity.png" alt="" />
                                </span> 
                                <asp:Label CssClass="cabana-current-value" runat="server" ID="CurrentCabana">0</asp:Label> 
                                <span class="caret"></span>
                            </button>
                            <asp:HiddenField ID="HidCabana" runat="server" ClientIDMode="Static" />
                            <ul class="dropdown-menu" runat="server" ID="CabanaDailyLimit" ClientIDMode="Static"></ul>
                        </div>
                    </div>
                </div>
                <div class="row margin-t-25">
                    <div class="col-md-8">
                        <h4>Spa Pass Daily Limit</h4>
                    </div>
                    <div class="col-md-4">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <span>
                                    <img src="Images/iconCapacity.png" alt="" />
                                </span> 
                                <asp:Label CssClass="spaPass-current-value" runat="server" ID="CurrentSpa">0</asp:Label> 
                                <span class="caret"></span>
                            </button>
                            <asp:HiddenField ID="HidSpaPass" runat="server" ClientIDMode="Static" />
                            <ul class="dropdown-menu" runat="server" ID="SpaPassDailyLimit" ClientIDMode="Static"></ul>
                        </div>
                    </div>
                </div>
                <div class="row margin-t-25">
                    <div class="col-md-8">
                        <h4>Daybeds Pass Daily Limit</h4>
                    </div>
                    <div class="col-md-4">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <span>
                                    <img src="Images/iconCapacity.png" alt="" />
                                </span> 
                                <asp:Label CssClass="dayBed-current-value" runat="server" ID="CurrentDayBed">0</asp:Label> 
                                <span class="caret"></span>
                            </button>
                            <asp:HiddenField ID="HidDayBed" runat="server" ClientIDMode="Static" />
                            <ul class="dropdown-menu" runat="server" ID="DayBedDailyLimit" ClientIDMode="Static"></ul>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-5">
                <span>Select max number of redemptions you are willing to accept per day.
                </span>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <hr/>
            </div>
            <div class="col-md-12">
                <label>
                    Add-Ons
                </label>
            </div>
            <div class="col-md-6">
                <asp:Repeater runat="server" ID="RptAddOns" OnItemDataBound="RptAddOns_OnItemDataBound">
                    <ItemTemplate>
                        <div class="row margin-bottom-25">
                            <div class="col-md-8">
                                <h4>
                                    <%#Eval("ProductName") %>
                                </h4>
                            </div>
                            <div class="col-md-4">
                                <div class="btn-group add-ons">
                                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span>
                                            <img src="Images/iconCapacity.png" alt="" />
                                        </span>
                                        <asp:Label CssClass="current-daily-sales-value" runat="server" ID="CurrentDailySales">0</asp:Label>
                                        <span class="caret"></span>
                                    </button>
                                    <asp:HiddenField ID="HidDailySales" runat="server" />
                                    <asp:HiddenField runat="server" ID="HidId" />
                                    <ul class="dropdown-menu" runat="server" id="DailySalesLimit"></ul>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div class="row padding-t-60">
            <div class="col-md-9 text-right">
                <div class="saving hidden" id="saving" runat="server">Saving...</div>
            </div>
            <div class="col-md-2 text-right">
                <asp:Button class="btn btn-save" runat="server" Text="Save" OnClientClick="$('.saving').text('Saving...').removeClass('hidden');" OnClick="SavePassClick"></asp:Button>
            </div>
        </div>
    </div>
</asp:Content>
