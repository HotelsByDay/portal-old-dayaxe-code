using System;
using System.Linq;
using System.Web.UI;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace dayaxe.com.Controls
{
    public partial class ControlsSurvey : UserControl
    {
        private readonly SurveyRepository _surveyRepository = new SurveyRepository();
        private Surveys _survey;
        private Bookings _booking;
        private int _productId;
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Page.RouteData.Values["id"] != null)
            {
                if (Session["Survey_Finish_" + Page.RouteData.Values["id"]] != null)
                {
                    Visible = false;
                }
                string surveyId = Page.RouteData.Values["id"].ToString().Replace(" ", "").Trim();
                if ((_survey == null || _survey.Code != surveyId) && !surveyId.Contains("reviews"))
                {
                    if (int.TryParse(Page.RouteData.Values["id"].ToString(), out _productId))
                    {
                        Visible = false;
                    }
                    else
                    {
                        _survey = _surveyRepository.GetSurveysByCode(surveyId);
                    }
                }

                if (_survey != null)
                {
                    _booking = _surveyRepository.BookingList.FirstOrDefault(b => b.BookingId == _survey.BookingId);
                    RedeemedDate.Text = _survey.RedeemedDate.HasValue
                        ? _survey.RedeemedDate.Value.ToString(Constant.SurveyDateFormat)
                        : string.Empty;
                    if (_booking != null)
                    {
                        Price.Text = Helper.FormatPrice(_booking.TotalPrice);
                    }

                    if (_booking != null && _booking.PassStatus == (int) Enums.BookingStatus.Refunded)
                    {
                        UpdatePanel1.Visible = false;
                        cancelledBookingModal.Visible = true;
                    }

                    ImageProduct.ImageUrl = _survey.ImageUrl;
                    HotelInfo.Text = _survey.HotelInfo;

                    if (Request.Params["tab"] != null)
                    {
                        int tab;
                        int.TryParse(Request.Params["tab"], out tab);
                        if (tab >= SurveyViews.Views.Count)
                        {
                            tab = 0;
                        }
                        SurveyViews.ActiveViewIndex = tab;
                    }

                    // Tab 1
                    if (_survey.Rating.HasValue && _survey.Rating.Value > 0)
                    {
                        Rating.Text = _survey.Rating.Value.ToString("N1");
                        Ratecommend.Text = _survey.RateCommend;
                        switch ((int)_survey.Rating.Value)
                        {
                            case 0:
                            case 1:
                                RateTitle.Text = "Poor";
                                break;
                            case 2:
                                RateTitle.Text = "Fair";
                                break;
                            case 3:
                                RateTitle.Text = "Neutral";
                                break;
                            case 4:
                                RateTitle.Text = "Very Good";
                                break;
                            case 5:
                                RateTitle.Text = "Excellent";
                                break;
                        }
                    }

                    // Tab 2
                    UsePool.Attributes["class"] += _survey.UsePool ? " btn-use-selected" : string.Empty;
                    UseGym.Attributes["class"] += _survey.UseGym ? " btn-use-selected" : string.Empty;
                    UseSpa.Attributes["class"] += _survey.UseSpa ? " btn-use-selected" : string.Empty;
                    UseBusinessCenter.Attributes["class"] += _survey.UseBusinessCenter
                        ? " btn-use-selected"
                        : string.Empty;
                    HidUsePool.Value = _survey.UsePool.ToString();
                    HidUseGym.Value = _survey.UseGym.ToString();
                    HiduseSpa.Value = _survey.UseSpa.ToString();
                    HidUseBusinessCenter.Value = _survey.UseBusinessCenter.ToString();

                    // Tab 3
                    if (_survey.IsBuyFoodAndDrink && _survey.FoodAndDrinkPrice.HasValue)
                    {
                        UseFoodDrinkText.Attributes["data-slider-value"] = _survey.FoodAndDrinkPrice.Value.ToString("0");
                        foodDrinkPrice.Attributes["class"] = "form-group use-food-drink-price";
                        useFoodDrinkButton.Attributes["class"] += " btn-use-selected";
                    }

                    // Tab 4
                    if (_survey.IsPayForParking)
                    {
                        PayForParkingYesButton.Attributes["class"] += " btn-use-selected";
                    }

                    // Tab 5
                    if (_survey.IsBuySpaService && _survey.SpaServicePrice.HasValue)
                    {
                        UseSpaServiceText.Attributes["data-slider-value"] = _survey.SpaServicePrice.Value.ToString("0");
                        SpaServicePrice.Attributes["class"] = "form-group use-spa-service-price";
                        UseSpaServiceButton.Attributes["class"] += " btn-use-selected";
                    }

                    // Tab 6
                    if (_survey.IsBuyAdditionalService && _survey.AdditionalServicePrice.HasValue)
                    {
                        UseAdditionalServiceText.Attributes["data-slider-value"] =
                            _survey.AdditionalServicePrice.Value.ToString("N1");
                        AdditionalServicePrice.Attributes["class"] = "form-group use-additional-service-price";
                        UseAdditionalServiceButton.Attributes["class"] += " btn-use-selected";
                    }
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void NextButton_OnServerClick(object sender, EventArgs e)
        {
            if (_survey != null)
            {
                var tab = 0;
                if (Request.Params["tab"] != null)
                {
                    int.TryParse(Request.Params["tab"], out tab);
                }
                switch (SurveyViews.ActiveViewIndex)
                {
                    case 0:
                        double rating;
                        double.TryParse(Rating.Text, out rating);
                        _survey.Rating = rating;
                        _survey.RateCommend = Ratecommend.Text;
                        break;
                    case 1:
                        _survey.UsePool = Convert.ToBoolean(HidUsePool.Value);
                        _survey.UseGym = Convert.ToBoolean(HidUseGym.Value);
                        _survey.UseSpa = Convert.ToBoolean(HiduseSpa.Value);
                        _survey.UseBusinessCenter = Convert.ToBoolean(HidUseBusinessCenter.Value);
                        break;
                    case 2:
                        _survey.IsBuyFoodAndDrink = true;
                        double foodDrinkPrice;
                        double.TryParse(UseFoodDrinkText.Text, out foodDrinkPrice);
                        _survey.FoodAndDrinkPrice = foodDrinkPrice;
                        break;
                    case 3:
                        //if(_survey.IsPayForParking);
                        //_survey.ParkingPrice = 0;
                        break;
                    case 4:
                        _survey.IsBuySpaService = true;
                        double spaServicePrice;
                        double.TryParse(UseSpaServiceText.Text, out spaServicePrice);
                        _survey.SpaServicePrice = spaServicePrice;
                        break;
                    case 5:
                        _survey.IsBuyAdditionalService = true;
                        double additionalServicePrice;
                        double.TryParse(UseAdditionalServiceText.Text, out additionalServicePrice);
                        _survey.AdditionalServicePrice = additionalServicePrice;
                        NextButton.Text = "Done";
                        CacheLayer.ClearAll();
                        //NextButton.Attributes["class"] = "";
                        _survey.IsFinish = true;
                        ScriptManager.RegisterClientScriptInclude(UpdatePanel1, typeof(string), "setTripAdvisorScript", "https://www.jscache.com/wejs?wtype=cdswritereviewlg&amp;uniq=455&amp;locationId=638765&amp;lang=en_US&amp;lang=en_US&amp;display_version=2");
                        break;
                    case 6:
                        _survey.IsFinish = true;
                        Session["Survey_Finish_" + Page.RouteData.Values["id"]] = true;
                        Visible = false;
                        ScriptManager.RegisterClientScriptBlock(UpdatePanel1, typeof(string), "setSurveySession", "window.userSurvey = 'true';", true);
                        break;
                }

                _survey.LastUpdatedDate = DateTime.UtcNow;
                _survey.LastStep = SurveyViews.ActiveViewIndex + 1;
                _surveyRepository.Update(_survey);
                if (SurveyViews.ActiveViewIndex != (SurveyViews.Views.Count - 1))
                {
                    SurveyViews.ActiveViewIndex = SurveyViews.ActiveViewIndex + 1;
                }
            }
        }

        protected void NotUseFoodDrinkButton_OnServerClick(object sender, EventArgs e)
        {
            _survey.IsBuyFoodAndDrink = false;
            _survey.FoodAndDrinkPrice = 0;
            _survey.LastStep = SurveyViews.ActiveViewIndex + 1;
            _surveyRepository.Update(_survey);
            SurveyViews.ActiveViewIndex = SurveyViews.ActiveViewIndex + 1;
        }

        protected void PayForParkingNoButton_OnServerClick(object sender, EventArgs e)
        {
            _survey.IsPayForParking = false;
            _survey.LastStep = SurveyViews.ActiveViewIndex + 1;
            _surveyRepository.Update(_survey);
            SurveyViews.ActiveViewIndex = SurveyViews.ActiveViewIndex + 1;
        }

        protected void PayForParkingYesButton_OnServerClick(object sender, EventArgs e)
        {
            _survey.IsPayForParking = true;
            _survey.ParkingPrice = 20;
            _survey.LastStep = SurveyViews.ActiveViewIndex + 1;
            _surveyRepository.Update(_survey);
            SurveyViews.ActiveViewIndex = SurveyViews.ActiveViewIndex + 1;
        }

        protected void NotUseSpaServiceButton_OnServerClick(object sender, EventArgs e)
        {
            _survey.IsBuySpaService = false;
            _survey.SpaServicePrice = 0;
            _survey.LastStep = SurveyViews.ActiveViewIndex + 1;
            _surveyRepository.Update(_survey);
            SurveyViews.ActiveViewIndex = SurveyViews.ActiveViewIndex + 1;
        }

        protected void NotUseAdditionalServiceButton_OnServerClick(object sender, EventArgs e)
        {
            _survey.IsBuyAdditionalService = false;
            _survey.AdditionalServicePrice = 0;
            _survey.LastStep = SurveyViews.ActiveViewIndex + 1;
            _surveyRepository.Update(_survey);
            NextButton.Text = "Done";
            SurveyViews.ActiveViewIndex = SurveyViews.ActiveViewIndex + 1;
        }
    }
}