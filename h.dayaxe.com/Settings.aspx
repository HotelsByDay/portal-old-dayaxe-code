<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Settings.aspx.cs" Inherits="h.dayaxe.com.Settings" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="Content">
    <div class="settings">
        <div class="row padding-t-100">
            <div class="col-md-12">
                <label>Daily Capacity</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-2">
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <span>
                            <img src="Images/iconCapacity.png" alt="" /></span> <span class="daily-current-value">2</span> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" runat="server" ID="DailyCapacity" ClientIDMode="Static"></ul>
                </div>
            </div>
            <div class="col-md-5">
                <span>Select maximum number of passes you are willing to sell per day. This limits the maximum number of people hotel is willing to receive on a given day.
                </span>
            </div>
        </div>
        <div class="row padding-t-60">
            <div class="col-md-12">
                <label>Distance</label>
            </div>
        </div>
        <div class="row">
            <div class="col-md-2">
                <div class="btn-group">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        <img src="Images/iconDistance.png" alt="" />
                        <span class="distance-current-value">50 mi</span> <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu" runat="server" ID="Distance" ClientIDMode="Static"></ul>
                </div>
            </div>
            <div class="col-md-5">
                Select maximum radius of your ideal customer's residence. Hotel zip code determines the starting point. If a customer is in the radius your listing will be shown to them.
            </div>
        </div>
        <div class="row padding-t-60">
            <div class="col-md-2">
                <button class="btn btn-cancel">Cancel</button>
            </div>
            <div class="col-md-5 text-right">
                <div class="btn btn-save">Save</div>
            </div>
        </div>
    </div>
</asp:Content>
