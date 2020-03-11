<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="LogList.aspx.cs" Inherits="h.dayaxe.com.LogListPage" %>
<%@ Import Namespace="DayaxeDal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
    <asp:Repeater runat="server" ID="LogRepeater" OnItemDataBound="LogRepeater_OnItemDataBound">
        <HeaderTemplate>
            <table class="table table-booking">
                <thead>
                    <th>Key</th>
                    <th>Content</th>
                    <th>Updated Date</th>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id="rowHotel" runat="server">
                <td>
                    <%#Eval("LogKey") %>
                </td>
                <td>
                    <%# HttpUtility.HtmlEncode(Eval("UpdatedContent")) %>
                </td>
                <td>
                    <%# Convert.ToDateTime(Eval("UpdatedDate")).ToString(Constant.DiscountDateTimeFormat) %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td colspan="2">
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
</asp:Content>

