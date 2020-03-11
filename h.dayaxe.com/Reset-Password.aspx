<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reset-Password.aspx.cs" Inherits="ResetPassword" MasterPageFile="~/SingleColumn.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeaderTitle">Hotel Admin Remind Password</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <asp:PlaceHolder runat="server" ID="phRemindPassword">
        <div class="login">
            <div class="row">
                <div class="col-md-12">
                    <form class="border-box-large" runat="server">
                        <p style="font-size: 20px; font-weight: 400;">Forgot Your Password? Enter your email and password link will be emailed to you</p>
                        <div class="form-group form-group-email">
                            <asp:TextBox runat="server" ID="tbEmail" CssClass="form-control" placeholder="Email" />
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" ID="lblMessage" ForeColor="Red"></asp:Label>
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="Email is required" ForeColor="Red" ControlToValidate="tbEmail"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group">
                            <asp:Button class="btn btn-save" Text="Get Password" runat="server" ID="btnGetPassword" OnClick="btnGetPassword_Click"></asp:Button>
                        </div>
                        <div class="form-group">
                            <p>Remembered password? <a href="/">Sign In</a></p>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="phResetPassword" Visible="false">
        <div class="login">
            <div class="row">
                <div class="col-md-12">
                    <form class="border-box-large" runat="server">
                        <p style="font-size: 20px; font-weight: 400;">Create new password:</p>
                        <div class="form-group form-group-email">
                            <asp:TextBox runat="server" ID="tbNewPassword" TextMode="Password" CssClass="form-control" placeholder="New Password" />
                        </div>
                        <div class="form-group form-group-email">
                            <asp:TextBox runat="server" ID="tbConfirmPassword" TextMode="Password" CssClass="form-control" placeholder="Confirm Password" />
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" ID="lblMessageResetPassword" ForeColor="Red" />
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="New Password is required" ForeColor="Red" ControlToValidate="tbNewPassword" />
                            <asp:RequiredFieldValidator runat="server" ErrorMessage="Confirm Password is required" ForeColor="Red" ControlToValidate="tbConfirmPassword" />
                            <asp:CompareValidator runat="server" ErrorMessage="Confirm Password must match Password" ForeColor="Red" ControlToValidate="tbConfirmPassword" ControlToCompare="tbNewPassword" />
                        </div>
                        <div class="form-group">
                            <asp:Button class="btn btn-save" Text="Submit" runat="server" ID="btnSubmitResetPassword" OnClick="btnSubmitResetPassword_Click" />
                        </div>
                        <div class="form-group">
                            <p>Remembered password? <a href="/">Sign In</a></p>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
