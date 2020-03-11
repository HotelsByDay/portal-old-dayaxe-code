<%@ Page Language="C#" AutoEventWireup="true" CodeFile="unauthorized.aspx.cs" Inherits="dayaxe.com.ErrorPages.unauthorized" %>
<%@ Import Namespace="DayaxeDal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Not Authorized</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, minimal-ui" />
    <!--[if lte IE 8]><script src="../assets/js/ie/html5shiv.js"></script><![endif]-->
    <link rel="stylesheet" href="../assets/css/main.css" />
    <!--[if lte IE 8]><link rel="stylesheet" href="../assets/css/ie8.css" /><![endif]-->
    <!--[if lte IE 9]><link rel="stylesheet" href="../assets/css/ie9.css" /><![endif]-->
    <!--bootstrap-->
    <link rel="stylesheet" type="text/css" href="../assets/css/bootstrap.min.css" media="all" />
    <link rel="stylesheet" type="text/css" href="../assets/css/bootstrap.min.css" media="all">
    <link rel="stylesheet" type="text/css" href="../assets/css/reset.css">
    <link rel="stylesheet" type="text/css" href="../assets/css/reset2.css">
    <link rel="stylesheet" type="text/css" href="../assets/css/responsive.css">
    <link rel="stylesheet" type="text/css" href="../assets/css/media.css">
    <link href="../assets/css/header-footer.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="../assets/css/booking-error-ext.css">
    <link href="../assets/css/custom-responsive.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="page-wrapper">
            <div id="main">
                <header>
                    <div class="container">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="title margin-t-mobile-18">
                                    Not Authorized
                                </div>
                            </div>
                        </div>
                    </div>
                </header>
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
                            <hr class="error-hr"/>
                        </div>
                    </div>
                    <div class="row main">
                    <div class="col-md-12 col-sm-12 col-xs-12 additional-text-2">
                        <asp:HyperLink onclick="delayer()" runat="server" class="back-home" ID="HomeLink">
                            home
                        </asp:HyperLink>
                    </div>
                </div>
                </div>
                <footer>
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
            </div>
        </div>
    </form>
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript" src="../assets/js/bootstrap.min.js"></script>
</body>
</html>
