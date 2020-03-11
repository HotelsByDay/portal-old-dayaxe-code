<%@ Page Title="" Language="C#" MasterPageFile="~/SingleColumn.master" AutoEventWireup="true" CodeFile="conf-error.aspx.cs" Inherits="ErrorPages_conf_error" %>

<asp:Content runat="server" ContentPlaceHolderID="HeaderTitle">We’re Sorry…</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="container book-error">
                <div class="row main">
                    <div class="col-md-7 col-md-push-5 col-sm-12 col-xs-12 margin-left-0 margin-bottom-25">
                        <div class="additional-text">
                            <p>There was an error processing your</p>
                            <p>request. Please try again in 30</p>
                            <p>seconds. That’s all we know.</p>
                        </div>
                        <div>
                            <hr class="error-hr"/>
                        </div>
                        <div class="additional-text-2">
                            <p>We sent this guy to find out what happened.</p>
                            <p>Rest assured, it will be dealt with.</p>
                        </div>
                        <div>
                            <a onclick="delayer()" runat="server" class="back-home" href="/">
                                home
                            </a>
                        </div>
                    </div>
                    <div class="col-md-5 col-md-pull-7 col-xs-12 col-sm-12">
                        <div class="picture-wrapper">
                            <img src="/images/appnovation404-472x295.png" class="img-responsive" alt="oops!"/>
                        </div>
                    </div>
                </div>
            </div>
</asp:Content>
