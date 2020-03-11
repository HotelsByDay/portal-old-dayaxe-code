<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="CloudinaryEx.aspx.cs" Inherits="h.dayaxe.com.Helper.HelperCloudinaryEx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
    <style>
        .margin-top-25 {
            margin-top: 25px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="row margin-top-25">
        <div class="col-md-4">
            <asp:FileUpload runat="server" ID="ImageFileUpload" ClientIDMode="Static" accept="image/*"/>
        </div>
    </div>
    <div class="row margin-top-25">
        <div class="col-md-4">
            <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Upload" OnClick="SaveButtonOnClick"/>
        </div>
    </div>
    <div class="row margin-top-25">
        <div class="col-md-3">
            <asp:DropDownList ID="DropDownList1" CssClass="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_OnSelectedIndexChanged">
                <Items>
                    <asp:ListItem Text="Normal" Selected="True" Value="Normal"></asp:ListItem>
                    <asp:ListItem Text="Thumbnail" Value="Thumbnail"></asp:ListItem>
                    <asp:ListItem Text="Raw" Value="Raw"></asp:ListItem>
                </Items>
            </asp:DropDownList>
        </div>
    </div>
    <div class="row margin-top-25">
        <div class="col-md-10">
            <asp:Image runat="server" CssClass="img-responsive" ID="ImageCloudinary"/>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
</asp:Content>

