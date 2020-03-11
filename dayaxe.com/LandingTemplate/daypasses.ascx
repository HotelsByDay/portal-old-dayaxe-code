<%@ Control Language="C#" AutoEventWireup="true" CodeFile="daypasses.ascx.cs" Inherits="LandingTemplate_daypasses" %>

<section class="banner my-day-passes">
    <div class="color-overlay"></div>
    <div class="border-holder">
        <div class="block-inner">
            <h1>My Day Passes</h1>
        </div>
    </div>
</section>
<section class="day-pass-content">
    <h4 class="title">
        My Day Passes
    </h4>
    <div class="item">
        <ul>
            <asp:Repeater ID="DayPassRepeater" runat="server">
                <ItemTemplate>
                    <li>
                        <asp:HyperLink ID="DayPassLinkButton" runat="server"></asp:HyperLink>
                        <span class="float-right">
                            <asp:HyperLink ID="SubmitSurveyLinkButton" runat="server">Submit Survey</asp:HyperLink>
                            <asp:LinkButton runat="server" ID="CancelLinkButton" Visible="False" OnClientClick="return confirm('Are you sure you\'d like to cancel your ticket?');">Cancel</asp:LinkButton>
                        </span>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</section>