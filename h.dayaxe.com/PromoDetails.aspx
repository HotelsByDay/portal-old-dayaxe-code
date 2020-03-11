<%@ Page Title="Promo Detail Page" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="PromoDetails.aspx.cs" Inherits="h.dayaxe.com.PromoDetails" %>
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
        <div class="row">
            <div class="col-md-12 margin-t-25">
                <asp:Label ID="LblMessage" ForeColor="Red" runat="server" Text="" Visible="False"></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-10">
                <label>Discount Name</label>
                <br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="DiscountNameText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-10">
                <label>Discount Fine Print</label><br/>
                <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" ID="FinePrintText"></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-2">
                <label>Start Date</label><br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="StartDateText" ClientIDMode="Static" CssClass="form-control datepicker"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <label>End Date</label><br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="EndDateText" ClientIDMode="Static" CssClass="form-control datepicker"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <label>Code</label><br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="CodeText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <label>Code Required</label><br/>
                <asp:HiddenField runat="server" ID="IsCodeRequiredHidden" ClientIDMode="Static"/>
                <div id="isCodeRequired" class="toggle toggle-iphone margin-t-15"></div>
            </div>
            <div class="col-md-2">
                <label>Number of billing cycles</label>
                <asp:DropDownList runat="server" ID="BillingCycleDdl" CssClass="form-control" ClientIDMode="Static"></asp:DropDownList>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-2">
                <label>Promo Type</label><br/>
                <asp:DropDownList runat="server" ID="PromoTypeDdl" CssClass="form-control" ClientIDMode="Static">
                    <Items>
                        <asp:ListItem Text="Percent Promo" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Fixed Promo" Value="1"></asp:ListItem>
                    </Items>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label>Amount Off (<span class="amount-off">%</span>)</label><br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="PercentOffText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <label>Min Amount($)</label><br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="MinAmountText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <label>All Products</label><br/>
                <asp:HiddenField runat="server" ID="IsAllProductHidden" ClientIDMode="Static"/>
                <div id="isAllProduct" class="toggle toggle-iphone margin-t-15"></div>
            </div>
            <div class="col-md-2">
                <label>Max Purchases</label>
                <asp:DropDownList runat="server" ID="MaxPurchasesDdl" CssClass="form-control" ClientIDMode="Static"></asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col-md-10 margin-t-25">
                <div class="hotel-manage margin-t-25">
                    <div class="form-group">
                        <label>
                            Products On Sale:
                        </label>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:DropDownList ID="DdlHotels" CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 amenties-list margin-t-25">
                                        <asp:Repeater runat="server" ID="RptHotelListings" OnItemDataBound="RptHotelListings_ItemDataBound">
                                            <HeaderTemplate>
                                                <ul class="row">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li runat="server" id="liAlternatie" class="amenity-item">
                                                    <span><%# Eval("HotelInfo") %></span>
                                                    <span class="text-right">
                                                        <asp:CheckBox runat="server" ID="ChkRemove" />
                                                        <asp:HiddenField runat="server" ID="HidProductId" Value='<%# Eval("ProductId") %>' />
                                                        <asp:HiddenField runat="server" ID="HidDiscountId" Value='<%# Eval("DiscountId") %>' />
                                                    </span>
                                                </li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                    <li class="footer text-right">
                                                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="UnAssignHotel" Text="Remove" OnClick="UnAssignHotelClick"></asp:Button>
                                                    </li>
                                                </ul>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Button ID="BtnAddDiscountHotel" CssClass="btn width-100" OnClick="BtnAddDiscountHotel_OnClick" runat="server" Text="Add Hotel"/>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Button ID="BtnAddAllDiscountHotel" CssClass="btn  width-100 margin-t-25" OnClick="BtnAddAllDiscountHotel_OnClick" runat="server" Text="Add All"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-10 margin-t-25">
                <div class="hotel-manage margin-t-25">
                    <div class="form-group">
                        <label>
                            Subscriptions On Sale:
                        </label>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:DropDownList ID="DdlSubscription" CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12 amenties-list margin-t-25">
                                        <asp:Repeater runat="server" ID="RptSubscriptionListings" OnItemDataBound="RptSubscriptionListings_OnItemDataBound">
                                            <HeaderTemplate>
                                                <ul class="row">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <li runat="server" id="liAlternatie" class="amenity-item">
                                                    <span><%# Eval("SubscriptionName") %></span>
                                                    <span class="text-right">
                                                        <asp:CheckBox runat="server" ID="ChkRemove" />
                                                        <asp:HiddenField runat="server" ID="HidProductId" Value='<%# Eval("SubscriptionId") %>' />
                                                        <asp:HiddenField runat="server" ID="HidDiscountId" Value='<%# Eval("DiscountId") %>' />
                                                    </span>
                                                </li>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                    <li class="footer text-right">
                                                        <asp:Button runat="server" CssClass="btn btn-cancel" Text="Remove" OnClick="RemovePromoSubscriptionClick"></asp:Button>
                                                    </li>
                                                </ul>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Button ID="BtnAddDiscountSubscription" CssClass="btn width-100" OnClick="BtnAddDiscountSubscription_OnClick" runat="server" Text="Add Subscription"/>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:Button ID="BtnAddAllDiscountSubscription" CssClass="btn  width-100 margin-t-25" OnClick="BtnAddAllDiscountSubscription_OnClick" runat="server" Text="Add All"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row margin-t-50">
            <div class="col-md-3">
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" Text="Cancel" OnClick="CancelClick"/>
            </div>
            <div class="col-md-3 text-center">
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteButton" Text="Delete" OnClick="DeleteClick"/>
            </div>
            <div class="col-md-6 text-right">
                <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" Text="Save" OnClick="SaveDiscountClick"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/jquery.ignore.js"></script>
    <script src="/Scripts/plugins/toggles/toggles.min.js"></script>
    <script src="/Scripts/bootstrap-datepicker.js"></script>
    <script src="/Scripts/discountpage.js"></script>
    <script>
        $(function() {
            $('.datepicker').datepicker();
        });
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
    </style>
</asp:Content>