<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EditHotel.aspx.cs" Inherits="h.dayaxe.com.EditHotel" %>
<%@ Import Namespace="DayaxeDal.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
<div class="hotel-listings">
    <div class="row">
        <div class="col-md-push-6 col-md-6 text-right">
            <div class="today">
                <div class="month"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("MMM").ToUpper()) %></div>
                <div class="date"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("dd")) %></div>
                <div class="day"><%= (DateTime.UtcNow.ToLosAngerlesTime().ToString("dddd")) %></div>
            </div>
        </div>
    </div>
    <asp:HiddenField runat="server" ID="HotelTypeId" Value="0" ClientIDMode="Static"/>
    <div class="row">
        <div class="col-md-8">
            <asp:Label runat="server" ID="ErrorMessageLabel" ForeColor="Red" Visible="False"></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-8">
            <div class="row">
                <div class="col-md-8">
                    <label>Hotel Name</label>
                </div>
                <div class="col-md-4 text-right">
                    <span id="cntrHotelName"></span> characters left
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox runat="server" ID="HotelName" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Hotel name is required" ControlToValidate="HotelName"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-6">
                    <label>Neighborhood</label>
                </div>
                <div class="col-md-6">
                    <label>Trip Advisor Rating</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="Neighborhood" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <asp:TextBox runat="server" TextMode="SingleLine" ID="Rating" Text="4" ClientIDMode="Static" CssClass="rating rating-loading" title=""></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Neighborhood is required" ControlToValidate="Neighborhood"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Hotel Type</label>
                </div>
            </div>
            <div class="row show-grid hotel-type-grid">
                <div class="col-md-3 pointer col-single" value="0" id="HotelType_Festive" runat="server">Festive</div>
                <div class="col-md-3 pointer col-single" value="1" id="HotelType_Tranquil" runat="server">Tranquil</div>
                <div class="col-md-3 pointer col-single" value="2" id="HotelType_Family" runat="server">Family</div>
                <div class="col-md-3 pointer col-single" value="3" id="HotelType_Basic" runat="server">Basic</div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-8">
                    <label>Market &amp; Location</label>
                </div>
                <div class="col-md-4">
                    <label>Published</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:DropDownList runat="server" ID="MarketDropdownList" CssClass="form-control"/>
                </div>
                <div class="col-md-4">
                     <asp:HiddenField runat="server" ID="IsPublishedHidden" ClientIDMode="Static" />
                    <div id="isPublished" class="toggle toggle-iphone"></div>
                </div>
            </div>
            <div class="row margin-t-25" runat="server">
                <div class="col-md-8">
                    <label>Check-in at</label>
                </div>
                <div class="col-md-4">
                    <label>Coming Soon</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="CheckInPlaceText" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-4">
                     <asp:HiddenField runat="server" ID="IsCommingSoonHidden" ClientIDMode="Static" />
                    <div id="isCommingSoon" class="toggle toggle-iphone"></div>
                </div>
            </div>
        </div>
        <div class="col-md-4 hotel-area hotel-discount">
            <div class="row">
                <div class="col-md-12">
                    <h3>
                        Hotel Discount Setup
                    </h3>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <label>Discount</label>
                </div>
                <div class="col-md-6">
                    <label>Discount Code</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="input-group">
                        <span class="input-group-addon">%</span>
                        <asp:TextBox runat="server" ID="DiscountPercent" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="DiscountCode" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:RegularExpressionValidator ID="RegularExpressionDiscountCode" ForeColor="Red" runat="server" CssClass="error-message" ErrorMessage="Discount should be alphanumeric characters" ValidationExpression="^[a-zA-Z\d]+$" ControlToValidate="DiscountCode"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <label>Discount Disclaimer</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox runat="server" ID="DiscountDisclaimer" TextMode="MultiLine" Rows="7" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <h3>
                        Hotel Parking Setup
                    </h3>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox runat="server" ID="ParkingText" TextMode="MultiLine" Rows="7" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <hr/>
        </div>
        <div class="col-md-8">
            <div class="row">
                <div class="col-md-12">
                    <label>Street Address</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox runat="server" ID="StreetAddress" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Address is required" ControlToValidate="StreetAddress"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-6">
                    <label>City</label>
                </div>
                <div class="col-md-6">
                    <label>State</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="City" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="State" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 state-error">
                    <asp:RequiredFieldValidator CssClass="error-message" ID="RequiredFieldValidator5" ForeColor="Red" runat="server" ErrorMessage="City is required" ControlToValidate="City"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="State is required" ControlToValidate="State"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ForeColor="Red" ControlToValidate="State" ValidationExpression="^[A-Z]{2}$" CssClass="error-message" ErrorMessage="State should be 2 letter abbreviation code"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-6">
                    <label>Zipcode</label>
                </div>
                <div class="col-md-6">
                    <label>Phone Number</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="ZipCode" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="PhoneNumber" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 state-error">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ForeColor="Red" CssClass="error-message" runat="server" ErrorMessage="Phone number is required" ControlToValidate="PhoneNumber"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" CssClass="error-message" ForeColor="Red" runat="server" ErrorMessage="Zip code is required" ControlToValidate="ZipCode"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ForeColor="Red" CssClass="error-message" runat="server" ValidationExpression="^[0-9]{5}$" ControlToValidate="ZipCode" ErrorMessage="Zip code should be 5 digit"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-6">
                    <label>Latitude</label>
                </div>
                <div class="col-md-6">
                    <label>Longitude</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="Latitude" CssClass="form-control disabled" Enabled="False"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="Longitude" CssClass="form-control disabled" Enabled="False"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <hr/>
        </div>
        <div class="col-md-8">
            <div class="row">
                <div class="col-md-12">
                    <label>General Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox runat="server" ID="GeneralHours" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="col-md-4 hotel-area hotel-terms">
            <div class="row">
                <div class="col-md-12">
                    <h3>
                        Hotel Terms & Conditions
                    </h3>
                </div>
            </div>            
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox runat="server" ID="TermsAndConditions" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>

    </div>
    <div class="row">
        <div class="col-md-12">
            <hr/>
        </div>
        <div class="col-md-8">
            <div class="row">
                <div class="col-md-8">
                    <label>Recommendation</label>
                </div>
                <div class="col-md-4 text-right">
                    <span id="cntrRecommendation"></span> words left
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <asp:TextBox TextMode="MultiLine" Rows="15" runat="server" ID="Recommendation" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="col-md-4 hotel-area hotel-terms">
            <div class="row">
                <div class="col-md-12">
                    <h3>
                        Select applicable policies:
                    </h3>
                </div>
            </div>     
            <div class="row">
                <div class="col-md-8">
                    <asp:DropDownList runat="server" ID="DdlPolicies" CssClass="form-control ddl-policies"/> 
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddPoliciesButton" CausesValidation="False" runat="server" Text="Add" OnClick="AddPoliciesButton_OnClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptPolicies">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server">
                                        <%# Eval("Name") %>
                                    </asp:Label>
                                    <asp:HiddenField runat="server" ID="Order" Value='<%#Eval("Order") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPolicies" />
                                        <asp:HiddenField runat="server" ID="HidId" Value='<%# Eval("PolicyId") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                    <li class="footer text-right">
                                        <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeletePolicesButton" CausesValidation="False" Text="Delete" OnClick="DeletePolicesButton_OnClick"></asp:Button>
                                    </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
        </div>
    </div>
        <div class="row">
            <div class="col-md-12">
                <hr/>
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <label>TripAdvisor Script</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-8">
                <asp:TextBox TextMode="MultiLine" Rows="15" runat="server" ID="TripAdvisorScriptTextBox" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
    <div class="row">
    <div class="col-md-12">
        <hr/>
    </div>
    <div class="col-md-12 amenties">
    <div class="row">
        <div class="col-md-12">
            <label>Amenties</label>
        </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <label>Select Amenty</label>
        </div>
    </div>
    <div class="row">
    <div class="col-md-12">
    <ul class="nav nav-tabs">
        <li class="active">
            <a data-toggle="tab" href="#pool">
                <img src="images/pool.png" class="pool"/><span>pool</span></a>
        </li>
        <li>
            <a data-toggle="tab" class="gym" href="#gym">
                <img src="images/gym.png" class="img-responsive"/><span>gym</span></a>
        </li>
        <li>
            <a data-toggle="tab" href="#spa" class="spa">
                <img src="images/spa.png" class="img-responsive"/><span>spa</span></a>
        </li>
        <li>
            <a data-toggle="tab" href="#businesscenter" class="business-center">
                <img src="images/business-center.png" class="img-responsive"/><span>business<br />
                                    services</span></a>
        </li>
        <li>
            <a data-toggle="tab" href="#dining" class="dining">
                <img src="images/icon_dinning.png" class="img-responsive"/><span>dining</span></a>
        </li>
        <li>
            <a data-toggle="tab" href="#event" class="event">
                <img src="images/icon_event.png" class="img-responsive"/><span>events</span></a>
        </li>
        <li>
            <a data-toggle="tab" href="#other" class="other"><span>other</span></a>
        </li>
    </ul>

    <div class="tab-content col-md-12">
        <div id="pool" class="tab-pane fade in active">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="PoolActive" Checked="True"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Pool Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="PoolHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" CausesValidation="False" ID="PoolSaveHours" runat="server" Text="Save Hours" OnClick="PoolSaveHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Pool Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="PoolAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="PoolAddAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptPoolAmenities">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPoolAmenities" />
                                        <asp:HiddenField runat="server" ID="HidPoolAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeletePoolAmenties" CausesValidation="False" Text="Delete" OnClick="DeletePoolAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Pool Upgrades</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="PoolUpgrades" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" runat="server" CausesValidation="False" Text="Add Upgrades" OnClick="PoolUpgradesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptPoolUpgrades">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPoolUpgrades" />
                                        <asp:HiddenField runat="server" ID="HidPoolUpgrades" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeletePoolUpgrades" CausesValidation="False" Text="Delete" OnClick="DeletePoolUpgradesList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div id="gym" class="tab-pane fade">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="GymActive" Checked="True"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Gym Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="GymHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="SaveGymHours" CausesValidation="False" runat="server" Text="Save Hours" OnClick="GymSaveHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Gym Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="GymAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddGymAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="GymAddAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptGymAmenties">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPoolAmenities" />
                                        <asp:HiddenField runat="server" ID="HidPoolAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteGymAmenties" CausesValidation="False" Text="Delete" OnClick="DeleteGymAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Gym Upgrades</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="GymUpgrades" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" runat="server" Text="Add Upgrades" CausesValidation="False" OnClick="GymUpgradesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptGymUpgrades">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkGymUpgrades" />
                                        <asp:HiddenField runat="server" ID="HidGymUpgrades" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteGymUpgrades" CausesValidation="False" Text="Delete" OnClick="DeleteGymUpgradesList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div id="spa" class="tab-pane fade">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="SpaActive" Checked="True"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Spa Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="SpaHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="SaveSpaHours" CausesValidation="False" runat="server" Text="Save Hours" OnClick="SpaSaveHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Spa Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="SpaAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddSpaAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="SpaAddAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptSpaAmenties">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPoolAmenities" />
                                        <asp:HiddenField runat="server" ID="HidPoolAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteSpaAmenties" CausesValidation="False" Text="Delete" OnClick="DeleteSpaAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Spa Upgrades</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="SpaUpgrades" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" CausesValidation="False" runat="server" Text="Add Upgrades" OnClick="SpaUpgradesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptSpaUpgrades">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkSpaUpgrades" />
                                        <asp:HiddenField runat="server" ID="HidSpaUpgrades" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteSpaUpgrades" CausesValidation="False" Text="Delete" OnClick="DeleteSpaUpgradesList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div id="businesscenter" class="tab-pane fade">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="BusinessCenterActive" Checked="True"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Business Services Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="BusinessCenterHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="SaveBusinessCenterHours" CausesValidation="False" runat="server" Text="Save Hours" OnClick="BusinessCenterSaveHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Business services Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="BusinessCenterAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddBusinessCenterAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="BusinessCenterAddAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptBusinessCenterAmenties">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPoolAmenities" />
                                        <asp:HiddenField runat="server" ID="HidPoolAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteBusinessCenterAmenties" CausesValidation="False" Text="Delete" OnClick="DeleteBusinessCenterAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Business services Upgrades</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="BusinessCenterUpgrades" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" runat="server" Text="Add Upgrades" CausesValidation="False" OnClick="BusinessCenterUpgradesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptBusinessCenterUpgrades">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkBusinessCenterUpgrades" />
                                        <asp:HiddenField runat="server" ID="HidBusinessCenterUpgrades" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteBusinessCenterUpgrades" CausesValidation="False" Text="Delete" OnClick="DeleteBusinessCenterUpgradesList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div id="dining" class="tab-pane fade">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="DiningActive" Checked="False"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Dining Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="DiningHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="SaveDiningHours" CausesValidation="False" runat="server" Text="Save Hours" OnClick="SaveDiningHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Dining Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="DiningAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddDiningAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="AddDiningAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptDiningAmenties">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkDiningAmenities" />
                                        <asp:HiddenField runat="server" ID="HidDiningAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteDiningAmenties" CausesValidation="False" Text="Delete" OnClick="DeleteDiningAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div id="event" class="tab-pane fade">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="EventActive" Checked="False"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Events Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="EventHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="SaveEventHours" CausesValidation="False" runat="server" Text="Save Hours" OnClick="SaveEventHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Dining Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="EventAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddEventAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="AddEventAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptEventAmenties">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkEventAmenities" />
                                        <asp:HiddenField runat="server" ID="HidEventAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteEventAmenties" CausesValidation="False" Text="Delete" OnClick="DeleteEventAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
        <div id="other" class="tab-pane fade">
            <div class="row">
                <div class="col-md-12">
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox runat="server" ID="OtherActive" Checked="True"/>
                            Active
                        </label>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Other Hours</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="OtherHours" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="SaveOtherHours" CausesValidation="False" runat="server" Text="Save Hours" OnClick="OtherSaveHoursClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Other Amenities List</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="OtherAmenitiesList" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" ID="AddOtherAmenties" CausesValidation="False" runat="server" Text="Add Amenity" OnClick="OtherAddAmentiesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptOtherAmenties">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkPoolAmenities" />
                                        <asp:HiddenField runat="server" ID="HidPoolAmenties" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteOtherAmenties" CausesValidation="False" Text="Delete" OnClick="DeleteOtherAmentyList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
            <div class="row margin-t-25">
                <div class="col-md-12">
                    <label>Other Upgrades</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-8">
                    <asp:TextBox runat="server" ID="OtherUpgrades" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-md-2">
                    <asp:Button CssClass="btn btn-cancel" runat="server" Text="Add Upgrades" CausesValidation="False" OnClick="OtherUpgradesClick"></asp:Button>
                </div>
            </div>
            <div class="row margin-t-15">
                <div class="col-md-12">
                    <div class="amenties-list">
                        <asp:Repeater runat="server" ID="RptOtherUpgrades">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li runat="server" id="liAlternatie" class="amenity-item">
                                    <asp:Label runat="server" ID="AmentiesName"><%# Eval("Name") %></asp:Label>
                                    <asp:HiddenField runat="server" ID="AmentyOrder" Value='<%#Eval("AmentyOrder") %>'/>
                                    <span class="text-right">
                                        <span class="glyphicon glyphicon-move" aria-hidden="true"></span>
                                        <asp:CheckBox runat="server" ID="ChkOtherUpgrades" />
                                        <asp:HiddenField runat="server" ID="HidOtherUpgrades" Value='<%# Eval("Id") %>' />
                                    </span>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                <li class="footer text-right">
                                    <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteOtherUpgrades" CausesValidation="False" Text="Delete" OnClick="DeleteOtherUpgradesList"></asp:Button>
                                </li>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </div>
    </div>
    </div>
    </div>
    <div class="row">
        <div class="col-md-12">
            <hr/>
        </div>
        <div class="col-md-10">
            <div class="row">
                <div class="col-md-12">
                    <label>Photos</label>
                </div>
            </div>
            <div class="row">
                <div class="col-md-3">
                    <div class="btn-group select-photo-type">
                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <span class="photo-current-value">Cover</span> <span class="caret"></span>
                        </button>
                        <asp:HiddenField ID="HidphotoType" runat="server" ClientIDMode="Static"/>
                        <ul class="dropdown-menu" id="PhotoType">
                            <li><a href="#" data-value="5">Cover</a></li>
                            <li><a href="#" data-value="0">Pool</a></li>
                            <li><a href="#" data-value="1">Gym</a></li>
                            <li><a href="#" data-value="2">Spa</a></li>
                            <li><a href="#" data-value="3">Business Services</a></li>
                            <li><a href="#" data-value="4">Other</a></li>
                        </ul>
                    </div>
                </div>
                <div class="col-md-6">
                    <asp:FileUpload runat="server" ID="HotelImage" ClientIDMode="Static" uploadUrl="/Handler/Upload.ashx" accept="image/*"/>
                </div>
                <div class="col-md-3">
                    <asp:Button ID="UploadImage" runat="server" Text="Upload" CausesValidation="False" CssClass="btn btn-info" OnClick="UploadImageClick" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-12 margin-t-25">
                    <div class="photo-list">
                        <asp:ListView ID="LVHotelImage" GroupItemCount="1" runat="server">
                            <LayoutTemplate>
                                <ul class="row">
                                    <div class="col-md-4" runat="server" id="groupPlaceholder"></div>
                                </ul>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <div><%#Eval("Title") %></div>
                                <asp:Image runat="server" ID="HotelImage" CssClass="img-responsive" ImageUrl='<%#Eval("Url") %>'/>
                                <asp:HiddenField runat="server" ID="Order" Value='<%#Eval("Order") %>'/>
                                <asp:HiddenField runat="server" ID="PhotoId" Value='<%#Eval("Id") %>'/>
                                <div class="file-footer-buttons" style="position: relative; top: -30px; right: 10px;">
                                    <button type="button" class="remove-image btn btn-xs btn-default" title="Remove file" data-value='<%#Eval("Id") %>'>
                                        <i class="glyphicon glyphicon-trash text-danger"></i>
                                    </button>
                                </div>
                                <div><%#Eval("ImagePath").ToString() %></div>
                            </ItemTemplate>
                            <GroupTemplate>
                                <li class="col-md-4 margin-t-25 photo-item">
                                    <div runat="server" id="itemPlaceholder"></div>
                                </li>
                            </GroupTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="row margin-t-50">
        <div class="col-md-3">
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="CancelButton" CausesValidation="False" Text="Cancel" OnClick="CancelClick"/>
        </div>
        <div class="col-md-3 text-center">
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="DeleteButton" CausesValidation="False" Text="Delete" OnClick="DeleteClick"/>
        </div>
        <div class="col-md-3 text-center">
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="Deactivebutton" CausesValidation="False" Text="De-Active" OnClick="DeactiveClick"/>
            <asp:Button runat="server" CssClass="btn btn-cancel" ID="ActiveButton" CausesValidation="False" Text="Active" OnClick="ActiveClick"/>
        </div>
        <div class="col-md-3 text-right">
            <asp:Button runat="server" CssClass="btn btn-save" ID="SaveButton" ClientIDMode="Static" Text="Save" OnClick="SaveHotelClick"/>
        </div>
    </div>
</div>
</div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="CssContentPlaceHolder">
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ScriptPlaceHolder">
    <script src="/Scripts/plugins/toggles/toggles.min.js"></script>
    <script src="/Scripts/star-rating.min.js"></script>
    <script src="/Scripts/fileinput.js"></script>
    <script src="/Scripts/edithotel.js" type="text/javascript"></script>
</asp:Content>