<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Feedback.aspx.cs" Inherits="h.dayaxe.com.Feedback" %>
<%@ Import Namespace="DayaxeDal" %>
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
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="row revenue">
        <div class="col-md-10">
            <div class="btn-group select-range padding-t-20">
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
    <div class="row">
        <div class="col-md-12">
            <asp:Label runat="server" ID="ErrorMessageLabel" ForeColor="Red"></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <div class="btn-group tab tab-3" role="group" aria-label="">
                <a type="button" class="btn btn-tab" href="Demographics.aspx">Demographics</a>
                <a type="button" class="btn btn-tab" href="Engagement.aspx">Engagement</a>
                <a type="button" class="btn btn-tab active" href="Feedback.aspx">Feedback</a>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <asp:Repeater runat="server" ID="RptFeedback" OnItemDataBound="RptFeedback_ItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking table-rating">
                            <thead>
                                <th>Name
                                </th>
                                <th>Tickets</th>
                                <th>Check-In</th>
                                <th>Feedback
                                </th>
                                <th>Est. Spend
                                </th>
                                <th>User Rating
                                </th>
                                <th runat="server" visible='<%#IsAdmin %>'>Actions
                            </th>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr runat="server" id="rowHotel">
                            <td>
                                <%# Eval("CustomerFullName") %>
                            </td>
                            <td>
                                <%# Eval("Quantity") %>
                            </td>
                            <td>
                                <%# ((DateTime?)Eval("RedeemedDate")).HasValue ? ((DateTime)Eval("RedeemedDate")).ToString(Constant.FullDateFormat) : string.Empty %>
                            </td>
                            <td>
                                <%# Eval("RateCommend") %>
                            </td>
                            <td>
                                <%# String.Format("${0}", Eval("EstSpend")) %>
                            </td>
                            <td style="white-space: nowrap;">
                                <%# Eval("UserRating") %>
                            </td>
                            <td data-id='<%# Eval("Id") %>' runat="server" visible='<%#IsAdmin %>' class="text-center">
                                <a href="/EditFeedback.aspx?id=<%# Eval("Id") %>" class="edit-hotel">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="<%#IsAdmin ? 6 : 5 %>">
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
                                <span class="result-hotel">
                                    <asp:Literal runat="server" ID="LitTotal"></asp:Literal>
                                </span>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                    </FooterTemplate>
                </asp:Repeater>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/jquery.ignore.js"></script>
    <script src="/Scripts/bootstrap-datepicker.js"></script>
    <script>
        $(function() {
            $('.datepicker').datepicker();
        });
    </script>
</asp:Content>

