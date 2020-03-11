<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="EditSubscriptions.aspx.cs" Inherits="h.dayaxe.com.EditSubscriptions" %>
<%@ Register TagPrefix="CKEditor" Namespace="CKEditor.NET" Assembly="CKEditor.NET, Version=3.6.6.2, Culture=neutral, PublicKeyToken=e379cdf2f8354999" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
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
        <div class="row">
            <div class="col-md-12">
                <asp:Label runat="server" ID="ErrorMessageLabel" ForeColor="Red" Visible="False"></asp:Label>
            </div>
        </div>
        <div class="row margin-bottom-25">
            <div class="col-md-12">
                <asp:HyperLink runat="server" Target="_blank" ID="PurchaseUrlLink">Purchase Subscription Url</asp:HyperLink>
            </div>
        </div>
        <div class="row amenties">
            <div class="col-md-12">
                <ul class="nav nav-tabs">
                    <li class="active">
                        <a data-toggle="tab" href="#detail">SUBSCRIPTION DETAILS
                        </a>
                    </li>
                    <li>
                        <a data-toggle="tab" href="#seo" class="spa">SEO
                        </a>
                    </li>
                    <li>
                        <a data-toggle="tab" href="#photos" class="business-center">PHOTOS
                        </a>
                    </li>
                </ul>
                <div class="tab-content col-md-12 product">
                    <div id="detail" class="tab-pane fade in active">
                        <div class="row margin-t-25">
                            <div class="col-md-3">
                                <label>Product Type</label>
                            </div>
                            <div class="col-md-2">
                                <label>Product Name</label>
                            </div>
                            <div class="col-md-4 text-right">
                                <span id="cntrProductName"></span>characters left
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <asp:DropDownList runat="server" ID="SubscriptionTypeDdl" CssClass="form-control" />
                            </div>
                            <div class="col-md-6">
                                <asp:TextBox runat="server" ID="SubscriptionNameText" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row margin-t-25">
                            <div class="col-md-2">
                                <label>Max Guests</label>
                            </div>
                            <div class="col-md-2">
                                <label> Price </label>
                            </div>
                            <div class="col-md-2">
                                <label> Max Reserve freepass </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-2">
                                <div class="btn-group">
                                    <button type="button" class="btn btn-default dropdown-toggle btn-kidallow" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span>
                                            <img src="Images/iconCapacity.png" alt="" /></span>
                                        <asp:Label CssClass="maxguest-current-value" runat="server" ID="CurrentMaxGuest">1</asp:Label>
                                        <span class="caret"></span>
                                    </button>
                                    <asp:HiddenField ID="HidMaxGuest" runat="server" ClientIDMode="Static" />
                                    <ul class="dropdown-menu" runat="server" id="MaxGuestControl" clientidmode="Static"></ul>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <asp:TextBox ID="PriceText" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <asp:TextBox ID="MaxPurchaseText" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Subscription name is required" ControlToValidate="SubscriptionNameText"></asp:RequiredFieldValidator>
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
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <CKEditor:CKEditorControl ID="WhatYouGetEditor" CssClass="form-control" AutoParagraph="False" TextMode="MultiLine" runat="server"></CKEditor:CKEditorControl>
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
                                <asp:FileUpload runat="server" ID="SubscriptionImage" ClientIDMode="Static" uploadUrl="/Handler/Upload.ashx" accept="image/*" />
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="UploadImage" runat="server" Text="Upload" CausesValidation="False" CssClass="btn btn-info" OnClick="UploadImageClick" />
                            </div>
                            <div class="col-md-3">
                                <div class="checkbox">
                                    <label>
                                        <asp:CheckBox runat="server" ID="IsCoverCheckbox"></asp:CheckBox>Is Cover</label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 margin-t-25">
                                <div class="photo-list">
                                    <asp:ListView ID="SubscriptionImageListView" GroupItemCount="1" runat="server">
                                        <LayoutTemplate>
                                            <ul class="row">
                                                <div class="col-md-4" runat="server" id="groupPlaceholder"></div>
                                            </ul>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="HotelImage" CssClass="img-responsive" ImageUrl='<%#Eval("Url") %>' />
                                            <asp:HiddenField runat="server" ID="Order" Value='<%#Eval("Order") %>' />
                                            <asp:HiddenField runat="server" ID="PhotoId" Value='<%#Eval("Id") %>' />
                                            <asp:HiddenField runat="server" ID="IsCover" Value='<%#Eval("IsCover") %>' />
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
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" CausesValidation="False" Text="Cancel" OnClick="CancelClick" />
            </div>
            <div class="col-md-3 text-center">
                <asp:Button runat="server" CssClass="btn btn-cancel delete-product" ID="DeleteButton" CausesValidation="False" Text="Delete" OnClick="DeleteClick" />
            </div>
            <div class="col-md-3 text-center">
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="Deactivebutton" CausesValidation="False" Text="De-Active" OnClick="DeactiveClick" />
                <asp:Button runat="server" CssClass="btn btn-cancel" ID="ActiveButton" CausesValidation="False" Text="Active" OnClick="ActiveClick" />
            </div>
            <div class="col-md-3 text-right">
                <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Save" OnClick="SaveSubscriptionClick" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/fileinput.js"></script>
    <script src="/Scripts/editSubscription.js" type="text/javascript"></script>
</asp:Content>

