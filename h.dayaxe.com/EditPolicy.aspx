<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="EditPolicy.aspx.cs" Inherits="h.dayaxe.com.EditPolicy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <div class="policies margin-t-50">
        <div class="row">
            <div class="col-md-12">
                <label>Policy Name:</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 policy-text">
                <asp:TextBox ID="PolicyNameText" CssClass="form-control" runat="server"></asp:TextBox>
                <asp:HiddenField runat="server" ID="PolicyId" Value="0"/>
            </div>
            <div class="col-md-4">
                <asp:Button ID="AddPolicyButton" CssClass="btn" OnClick="AddPolicyButton_OnClick" runat="server" Text="Add Policy"/>
            </div>
        </div>
        <div class="row">
            <div class="col-md-8">
                <div class="row margin-t-15">
                    <div class="col-md-8 amenties-list">
                        <asp:Repeater runat="server" ID="RptPoliciesListing" OnItemDataBound="RptPoliciesListing_OnItemDataBound">
                            <HeaderTemplate>
                                <ul class="row">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <span><%# Eval("Name") %></span>
                                    <span class="text-right">
                                        <asp:CheckBox runat="server" ID="ChkRemove"/>
                                        <asp:HiddenField runat="server" ID="HidId" Value='<%# Eval("Id") %>'/>
                                        <a href="javascript:void(0);" data-name='<%# Eval("Name") %>' data-id='<%# Eval("Id") %>' class="edit-policy">
                                            <span class="glyphicon glyphicon-edit"></span>
                                        </a>
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="RemoveButton" Text="Delete" OnClick="RemoveButton_OnClick"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/editPolicy.js"></script>
</asp:Content>

