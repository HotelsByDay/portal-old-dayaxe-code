<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="BookingPage.aspx.cs" Inherits="h.dayaxe.com.BookingPage" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="DayaxeDal" %>
<asp:Content runat="server" ContentPlaceHolderID="CssContentPlaceHolder">
    <link href="/Content/bootstrap-datepicker3.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>
    <div class="booking">
        <div class="row revenue">
            <div class="col-md-10">
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
                <div class="form-inline">
                    <asp:Button runat="server" ID="ExportToExcelButton" OnClick="ExportToExcelButtonOnClick" Text="Export to Excel" CssClass="btn btn-export"/>
                </div>
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
            <div class="col-md-4">
                <asp:TextBox ID="SearchText" placeholder="Search guest name or booking code" CssClass="form-control search-text" runat="server"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="btn btn-blue-all" OnClick="SearchBooking" />
            </div>
            <div class="col-md-2">
                <asp:Button runat="server" Text="Clear" CssClass="btn btn-cancel btn-cancel-search" OnClick="ClearSearchButton_OnClick" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:Label runat="server" ID="MessageLabel" ForeColor="Red"></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <table class="table table-booking">
                    <asp:Repeater ID="HistoricalRepeater" runat="server" OnItemDataBound="HistoricalRepeater_ItemDataBound">
                        <HeaderTemplate>
                            <thead>
                                <tr>
                                    <th>Passholder Name</th>
                                    <th>Product Name</th>
                                    <th>Booking Code</th>
                                    <th>Check-In Date & Time</th>
                                    <th>Tickets</th>
                                    <th>User Rating</th>
                                    <th>Pass Status</th>
                                    <th runat="server" visible='<%#(PublicUser.IsAdmin || PublicUser.IsCheckInOnly) %>'></th>
                                    <th runat="server" visible='<%#PublicUser.IsSuperAdmin %>'>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr id="rowHistory" runat="server">
                                <td>
                                    <%# Eval("FullName") %>
                                </td>
                                <td>
                                    <%# Eval("ProductName") %>
                                </td>
                                <td>
                                    <%#Eval("BookingIdString") %>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="CheckInDateLabel"></asp:Label>
                                </td>
                                <td>
                                    <%# Eval("Quantity")%>
                                </td>
                                <td>
                                    <%# (Convert.ToInt32(Eval("UserRating"))>0 ? Eval("UserRating") : "") %>
                                </td>
                                <td>
                                    <%# Eval("BookingStatusString") %>
                                </td>
                                <td runat="server" Visible="<%#(PublicUser.IsAdmin || PublicUser.IsCheckInOnly) %>">
                                    <asp:Button ID="RedeemButton" Visible="False" CommandArgument='<%#Eval("BookingId") %>' OnCommand="RedeemButton_OnCommand" CssClass="btn btn-blue-all redeemed-ticket" runat="server" Text="Redeem" />
                                </td>
                                <td data-id='<%# Eval("BookingId") %>'  runat="server" visible='<%#PublicUser.IsSuperAdmin %>'>
                                    <a href="/BookingDetails.aspx?id=<%# Eval("BookingId") %>" class="edit-hotel">
                                        <span class="glyphicon glyphicon-edit"></span>
                                    </a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr>
                                <td colspan="3">
                                    <div class="btn-group">
                                        <button runat="server" id="Previous" clientidmode="Static" class="btn btn-prev" onserverclick="Previous_OnClick">
                                            <img src="/images/arrow-left.png" alt="" class="img-responsive" />
                                        </button>
                                        <button runat="server" id="Next" clientidmode="Static" class="btn btn-next" onserverclick="Next_OnClick">
                                            <img src="/images/arrow-right.png" alt="" class="img-responsive" />
                                        </button>
                                    </div>
                                    <b>
                                        <asp:Literal runat="server" ID="LitPage"></asp:Literal>
                                    </b>
                                </td>
                                <td>
                                    <span class="result">
                                        <asp:Literal ID="TotalPass" runat="server"></asp:Literal>
                                    </span>
                                </td>
                                <td>
                                    <span class="result">
                                        <asp:Literal ID="SpendAvg" runat="server" Visible="False"></asp:Literal>
                                    </span>
                                </td>
                                <td>
                                    <span class="result">
                                        <asp:Literal ID="RatingAvg" runat="server" Visible="False"></asp:Literal>
                                    </span>
                                </td>
                                <td>
                                    <span class="result">
                                        <asp:Literal ID="TotalRedeemed" runat="server"></asp:Literal>
                                    </span>
                                </td>
                                <td runat="server" visible='<%#(PublicUser.IsAdmin || PublicUser.IsCheckInOnly) %>'></td>
                                <td runat="server" visible='<%#PublicUser.IsSuperAdmin %>'></td>
                            </tr>
                            </tbody>
                        </FooterTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </div>
    
<asp:UpdatePanel ID="ValidateUpdatePanel" UpdateMode="Conditional" runat="server">
    <ContentTemplate>
    <div id="redeemModal" class="modal" role="dialog" data-keyboard="false" data-backdrop="static">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title text-center">Ticket Validation</h4>
                </div>
                <div class="modal-body text-center">
                    <div class="row">
                        <div class="col-md-12 col-validate">
                            <p class="h">
                                Please verify ticket details:
                            </p>
                            <p>
                                <b>Name: </b>
                                <asp:HiddenField ID="BookingIdHidden" runat="server" />
                                <asp:Label runat="server" ID="FullNameLabel"></asp:Label>
                            </p>
                            <p>
                                <b>Check-In Date: </b>
                                <asp:Label runat="server" ID="CheckInDateLabel"></asp:Label>
                            </p>
                            <p>
                                <b>Tickets: </b><asp:Label runat="server" ID="TicketsLabel"></asp:Label>
                            </p>
                            <p>
                                <b>Each Ticket: </b><asp:Label runat="server" ID="EachTicketLabel"></asp:Label>
                            </p>
                            <p>
                                <b>Booking Code: </b><asp:Label runat="server" ID="BookingCodeLabel"></asp:Label>
                            </p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 margin-t-15 margin-bottom-15 text-center">
                            <asp:Label ID="ValidateMessage" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="row button-submit">
                        <div class="col-md-12 text-center">
                            <asp:Button ID="ValidateButton" runat="server" CssClass="btn btn-save" Text="VALIDATE" OnClick="ValidateButton_OnClick" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID ="HistoricalRepeater" EventName ="ItemCommand" />
        <asp:AsyncPostBackTrigger ControlID ="ValidateButton" EventName ="Click" />
    </Triggers>
</asp:UpdatePanel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptPlaceHolder">
    <script src="/Scripts/jquery.ignore.js"></script>
    <script src="/Scripts/bootstrap-datepicker.js"></script>
    <script>
        window.searchBookings = '<%=SearchButton.ClientID %>';
        function searchBooking() {
            $('#redeemModal').on('hidden.bs.modal', function (e) {
                window.location.reload();
            });
        }
        $(function() {
            $('.datepicker').datepicker();
            $('.search-text').keydown(function(event) {
                var keyCode = event.keyCode || event.which;
                if (keyCode === 13) {
                    event.preventDefault();
                    $(this).closest('.row').find('.btn-blue-all').click();
                    return false;
                }
            });
        });
        var _learnq = _learnq || [];
        _learnq.push(['account', '<%=AppConfiguration.KlaviyoApiKey %>']);
    </script>
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
        .redeemed-ticket:hover,
        .redeemed-ticket {
            padding: 0;
        }
        .col-validate {
            
        }
        .col-validate b {
            font-weight: 700;
            font-size: 18px;
            text-transform: uppercase;
        }
        p.h {
            font-size: 20px;
            font-weight: 500;
            color: #4A4A4A;
        }
        #redeemModal .modal-title {
            font-size: 36px;
            font-weight: 700;
        }
        #redeemModal .modal-body > div:nth-child(1) {
            width: 350px;
            margin: 0 auto;
        }
        #redeemModal .modal-body .row {
            height: auto;
            line-height: inherit;
        }
        .modal-body p {
            font-size: 18px;
            font-weight: normal;
            color: #4A4A4A;
            margin-bottom: 5px;
        }
        .success-message {
            font-size: 12px;
            font-weight: 700;
            color: #3bbe81;
        }
        .error-message {
            font-size: 12px;
            font-weight: 700;
            color: #db4658;
            position: inherit;
            left: 0;
            top: 0;
        }
        .margin-bottom-15 {
            margin-bottom: 15px;
        }
    </style>
</asp:Content>