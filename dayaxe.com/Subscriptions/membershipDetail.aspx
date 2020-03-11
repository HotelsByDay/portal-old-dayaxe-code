<%@ Page Title="" Language="C#" MasterPageFile="~/Client.master" AutoEventWireup="true" CodeFile="membershipDetail.aspx.cs" Inherits="dayaxe.com.Subscriptions.MembershipDetailPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContentPlaceHolder" Runat="Server">
    <asp:Literal runat="server" ID="TitleLiteral"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MetaContentPlaceHolder" Runat="Server">
    <asp:PlaceHolder runat="server" ID="MetaPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="StyleHeader" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/main" />
    <webopt:bundlereference runat="server" path="~/Content/membership" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ScriptAnalyticsHeader" Runat="Server">
    <asp:PlaceHolder runat="server" ID="ScriptAnalyticsHeaderPlaceHolder"></asp:PlaceHolder>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentPlaceHolder" Runat="Server">
    <header class="reviews">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <a class="go-back hidden-lg hidden-md" href="#" onclick="javascript:history.go(-1);">
                        <img src="/images/arrow-w.png" alt="arrow-left"/>
                    </a>
                    <h3 class="title">
                        Membership
                    </h3>
                </div>
            </div>
        </div>
    </header>
    
    <div class="container wrapper-body">
        <div class="row">
            <div class="col-md-12">
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <h4 class="title">
                            Membership Details
                        </h4>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <asp:Label runat="server" ID="ErrorMessageLabel" CssClass="error-message" Visible="False"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-sm-12 col-xs-12">
                        <h5>
                            Your Plan
                        </h5>
                        <asp:Label runat="server" CssClass="your-plan-label" ID="YourPlanLabel"></asp:Label>
                    </div>
                </div>
                <asp:MultiView runat="server" ID="MemberShipDetailView">
                    <asp:View runat="server">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <asp:LinkButton ID="ActiveMembershipLinkButton" CssClass="btn btn-blue-all btn-active-now" OnClick="ActiveMembershipLinkButton_OnClick" runat="server">Activate Now</asp:LinkButton>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <asp:LinkButton ID="ActiveNowLinkButton" OnClick="ActiveNowLinkButton_OnClick" runat="server">
                                    <img src="<%=Page.ResolveClientUrl("~/images/gold-pass.jpg") %>" class="img-responsive img-gold-pass" alt="" />
                                </asp:LinkButton>
                            </div>
                        </div>
                    </asp:View>
                    <asp:View runat="server">
                        <div id="cancelMembershipModal" class="modal fade" role="dialog" data-keyboard="false" runat="server" ClientIDMode="Static">
                            <div class="vertical-alignment-h">
                                <div class="modal-dialog vertical-align-c">
                                    <!-- Modal content-->
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span>
                                            </button>
                                            <h4 class="modal-title">Are you sure?</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    You are about to cancel your subscription. We hate to see you go! We wanted to remind you that there is a $79 reactivation fee, in case you change your mind.
                                                </div>
                                            </div>
                                            <div class="row button-submit">
                                                <div class="col-md-6 col-sm-6 col-xs-6">
                                                    <asp:LinkButton ID="CancelMembershipLinkButton" CssClass="btn btn-save" OnClick="CancelMembershipLinkButton_OnClick" runat="server">Confirm</asp:LinkButton>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6">
                                                    <button type="button" data-dismiss="modal" class="btn">Cancel</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="activeMembershipModal" class="modal fade" role="dialog" data-keyboard="false" runat="server" ClientIDMode="Static">
                            <div class="vertical-alignment-h">
                                <div class="modal-dialog vertical-align-c">
                                    <!-- Modal content-->
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span>
                                            </button>
                                            <h4 class="modal-title">Are you sure?</h4>
                                        </div>
                                        <div class="modal-body">
                                            <div class="row">
                                                <div class="col-md-12 col-sm-12 col-xs-12">
                                                    You are about to re-activate your subscription. Please confirm.
                                                </div>
                                            </div>
                                            <div class="row button-submit">
                                                <div class="col-md-6 col-sm-6 col-xs-6">
                                                    <asp:LinkButton ID="ReActiveMembershipLinkButton" CssClass="btn btn-save" OnClick="ReActiveMembershipLinkButton_OnClick" runat="server">Confirm</asp:LinkButton>
                                                </div>
                                                <div class="col-md-6 col-sm-6 col-xs-6">
                                                    <button type="button" data-dismiss="modal" class="btn">Cancel</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="c"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <h5>
                                    Your Current Cycle
                                </h5>
                                <div class="valid">
                                    <asp:Literal runat="server" ID="ValidLit"></asp:Literal>
                                </div>
                                <div class="next-cycle">
                                    <asp:Literal runat="server" ID="NextCycleLit"></asp:Literal>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="c"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <h5>
                                    Membership ID
                                </h5>
                                <asp:Label runat="server" CssClass="font-b-14" ID="MembershipIdLabel"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="c"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <h5>
                                    Additional Passes Discount
                                </h5>
                                <asp:Label runat="server" CssClass="font-b-14">GOLDFRIEND</asp:Label>
                                <div>
                                    10% OFF additional passes for friends & family, limit 2 uses per month
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="c"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <h5>
                                    Plan Rules
                                </h5>
                                <div class="plan-rules">
                                    <asp:Literal runat="server" ID="WhatYouGetLit"></asp:Literal>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <hr class="c"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <a id="cancelMembershipLink" runat="server" href="javascript:void(0);" class="btn-cancel-membership">Cancel My Membership</a>
                                <a id="reactiveMembershipLink" runat="server" href="javascript:void(0);" class="btn-active-membership" Visible="False">Re-activate Subscription</a>
                                <div class="faq-membership">
                                    Questions? <a href="mailto:help@dayaxe.com">Reach out to us</a>
                                </div>
                            </div>
                        </div>
                    </asp:View>
                </asp:MultiView>
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
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ScriptContentPlaceHolder" Runat="Server">
    <%: Scripts.Render("~/bundles/membership") %>
</asp:Content>

