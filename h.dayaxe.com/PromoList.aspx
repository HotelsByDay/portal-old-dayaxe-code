<%@ Page Title="Promo List Page" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="PromoList.aspx.cs" Inherits="h.dayaxe.com.PromoList" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
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
        <div class="row" runat="server" id="AddNewRow">
            <div class="col-md-12 text-right margin-t-15">
                <a href="/PromoDetails.aspx?id=0" class="btn btn-cancel">Add New</a>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 margin-t-15">
                <asp:Repeater runat="server" ID="RptDiscountListings" OnItemDataBound="RptDiscountListings_ItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                            <thead>
                                <th>Name</th>
                                <th>Start</th>
                                <th>End</th>
                                <th>Discount</th>
                                <th>Discount Code</th>
                                <th>Code Required</th>
                                <th>Status</th>
                                <th>Uses</th>
                                <th>Actions</th>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHotel" runat="server">
                            <td>
                                <%# Eval("DiscountName") %>
                            </td>
                            <td>
                                <%# ((DateTime)Eval("StartDate")).ToString("MM/dd/yyyy") %>
                            </td>
                            <td>
                                <%# ((DateTime)Eval("EndDate")).ToString("MM/dd/yyyy") %>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="PercentOffLit"></asp:Literal>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("CodeRequired")) %>' />
                            </td>
                            <td>
                                <asp:Label runat="server" ID="LitStatus"><%# Eval("Status") %></asp:Label>
                            </td>
                            <td>
                                <%# Eval("DiscountUses") %>
                            </td>
                            <td>
                                <a href="/PromoDetails.aspx?id=<%# Eval("Id") %>" class="edit-hotel">
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
                            <td colspan="8">
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
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    
</asp:Content>