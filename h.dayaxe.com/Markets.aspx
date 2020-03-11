<%@ Page Title="" Language="C#" MasterPageFile="~/SiteNew.master" AutoEventWireup="true" CodeFile="Markets.aspx.cs" Inherits="h.dayaxe.com.MarketPage" %>
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
        <asp:MultiView runat="server" ID="MarketMultiView" ActiveViewIndex="0">
            <asp:View runat="server">
                <div class="row">
                    <div class="col-md-12 text-right margin-t-15">
                        <a href="?id=0" class="btn btn-cancel">Add Location</a>
                    </div>
                </div>
                <asp:Repeater runat="server" ID="MarketRepeater" OnItemDataBound="MarketRepeaterOnItemDataBound">
                    <HeaderTemplate>
                        <table class="table table-booking">
                            <thead>
                                <th>Location Name
                                </th>
                                <th>State
                                </th>
                                <th>Market
                                </th>
                                <th>Number Of Hotels
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
                                <a href="?id=<%#Eval("Id") %>"><%#Eval("LocationName") %></a>
                            </td>
                            <td>
                                <%# Eval("State") %>
                            </td>
                            <td>
                                <%# Eval("MarketCode") %>
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="HotelsOfMarketLiteral"></asp:Literal>
                            </td>
                            <td class="text-center">
                                <asp:CheckBox runat="server" ID="ChkActive" Enabled="False" Checked='<%# Convert.ToBoolean(Eval("IsActive")) %>' />
                            </td>
                            <td class="text-center" data-id='<%# Eval("Id") %>'>
                                <a href="?id=<%# Eval("Id") %>">
                                    <span class="glyphicon glyphicon-edit"></span>
                                </a>
                                <a href="#">
                                    <span class="glyphicon glyphicon-trash"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                             <td colspan="5">
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
                    <div class="col-md-8">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Location Name</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="LocationNameText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Market</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="MarketText" Enabled="False" Text="SoCal" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Permalink</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="PermalinkText" Enabled="False" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>State</label>
                                    <asp:DropDownList runat="server" ID="DdlState" CssClass="form-control">
                                        <asp:ListItem Text="CA" Value="CA"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Latitude</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="LatitudeText" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label>Longtitude</label>
                                    <asp:TextBox runat="server" EnableViewState="False" ID="LongtitudeText" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group checkbox">
                                    <label>
                                        <asp:CheckBox runat="server" ID="IsCollectTax"/>
                                        Collect Tax
                                    </label>
                                </div>
                            </div>
                            <div class="col-md-6">
                                
                            </div>
                        </div>
                        <div class="hotel-manage">
                            <div class="form-group">
                                <label>
                                    Hotels at location:
                                </label>
                                <div class="row">
                                    <div class="col-md-8">
                                        <asp:DropDownList ID="DdlHotels" CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:Button ID="AddHotelMarketButton" CssClass="btn" OnClick="AddHotelMarketClick" runat="server" Text="Add Hotel"/>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12 amenties-list">
                                    <asp:Repeater runat="server" ID="RptHotelListings" OnItemDataBound="RptHotelListingsItemDataBound">
                                        <HeaderTemplate>
                                            <ul class="row">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li runat="server" id="liAlternatie" class="amenity-item">
                                                <span><%# Eval("HotelInfo") %></span>
                                                <span class="text-right">
                                                    <asp:CheckBox runat="server" ID="ChkRemove" />
                                                    <asp:HiddenField runat="server" ID="HidHotelId" Value='<%# Eval("HotelId") %>' />
                                                </span>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                                <li class="footer text-right">
                                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="UnAssignHotel" Text="Delete" OnClick="RemoveHotelMarketClick"></asp:Button>
                                                </li>
                                            </ul>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-12">
                                <label>
                                    Image
                                </label>
                            </div>
                            <div class="col-md-6">
                                <asp:FileUpload runat="server" ID="MarketImage" ClientIDMode="Static" uploadUrl="/Handler/Upload.ashx" accept="image/*"/>
                            </div>
                            <div class="col-md-6">
                                <asp:Button ID="UploadImage" runat="server" Text="Upload" CausesValidation="False" CssClass="btn btn-info" OnClick="UploadImageClick" />
                            </div>
                        </div>
                        
                        <div class="row">
                            <div class="col-md-6">
                                <asp:Image ID="MarketImageControl" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row margin-t-50">
                    <div class="col-md-3">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" Text="Cancel" OnClick="CancelClick"/>
                    </div>
                    <div class="col-md-3 text-center">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteButton" Text="Delete" OnClick="DeleteClick"/>
                    </div>
                    <div class="col-md-3 text-center">
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="Deactivebutton" Text="De-Active" OnClick="DeactiveClick"/>
                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="ActiveButton" Text="Active" OnClick="ActiveClick"/>
                    </div>
                    <div class="col-md-3 text-right">
                        <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" Text="Save" OnClick="SaveHotelMarketClick"/>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <script src="/Scripts/fileinput.js"></script>
    <script src="/Scripts/market.js"></script>
</asp:Content>