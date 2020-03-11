<%@ Page Title="" Language="C#" MasterPageFile="~/SingleColumn.master" AutoEventWireup="true" CodeFile="resource-not-found.aspx.cs" Inherits="ErrorPages_resource_not_found" %>

<asp:Content runat="server" ContentPlaceHolderID="HeaderTitle">Page Not Available</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <div class="container book-error">
        <div class="row main">
            <div class="col-md-12 col-sm-12 col-xs-12 additional-text">
                <p>Oops!</p>
                <p>You requested a page </p>
                <p>that is no longer there.</p>
            </div>
        </div>
        <div class="row main">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <hr class="error-hr" />
            </div>
        </div>
        <div class="row main">
            <div class="col-md-12 col-sm-12 col-xs-12 additional-text-2">
                <a onclick="delayer()" runat="server" class="back-home" href="/">home
                </a>
            </div>
        </div>
    </div>
</asp:Content>
