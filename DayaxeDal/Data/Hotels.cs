using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DayaxeDal
{
    public partial class Hotels
    {
        [JsonIgnore]
        public Amenties AmentiesItem{ get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> PoolAmentyListses { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> PoolAmentyUpgrages { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> GymAmentyListses { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> GymAmentyUpgrages { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> SpaAmentyListses { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> SpaAmentyUpgrages { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> BusinessCenterAmentyListses { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> BusinessCenterAmentyUpgrages { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> DinningAmentyListes { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> EventAmentyListes{ get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> OtherAmentyListses { get; set; }

        [JsonIgnore]
        public IEnumerable<AmentyLists> OtherAmentyUpgrages { get; set; }

        [JsonIgnore]
        public IEnumerable<Photos> PhotoList { get; set; }

        [JsonIgnore]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public string ImageSurveyUrl { get; set; }

        public int TotalCustomerReviews { get; set; }

        public double CustomerRating { get; set; }

        public string CustomerRatingString {
            get
            {
                string str = string.Empty;
                if (CustomerRating > 0)
                {
                    for (var i = 0; i <= 4; i++)
                    {

                        string url = String.Empty;

                        url = CustomerRating - i >= 0.5 ? Constant.FullStar : Constant.EmptyStar;
                        str += string.Format("<img src=\"{0}\" class=\"img-responsive star\" alt=\"star\" />", url);
                    }
                    str += string.Format("<span class=\"t\">{0}</span>", TotalCustomerReviews);
                }
                return str;
            }
        }

        public string Rating
        {
            get
            {
                string str = string.Empty;
                for (var i = 0; i <= 4; i++)
                {

                    string url = String.Empty;

                    if (TripAdvisorRating - i >= 1)
                    {
                        url = Constant.FullStar;
                    }
                    else if (TripAdvisorRating - i > 0)
                    {
                        url =Constant.HalfStar;
                    }
                    else
                    {
                        url = Constant.EmptyStar;
                    }
                    str += string.Format("<img src=\"{0}\" class=\"img-responsive star\" alt=\"star\" />", url);
                }
                return str;
            }
        }

        public bool IsMenuVisible
        {
            get { return HotelId != 0; }
        }

        public string HotelInfo
        {
            get { return string.Format("{0}, {1}", HotelName, Neighborhood); }
        }

        public string[] PoolFeatures { get; set; }

        public string[] GymFeatures { get; set; }

        public string[] SpaFeatures { get; set; }

        public string[] OfficeFeatures { get; set; }

        public string[] DinningFeatures { get; set; }

        public string[] EventFeatures { get; set; }

        public string[] PhotoUrls { get; set; }

        // public List<BlockedDates> GetNearBlockedDates { get; set; }

        public Amenties AmentiesHotels { get; set; }

        // public bool IsOnBlackOutDay { get; set; }

        public string NextAvailableDay { get; set; }

        private bool HasDistance { get; set; }

        public double DistanceWithUser { get; set; }

        public void GetDistanceWithUser(string lat, string lng)
        {
            if (!string.IsNullOrEmpty(lat) 
                && !string.IsNullOrEmpty(lng) 
                && !string.IsNullOrEmpty(Latitude) 
                && !string.IsNullOrEmpty(Longitude))
            {
                float userLat;
                float userLng;
                float.TryParse(lat, out userLat);
                float.TryParse(lng, out userLng);

                float hotelLat;
                float hotelLng;
                float.TryParse(Latitude, out hotelLat);
                float.TryParse(Longitude, out hotelLng);

                DistanceWithUser = Helper.GetDistanceFromLatLonInMiles(userLng, userLat, hotelLng, hotelLat);
                HasDistance = true;
                return;
            }

            HasDistance = false;
            DistanceWithUser = 0;
        }

        public int CustomOrder { get; set; }

        public void CalculateCustomOrder(bool isCurrentMarket)
        {
            if (isCurrentMarket)
            {
                if (IsPublished && (IsComingSoon == null || !IsComingSoon.Value))
                {
                    CustomOrder = 1;
                }
                else
                {
                    CustomOrder = 2;
                }
            }
            else
            {
                if (IsPublished && (IsComingSoon == null || !IsComingSoon.Value))
                {
                    CustomOrder = 3;
                }
                else
                {
                    CustomOrder = 4;
                }
            }
            //if (IsPublished && (IsComingSoon == null || !IsComingSoon.Value))
            //{
            //    CustomOrder = HasDistance ? 1 : 2;
            //}
            //if (!IsPublished && (IsComingSoon == null || !IsComingSoon.Value))
            //{
            //    CustomOrder = HasDistance ? 3 : 4;
            //}
            //if (IsPublished && IsComingSoon.HasValue && IsComingSoon.Value)
            //{
            //    CustomOrder = HasDistance ? 5 : 6;
            //}
            //if (!IsPublished && IsComingSoon.HasValue && IsComingSoon.Value)
            //{
            //    CustomOrder = HasDistance ? 7 : 8;
            //}
        }

        public string HotelNameUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(HotelName))
                {
                    return HotelName.Trim().Replace(" ", "-").Replace("-&-", "-").Replace("$", "").ToLower();
                }
                return string.Empty;
            }
        }
        public string CityUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(City))
                {
                    return City.Trim().Replace(" ", "-").Replace("-&-", "-").Replace("$", "").ToLower();
                }
                return string.Empty;
            }
        }

        public string TotalReviews { get; set; }

        public string MoreAtHotelString { get; set; }
    }
}
