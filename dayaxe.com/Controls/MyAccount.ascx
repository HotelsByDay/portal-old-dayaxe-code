<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" CodeFile="MyAccount.ascx.cs" Inherits="dayaxe.com.Controls.MyAccount" %>
<script>
    window.changePassId = '<%=ChangePaswordUpdatePanel.ClientID %>';
</script>
<header class="ma">
    <div class="container">
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <a class="go-back hidden-lg hidden-md" href="#" onclick="javascript:history.go(-1);">
                    <img src="/images/arrow-w.png" alt="arrow-left"/>
                </a>
                <h3 class="title">
                    My Account
                </h3>
            </div>
        </div>
    </div>
</header>
<div class="container wrapper-body">
    <div class="my-account">
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <h4 class="title" runat="server" ID="FullNameHeader"></h4>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:Label ID="MessageLabel" runat="server" CssClass="error-message" Text="" Visible="False"></asp:Label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:TextBox ID="FirstNameText" placeholder="First Name" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:TextBox ID="LastNameText" placeholder="Last Name" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:TextBox ID="EmailAddressText" Enabled="False" CssClass="form-control" runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:TextBox ID="NewPasswordText" AutoPostBack="False" disableautocomplete autocomplete="off" TextMode="Password" placeholder="New Password" CssClass="form-control new-password" runat="server"></asp:TextBox>
            </div>
        </div>
        <asp:UpdatePanel runat="server" ID="ChangePaswordUpdatePanel" UpdateMode="Conditional" Visible="True">
            <ContentTemplate>
                <div id="changePass" runat="server">
                    <div class="row row-h-20">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <asp:CompareValidator ID="CompareValidator1" CssClass="error-message" ControlToValidate="NewPasswordText" ControlToCompare="ConfirmNewPasswordText" runat="server" ErrorMessage="New Password and Confirm Password do not match"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <asp:TextBox ID="ConfirmNewPasswordText" TextMode="Password" placeholder="Confirm Password" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row margin-bottom-50">
                        <div class="col-md-12 col-sm-12 col-xs-12">
                            <asp:TextBox ID="OldPasswordText" TextMode="Password" placeholder="Old Password" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID ="NewPasswordText" EventName ="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="row row-save">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <asp:Button ID="SaveButton" OnClick="SaveButton_OnClick" CssClass="btn btn-blue-all" runat="server" Text="Save" />
            </div>
        </div>
    </div>
</div>
<footer class="product-footer" id="footer" runat="server">
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <div id="navbar">
                    <ul class="nav navbar-nav">
                        <li class="active">
                            <a href="https://dayaxe.zendesk.com/hc/en-us/requests/new" target="_blank">Contact Us</a>
                        </li>
                        <li>
                            <a href="http://land.dayaxe.com/how-it-works">How it Works</a>
                        </li>
                        <li>
                            <a href="http://land.dayaxe.com/terms">Terms &amp; Conditions</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <div class="copyright">
                    Copyright 2016 - Dayaxe.com
                </div>
            </div>
        </div>
    </div>
</footer>