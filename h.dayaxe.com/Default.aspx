<%@ Page Title="" Language="C#" MasterPageFile="~/SingleColumn.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="h.dayaxe.com.Default" %>
<asp:Content runat="server" ContentPlaceHolderID="HeaderTitle">Hotel Admin Login</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="login">
        <div class="row">
            <div class="col-md-12">
                <form class="border-box-large" runat="server">
                    <div class="form-group form-group-email">
                        <asp:TextBox type="text" CssClass="form-control" placeholder="Email Address" runat="server" ID="Email" />
                    </div>
                    <div class="form-group form-group-pass">
                        <asp:TextBox type="password" CssClass="form-control" placeholder="Password" runat="server" ID="Password" />
                    </div>
                    <div>
                        <a href="/reset-password">Forgot Password</a>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="LblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Email is required" ForeColor="Red" ControlToValidate="Email"></asp:RequiredFieldValidator>
                        <br />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Password is required" ForeColor="Red" ControlToValidate="Password"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Button class="btn btn-save" Text="Login" runat="server" ID="Login" OnClick="LoginClick"></asp:Button>
                    </div>
                    <div class="form-group height-50">
                        <label class="have-account">Don't have an account?</label>
                        <a href="http://hotels.dayaxe.com" class="btn btn-cancel">Become a Partner</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
</asp:Content>