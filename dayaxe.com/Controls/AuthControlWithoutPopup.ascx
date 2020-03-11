<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AuthControlWithoutPopup.ascx.cs" Inherits="dayaxe.com.Controls.AuthControlWithoutPopup" %>
<div id="overlayAuth" class="hidden">
    <div id="loadingAuth">
        <span>Processing...please wait</span>
        <img src="/images/loading.gif" class="img-responsive loading" alt="Processing...please wait"/>
    </div>
</div>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="authModal" class="n-popup">
            <div class="vertical-alignment-h">
                <asp:MultiView runat="server" ID="AuthMultiView" ActiveViewIndex="1">
                    <asp:View runat="server">
                        <div class="modal-dialog vertical-align-c">
                            <!-- Modal content-->
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title">Welcome to DayAxe!</h4>
                                </div>
                                <div class="modal-body text-center">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <span class="location">
                                                Let’s get you signed up. It’ll just take a minute.
                                                <asp:Literal runat="server" ID="AccessHeaderLiteral"></asp:Literal>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 error-message" id="wrapmessagelabel" runat="server">
                                            <asp:Label ID="MessageLabel" runat="server" ForeColor="Red" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 text-center">
                                            <asp:TextBox ID="YourEmail" placeholder="Email Address:" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row button-submit margin-top-20 margin-bottom-10">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <asp:Button ID="GetVipAccess" runat="server" CssClass="btn btn-save" OnClientClick="showModal();" Text="Sign Up" OnClick="GetVipAccessClick" />
                                        </div>
                                    </div>
                                    <div class="row hidden-md hidden-lg">
                                        <div class="col-md-12 col-sm-12 col-xs-12 have-account">
                                            Have Account? <asp:LinkButton ID="SignInLinkMobile" OnClick="SignInLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Sign In</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 terms">
                                            By continuing, you agree to our <a href="http://land.dayaxe.com/terms" class="link-terms" target="_blank">Terms & Conditions</a>
                                        </div>
                                    </div>
                                    <div class="row hidden-sm hidden-xs">
                                        <div class="col-md-12 col-sm-12 col-xs-12 have-account">
                                            Have Account? <asp:LinkButton ID="SignInLink" OnClick="SignInLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Sign In</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server">
                        <div class="modal-dialog vertical-align-c">
                            <!-- Modal content-->
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title">Have Account?</h4>
                                </div>
                                <div class="modal-body text-center">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-5 margin-bottom-5">
                                            <span class="sign-in-label">Sign in to DayAxe:</span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 error-message">
                                            <asp:Label ID="MessageSignInLabel" runat="server" ForeColor="Red" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 margin-bottom-20">
                                            <asp:TextBox ID="EmailText" placeholder="Email" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <asp:TextBox ID="PasswordText" placeholder="Password" TextMode="Password" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 text-left forgot-password">
                                            <asp:LinkButton ID="ForgotPasswordLink" OnClick="ForgotPasswordLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Forgot Password</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="row button-submit">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <asp:Button ID="SignInButton" runat="server" CssClass="btn btn-save" Text="Sign In" OnClick="SignInButtonClick" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 not-a-member">
                                            Don't have account? <asp:LinkButton ID="SignUpLink" OnClick="SignUpLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Sign Up</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server">
                        <div class="modal-dialog vertical-align-c">
                            <!-- Modal content-->
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title">DayAxe</h4>
                                </div>
                                <div class="modal-body text-center">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 margin-top-10 margin-bottom-10">
                                            <span class="forgot-password-label">Forgot Your Password?<br class="hidden-md hidden-lg"/> Enter your email and <br class="hidden-sm hidden-xs"/> password<br class="hidden-md hidden-lg"/> link will be emailed to you</span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 error-message">
                                            <asp:Label ID="MessageErrorForgotlabel" runat="server" ForeColor="Red" Text=""></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 forgot-password-email">
                                            <asp:TextBox ID="EmailForgotText" placeholder="Email" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row hidden-md hidden-lg">
                                        <div class="col-md-12 col-sm-12 col-xs-12 remember-password">
                                            Remembered password? <asp:LinkButton ID="SignIn2Link" OnClick="SignInLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Sign In</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="row button-submit">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <asp:Button ID="GetPasswordButton" runat="server" CssClass="btn btn-save btn-getpass" Text="Get Password" OnClientClick="showModal();" OnClick="GetPasswordButtonClick" />
                                        </div>
                                    </div>
                                    <div class="row hidden-xs hidden-sm">
                                        <div class="col-md-12 col-sm-12 col-xs-12 remember-password">
                                            Remembered password? <asp:LinkButton ID="SignIn3Link" OnClick="SignInLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Sign In</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server">
                        <div class="modal-dialog vertical-align-c">
                            <!-- Modal content-->
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title">DayAxe</h4>
                                </div>
                                <div class="modal-body text-center">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 text-center margin-top-10 margin-bottom-10">
                                            <span>Create new password:</span>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 error-message">
                                            <asp:Label ID="MessageChangePasswordLabel" runat="server" ForeColor="Red" Text=""></asp:Label>
                                            <asp:TextBox ID="HidePassword" autocomplete="off" TextMode="Password" CssClass="form-control hidden" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 margin-bottom-20">
                                            <asp:TextBox ID="NewPassText" autocomplete="off" TextMode="Password" placeholder="New Password" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 margin-bottom-15 confirm-password">
                                            <asp:TextBox ID="ConfirmPasswordText" autocomplete="off" TextMode="Password" placeholder="Confirm Password" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row button-submit button-changep">
                                        <div class="col-md-6 col-sm-6 col-xs-6">
                                            <asp:Button ID="CancelChangePasswordButton" runat="server" CssClass="btn btn-cancel" Text="Cancel" OnClientClick="showModal();" OnClick="CancelChangePasswordButtonOnClick" />
                                        </div>
                                        <div class="col-md-6 col-sm-6 col-xs-6">
                                            <asp:Button ID="ChangePasswordButton" runat="server" CssClass="btn btn-save" Text="Submit" OnClientClick="showModal();" OnClick="ChangePasswordButtonClick" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12 not-a-member">
                                            Remembered password? <asp:LinkButton OnClick="RememberPasswordLinkClick" OnClientClick="showModal();" CssClass="link" runat="server">Sign In</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
            </div>
        </div>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="GetVipAccess" EventName="Click" />
        <asp:AsyncPostBackTrigger ControlID="SignInLink" EventName="Click"/>
        <asp:AsyncPostBackTrigger ControlID="ForgotPasswordLink" EventName="Click"/>
        <asp:AsyncPostBackTrigger ControlID="SignUpLink" EventName="Click"/>
        <asp:AsyncPostBackTrigger ControlID="SignInButton" EventName="Click"/>
        <asp:AsyncPostBackTrigger ControlID="GetPasswordButton" EventName="Click"/>
        <asp:AsyncPostBackTrigger ControlID="ChangePasswordButton" EventName="Click"/>
    </Triggers>
</asp:UpdatePanel>
<script type="text/javascript">
    function showModal() {
        $('#overlayAuth').removeClass('hidden');
    }
</script>