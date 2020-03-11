using System;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

public static class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.Ignore("favicon.ico");

        //Resource
        routes.Ignore("WebResource.axd");
        routes.Ignore("ScriptResource.axd");

        // Old Page
        routes.MapPageRoute("Search", "s/{market}/search.aspx", "~/Maintain.aspx");
        routes.MapPageRoute("Confirm", "{market}/{bookingId}/confirm.aspx", "~/Maintain.aspx");
        routes.MapPageRoute("Hotel", "{market}/{city}/{hotelName}-day-pass", "~/Maintain.aspx");
        routes.MapPageRoute("HotelSurvey", "{market}/{city}/{hotelName}-day-pass/{id}", "~/Maintain.aspx");
        routes.MapPageRoute("ReviewProduct", "{market}/{city}/{hotelName}/reviews", "~/Maintain.aspx");
        routes.MapPageRoute("Book", "{market}/{hotelId}/book.aspx", "~/Maintain.aspx");

        // Content Page
        routes.MapPageRoute("Default", "", "~/Default.aspx");
        routes.MapPageRoute("Credits", "credits", "~/Credits.aspx");
        routes.MapPageRoute("DayAxeReviews", "reviews", "~/AllReviews.aspx");

        // Gift Card
        routes.MapPageRoute("GiftCard", "gift-card", "~/GiftCards/bookGiftCard.aspx");
        routes.MapPageRoute("GiftCardConfirm", "gift-card/confirm/{GiftCardBookingId}", "~/GiftCards/confirmGiftCard.aspx");

        // Membership
        routes.MapPageRoute("MembershipPage", "membership", "~/Subscriptions/membershipDetail.aspx");

        routes.MapPageRoute("DayAxeReviewsHotel", "reviews/{hotelId}", "~/AllReviews.aspx");
        routes.MapPageRoute("Content", "{urlSegment}", "~/Default.aspx");

        // Subscriptions Page
        routes.MapPageRoute("SubscriptionBooking", "subscription/{SubscriptionName}/{SubscriptionId}", "~/Subscriptions/bookSubscription.aspx");
        routes.MapPageRoute("SubscriptionConfirm", "subscription/{SubscriptionName}/confirm/{SubscriptionBookingId}", "~/Subscriptions/ConfirmSubscription.aspx");
        routes.MapPageRoute("SubscriptionConfirmUpgrade", "subscription/{SubscriptionName}/confirm/{SubscriptionBookingId}/{BookingId}", "~/Subscriptions/ConfirmSubscription.aspx");

        // Search Page Products
        routes.MapPageRoute("DefaultSearchPage", "s/day-passes", "~/SearchProduct.aspx");
        routes.MapPageRoute("SearchDayPass", "s/{market}/day-passes", "~/SearchProduct.aspx");
        routes.MapPageRoute("SearchCabanas", "s/{market}/cabanas", "~/SearchProduct.aspx");
        routes.MapPageRoute("SearchDaybed", "s/{market}/daybeds", "~/SearchProduct.aspx");
        routes.MapPageRoute("SearchSpa", "s/{market}/spa-passes", "~/SearchProduct.aspx");

        // Book Page Products
        routes.MapPageRoute("BookProduct", "{market}/{city}/{hotelName}/{productName}-book", "~/bookProduct.aspx");
        routes.MapPageRoute("BookProductWithProductId", "{market}/{city}/{hotelName}/{productName}-book/{productId}", "~/bookProduct.aspx");

        // Confirm Page Products
        routes.MapPageRoute("ConfirmProduct", "{market}/{city}/{hotelName}/{productName}-confirm-{bookingId}", "~/confirm.aspx");

        // Reviews Page Products
        routes.MapPageRoute("ProductsReviews", "{market}/{city}/{hotelName}/{productName}/reviews", "~/reviews.aspx");
        
        // Products Page
        routes.MapPageRouteWithName("DayPass", "{market}/{city}/{hotelName}/{productName}", "~/HotelProduct.aspx");
        routes.MapPageRouteWithName("DayPassSurvey", "{market}/{city}/{hotelName}/{productName}/{id}", "~/HotelProduct.aspx");

        // Tickets Page
        routes.MapPageRoute("ViewDayPass", "{bookingId}/ViewDayPass.aspx", "~/ViewDayPass.aspx");

        // Upgrade Ticket Page
        routes.MapPageRoute("Upgrade", "{market}/{city}/{hotelName}/{productName}-upgrade-{bookingTempId}", "~/Upgrade.aspx");

        var settings = new FriendlyUrlSettings();
        settings.AutoRedirectMode = RedirectMode.Off;

        routes.EnableFriendlyUrls(settings);
    }

    public static void MapPageRouteWithName(this RouteCollection routes, string routeName, string routeUrl, string physicalFile, bool checkPhysicalUrlAccess = true,
        RouteValueDictionary defaults = default(RouteValueDictionary), RouteValueDictionary constraints = default(RouteValueDictionary), RouteValueDictionary dataTokens = default(RouteValueDictionary))
    {
        if (dataTokens == null)
            dataTokens = new RouteValueDictionary();

        dataTokens.Add("route-name", routeName);
        routes.MapPageRoute(routeName, routeUrl, physicalFile, checkPhysicalUrlAccess, defaults, constraints, dataTokens);
    }

    public static string GetRouteName(this RouteData routeData)
    {
        if (routeData.DataTokens["route-name"] != null)
            return routeData.DataTokens["route-name"].ToString();
        else return String.Empty;
    }
}