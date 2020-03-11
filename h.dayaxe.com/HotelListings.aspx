<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="HotelListings.aspx.cs" Inherits="h.dayaxe.com.HotelListings" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <div class="hotel-listings">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
        <div class="row" runat="server" id="AddNewRow" Visible="False">
            <div class="col-md-12 text-right margin-t-15">
                <a href="EditHotel.aspx?id=0" class="btn btn-cancel">Add New</a>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 margin-t-15">
                <asp:Repeater runat="server" ID="RptHotelListings" OnItemDataBound="RptHotelListings_ItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                            <thead>
                                <th style="text-align: center;">Thumbnail
                                </th>
                                <th>Hotel Name
                                </th>
                                <th>Neighborhood
                                </th>
                                <th>Hotel Type
                                </th>
                                <th>Active
                                </th>
                                <th  runat="server" visible='<%#IsAdmin %>'>Actions
                                </th>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHotel" runat="server" class="hotel-item" data-href='<%#string.Format("/Revenues.aspx?hotelId={0}", Eval("HotelId")) %>'>
                            <td>
                                <asp:Image runat="server" ID="ThumbnailImage" Width="140px" ImageUrl='<%# Eval("ImageSurveyUrl") %>' />
                            </td>
                            <td>
                                <%# Eval("HotelName") %>
                            </td>
                            <td>
                                <%# Eval("Neighborhood") %>
                            </td>
                            <td>
                                <%# ((DayaxeDal.Enums.Hoteltype)Eval("HotelTypeId")).ToString() %>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("IsActive")) %>' />
                            </td>
                            <td data-id='<%# Eval("HotelId") %>'  runat="server" visible='<%#IsAdmin %>'>
                                <a href="#" class="de-active">
                                    <span class="glyphicon glyphicon-download-alt"></span>
                                </a>
                                <a href="/EditHotel.aspx?id=<%# Eval("HotelId") %>" class="edit-hotel">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                                <a href="#" class="delete-hotel">
                                    <span class="glyphicon glyphicon-trash"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan='<%#IsAdmin ? 5 : 4 %>'>
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
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptPlaceHolder">
    <script src="/Scripts/hotellist.js" type="text/javascript"></script>
</asp:Content>