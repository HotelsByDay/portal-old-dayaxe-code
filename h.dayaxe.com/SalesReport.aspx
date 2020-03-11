<%@ Page Title="Sale Report Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SalesReport.aspx.cs" Inherits="h.dayaxe.com.SalesReport" %>

<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="DayaxeDal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" runat="Server">
    <style>
        .table-booking thead tr th {
            padding-left: 0;
        }

        .margin-top-20 {
            margin-top: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <div class="booking sales-report">
        <div class="row revenue">
            <div class="col-md-2">
                <asp:DropDownList runat="server" ID="YearDdl" AutoPostBack="True" OnSelectedIndexChanged="YearChange" CssClass="form-control margin-top-20">
                    <asp:ListItem Text="2017" Value="2017"></asp:ListItem>
                    <asp:ListItem Text="2018" Value="2018"></asp:ListItem>
                    <asp:ListItem Text="2019" Value="2019" Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-8">
                <div class="form-inline">
                    <asp:Button runat="server" ID="ExportToExcelButton" OnClick="ExportToExcelButtonOnClick" Text="Export to Excel" CssClass="btn btn-export btn-sales-export" />
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
            <div class="col-md-12">
                <asp:Label runat="server" ID="MessageLabel" ForeColor="Red"></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:Repeater ID="SaleReportRpt" OnItemDataBound="SaleReportRpt_OnItemDataBound" runat="server">
                    <ItemTemplate>
                        <div class="row margin-top-50">
                            <div class="col-md-12">
                                <h4 class="title-sales">Sales Report: <span><%#Eval("ProductObject.ProductName")%></span>
                                </h4>
                            </div>
                        </div>
                        <div class="row margin-bottom-50">
                            <div class="col-md-12">
                                <table class="table table-booking">
                                    <asp:Repeater ID="ProductsRpt" DataSource='<%# Eval("SalesReportObject") %>' OnItemDataBound="ProductsRpt_OnItemDataBound" runat="server">
                                        <HeaderTemplate>
                                            <thead>
                                                <tr>
                                                    <th class="text-center">Month</th>
                                                    <th class="text-center">Inventory</th>
                                                    <th class="text-center">Utilization %</th>
                                                    <th class="text-center">Tickets<br />
                                                        Sold</th>
                                                    <th class="text-center">Tickets<br />
                                                        Redeemed</th>
                                                    <th class="text-center">Tickets<br />
                                                        Expired</th>
                                                    <th class="text-center">Tickets<br />
                                                        Refunded</th>
                                                    <th class="text-center">Gross<br />
                                                        Sales</th>
                                                    <th class="text-center">Net<br />
                                                        Sales</th>
                                                    <th class="text-center">Net<br />
                                                        Revenue</th>
                                                    <th class="text-center">Avg Incremental<br />
                                                        Revenue</th>
                                                    <th class="text-center">%
                                                        <br />
                                                        sold</th>
                                                    <th class="text-center">%
                                                        <br />
                                                        redeemed</th>
                                                    <th class="text-center">%
                                                        <br />
                                                        expired</th>
                                                    <th class="text-center">%
                                                        <br />
                                                        refunds</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr id="rowReport" runat="server">
                                                <td class="text-left text-bold">
                                                    <%# Eval("Month") %>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("Inventory") %>
                                                </td>
                                                <td class="text-center">
                                                    <%# Math.Round((double)Eval("Utilization")) %>%
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("TicketsSold") %>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("TicketsRedeemed") %>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("TicketsExpired") %>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("TicketsRefunded") %>
                                                </td>
                                                <td class="text-right">
                                                    <%# Helper.FormatPrice((double)Eval("GrossSales")) %>
                                                </td>
                                                <td class="text-right">
                                                    <%# Helper.FormatPrice((double)Eval("NetSales")) %>
                                                </td>
                                                <td class="text-right">
                                                    <%# Helper.FormatPrice((double)Eval("NetRevenue")) %>
                                                </td>
                                                <td class="text-right">
                                                    <%# Helper.FormatPrice((double)Eval("AvgIncrementalRevenue")) %>
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("PercentSold") %>%
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("PercentRedeemed") %>%
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("PercentExpired") %>%
                                                </td>
                                                <td class="text-center">
                                                    <%# Eval("PercentRefunded") %>%
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <tr class="footer">
                                                <td class="text-left">Total <%#YearDdl.SelectedValue %></td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalIventoryLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalUtilizationLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalTicketsSoldLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalTicketsRedeemedLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalTicketsExpiredLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalTicketsRefundedLit"></asp:Literal>
                                                </td>
                                                <td class="text-right">
                                                    <asp:Literal runat="server" ID="TotalGrossSalesLit"></asp:Literal>
                                                </td>
                                                <td class="text-right">
                                                    <asp:Literal runat="server" ID="TotalNetSalesLit"></asp:Literal>
                                                </td>
                                                <td class="text-right">
                                                    <asp:Literal runat="server" ID="TotalNetRevenueLit"></asp:Literal>
                                                </td>
                                                <td class="text-right">
                                                    <asp:Literal runat="server" ID="TotalAvgIncrementalRevenueLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalPercentSoldLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalPercentRedeemedLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalPercentExpiredLit"></asp:Literal>
                                                </td>
                                                <td class="text-center">
                                                    <asp:Literal runat="server" ID="TotalPercentRefundedLit"></asp:Literal>
                                                </td>
                                            </tr>
                                            </tbody>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>