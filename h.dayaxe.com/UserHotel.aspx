<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="UserHotel.aspx.cs" Inherits="h.dayaxe.com.UserHotel" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <div class="user-hotel">
        <div class="row">
            <div class="col-md-push-6 col-md-6 text-right">
                <div class="today">
                    <div class="month"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                    <div class="date"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                    <div class="day"><%=(DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
                </div>
            </div>
        </div>
        <asp:MultiView runat="server" ID="MVUserHotel" ActiveViewIndex="0">
            <asp:View runat="server" ID="UserHotelListView">
                <div class="row">
                    <div class="col-md-12 text-right margin-t-15">
                        <a href="UserHotel.aspx?userId=0" class="btn btn-cancel">Add New</a>
                    </div>
                </div>
                <asp:Repeater runat="server" ID="RptUserHotel" OnItemDataBound="RptUserHotel_OnItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                            <thead>
                                <th>Name
                                </th>
                                <th>Email
                                </th>
                                <th style="width: 50px;">Active
                                </th>
                                <th style="width: 50px;">Actions
                                </th>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr id="rowHotel" runat="server">
                            <td>
                                <%#Eval("Fullname") %>
                            </td>
                            <td>
                                <a href="?userId=<%#Eval("CustomerId") %>"><%# Eval("EmailAddress") %></a>
                            </td>
                            <td class="text-center">
                                <asp:CheckBox runat="server" ID="ChkActive" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("IsActive")) %>' />
                            </td>
                            <td class="text-center" data-id='<%# Eval("CustomerId") %>'>
                                <a href="?userId=<%# Eval("CustomerId") %>" class="edit-user">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                                <a href="#" class="delete-user">
                                    <span class="glyphicon glyphicon-trash"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                             <td colspan="3">
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
            <asp:View runat="server" ID="UserHotelManage">
                <div class="row">
                    <div class="col-md-12 margin-t-25">
                        <asp:Label ID="LblMessage" ForeColor="Red" runat="server" Text="" Visible="False"></asp:Label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-8">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>First Name</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="TxtFirstName" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Last Name</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="TxtLastName" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Email</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="TxtUsername" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Role</label>
                                    <asp:DropDownList ID="DdlRole" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="2" Text="Manager" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Checkin Only"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="hotel-manage">
                            <div class="form-group">
                                <label>
                                    Hotels Managed:
                                </label>
                                <div class="row">
                                    <div class="col-md-8">
                                        <asp:DropDownList ID="DdlHotels" CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Button ID="BtnAddUserHotel" CssClass="btn" OnClick="BtnAddUserHotel_OnClick" runat="server" Text="Assign Hotel"/>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 amenties-list">
                                    <asp:Repeater runat="server" ID="RptHotelListings" OnItemDataBound="RptHotelListings_ItemDataBound">
                                        <HeaderTemplate>
                                            <ul class="row">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li runat="server" id="liAlternatie" class="amenity-item">
                                                <span><%# Eval("HotelInfo") %></span>
                                                <span class="text-right">
                                                    <asp:CheckBox runat="server" ID="ChkRemove" />
                                                    <asp:HiddenField runat="server" ID="HidHotelId" Value='<%# Eval("HotelId") %>' />
                                                    <asp:HiddenField runat="server" ID="HidUserId" Value='<%# Eval("CustomerId") %>' />
                                                </span>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                                <li class="footer text-right">
                                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="UnAssignHotel" Text="Delete" OnClick="UnAssignHotelClick"></asp:Button>
                                                </li>
                                            </ul>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row margin-t-50">
                    <div class="col-md-2">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" Text="Cancel" OnClick="CancelClick"/>
                    </div>
                    <div class="col-md-2 text-center">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteButton" Text="Delete" OnClick="DeleteClick"/>
                    </div>
                    <div class="col-md-2 text-center">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="Deactivebutton" Text="De-Active" OnClick="DeactiveClick"/>
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="ActiveButton" Text="Active" OnClick="ActiveClick"/>
                    </div>
                    <div class="col-md-3 text-right">
                        <asp:Button runat="server" CssClass="btn btn-save" ID="btnSendActivationLink" Text="Send Activation Link" OnClick="btnSendActivationLink_Click" Visible="false"/>
                    </div>
                    <div class="col-md-3 text-right">
                        <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" Text="Save" OnClick="SaveUserClick"/>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/user.js"></script>
</asp:Content>