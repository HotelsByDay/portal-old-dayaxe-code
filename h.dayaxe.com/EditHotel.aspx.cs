using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class EditHotel : BasePage
    {
        private int CurrentHotelId { get; set; }

        private HotelRepository _hotelRepository = new HotelRepository();
        private readonly MarketRepository _marketRepositoty = new MarketRepository();
        private readonly CustomerInfoHotelRepository _userHotelRepository = new CustomerInfoHotelRepository();
        private readonly MarketHotelRepository _marketHotelRepository = new MarketHotelRepository();
        private readonly AmentyRepository _amentyRepository = new AmentyRepository();
        private readonly AmentyListRepository _amentyListRepository = new AmentyListRepository();
        private readonly PhotoRepository _photoRepository = new PhotoRepository();
        private Hotels _hotels;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "HotelListing";
            HotelImage.Attributes["name"] = HotelImage.ClientID + "[]";

            if (Request.Params["id"] != null)
            {
                int hotelId;
                int.TryParse(Request.Params["id"], out hotelId);
                CurrentHotelId = hotelId;

                _hotels = CurrentHotelId == 0 ? new Hotels() : _hotelRepository.GetHotel(CurrentHotelId, PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : string.Empty);
            }
            if (!IsPostBack)
            {
                if (Request.Params["id"] != null)
                {
                    // Bind Market Hotels
                    MarketDropdownList.DataSource = _marketRepositoty.GetAll();
                    MarketDropdownList.DataTextField = "HotelDisplayName";
                    MarketDropdownList.DataValueField = "Id";
                    MarketDropdownList.DataBind();

                    HidphotoType.Value = "5";
                    if (CurrentHotelId == 0) // Add new
                    {
                        string sonAmentyList = JsonConvert.SerializeObject(new List<AmentyLists>(),
                            CustomSettings.SerializerSettings());
                        string hotelPolicies = JsonConvert.SerializeObject(new List<HotelPolicies>(),
                            CustomSettings.SerializerSettings());

                        Session["AmentyList"] = sonAmentyList;
                        Session["HotelPolicesList"] = hotelPolicies;

                        HotelType_Festive.Attributes.Add("class",
                            HotelType_Festive.Attributes["class"] + " alter-target");
                        DeleteButton.Visible = false;
                        Deactivebutton.Visible = false;
                        ActiveButton.Visible = false;
                        UploadImage.Visible = false;
                        IsPublishedHidden.Value = "true";
                        _hotels.PassCapacity = 2;
                    }
                    else
                    {
                        // Edit
                        if (_hotels == null)
                        {
                            Response.Redirect(Constant.HotelList);
                        }
                        BindHotelData();
                    }
                    Session["Hotel"] = _hotels.HotelId;
                }
                else
                {
                    Response.Redirect(Constant.HotelList);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.HotelList);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            _hotelRepository.Delete(_hotels);

            _hotelRepository.ResetCache();

            Response.Redirect(Constant.HotelList);
        }

        protected void DeactiveClick(object sender, EventArgs e)
        {
            _hotels.IsActive = false;
            _hotelRepository.Update(_hotels);

            _hotelRepository.ResetCache();

            Deactivebutton.Visible = false;
            ActiveButton.Visible = true;
        }

        protected void SaveHotelClick(object sender, EventArgs e)
        {
            Hotels hotel;
            var isNew = true;
            if (Request.Params["id"] != "0")
            {
                isNew = false;
                hotel = _hotels;
            }
            else
            {
                hotel = new Hotels
                {
                    IsActive = true,
                    IsDelete = false
                };
            }
            
            hotel.TimeZoneId = Constant.UsDefaultTime;
            hotel.Latitude = string.Empty;
            hotel.Longitude = string.Empty;
            Latitude.Text = string.Empty;
            Longitude.Text = string.Empty;

            short offset = 0;
            if (PublicHotel != null)
            {
                offset = Helper.GetTimezoneOffset(PublicHotel.TimeZoneId, DateTime.UtcNow);
            }
            hotel.TimeZoneOffset = offset != 0 ? offset : (short)Constant.LosAngelesTimeWithUtc;

            if (!string.IsNullOrEmpty(StreetAddress.Text))
            {
                var url = string.Format(Constant.GoogleTimeZoneWithAddress,
                    string.Format("{0},{1},{2},{3}", StreetAddress.Text, ZipCode.Text, City.Text, State.Text), AppConfiguration.GoogleApiKey);
                var response = Helper.Get(url);
                var responseAddress = JsonConvert.DeserializeObject<GoogleAddressResultsObject>(response);
                while (responseAddress != null && responseAddress.Status != "OK")
                {
                    Thread.Sleep(200);
                    response = Helper.Get(url);
                    responseAddress = JsonConvert.DeserializeObject<GoogleAddressResultsObject>(response);
                }

                var newLat = responseAddress.Results[0].Geometry.Location.Lat;
                var newLng = responseAddress.Results[0].Geometry.Location.Lng;
                var urlTimezone = string.Format(Constant.GoogleTimeZoneUrl,
                    newLat,
                    newLng,
                    DateTime.UtcNow.GetTimestamp(),
                    AppConfiguration.GoogleApiKey);
                var responseData = Helper.Get(urlTimezone);
                var responseObj = JsonConvert.DeserializeObject<GoogleTimeZoneObject>(responseData);

                hotel.Latitude = newLat.ToString(CultureInfo.InvariantCulture);
                hotel.Longitude = newLng.ToString(CultureInfo.InvariantCulture);

                if (responseObj != null)
                {
                    hotel.TimeZoneId = responseObj.TimeZoneId;
                    //offset = Helper.GetTimezoneOffset(responseObj.TimeZoneId, DateTime.UtcNow);
                    hotel.TimeZoneOffset = (Int16) (responseObj.RawOffset / 3600);
                    //hotel.TimeZoneOffset = offset != 0 ? offset : (Int16)(responseObj.RawOffset / 3600);
                }
            }

            if (string.IsNullOrEmpty(hotel.Latitude) || string.IsNullOrEmpty(hotel.Longitude))
            {
                ErrorMessageLabel.Visible = true;
                ErrorMessageLabel.Text = "Could not save lat/long. Please check hotel address.";
                return;
            }

            hotel.HotelName = HotelName.Text;
            hotel.Neighborhood = Neighborhood.Text;
            hotel.TripAdvisorRating = float.Parse(Rating.Text);
            hotel.HoteltypeId = int.Parse(HotelTypeId.Value);
            bool isCommingSoon;
            bool.TryParse(IsCommingSoonHidden.Value, out isCommingSoon);
            hotel.IsComingSoon = isCommingSoon;

            bool isPublished;
            bool.TryParse(IsPublishedHidden.Value, out isPublished);
            hotel.IsPublished = isPublished;

            hotel.HotelDiscountCode = DiscountCode.Text.ToUpper();
            double discountPercent;
            double.TryParse(DiscountPercent.Text, out discountPercent);
            hotel.HotelDiscountPercent = discountPercent;
            hotel.HotelDiscountDisclaimer = DiscountDisclaimer.Text;
            hotel.HotelParking = ParkingText.Text;

            hotel.StreetAddress = StreetAddress.Text;
            hotel.City = City.Text;
            hotel.State = State.Text;
            hotel.ZipCode = ZipCode.Text;
            hotel.PhoneNumber = PhoneNumber.Text;

            hotel.GeneralHours = GeneralHours.Text;
            hotel.TermsAndConditions = TermsAndConditions.Text;
        
            hotel.Recommendation = Recommendation.Text;
            hotel.TripAdvisorScript = TripAdvisorScriptTextBox.Text;

            hotel.CheckInPlace = CheckInPlaceText.Text.Trim();
            
            int hotelId = _hotels.HotelId;
            if (isNew)
            {
                hotel.HotelPinCode = DateTime.UtcNow.ToString("yyyy");
                hotel.Order = _hotelRepository.GetHotelOrder();
                hotel.Income = DefaultValue.Income;
                hotel.Distance = DefaultValue.Distance;
                hotel.AgeFrom = DefaultValue.AgeFrom;
                hotel.AgeFrom = DefaultValue.AgeTo;
                hotel.TargetGroups = DefaultValue.TargetGroups;
                hotel.Gender = DefaultValue.Genders;
                hotel.Education = DefaultValue.Education;
                hotelId = _hotelRepository.Add(hotel);

                var userHotels = new CustomerInfosHotels()
                {
                    HotelId = hotelId,
                    CustomerId = PublicCustomerInfos.CustomerId
                };
                _userHotelRepository.Add(userHotels);
            }
            else
            {
                _hotelRepository.Update(hotel);

                string json = JsonConvert.SerializeObject(hotel, CustomSettings.SerializerSettings());
                var log = new Logs
                {
                    LogKey = "UpdateHotels",
                    UpdatedContent = string.Format("{0} - {1} - {2}", "Update Hotel Infor", hotel.HotelName, json),
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = PublicCustomerInfos.CustomerId
                };
                _hotelRepository.AddLog(log);
            }

            var markets = _marketHotelRepository.GetMarketByHotelId(hotelId);
            int currentMarketId = int.Parse(MarketDropdownList.SelectedValue);
            if (markets == null)
            {
                var marketHotels = new MarketHotels
                {
                    MarketId = currentMarketId,
                    HotelId = hotelId
                };
                _marketHotelRepository.Add(marketHotels);
            }
            else
            {
                if (markets.MarketId != currentMarketId)
                {
                    markets.MarketId = currentMarketId;
                    _marketHotelRepository.Update(markets);
                }
            }

            // Ameninies
            var amenity = new Amenties();
            if (!isNew)
            {
                amenity = _hotels.AmentiesItem;
            }
            amenity.HotelId = hotelId;
            amenity.IsActive = true;
            amenity.PoolHours = PoolHours.Text;
            amenity.PoolActive = PoolActive.Checked;
            amenity.GymHours = GymHours.Text;
            amenity.GymActive = GymActive.Checked;
            amenity.SpaActive = SpaActive.Checked;
            amenity.SpaHours = SpaHours.Text;
            amenity.BusinessActive = BusinessCenterActive.Checked;
            amenity.BusinessCenterHours = BusinessCenterHours.Text;
            amenity.OtherActive = OtherActive.Checked;
            amenity.OtherHours = OtherHours.Text;
            amenity.DinningActive = DiningActive.Checked;
            amenity.DinningHours = DiningHours.Text;
            amenity.EventActive = EventActive.Checked;
            amenity.EventHours = EventHours.Text;
            int amentyId;
            if (isNew)
            {
                amentyId = _amentyRepository.Add(amenity);
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyLists.ForEach(item =>
                {
                    item.Id = 0;
                    item.AmentyId = amentyId;
                });
                _amentyListRepository.Add(amentyLists);

                string sessionHotelPolicies = Session["HotelPolicesList"] != null ? Session["HotelPolicesList"].ToString() : string.Empty;
                var hotelPolicies = JsonConvert.DeserializeObject<List<HotelPolicies>>(sessionHotelPolicies);
                int idx = 1;
                hotelPolicies.ForEach(item =>
                {
                    item.Id = 0;
                    item.Order = idx;
                    item.HotelId = hotelId;
                    idx++;
                });
                _hotelRepository.AddHotelPolicies(hotelPolicies);
            }
            else
            {
                var amentiesList = new List<AmentyLists>();
                foreach (RepeaterItem item in RptPoolAmenities.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidPoolAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptPoolUpgrades.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidPoolUpgrades");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptGymAmenties.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidPoolAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptGymUpgrades.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidGymUpgrades");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptSpaAmenties.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidPoolAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptSpaUpgrades.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidSpaUpgrades");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptBusinessCenterAmenties.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidPoolAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptBusinessCenterUpgrades.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidBusinessCenterUpgrades");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptOtherAmenties.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidPoolAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptOtherUpgrades.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidOtherUpgrades");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptDiningAmenties.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidDiningAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                foreach (RepeaterItem item in RptEventAmenties.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("HidEventAmenties");

                    var amentyItem = _amentyListRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("AmentyOrder");
                    amentyItem.AmentyOrder = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        amentyItem.AmentyOrder = int.Parse(orderItem.Value);
                    }

                    amentiesList.Add(amentyItem);
                }
                _amentyRepository.Update(amenity);
                _amentyListRepository.Update(amentiesList);

                var photos = new List<Photos>();
                foreach (ListViewDataItem item in LVHotelImage.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidAmentyList = (HiddenField)item.FindControl("PhotoId");

                    var photo = _photoRepository.GetById(int.Parse(hidAmentyList.Value));
                    var orderItem = (HiddenField)item.FindControl("Order");
                    photo.Order = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        photo.Order = int.Parse(orderItem.Value);
                    }

                    photos.Add(photo);
                }

                _photoRepository.Update(photos);

                // Update Hotel Policies
                var hotelPolicies = new List<HotelPolicies>();
                foreach (RepeaterItem item in RptPolicies.Items)
                {
                    //to get the dropdown of each line
                    HiddenField hidId = (HiddenField)item.FindControl("HidId");

                    var policy = _hotelRepository.GetHotelPolicesById(int.Parse(hidId.Value), hotelId);
                    var orderItem = (HiddenField)item.FindControl("Order");
                    policy.Order = 1;
                    if (!string.IsNullOrEmpty(orderItem.Value))
                    {
                        policy.Order = int.Parse(orderItem.Value);
                    }

                    hotelPolicies.Add(policy);
                }

                _hotelRepository.UpdateHotelPolicies(hotelPolicies, hotelId);
            }

            string hotelImageDefault = Constant.ImageDefault;
            var hotelImage = _photoRepository.GetAll().FirstOrDefault(x => x.HotelId == _hotels.HotelId && x.ImageTypeId > 3 && x.IsActive.HasValue && x.IsActive.Value);
            if (hotelImage != null)
            {
                hotelImageDefault = hotelImage.Url;
            }

            if (!string.IsNullOrEmpty(hotelImageDefault))
            {
                var imageName = Helper.ReplaceLastOccurrence(hotelImageDefault, ".", "-ovl.");
                string imageUrl = Server.MapPath(imageName);
                if (!File.Exists(imageUrl))
                {
                    System.Drawing.Image image = System.Drawing.Image.FromFile(Server.MapPath(hotelImageDefault));
                    Bitmap newImage = Helper.ChangeOpacity(image, 0.7f);
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(imageUrl, FileMode.Create, FileAccess.ReadWrite))
                        {
                            newImage.Save(memory, ImageFormat.Jpeg);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }

            _hotelRepository.ResetCache();
            Session.Remove("AmentyList");
            Session.Remove("HotelPolicesList");

            Response.Redirect(Constant.HotelList);

            // Photo
        }
    
        protected void ActiveClick(object sender, EventArgs e)
        {
            _hotels.IsActive = true;
            _hotelRepository.Update(_hotels);

            _hotelRepository.ResetCache();

            Deactivebutton.Visible = true;
            ActiveButton.Visible = false;
        }
    
        #region Support Function 

        private void BindHotelData()
        {
            HotelName.Text = _hotels.HotelName;
            Neighborhood.Text = _hotels.Neighborhood;
            Rating.Text = (_hotels.TripAdvisorRating ?? 4).ToString(CultureInfo.InvariantCulture);
            int hotelTypeId = _hotels.HoteltypeId ?? 0;
            switch (hotelTypeId)
            {
                case (int)Enums.Hoteltype.FESTIVE:
                    HotelType_Festive.Attributes.Add("class", HotelType_Festive.Attributes["class"] + " alter-target");
                    break;
                case (int)Enums.Hoteltype.TRANQUIL:
                    HotelType_Tranquil.Attributes.Add("class", HotelType_Tranquil.Attributes["class"] + " alter-target");
                    break;
                case (int)Enums.Hoteltype.FAMILY:
                    HotelType_Family.Attributes.Add("class", HotelType_Family.Attributes["class"] + " alter-target");
                    break;
                case (int)Enums.Hoteltype.BASIC:
                    HotelType_Basic.Attributes.Add("class", HotelType_Basic.Attributes["class"] + " alter-target");
                    break;
            }
            HotelTypeId.Value = hotelTypeId.ToString();

            IsCommingSoonHidden.Value = _hotels.IsComingSoon == true? "true" : "false";
            IsPublishedHidden.Value = _hotels.IsPublished ? "true" : "false";
            DiscountCode.Text = _hotels.HotelDiscountCode;
            DiscountPercent.Text = _hotels.HotelDiscountPercent.ToString("0");
            DiscountDisclaimer.Text = _hotels.HotelDiscountDisclaimer;
            ParkingText.Text = _hotels.HotelParking;

            StreetAddress.Text = _hotels.StreetAddress;
            City.Text = _hotels.City;
            State.Text = _hotels.State;
            ZipCode.Text = _hotels.ZipCode;
            PhoneNumber.Text = _hotels.PhoneNumber;
            Latitude.Text = _hotels.Latitude;
            Longitude.Text = _hotels.Longitude;

            GeneralHours.Text = _hotels.GeneralHours;
            TermsAndConditions.Text = _hotels.TermsAndConditions;
            Recommendation.Text = _hotels.Recommendation;
            TripAdvisorScriptTextBox.Text = _hotels.TripAdvisorScript;

            PoolActive.Checked = _hotels.AmentiesItem.PoolActive;
            GymActive.Checked = _hotels.AmentiesItem.GymActive;
            BusinessCenterActive.Checked = _hotels.AmentiesItem.BusinessActive;
            SpaActive.Checked = _hotels.AmentiesItem.SpaActive;
            OtherActive.Checked = _hotels.AmentiesItem.OtherActive;
            DiningActive.Checked = _hotels.AmentiesItem.DinningActive;
            EventActive.Checked = _hotels.AmentiesItem.EventActive;

            PoolHours.Text = _hotels.AmentiesItem.PoolHours;
            GymHours.Text = _hotels.AmentiesItem.GymHours;
            BusinessCenterHours.Text = _hotels.AmentiesItem.BusinessCenterHours;
            SpaHours.Text = _hotels.AmentiesItem.SpaHours;
            OtherHours.Text = _hotels.AmentiesItem.OtherHours;
            DiningHours.Text = _hotels.AmentiesItem.DinningHours;
            EventHours.Text = _hotels.AmentiesItem.EventHours;
            CheckInPlaceText.Text = _hotels.CheckInPlace;

            var marketsHotels = _marketHotelRepository.GetMarketByHotelId(_hotels.HotelId);
            if (marketsHotels != null)
            {
                MarketDropdownList.SelectedValue = marketsHotels.MarketId.ToString();
            }

            if (_hotels.IsActive)
            {
                Deactivebutton.Visible = true;
                ActiveButton.Visible = false;
            }
            else
            {
                Deactivebutton.Visible = false;
                ActiveButton.Visible = true;
            }

            RebindPool(false);

            RebindPhoto(false);

            BindPolicies(false);
        }

        private void RebindPool(bool isGetNewData = true)
        {
            if (isGetNewData)
            {
                _hotelRepository = new HotelRepository();
                _hotels = _hotelRepository.GetHotel(_hotels.HotelId, PublicCustomerInfos.EmailAddress);
            }
        
            RptPoolAmenities.DataSource = _hotels.PoolAmentyListses;
            RptPoolAmenities.DataBind();

            RptPoolUpgrades.DataSource = _hotels.PoolAmentyUpgrages;
            RptPoolUpgrades.DataBind();

            RptGymAmenties.DataSource = _hotels.GymAmentyListses;
            RptGymAmenties.DataBind();

            RptGymUpgrades.DataSource = _hotels.GymAmentyUpgrages;
            RptGymUpgrades.DataBind();

            RptSpaAmenties.DataSource = _hotels.SpaAmentyListses;
            RptSpaAmenties.DataBind();

            RptSpaUpgrades.DataSource = _hotels.SpaAmentyUpgrages;
            RptSpaUpgrades.DataBind();

            RptOtherAmenties.DataSource = _hotels.OtherAmentyListses;
            RptOtherAmenties.DataBind();

            RptOtherUpgrades.DataSource = _hotels.OtherAmentyUpgrages;
            RptOtherUpgrades.DataBind();

            RptBusinessCenterAmenties.DataSource = _hotels.BusinessCenterAmentyListses;
            RptBusinessCenterAmenties.DataBind();

            RptBusinessCenterUpgrades.DataSource = _hotels.BusinessCenterAmentyUpgrages;
            RptBusinessCenterUpgrades.DataBind();

            RptDiningAmenties.DataSource = _hotels.DinningAmentyListes;
            RptDiningAmenties.DataBind();

            RptEventAmenties.DataSource = _hotels.EventAmentyListes;
            RptEventAmenties.DataBind();
        }

        private void RebindPhoto(bool isGetNewData = true)
        {
            if (isGetNewData)
            {
                _hotels = _hotelRepository.GetHotel(_hotels.HotelId, PublicCustomerInfos.EmailAddress);
            }
            LVHotelImage.DataSource = _hotels.PhotoList;
            LVHotelImage.DataBind();
        }

        private void RebindAllAmentyListFromSession()
        {
            string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
            var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
            var poolAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                         && x.AmentyTypeId.Value == (int)Enums.AmentyType.Pool
                                                         && x.IsAmenty.HasValue && x.IsAmenty.Value);
            var poolAmentyUpgrades = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                            && x.AmentyTypeId.Value == (int)Enums.AmentyType.Pool
                                                            && x.IsAmenty.HasValue && !x.IsAmenty.Value);

            var gymAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                        && x.AmentyTypeId.Value == (int)Enums.AmentyType.Gym
                                                        && x.IsAmenty.HasValue && x.IsAmenty.Value);
            var gymAmentyUpgrades = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                           && x.AmentyTypeId.Value == (int)Enums.AmentyType.Gym
                                                           && x.IsAmenty.HasValue && !x.IsAmenty.Value);

            var bcAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                       && x.AmentyTypeId.Value == (int)Enums.AmentyType.BusinessCenter
                                                       && x.IsAmenty.HasValue && x.IsAmenty.Value);
            var bcAmentyUpgrades = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                          && x.AmentyTypeId.Value == (int)Enums.AmentyType.BusinessCenter
                                                          && x.IsAmenty.HasValue && !x.IsAmenty.Value);

            var spaAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                        && x.AmentyTypeId.Value == (int)Enums.AmentyType.Spa
                                                        && x.IsAmenty.HasValue && x.IsAmenty.Value);
            var spaAmentyUpgrades = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                           && x.AmentyTypeId.Value == (int)Enums.AmentyType.Spa
                                                           && x.IsAmenty.HasValue && !x.IsAmenty.Value);

            var otherAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                          && x.AmentyTypeId.Value == (int)Enums.AmentyType.Other
                                                          && x.IsAmenty.HasValue && x.IsAmenty.Value);
            var otherAmentyUpgrades = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                             && x.AmentyTypeId.Value == (int)Enums.AmentyType.Other
                                                             && x.IsAmenty.HasValue && !x.IsAmenty.Value);

            var dinningAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                          && x.AmentyTypeId.Value == (int)Enums.AmentyType.Dining
                                                          && x.IsAmenty.HasValue && x.IsAmenty.Value);

            var eventAmentyLists = amentyLists.Where(x => x.AmentyTypeId.HasValue
                                                          && x.AmentyTypeId.Value == (int)Enums.AmentyType.Event
                                                          && x.IsAmenty.HasValue && x.IsAmenty.Value);

            RptPoolAmenities.DataSource = poolAmentyLists;
            RptPoolAmenities.DataBind();

            RptPoolUpgrades.DataSource = poolAmentyUpgrades;
            RptPoolUpgrades.DataBind();

            RptSpaAmenties.DataSource = spaAmentyLists;
            RptSpaAmenties.DataBind();

            RptSpaUpgrades.DataSource = spaAmentyUpgrades;
            RptSpaUpgrades.DataBind();

            RptGymAmenties.DataSource = gymAmentyLists;
            RptGymAmenties.DataBind();

            RptGymUpgrades.DataSource = gymAmentyUpgrades;
            RptGymUpgrades.DataBind();

            RptOtherAmenties.DataSource = otherAmentyLists;
            RptOtherAmenties.DataBind();

            RptOtherUpgrades.DataSource = otherAmentyUpgrades;
            RptOtherUpgrades.DataBind();

            RptBusinessCenterAmenties.DataSource = bcAmentyLists;
            RptBusinessCenterAmenties.DataBind();

            RptBusinessCenterUpgrades.DataSource = bcAmentyUpgrades;
            RptBusinessCenterUpgrades.DataBind();

            RptDiningAmenties.DataSource = dinningAmentyLists;
            RptDiningAmenties.DataBind();

            RptEventAmenties.DataSource = eventAmentyLists;
            RptEventAmenties.DataBind();

            string session = Session["HotelPolicesList"] != null ? Session["HotelPolicesList"].ToString() : string.Empty;
            var hotelPolicies = JsonConvert.DeserializeObject<List<HotelPolicies>>(session);

            RptPolicies.DataSource = hotelPolicies;
            RptPolicies.DataBind();
        }

        #endregion

        #region Pool Amenties

        protected void PoolSaveHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.PoolHours = PoolHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void PoolAddAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = PoolAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.Pool
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void PoolUpgradesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = PoolUpgrades.Text,
                IsActive = true,
                IsAmenty = false,
                AmentyTypeId = (int)Enums.AmentyType.Pool
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeletePoolAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptPoolAmenities.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptPoolAmenities.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeletePoolUpgradesList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptPoolUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptPoolUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        #region Gym Amenties
        protected void GymSaveHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.GymHours = GymHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void GymAddAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = GymAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.Gym
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);
                _hotelRepository.ResetCache();
                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteGymAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptGymAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptGymAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void GymUpgradesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = GymUpgrades.Text,
                IsActive = true,
                IsAmenty = false,
                AmentyTypeId = (int)Enums.AmentyType.Gym
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteGymUpgradesList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptGymUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkGymUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidGymUpgrades");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptGymUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkGymUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidGymUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        #region Business Services Amenties
        protected void BusinessCenterSaveHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.BusinessCenterHours = BusinessCenterHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void BusinessCenterAddAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = BusinessCenterAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.BusinessCenter
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteBusinessCenterAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptBusinessCenterAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptBusinessCenterAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void BusinessCenterUpgradesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = BusinessCenterUpgrades.Text,
                IsActive = true,
                IsAmenty = false,
                AmentyTypeId = (int)Enums.AmentyType.BusinessCenter
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteBusinessCenterUpgradesList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptBusinessCenterUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkBusinessCenterUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidBusinessCenterUpgrades");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptBusinessCenterUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkBusinessCenterUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidBusinessCenterUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        #region Spa Amenties

        protected void SpaSaveHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.SpaHours = SpaHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void SpaAddAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = SpaAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.Spa
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteSpaAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptSpaAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptSpaAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void SpaUpgradesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = SpaUpgrades.Text,
                IsActive = true,
                IsAmenty = false,
                AmentyTypeId = (int)Enums.AmentyType.Spa
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteSpaUpgradesList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptSpaUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkSpaUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidSpaUpgrades");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptSpaUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkSpaUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidSpaUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        #region Other Amenties

        protected void OtherSaveHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.OtherHours = OtherHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void OtherAddAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = OtherAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.Other
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteOtherAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptOtherAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptOtherAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPoolAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidPoolUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }
    
        protected void OtherUpgradesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = OtherUpgrades.Text,
                IsActive = true,
                IsAmenty = false,
                AmentyTypeId = (int)Enums.AmentyType.Other
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteOtherUpgradesList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptOtherUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkOtherUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidOtherUpgrades");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptOtherUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkOtherUpgrades");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidOtherUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        protected void UploadImageClick(object sender, EventArgs e)
        {
            if (HotelImage.HasFile)
            {
                var file = HotelImage.PostedFile;
                string hotelId = Request.Params["id"];
                if (Request.Params["id"] != "0")
                {
                    string pathString = Server.MapPath(string.Format("/HotelImage/{0}/", hotelId));
                    string localImageFile = string.Format("/HotelImage/{0}/{1}", hotelId, file.FileName);

                    if (!Directory.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);
                    }
                    try
                    {
                        file.SaveAs(Server.MapPath(localImageFile));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    int id;
                    int.TryParse(hotelId, out id);
                    var photo = new Photos
                    {
                        HotelId = id,
                        Url = localImageFile,
                        ImageTypeId = int.Parse(HidphotoType.Value),
                        IsActive = true
                    };
                    int maxOrder = 1;
                    var hotel = _hotelRepository.GetHotel(id, PublicCustomerInfos.EmailAddress);
                    if (hotel.PhotoList.Any())
                    {
                        maxOrder = hotel.PhotoList.ToList().Count + 1;
                    }
                    photo.Order = maxOrder;
                    _photoRepository.Add(photo);

                    _hotelRepository.ResetCache();

                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
            }

        }

        #region Dinning

        protected void SaveDiningHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.DinningHours = DiningHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void AddDiningAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = DiningAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.Dining
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteDiningAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptDiningAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkDiningAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidDiningAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptDiningAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkDiningAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidDiningUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        #region Event

        protected void SaveEventHoursClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                var amenity = _hotels.AmentiesItem;
                amenity.EventHours = EventHours.Text;
                _amentyRepository.Update(amenity);

                _hotelRepository.ResetCache();
            }
        }

        protected void AddEventAmentiesClick(object sender, EventArgs e)
        {
            var amentyList = new AmentyLists
            {
                AmentyId = _hotels.AmentiesItem != null ? _hotels.AmentiesItem.Id : 0,
                Name = EventAmenitiesList.Text,
                IsActive = true,
                IsAmenty = true,
                AmentyTypeId = (int)Enums.AmentyType.Event
            };
            if (Request.Params["id"] != "0")
            {
                _amentyListRepository.Add(amentyList);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                amentyList.Id += amentyLists.Any() ? amentyLists.Max(x => x.Id) + 1 : 1;
                amentyLists.Add(amentyList);
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        protected void DeleteEventAmentyList(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<int> ids = new List<int>();
                foreach (RepeaterItem item in RptEventAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkEventAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidEventAmenties");
                        ids.Add(int.Parse(hidPool.Value));
                    }
                }
                _amentyListRepository.Delete(ids);

                _hotelRepository.ResetCache();

                RebindPool();
            }
            else
            {
                string sessionAmentyList = Session["AmentyList"] != null ? Session["AmentyList"].ToString() : string.Empty;
                var amentyLists = JsonConvert.DeserializeObject<List<AmentyLists>>(sessionAmentyList);
                //AmentiesName
                foreach (RepeaterItem item in RptEventAmenties.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkEventAmenities");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidEventUpgrades");
                        amentyLists.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string jsonAmentyList = JsonConvert.SerializeObject(amentyLists, CustomSettings.SerializerSettings());
                Session["AmentyList"] = jsonAmentyList;
                RebindAllAmentyListFromSession();
            }
        }

        #endregion

        #region Policies

        private void BindPolicies(bool isReload = false)
        {
            if (isReload)
            {
                _hotelRepository = new HotelRepository();
            }

            var policies = _hotelRepository.GetAllPolices();

            var hotelPolicies = _hotelRepository.GetAllHotelPolices(_hotels.HotelId);
            RptPolicies.DataSource = hotelPolicies;
            RptPolicies.DataBind();

            var availPolicies = policies.Where(p => !hotelPolicies.Select(hp => hp.PolicyId).Contains(p.Id)).ToList();
            DdlPolicies.DataSource = availPolicies;
            DdlPolicies.DataTextField = "Name";
            DdlPolicies.DataValueField = "Id";
            DdlPolicies.DataBind();
        }

        protected void DeletePolicesButton_OnClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<long> ids = new List<long>();
                foreach (RepeaterItem item in RptPolicies.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPolicies");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidId");
                        ids.Add(long.Parse(hidPool.Value));
                    }
                }
                _hotelRepository.DeleteHotelPolicies(ids, PublicHotel.HotelId);

                _hotelRepository.ResetCache();

                BindPolicies(true);
            }
            else
            {
                string session = Session["HotelPolicesList"] != null ? Session["HotelPolicesList"].ToString() : string.Empty;
                var hotelPolicies = JsonConvert.DeserializeObject<List<HotelPolicies>>(session);
                //AmentiesName
                foreach (RepeaterItem item in RptPoolUpgrades.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkPolicies");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidPool = (HiddenField)item.FindControl("HidId");
                        hotelPolicies.RemoveAll(x => x.Id == int.Parse(hidPool.Value));
                    }
                }
                string json = JsonConvert.SerializeObject(hotelPolicies, CustomSettings.SerializerSettings());
                Session["HotelPolicesList"] = json;
                RebindAllAmentyListFromSession();
            }
        }

        protected void AddPoliciesButton_OnClick(object sender, EventArgs e)
        {
            if (DdlPolicies.Items.Count > 0)
            {
                var hotelPolicyItem = new HotelPolicies
                {
                    PolicyId = int.Parse(DdlPolicies.SelectedValue),
                    ProductId = 0,
                    HotelId = 0
                };
                if (Request.Params["id"] != "0")
                {
                    hotelPolicyItem.HotelId = _hotels.HotelId;
                    _hotelRepository.AddHotelPolicies(hotelPolicyItem);

                    _hotelRepository.ResetCache();

                    BindPolicies(true);
                }
                else
                {
                    string session = Session["HotelPolicesList"] != null
                        ? Session["HotelPolicesList"].ToString()
                        : string.Empty;
                    var hotelPolicies = JsonConvert.DeserializeObject<List<HotelPolicies>>(session);
                    hotelPolicyItem.Order += hotelPolicies.Any() ? hotelPolicies.Max(x => x.Id) + 1 : 1;
                    hotelPolicies.Add(hotelPolicyItem);
                    string jsonAmentyList =
                        JsonConvert.SerializeObject(hotelPolicies, CustomSettings.SerializerSettings());
                    Session["HotelPolicesList"] = jsonAmentyList;
                    RebindAllAmentyListFromSession();
                }
            }
        }

        #endregion
    }
}