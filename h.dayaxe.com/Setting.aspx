<%@ Page Title="Setting Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Setting.aspx.cs" Inherits="h.dayaxe.com.Setting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="hotel-listings">
        <div class="row padding-top-45">
            <div class="col-md-12">
                <asp:Label ID="MessageLabel" runat="server" ForeColor="Red" Text=""></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-8">
                <label>Redemption Alerts Sent to Email(s):</label>
                <br/>
                <label class="margin-t-25">Day Passes:</label>
                <asp:TextBox runat="server" ID="BookingConfirmEmailText" CssClass="form-control"></asp:TextBox>
                <label class="margin-t-25">Cabanas & Daybeds:</label>
                <asp:TextBox runat="server" ID="BookingConfirmEmailCabanasText" CssClass="form-control"></asp:TextBox>
                <label class="margin-t-25">Spa Passes:</label>
                <asp:TextBox runat="server" ID="BookingConfirmEmailSpaText" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-4 padding-top-25">
                <span>Add an email address where we should send the hotel a copy of the guest redemption details, separating multiple email recipients by semicolon.
                </span>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-3">
                <label>Validation Pin Code:</label>
                <asp:TextBox runat="server" ID="PinCode" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="row margin-t-25">
            <div class="col-md-8">
                <label>Report Subscribers:</label>
                <asp:TextBox runat="server" ID="ReportSubscribersText" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <div class="row padding-t-60">
            <div class="col-md-10 text-right">
                <div class="saving hidden">Saving...</div>
            </div>
            <div class="col-md-2 text-right">
                <asp:Button class="btn btn-save" runat="server" Text="Save" OnClientClick="$('.saving').removeClass('hidden');" OnClick="SaveSettingClick"></asp:Button>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
</asp:Content>