<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsletterControl.ascx.cs" Inherits="dayaxe.com.Controls.NewsletterControl" %>
<link href='<%=Page.ResolveUrl("~/assets/css/newsletter.css")%>' rel="stylesheet" />
<hr class="newsletter-hr"/>
<div class="wrapper wrapper-newsletter">
    <div class="row row-newsletter">
        <div class="col-md-7 col-xs-12 col-n-left">
            <%--<div class="icon icon-newsletter">
            <img src="../images/email-icon.png" class="img-responsive" />
        </div>--%>
            <div class="text">
                <h4>
                    Save 15% Off Your First Purchase
                </h4>
                <span>
                    Sign up today for free &amp; be the first to know when we add new luxury hotels and run special offers & sales.
                </span>
            </div>
        </div>
        <div class="col-md-5 col-xs-12 col-n-right">
            <div class="form-horizontal">
                <div class="form-group form-group-1">
                    <div class="col-sm-12">
                        <span class="message-newsletter"></span>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-xs-8">
                        <input type="text" class="form-control email-newsletter" placeholder="Enter Email Address" />
                    </div>
                    <div class="col-xs-4">
                        <input type="submit" value="Sign Up" id="SubmitNewsLetter" class="btn btn-submit"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
