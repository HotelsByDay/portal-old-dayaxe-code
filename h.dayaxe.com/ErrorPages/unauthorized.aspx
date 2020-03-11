<%@ Page Title="" Language="C#" MasterPageFile="~/SingleColumn.master" AutoEventWireup="true" CodeFile="unauthorized.aspx.cs" Inherits="ErrorPages_unauthorized" %>

<asp:Content runat="server" ContentPlaceHolderID="HeaderTitle">Not Authorized</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="container book-error">
        <div class="row main">
            <div class="col-md-12 col-sm-12 col-xs-12 additional-text">
                <p>Oops!</p>
                <p>Looks like you are not authorized</p>
                <p>to access this page.</p>
            </div>
        </div>
        <div class="row main">
            <div class="col-md-12 col-sm-12 col-xs-12">
                <hr class="error-hr" />
            </div>
        </div>
        <div class="row main">
            <div class="col-md-12 col-sm-12 col-xs-12 additional-text-2">
                <a onclick="delayer()" runat="server" class="back-home" href="/">home</a>
            </div>
        </div>
    </div>
</asp:Content>