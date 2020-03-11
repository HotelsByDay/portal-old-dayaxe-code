<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Engagement.aspx.cs" Inherits="h.dayaxe.com.Engagement" %>
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
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <div class="row revenue">
        <div class="col-md-2">
            <div class="btn-group padding-t-20">
                <span class="revenue-select-product-type">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label runat="server" ID="ProductTypeLabel"></asp:Label>
                        <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a data-val="All" href="#">All</a></li>
                        <li><a data-val="DayPasses" href="#">Day passes</a></li>
                        <li><a data-val="Cabanas" href="#">Cabanas</a></li>
                        <li><a data-val="Daybeds" href="#">Daybeds</a></li>
                        <li><a data-val="SpaPasses" href="#">Spa passes</a></li>
                    </ul>
                </span>
                <asp:DropDownList OnSelectedIndexChanged="ProductTypeDdl_OnSelectedIndexChanged" ID="ProductTypeDdl" ClientIDMode="Static" CssClass="menu revenue-select-product-type hidden" runat="server" AutoPostBack="True">
                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                    <asp:ListItem Text="Day passes" Value="DayPasses"></asp:ListItem>
                    <asp:ListItem Text="Cabanas" Value="Cabanas"></asp:ListItem>
                    <asp:ListItem Text="Daybeds" Value="Daybeds"></asp:ListItem>
                    <asp:ListItem Text="Spa passes" Value="SpaPasses"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-md-8">
            <div class="btn-group select-range padding-t-20">
                <span class="revenue-wrap-calendar">
                    <span class="glyphicon glyphicon-calendar" aria-hidden="true"></span>
                    <span class="glyphicon glyphicon-triangle-right" aria-hidden="true"></span>
                </span>
                <span class="revenue-select-date">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label runat="server" ID="SelectedFilterBy"></asp:Label>
                        <span class="caret"></span>
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
                    <asp:ListItem Text="This Month" Value="ThisMonth" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Custom" Value="Custom"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <asp:Panel class="form-inline search" runat="server" Visible="False" ID="CustomForm">
                <div class="form-group">
                    <label>From </label>
                    <asp:TextBox runat="server" ID="DateFrom" placeholder="Choose date" CssClass="form-control datepicker" Visible="False"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>to </label>
                    <asp:TextBox runat="server" ID="DateTo" placeholder="Choose date" ClientIDMode="Static" CssClass="form-control datepicker" Visible="False"></asp:TextBox>
                </div>
                <asp:Button runat="server" ID="Search" OnClick="Search_OnClick" Text="Search" CssClass="btn btn-search" />
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
    <div class="row">
        <div class="col-md-12">
            <div class="btn-group tab tab-3" role="group" aria-label="">
                <a type="button" class="btn btn-tab" href="Demographics.aspx">Demographics</a>
                <a type="button" class="btn btn-tab active" href="Engagement.aspx">Engagement</a>
                <a type="button" class="btn btn-tab" href="Feedback.aspx">Feedback</a>
            </div>
        </div>
    </div>
    <asp:MultiView runat="server" ActiveViewIndex="0" ID="MVEngagement">
        <asp:View runat="server">
            <div class="row margin-t-130 text-center demo-nodata">
                <img src="images/graphics.png" class="img-responsive img-nodata" alt="" />
                <div class="text-capitalize margin-t-15">
                    There is no data to display
                </div>
            </div>
        </asp:View>
        <asp:View runat="server">
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div id="container"></div>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-6">
                    <div id="daysRedeemed"></div>
                </div>
                <div class="col-md-6">
                    <div id="mostRedeemedHour"></div>
                </div>
            </div>
            <div class="row margin-t-130">
                <div class="col-md-6">
                    <div id="redemptionFrequency"></div>
                </div>
                <div class="col-md-6">
                    <div id="mostRedeemDay"></div>
                </div>
            </div>
            <div class="row margin-t-130">
                <div class="col-md-6">
                    <div id="otherHotelvisited"></div>
                </div>
                <div class="col-md-6">
                    <div id="customerSatisfaction"></div>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptPlaceHolder">
    <script src="/Scripts/jquery.ignore.js"></script>
    <script src="/Scripts/bootstrap-datepicker.js"></script>
    <script>
        $(function() {
            $('.datepicker').datepicker();
        });
    </script>
    <script>
        window.redeeemDayData = [
            {
                name: 'Days',
                showInLegend: false,
                data: [<%=MostRedeemedDays%>]
            }
        ];
        window.customerSatisfactionData = [
            {
                name: 'Star',
                showInLegend: false,
                data: [<%=CustomerSatisfaction%>]
            }
        ];

        window.hotelVisitedCategory = [<%=HotelVisitedCateogries%>];
        window.hotelVisitedData = [
            {
                name: 'Visited',
                showInLegend: false,
                data: [<%=HotelVisited%>]
            }
        ];

        window.newAndReturningDataCategory = [<%= NewAndReturningCategory%>];
        window.newAndReturningData = 
            [
                {
                    name: '<%=PercentNew%>',
                    data: [<%=NewData%>]

                }, {
                    name: '<%=PercentReturning%>',
                    data: [<%=ReturningData%>]

                }
            ];
        window.frequencyDataCategory = [<%=FrequencyCategory%>];
        window.frequencyData = [<%=Frequency%>];

        window.daysRedeemedCategories = [<%=DaysRedeemedCategories%>];
        window.daysRedeemedData = [
            {
                name: 'Days',
                showInLegend: false,
                data: [<%=DaysRedeemed%>]
            }
        ];

        window.mostRedeemedHourCategories = [<%=HoursRedeemedCategories%>];
        window.mostRedeemedHourData = [
            {
                name: 'Hour',
                showInLegend: false,
                data: [<%=HoursRedeemed%>]
            }
        ];
    </script>
    <script src="/Scripts/chartdata.js"></script>
</asp:Content>
