<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="Calendar.aspx.cs" Inherits="h.dayaxe.com.Calendar" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>
<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ID="Content3" ContentPlaceHolderID="CssContentPlaceHolder" Runat="Server">
    <webopt:bundlereference runat="server" path="~/Content/calendar" />
    <style>
        .margin-top-25 {
            margin-top: 25px;
        }
        .margin-top-5 {
            margin-top: 5px;
        }
        .border-sol {
            border-right: solid 0.5px #D4D3D5;
            padding-bottom: 30px;
            padding-top: 10px;
        }
        .right-content {
            padding: 0;
        }
        .hr {
            margin-top: 10px;
            margin-bottom: 15px;
        }
        #page-wrapper {
            padding: 0;
            padding-top: 113px;
        }
        #page-wrapper .container-fluid {
            margin: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>
    <div class="calendar">
        <div class="row">
            <div class="col-md-9">
                <asp:UpdatePanel ID="CalendarUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label runat="server" ID="MessageLabel" ForeColor="Red" Visible="False"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID ="saveCalendar" EventName ="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
        <div class="row">
            <div class="col-md-8 border-sol">
                <div class="row margin-top-5">
                    <div class="col-md-12">
                        Select product to reflect inventory: 
                        <asp:Repeater ID="ProductRpt" runat="server" OnItemDataBound="ProductRpt_OnItemDataBound">
                            <HeaderTemplate>
                                <ul class="product-list-calendar">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <a href="#" onclick="javascript:bindCalendar(this, <%#Eval("ProductId") %>);" class="btn">
                                        <%# Eval("ProductName") %>
                                    </a>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 col-calendar">
                            <div id="datepicker"></div>
                    </div>
                </div>
                <div class="row instruction">
                    <div class="col-md-2">
                        <div class="blocked-carlendar"></div>
                        <label>Blocked</label>
                    </div>
                    <div class="col-md-2">
                        <div class="open-carlendar"></div>
                        <label>Open</label>
                    </div>
                    <div class="col-md-3 text-center redeemed-passes">
                        <div class="highlight-span">&nbsp;</div>
                        <label>Ticket allocation</label>
                    </div>
                    <div class="col-md-2 text-center redeemed-passes">
                        <div class="highlight-span price">&nbsp;</div>Bookings
                    </div>
                    <div class="col-md-2 text-center redeemed-passes">
                        <div class="highlight-span price redeem">&nbsp;</div>Redemptions
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="row manage-blackout">
                    <div class="col-md-12">
                        <div class="calendar-right">
                            <h4>Manage Inventory
                            </h4>
                            <p>
                                To start, select days you need to adjust. You can select several days or you can select a single day. Note, you can only work in one month at a time.
                            </p>
                            <p>
                                <b>Blocking Days</b>: change quantity of ticket for a specific product to 0.
                            </p>
                        </div>
                    </div>
                </div>
                <div id="selectedDate" class="hidden">
                    <form role="form">
                        <div class="row">
                            <div class="col-md-12">
                                <label class="title">
                                    Selected Dates
                                </label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="input-group input-daterange">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="startDate">Start Date</label>
                                        <input type="text" class="form-control actual_range" id="startDate" runat="server" ClientIDMode="Static"/>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="endDate">End Date</label>
                                        <input type="text" class="form-control actual_range" id="endDate" runat="server" ClientIDMode="Static"/>
                                    </div>
                                </div>
                                <div class="col-md-12 margin-t-25">
                                    <asp:Repeater ID="ProductPriceRpt" runat="server" OnItemDataBound="ProductPriceRpt_OnItemDataBound">
                                        <HeaderTemplate>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <label class="title">
                                                        Products
                                                    </label>
                                                </div>
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <div class="row custom-price">
                                                <div class="col-md-5">
                                                    <label class="product-type">
                                                        <%# Eval("ProductTypeString") %>
                                                    </label><br/>
                                                    <label class="product-name">
                                                        <%# Eval("ProductName") %>
                                                        <asp:HiddenField ID="HidProductId" runat="server" />
                                                    </label>
                                                </div>
                                                <div class="col-md-7">
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <label class="small">Price:</label>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <label class="small">Tickets:</label>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="RegularPriceText" CssClass="form-control regular-price"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <asp:DropDownList runat="server" ID="CapacityDropdownList" CssClass="form-control product-item"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <hr class="hr"/>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">
                                <button class="btn btn-cancel" onclick="javascript:hideSelectDate(event);">Cancel</button>
                            </div>
                            <div class="col-md-6 right">
                                <div class="saving hidden">Saving...</div>
                                <asp:Button runat="server" ID="saveCalendar" OnClick="saveCalendar_OnClick" OnClientClick="$('.saving').removeClass('hidden');" Text="Save" CssClass="btn btn-save float-right" ClientIDMode="Static"/>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script>
        window.selectedMonth = <%=SelectedMonth%>;
        window.selectedYear = <%=SelectedYear%>;
        window.hotelId = <%=PublicHotel.HotelId%>;
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ScriptPlaceHolder" Runat="Server">
    <%: Scripts.Render("~/bundles/calendar") %>
    <script type="text/javascript">
        function bindCalendar(sender, productId) {
            console.log('calendar binned');
            $('.product-list-calendar a').removeClass('active');
            $(sender).addClass('active');
            PageMethods.GetCalendarOfProduct(productId, function (response, userContext, methodName) {
                var obj = JSON.parse(response);
                $(function () {
                    reInitCalendar(obj.DateDisabled,
                        obj.CustomPrice,
                        obj.DefaultPrice,
                        obj.BookData,
                        obj.RedemptionData);
                });
            });
        }

        function getPrice(date) {
            PageMethods.GetPriceOfHotelByDate(date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate(), window.hotelId, function (response, userContext, methodName) {
                var obj = JSON.parse(response);

                for (var l = 0; l < obj.length; l++) {
                    $('.regular-price-' + obj[l].ProductId).val(parseFloat(obj[l].Price, 10).toFixed(2));
                    $('.product-' + obj[l].ProductId).val(obj[l].Quantity);
                }
            });
        }
        // jquery on load to force loading items
        $(document).ready(function () {
            var cle = document.createEvent("MouseEvent");
            cle.initEvent("click", true, true);
            var elem = document
                .getElementsByClassName('product-list-calendar')[0]
                .getElementsByTagName('li')[0]
                .getElementsByTagName('a')[0];
            elem.dispatchEvent(cle);
            var productItem = document
                .getElementsByClassName('product-list-calendar')[0]
                .getElementsByTagName('li')[0]
                .getElementsByTagName('a')[0];

            if (typeof productItem !== "undefined") {
                productItem.click();
            }
        })
    </script>
</asp:Content>