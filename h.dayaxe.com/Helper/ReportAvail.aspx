<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="ReportAvail.aspx.cs" Inherits="h.dayaxe.com.Helper.HelperReportAvail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
    <link href="/Content/bootstrap-datepicker3.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="row margin-t-25">
        <div class="col-md-12">
            <asp:Label runat="server" ID="MessageLabel" ForeColor="Red"></asp:Label>
        </div>
        <div class="col-md-12">
            <label>
                Select Hotel to Export:
            </label>
        </div>
        <div class="col-md-12">
            <asp:Repeater runat="server" ID="RptHotelList">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li style="list-style: none;">
                        <span class="checkbox-inline">
                            <asp:CheckBox runat="server" ID="HotelChecked" Checked="True"/>
                            <%# Eval("HotelName") %>
                        </span>
                        <asp:HiddenField runat="server" ID="HidHotelId" Value='<%#Eval("HotelId") %>'/>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="col-md-12">
            <div class="form-inline">
                <div class="form-group">
                    <asp:DropDownList ID="ProductTypeDdl" ClientIDMode="Static" CssClass="form-control" runat="server">
                        <asp:ListItem Text="All" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Day passes" Value="DayPasses" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Cabanas" Value="Cabanas"></asp:ListItem>
                        <asp:ListItem Text="Daybeds" Value="Daybeds"></asp:ListItem>
                        <asp:ListItem Text="Spa passes" Value="SpaPasses"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label>From </label>
                    <asp:TextBox runat="server" ID="DateFrom" placeholder="Choose date" CssClass="form-control datepicker"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>to </label>
                    <asp:TextBox runat="server" ID="DateTo" placeholder="Choose date" ClientIDMode="Static" CssClass="form-control datepicker"></asp:TextBox>
                </div>
                <asp:Button runat="server" ID="Export" OnClick="Export_OnClick" Text="Export" CssClass="btn btn-blue-all" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/jquery.ignore.js"></script>
    <script src="/Scripts/bootstrap-datepicker.js"></script>
    <script>
        $(function() {
            $('.datepicker').datepicker();
        });
    </script>
</asp:Content>

