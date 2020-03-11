<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="TargetAudience.aspx.cs" Inherits="h.dayaxe.com.TargetAudience" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" Runat="Server">
    <asp:HiddenField ID="HidHotelId" runat="server" ClientIDMode="Static" />
    <div id="targetAudience">
        <div class="row margin-t-15">
            <div class="col-md-12 padding-l-0">
                <label>
                    Select specific customers who you want your passes sold to. We will target and drive those customers to your hotel listing.
                </label>
            </div>
        </div>
        <div class="row margin-t-100">
            <div class="col-md-6 padding-l-0">
                <label>Income</label>
            </div>
            <div class="col-md-6 padding-l-0">
                <label>Distance</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 padding-l-0">
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle income-target" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label CssClass="income-current-value" runat="server" ID="IncomCurrent">$ 150,000 +</asp:Label> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" runat="server" ID="DropdownIncome" ClientIDMode="Static"></ul>
                </div>
            </div>
            <div class="col-md-6">
                <div class="row">
                    <div class="col-md-4 padding-l-0">
                        <div class="btn-group">
                            <button type="button" class="btn btn-default dropdown-toggle distance-target" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <img src="Images/iconDistance.png" alt=""/>
                                <asp:Label CssClass="distance-current-value" runat="server" ID="DistanceCurrent">50 mi</asp:Label> <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu" runat="server" ID="Distance" ClientIDMode="Static"></ul>
                        </div>
                    </div>
                    <div class="col-md-8">
                        Select maximum radius of your ideal customer's residence. Hotel zip code determines the starting point. If a customer is in the radius your listing will be shown to them.
                    </div>
                </div>
            </div>
        </div>
        <div class="row margin-t-50">
            <div class="col-md-12 padding-l-0">
                <label>Target Groups</label>
            </div>
        </div>
        <div class="row show-grid target-group" id="TargetAudienceDiv" runat="server" ClientIDMode="Static"></div>
        <div class="row margin-t-70">
            <div class="col-md-6 padding-l-0">
                <label>Age</label>
            </div>
            <div class="col-md-6">
                <label>Gender</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-6 padding-l-0">
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle distance-target" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label CssClass="age-from-current-value" runat="server" ID="AgeFromCurrent">18</asp:Label> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" runat="server" ID="AgeFrom" ClientIDMode="Static"></ul>
                </div>
                to 
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle distance-target" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <asp:Label CssClass="age-to-current-value" runat="server" ID="AgeToCurrent">65</asp:Label> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" runat="server" ID="AgeTo" ClientIDMode="Static"></ul>
                </div>
            </div>
            <div class="col-md-6">
                <div class="row show-grid" id="GenderDiv" runat="server" ClientIDMode="Static">
                </div>
            </div>
        </div>
        <div class="row margin-t-70">
            <div class="col-md-12 padding-l-0">
                <label>Education</label>
            </div>
        </div>
        <div class="row show-grid target-group" id="EducationDiv" runat="server" ClientIDMode="Static">
        </div>
        <div class="row margin-t-70">
            <div class="col-md-12 padding-r-0 text-right">
                <div class="saving hidden" id="saving" runat="server" clientidmode="Static">Saving...</div>
                <asp:Button ID="Save" runat="server" CssClass="btn btn-save" ClientIDMode="Static" Text="Save" OnClick="Save_Click" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="ScriptPlaceHolder">
    <script src="/Scripts/targetAudience.js?t=new"></script>
    <script>
        window.userName = "<%= UserName%>";
    </script>
</asp:Content>