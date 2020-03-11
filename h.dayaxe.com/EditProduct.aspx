<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EditProduct.aspx.cs" Inherits="h.dayaxe.com.EditProduct" %>
<%@ Register TagPrefix="CKEditor" Namespace="CKEditor.NET" Assembly="CKEditor.NET, Version=3.6.6.2, Culture=neutral, PublicKeyToken=e379cdf2f8354999" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="hotel-listings product">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
        <div class="row amenties">
            <div class="col-md-12">
                <ul class="nav nav-tabs">
                    <li class="active">
                        <a data-toggle="tab" href="#detail">
                            PRODUCT DETAILS
                        </a>
                    </li>
                    <li>
                        <a data-toggle="tab" class="gym" href="#price">
                            PRICE
                        </a>
                    </li>
                    <li>
                        <a data-toggle="tab" href="#seo" class="spa">
                            SEO
                        </a>
                    </li>
                    <li>
                        <a data-toggle="tab" href="#photos" class="business-center">
                            PHOTOS
                        </a>
                    </li>
                </ul>
                <div class="tab-content col-md-12 product">
                    <div id="detail" class="tab-pane fade in active">
                        <div class="row margin-t-25">
                            <div class="col-md-3">
                                <label>Product Type</label>
                            </div>
                            <div class="col-md-3">
                                <label>Product Name</label>
                            </div>
                            <div class="col-md-3 text-right">
                                <span id="cntrProductName"></span> characters left
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <asp:DropDownList runat="server" ID="ProductTypeDdl" CssClass="form-control"/>
                            </div>
                            <div class="col-md-6">
                                 <asp:TextBox runat="server" ID="ProductNameText" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row margin-t-25">
                            <div class="col-md-3">
                                <label>Max Guests</label>
                            </div>
                            <div class="col-md-3">
                                <label>Is Featured?</label>
                            </div>
                        </div>
                        <div class="row margin-t-25">
                            <div class="col-md-3">
                                <div class="btn-group">
                                    <button type="button" class="btn btn-default dropdown-toggle btn-kidallow" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span>
                                            <img src="Images/iconCapacity.png" alt="" /></span> 
                                        <asp:Label CssClass="maxguest-current-value" runat="server" ID="CurrentMaxGuest">2</asp:Label> <span class="caret">
                                        </span>
                                    </button>
                                    <asp:HiddenField ID="HidMaxGuest" runat="server" ClientIDMode="Static" />
                                    <ul class="dropdown-menu" runat="server" ID="MaxGuestControl" ClientIDMode="Static"></ul>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <asp:HiddenField runat="server" ID="IsFeaturedRequiredHidden" ClientIDMode="Static" />
                                <div id="isFeaturedRequired" class="toggle toggle-iphone"></div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Product name is required" ControlToValidate="ProductNameText"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="row margin-t-25">
                            <div class="col-md-12">
                                <label>
                                    Product Highlight
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:TextBox runat="server" ID="ProductHighlightText" TextMode="MultiLine" Rows="7" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row margin-t-25">
                            <div class="col-md-6">
                                <label>
                                    What You Get
                                </label>
                            </div>
                            <div class="col-md-6">
                                <label>
                                    Service
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <CKEditor:CKEditorControl ID="WhatYouGetEditor" CssClass="form-control" AutoParagraph="False" TextMode="MultiLine" runat="server"></CKEditor:CKEditorControl>
                            </div>
                            <div class="col-md-6">
                                <CKEditor:CKEditorControl ID="ServiceEditor" CssClass="form-control" AutoParagraph="False" TextMode="MultiLine" runat="server"></CKEditor:CKEditorControl>
                            </div>
                        </div>
                    </div>
                    <div id="price" class="tab-pane  fade">
                        <div class="row margin-t-25">
                            <div class="col-md-3">
                                <label>Redemption Period (In Days)</label>
                            </div>
                            <div class="col-md-3">
                                <label>Check-In Date Required</label>
                            </div>
                            <div class="col-md-3">
                                <label>Kids Allowed?</label>
                            </div>
                            <div class="col-md-3">
                                <%--<label>Daily Sales</label>--%>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="btn-group">
                                    <button type="button" class="btn btn-default btn-kidallow dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span class="glyphicon glyphicon-calendar" aria-hidden="true"></span>
                                        <asp:Label CssClass="redemp-period-value" runat="server" ID="CurrentRedemptionPeriod">30</asp:Label> <span class="caret"></span>
                                    </button>
                                    <asp:HiddenField ID="HidRedemptionPeriod" runat="server" ClientIDMode="Static" />
                                    <ul class="dropdown-menu" runat="server" ID="RedemptionPeriod" ClientIDMode="Static"></ul>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <asp:HiddenField runat="server" ID="IsCheckedInRequiredHidden" ClientIDMode="Static" />
                                <div id="isCheckedInRequired" class="toggle toggle-iphone"></div>
                            </div>
                            <div class="col-md-3">
                                <%--<asp:HiddenField runat="server" ID="IsKidAllowHidden" ClientIDMode="Static" />
                                <div id="isKidAllow" class="toggle toggle-iphone"></div>--%>
                                <asp:DropDownList ID="KidAllowedDdl" CssClass="form-control" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <%--<div class="btn-group">
                                    <button type="button" class="btn btn-default btn-kidallow dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span>
                                            <img src="Images/iconCapacity.png" alt="" /></span> <asp:Label CssClass="daily-current-value" runat="server" ID="CurrentPass" ClientIDMode="Static">2</asp:Label> <span class="caret"></span>
                                    </button>
                                    <asp:HiddenField ID="HidPass" runat="server" ClientIDMode="Static" />
                                    <ul class="dropdown-menu" runat="server" ID="DailyCapacity" ClientIDMode="Static"></ul>
                                </div>--%>
                            </div>
                        </div>
                        <div class="row margin-t-25 margin-left-15">
                            <div class="col-md-6">
                                <div class="background-blue">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <label>Available Upgrades:</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ProductAvailableUpgradesDdl" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Button ID="AddUpgradesButton" CssClass="btn" OnClick="AddUpgradesButtonClick" runat="server" Text="Add Upgradees"/>
                                        </div>
                                    </div>
                                    <div class="margin-t-15">
                                        <div class="col-md-8 amenties-list">
                                            <asp:Repeater runat="server" ID="RptProductListings" OnItemDataBound="RptProductListingsItemDataBound">
                                                <HeaderTemplate>
                                                    <ul class="row">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li runat="server" id="liAlternatie" class="amenity-item">
                                                        <span><%# Eval("ProductName") %></span>
                                                        <span class="text-right">
                                                            <asp:CheckBox runat="server" ID="ChkRemove"/>
                                                            <asp:HiddenField runat="server" ID="HidProductId" Value='<%# Eval("ProductId") %>'/>
                                                        </span>
                                                    </li>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <li class="footer text-right">
                                                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="UnAssignHotel" Text="Delete" OnClick="RemoveProductClick"></asp:Button>
                                                    </li>
                                                    </ul>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="background-blue">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <label>Available Add-Ons:</label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ProductAvailableAddOnsDdl" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Button ID="AddAddOnsButton" CssClass="btn" OnClick="AddAddOnsButtonClick" runat="server" Text="Add Add-On"/>
                                        </div>
                                    </div>
                                    <div class="margin-t-15">
                                        <div class="col-md-8 amenties-list">
                                            <asp:Repeater runat="server" ID="RptAddOnsListings" OnItemDataBound="RptAddOnsListingsItemDataBound">
                                                <HeaderTemplate>
                                                    <ul class="row">
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <li runat="server" id="liAlternatie" class="amenity-item">
                                                        <span><%# Eval("ProductName") %></span>
                                                        <span class="text-right">
                                                            <asp:CheckBox runat="server" ID="ChkRemove"/>
                                                            <asp:HiddenField runat="server" ID="HidProductId" Value='<%# Eval("ProductId") %>'/>
                                                        </span>
                                                    </li>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <li class="footer text-right">
                                                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="UnAssignHotel" Text="Delete" OnClick="RemoveAddOnsClick"></asp:Button>
                                                    </li>
                                                    </ul>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div></div>
                            </div>
                        </div>
                    </div>
                    <div id="seo" class="tab-pane  fade">
                        <div class="row margin-t-25">
                            <div class="col-md-8">
                                <label>Meta Description</label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:TextBox TextMode="MultiLine" Rows="5" runat="server" ID="MetaDescription" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row margin-t-25">
                            <div class="col-md-8">
                                <label>Meta Keywords</label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:TextBox TextMode="MultiLine" Rows="5" runat="server" ID="MetaKeyword" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div id="photos" class="tab-pane  fade">
                        <div class="row margin-t-25">
                            <div class="col-md-12">
                                <label>Photos</label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <asp:FileUpload runat="server" ID="ProductImage" ClientIDMode="Static" uploadUrl="/Handler/Upload.ashx" accept="image/*"/>
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="UploadImage" runat="server" Text="Upload" CausesValidation="False" CssClass="btn btn-info" OnClick="UploadImageClick" />
                            </div>
                            <div class="col-md-3">
                                <div class="checkbox">
                                    <label><asp:CheckBox runat="server" ID="IsCoverCheckbox"></asp:CheckBox>Is Cover</label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 margin-t-25">
                                <div class="photo-list">
                                    <asp:ListView ID="ProductImageListView" GroupItemCount="1" runat="server">
                                        <LayoutTemplate>
                                            <ul class="row">
                                                <div class="col-md-4" runat="server" id="groupPlaceholder"></div>
                                            </ul>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="HotelImage" CssClass="img-responsive" ImageUrl='<%#Eval("Url") %>'/>
                                            <asp:HiddenField runat="server" ID="Order" Value='<%#Eval("Order") %>'/>
                                            <asp:HiddenField runat="server" ID="PhotoId" Value='<%#Eval("Id") %>'/>
                                            <asp:HiddenField runat="server" ID="IsCover" Value='<%#Eval("IsCover") %>'/>
                                            <div class="file-footer-buttons" style="position: relative; top: -30px; right: 10px;">
                                                <button type="button" class="remove-image btn btn-xs btn-default" title="Remove file" data-value='<%#Eval("Id") %>'>
                                                    <i class="glyphicon glyphicon-trash text-danger"></i>
                                                </button>
                                            </div>
                                            <div><%#Eval("ImagePath").ToString() %></div>
                                        </ItemTemplate>
                                        <GroupTemplate>
                                            <li class="col-md-4 margin-t-25 photo-item">
                                                <div runat="server" id="itemPlaceholder"></div>
                                            </li>
                                        </GroupTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
            </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row margin-t-50">
            <div class="col-md-3">
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" CausesValidation="False" Text="Cancel" OnClick="CancelClick"/>
            </div>
            <div class="col-md-3 text-center">
                <asp:Button runat="server" CssClass="btn btn-cancel delete-product" ID="DeleteButton" CausesValidation="False" Text="Delete" OnClick="DeleteClick"/>
            </div>
            <div class="col-md-3 text-center">
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="Deactivebutton" CausesValidation="False" Text="De-Active" OnClick="DeactiveClick"/>
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="ActiveButton" CausesValidation="False" Text="Active" OnClick="ActiveClick"/>
            </div>
            <div class="col-md-3 text-right">
                <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Save" OnClick="SaveProductClick"/>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/plugins/toggles/toggles.min.js"></script>
    <script src="/Scripts/fileinput.js"></script>
    <script src="/Scripts/editProduct.js" type="text/javascript"></script>
</asp:Content>

