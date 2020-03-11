<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="ImportDiscount.aspx.cs" Inherits="h.dayaxe.com.Helper.HelperImportDiscount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="row margin-t-25">
        <div class="col-md-12">
            <label>
                Choose Excel Files
            </label>
            <asp:FileUpload CssClass="form-control margin-t-25" ID="ImportDiscountFileUpload" runat="server" />
        </div>
    </div>
    <div class="row margin-t-25">
        <div class="col-md-12">
            <asp:Button runat="server" CssClass="btn btn-save" ID="Import" Text="Import" OnClick="Import_OnClick"/>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
</asp:Content>

