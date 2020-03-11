<%@ Page Title="Redemption Setting" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="InventoryAndPricing.aspx.cs" Inherits="h.dayaxe.com.InventoryAndPricing" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content runat="server" ContentPlaceHolderID="CssContentPlaceHolder">
    <style>
        .tab-content {
            border: solid 1px #d2d2d2;
            padding: 15px;
            margin-bottom: 35px;
        }
        label.title-r {
            font-size: 14px;
        }
        .product-type {
            font-size: 10px;
            text-transform: uppercase;
            font-weight: bold;
            margin-top: 15px;
            margin-bottom: 10px;
            color: #666;
        }
        .product-name {
            font-size: 14px;
            text-transform: none;
            color: #9b9b9b;
            font-weight: 400;
            white-space: nowrap;
        }
        .settings hr {
            margin-top: 10px;
            margin-bottom: 10px;
        }
        .settings .form-control {
            font-size: 18px;
            background-color: #f6f6f6;
            border: solid 1px #d2d2d2;
            height: 45px;
            border-radius: 0;
        }
        .row-price {
            margin-right: 0px;
        }
        .row-price > div > div > div {
            padding-left: 5px;
            padding-right: 5px;
        }
        .row-label .col-md-9{
            padding-left: 5px;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="Content">
    <div class="row margin-t-50">
        <div class="col-md-12">
            <div class="btn-group tab tab-2" role="group" aria-label="">
                <a type="button" class="btn btn-tab active" href="InventoryAndPricing.aspx">Core Products</a>
                <a type="button" class="btn btn-tab" href="InventoryAndPricingAddOns.aspx">Add Ons</a>
            </div>
        </div>
    </div>
    <div class="settings tab-content">
        <div class="row">
            <div class="col-md-12">
                <asp:Repeater runat="server" ID="RptProducts" OnItemDataBound="RptProductsOnItemDataBound">
                    <HeaderTemplate>
                        <div class="row">
                            <div class="col-md-2">
                            
                            </div>
                            <div class="col-md-10">
                                <div class="row row-price">
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3">
                                                
                                            </div>
                                            <div class="col-md-3">
                                                <label class="month">Mon</label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="month">Tue</label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="month">Wed</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3">
                                                <label class="month">Thu</label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="month">Fri</label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="month">Sat</label>
                                            </div>
                                            <div class="col-md-3">
                                                <label class="month">Sun</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-md-12">
                                <hr/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-2">
                                <div class="row">
                                    <div class="col-md-12">
                                        <img class="img-responsive" src='<%#Eval("AdminImageUrl") %>' style="max-width: 140px; margin-top: 28px;" alt=""/>
                                        <h4 class="product-type">
                                            <%# ((Enums.ProductType)Eval("ProductType")).ToDescription() %>
                                        </h4>
                                        <h5 class="product-name">
                                            <%#Eval("ProductName") %>
                                        </h5>
                                        <asp:HiddenField runat="server" ID="HidId" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-10">
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="row row-label">
                                            <div class="col-md-3"></div>
                                            <div class="col-md-9">
                                                <label class="title-r">Price:</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6"></div>
                                </div>
                                <div class="row row-price">
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3">
                                                
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularMonText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularTueText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularWedText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularThuText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularFriText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularSatText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="RegularSunText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <%--<div class="row">
                                    <div class="col-md-6">
                                        <div class="row row-label">
                                            <div class="col-md-3"></div>
                                            <div class="col-md-9">
                                                <label class="title-r">Upgrade Prices:</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6"></div>
                                </div>
                                <div class="row row-price">
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3"></div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeMonText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeTueText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeWedText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeThuText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeFriText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeSatText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="UpgradeSunText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>--%>
                                <div class="row">
                                    <div class="col-md-3"></div>
                                    <div class="col-md-9">
                                        <asp:RequiredFieldValidator ID="RequiredMon" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularMonText"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="RequiredTue" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularTueText"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="RequiredWed" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularWedText"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="RequiredThu" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularThuText"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFri" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularFriText"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="RequiredSat" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularSatText"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="RequiredSun" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Regular price is required" ControlToValidate="RegularSunText"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="row row-label">
                                            <div class="col-md-3"></div>
                                            <div class="col-md-9">
                                                <label class="title-r">Tickets:</label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6"></div>
                                </div>
                                <div class="row row-price">
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3"></div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantityMonText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantityTueText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantityWedText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="row">
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantityThuText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantityFriText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantitySatText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-md-3">
                                                <asp:TextBox ID="QuantitySunText" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-9 text-right">
            <div class="saving hidden" id="saving" runat="server">Saving...</div>
        </div>
        <div class="col-md-3 text-right">
            <asp:Button class="btn btn-save" runat="server" Text="Save" OnClientClick="$('.saving').text('Saving...').removeClass('hidden');" OnClick="SavePassClick"></asp:Button>
        </div>
    </div>
</asp:Content>
