<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="BookingHandler.aspx.cs" Inherits="h.dayaxe.com.BookingHandler" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <div class="booking">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <table class="table table-booking">
                    <asp:Repeater ID="HistoricalRepeater" runat="server" OnItemDataBound="HistoricalRepeater_ItemDataBound">
                        <HeaderTemplate>
                            <thead>
                                <tr>
                                    <th>Booking Info</th>
                                    <th>QR Code</th>
                                </tr>
                            </thead>
                            <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr id="rowHistory" runat="server">
                                <td>
                                    <%# Eval("FullName") %>
                                    <br/>
                                    <%# ((DateTime)Eval("BookedDate")).ToString("M/d/yy") %>
                                    <br/>
                                    <%# ((DateTime?)Eval("CheckinDate")).HasValue ? ((DateTime?)Eval("CheckinDate")).Value.ToString("M/d/yy hh:mm tt") : "" %>
                                    <br/>
                                    <%#Eval("PassExpires") %>
                                    <br/>
                                    <%# ((Enums.PassStatus)Eval("PassStatus") ==Enums.PassStatus.Redeemed ? "<span class=\"redeemed\">Redeemed</span>" : "<span class=\"not-redeemed\">Not Redeemed</span>") %>
                                </td>
                                <td>
                                    <asp:Image ID="QRImage" runat="server" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                        </FooterTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

