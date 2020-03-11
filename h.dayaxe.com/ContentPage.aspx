<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="ContentPage.aspx.cs" Inherits="h.dayaxe.com.ContentPage" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="DayaxeDal" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="user-hotel content-page-admin">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
        <asp:MultiView runat="server" ID="MarketMultiView" ActiveViewIndex="0">
            <asp:View runat="server">
                <div class="row">
                    <div class="col-md-12 text-right margin-t-15">
                        <a href="?id=0" class="btn btn-cancel">Add</a>
                    </div>
                </div>
                <asp:Repeater runat="server" ID="ContentPageRpt" OnItemDataBound="ContentPageRptOnItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                            <thead>
                                <th>Page Name
                                </th>
                                <th>Url
                                </th>
                                <th style="width: 70px;">
                                    Is Home Page
                                </th>
                                <th style="width: 70px;">
                                    Is Active
                                </th>
                                <th style="width: 50px;">Actions
                                </th>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHotel" runat="server">
                            <td>
                                <a href="?id=<%#Eval("Id") %>"><%#Eval("Name") %></a>
                            </td>
                            <td>
                                <%# Eval("UrlSegment") %>
                            </td>
                            <td style="text-align: center;">
                                <asp:CheckBox runat="server" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("IsHomePage")) %>' />
                            </td>
                            <td style="text-align: center;">
                                <asp:CheckBox runat="server" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("IsActive")) %>' />
                            </td>
                            <td class="text-center" data-id='<%# Eval("Id") %>'>
                                <a href="?id=<%# Eval("Id") %>">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            <tr>
                                <td colspan="4">
                                    <div class="btn-group">
                                        <button runat="server" id="Previous" clientidmode="Static" class="btn btn-prev" onserverclick="Previous_OnClick">
                                            <img src="/images/arrow-left.png" alt="" class="img-responsive" />
                                        </button>
                                        <button runat="server" id="Next" clientidmode="Static" class="btn btn-next" onserverclick="Next_OnClick">
                                            <img src="/images/arrow-right.png" alt="" class="img-responsive" />
                                        </button>
                                    </div>
                                    <b>
                                        <asp:Literal runat="server" ID="LitPage"></asp:Literal>
                                    </b>
                                </td>
                                <td class="text-right">
                                    <span class="result-hotel">
                                        <asp:Literal runat="server" ID="LitTotal"></asp:Literal>
                                    </span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:View>
            <asp:View runat="server">
                <div class="row">
                    <div class="col-md-12 margin-t-25">
                        <asp:Label ID="LblMessage" ForeColor="Red" runat="server" Text="" Visible="False"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Page Name</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="PageNameText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Page Url</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="UrlSegmentText" ClientIDMode="Static" Enabled="False" Text="" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <label>Client Url</label>
                                <br/>
                                <asp:Label runat="server" ID="UrlLabel" ClientIDMode="Static" ForeColor="Green"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="checkbox">
                                    <label><asp:CheckBox runat="server" ID="IsHomePageCheckBox"/>Is Home Page</label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="checkbox">
                                    <label><asp:CheckBox runat="server" ID="IsActiveCheckBox"/>Is Active</label>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="form-group">
                                    <label>Page Title</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="PageTitleText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="form-group">
                                    <label>Meta Description</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="DescriptionText" TextMode="MultiLine" Rows="5" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="form-group">
                                    <label>Meta Keyword</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="KeywordText" TextMode="MultiLine" Rows="5" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="form-group">
                                    <label>Body Class</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="BodyClassText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <label>Landing Image Desktop</label>
                                <asp:Image runat="server" CssClass="img-responsive image-content-page" ID="LandingImageDesktop"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="form-group">
                                    <asp:FileUpload runat="server" ID="ImageLandingDesktopFileUpload" ClientIDMode="Static" uploadUrl="/Handler/Upload.ashx" accept="image/*"/>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="UploadImageLandingDesktop" runat="server" Text="Upload" CausesValidation="False" CssClass="btn btn-info" OnClick="UploadImageLandingDesktopClick" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <label>Landing Image Mobile</label>
                                <asp:Image runat="server"  CssClass="img-responsive image-content-page" ID="LandingImageMobile"/>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-8">
                                <div class="form-group">
                                    <asp:FileUpload runat="server" ID="ImageLandingMobileFileUpload" ClientIDMode="Static" uploadUrl="/Handler/Upload.ashx" accept="image/*"/>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="UploadImageLandingMobile" runat="server" Text="Upload" CausesValidation="False" CssClass="btn btn-info" OnClick="UploadImageLandingMobileClick" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="form-group">
                                    <label>Script Analytics</label>
                                    <CKEditor:CKEditorControl ID="ScriptAnalyticsEditor" CssClass="form-control" AutoParagraph="False" TextMode="MultiLine" runat="server"></CKEditor:CKEditorControl>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-10">
                                <div class="form-group">
                                    <label>Html Content</label>
                                    <CKEditor:CKEditorControl ID="ContentHtmlEditor" CssClass="form-control" AutoParagraph="False" TextMode="MultiLine" runat="server"></CKEditor:CKEditorControl>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row margin-t-50">
                    <div class="col-md-6">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" Text="Cancel" OnClick="CancelClick"/>
                    </div>
                    <div class="col-md-6 text-right">
                        <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" Text="Save" OnClick="SaveHtmlContentsClick"/>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script>
        window.baseUrl = '<%=AppConfiguration.DayaxeClientUrl + "/" %>';
    </script>
    <script src="/Scripts/fileinput.js"></script>
    <script src="/Scripts/contentsPage.js"></script>
</asp:Content>