<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="GiftCardDetails.aspx.cs" Inherits="h.dayaxe.com.GiftCardDetails" %>
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
                <label>Name</label>
                <br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="NameText" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-10">
                <label>Code</label><br/>
                <asp:TextBox runat="server" ID="CodeText" ReadOnly="True" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-10">
                <label>Amount($)</label><br/>
                <asp:TextBox runat="server" EnableViewState="False" ID="AmountText" CssClass="form-control"></asp:TextBox>
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
    
</asp:Content>