<%@ Page Title="Revenues Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Revenues.aspx.cs" Inherits="h.dayaxe.com.Revenues" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<asp:Content runat="server" ContentPlaceHolderID="CssContentPlaceHolder">
    <link href="/Content/bootstrap-datepicker3.css" rel="stylesheet" />
    <style>
        .datepicker-days .table-condensed thead tr:nth-child(2) {
            float: none !important;
            height: auto;
        }

        .datepicker-dropdown:after {
            content: '';
            display: inline-block;
            border-left: 6px solid transparent;
            border-right: 6px solid transparent;
            border-bottom: 6px solid #444;
            position: absolute;
            top: -6px;
            left: 7px;
        }
        .datepicker table tr th:hover,
        .datepicker table tr th {
            background-color: #2d3134 !important;
            color: #e9f0f4 !important;
            border-radius: 0 !important;
        }
        .datepicker-dropdown {
            padding: 0;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="Content">
    <div class="row revenue">
        <div class="col-md-2">
            <div class="btn-group padding-t-20">
                <span class="revenue-select-product-type">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label runat="server" ID="ProductTypeLabel"></asp:Label> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a data-val="DayPasses" href="#">Day passes</a></li>
                        <li><a data-val="Cabanas" href="#">Cabanas</a></li>
                        <li><a data-val="Daybeds" href="#">Daybeds</a></li>
                        <li><a data-val="SpaPasses" href="#">Spa passes</a></li>
                    </ul>
                </span>
                <asp:DropDownList OnSelectedIndexChanged="ProductTypeDdl_OnSelectedIndexChanged" ID="ProductTypeDdl" ClientIDMode="Static" CssClass="menu revenue-select-product-type hidden" runat="server" AutoPostBack="True">
                    <asp:ListItem Text="Day passes" Value="DayPasses"></asp:ListItem>
                    <asp:ListItem Text="Cabanas" Value="Cabanas"></asp:ListItem>
                    <asp:ListItem Text="Daybeds" Value="Daybeds"></asp:ListItem>
                    <asp:ListItem Text="Spa passes" Value="SpaPasses"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-md-8">
            <div class="btn-group padding-t-20">
                <span class="revenue-wrap-calendar">
                    <span class="glyphicon glyphicon-calendar" aria-hidden="true"></span>
                    <span class="glyphicon glyphicon-triangle-right" aria-hidden="true"></span>
                </span>
                <span class="revenue-select-date">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label runat="server" ID="SelectedFilterBy"></asp:Label> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a data-val="All" href="#">All</a></li>
                        <li><a data-val="Today" href="#">Today</a></li>
                        <li><a data-val="ThisWeek" href="#">This Week</a></li>
                        <li><a data-val="ThisMonth" href="#">This Month</a></li>
                        <li><a data-val="Custom" href="#">Custom</a></li>
                    </ul>
                </span>
                <asp:DropDownList OnSelectedIndexChanged="SelectedFilterDdl_OnSelectedIndexChanged" ID="SelectedFilterDdl" ClientIDMode="Static" CssClass="menu revenue-select-date hidden" runat="server" AutoPostBack="True">
                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Today" Value="Today"></asp:ListItem>
                    <asp:ListItem Text="This Week" Value="ThisWeek"></asp:ListItem>
                    <asp:ListItem Text="This Month" Value="ThisMonth"></asp:ListItem>
                    <asp:ListItem Text="Custom" Value="Custom"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <asp:Panel class="form-inline search" runat="server" Visible="False" id="CustomForm">
                <div class="form-group">
                    <label>From </label>
                    <asp:TextBox runat="server" ID="DateFrom" placeholder="Choose date" CssClass="form-control datepicker" Visible="False"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>to </label>
                    <asp:TextBox runat="server" ID="DateTo"  placeholder="Choose date" ClientIDMode="Static" CssClass="form-control datepicker" Visible="False"></asp:TextBox>
                </div>
                <asp:Button runat="server" ID="Search" OnClick="Search_OnClick" Text="Search" CssClass="btn btn-search"/>
            </asp:Panel>
        </div>
        <div class="col-md-2 text-right">
            <div class="today">
                <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
            </div>
        </div>
    </div>
    <div class="row padding-t-60 row-revenue">
        <div class="col-md-5">
            <div class="row">
                <div class="col-md-3 first">
                    <div class="border-box">
                        <div class="value">
                            <asp:Literal runat="server" ID="AvgGuestPerBookingLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Avg Guest <br/> Per Booking
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="border-box">
                        <div class="value">
                            <asp:Literal runat="server" ID="TicketRedeemedLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Tickets <br/> Redeemed
                        </div>
                    </div>
                </div>
                <div class="col-md-5">
                    <div class="border-box">
                        <div class="value">
                            <asp:Literal runat="server" ID="TicketRevenueLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Ticket <br/> Revenue
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-7">
            <div class="row">
                <div class="col-md-4">
                    <div class="border-box">
                        <div class="value">
                            <asp:Literal runat="server" ID="AvgPerTicketSpendLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Avg. Per <br/> Ticket Spend
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="border-box">
                        <div class="value">
                            <asp:Literal runat="server" ID="IncrementalRevenueLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Incremental <br/> Revenue
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="border-box">
                        <div class="value">
                            <asp:Literal runat="server" ID="TotalRevenueLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Total <br/> Revenue
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row padding-t-60">
        <div class="col-md-12 avg-revenue">
            <div class="border-box">
                <div class="row">
                    <div class="col-md-12">
                        <h3 class="title">Avg. Auxiliary Spend/Ticket Breakdown</h3>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <div class="value">
                            <asp:Literal runat="server" ID="FoodDrinkLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Food & Drink
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="value">
                            <asp:Literal runat="server" ID="GiftShopLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Gift Shop
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="value">
                            <asp:Literal runat="server" ID="AvgSpaLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Spa
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="value">
                            <asp:Literal runat="server" ID="ParkingLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Parking
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="value">
                            <asp:Literal runat="server" ID="OtherLit"></asp:Literal>
                        </div>
                        <div class="description">
                            Other
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row padding-t-60">
        <div class="col-md-12 avg-revenue amenties-usage">
            <div class="border-box">
                <div class="row">
                    <div class="col-md-12">
                        <h3 class="title">Amenities Usage</h3>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div class="value">
                            <asp:Literal runat="server" ID="PoolPercentLit"></asp:Literal>
                        </div>
                        <div class="description">
                            <img src="Images/pool.png" />&nbsp;Pool
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="value">
                            <asp:Literal runat="server" ID="GymPercentLit"></asp:Literal>
                        </div>
                        <div class="description gym">
                            <img src="Images/gym.png" />&nbsp;Gym
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="value">
                            <asp:Literal runat="server" ID="SpaPercentLit"></asp:Literal>
                        </div>
                        <div class="description spa">
                            <img src="Images/spa.png" /><span>&nbsp;Spa</span>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="value">
                            <asp:Literal runat="server" ID="BusinessCenterPercentLit"></asp:Literal>
                        </div>
                        <div class="description business-center">
                            <img src="Images/business-center.png" /><span>&nbsp;Business
                            <br/>Services</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptPlaceHolder">
    <script src="/Scripts/jquery.ignore.js"></script>
    <script src="/Scripts/bootstrap-datepicker.js"></script>
    <script>
        $(function() {
            $('.datepicker').datepicker();
        });
    </script>
</asp:Content>