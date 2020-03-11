using System;
using System.Globalization;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class EditFeedback : BasePageProduct
    {
        private readonly SurveyRepository _surveyRepository = new SurveyRepository();
        protected Surveys PublicSurveys;

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "CustomerInsight";
            if (Request.Params["id"] != null)
            {
                int id;
                int.TryParse(Request.Params["id"], out id);

                PublicSurveys = _surveyRepository.GetById(id);
            }

            if (!IsPostBack)
            {
                if (PublicSurveys != null)
                {
                    SurveyRating.Text = (PublicSurveys.Rating ?? 4).ToString(CultureInfo.InvariantCulture);
                    CommentText.Text = PublicSurveys.RateCommend;
                    UsePoolCheckBox.Checked = PublicSurveys.UsePool;
                    UseGymCheckBox.Checked = PublicSurveys.UseGym;
                    UseSpaCheckBox.Checked = PublicSurveys.UseSpa;
                    UseBusinessCenterCheckBox.Checked = PublicSurveys.UseBusinessCenter;

                    IsBuyFoodAndDrinkCheckBox.Checked = PublicSurveys.IsBuyFoodAndDrink;
                    FoodAndDrinkPriceTextBox.Text = PublicSurveys.FoodAndDrinkPrice.HasValue
                        ? PublicSurveys.FoodAndDrinkPrice.Value.ToString("0.##")
                        : "";
                    if (!PublicSurveys.IsBuyFoodAndDrink)
                    {
                        FoodAndDrinkPriceTextBox.Attributes["disabled"] = "disabled";
                    }

                    IsPayForParkingCheckBox.Checked = PublicSurveys.IsPayForParking;

                    IsBuySpaServiceCheckBox.Checked = PublicSurveys.IsBuySpaService;
                    BuySpaServicePriceText.Text = PublicSurveys.SpaServicePrice.HasValue
                        ? PublicSurveys.SpaServicePrice.Value.ToString("0.##")
                        : "";
                    if (!PublicSurveys.IsBuySpaService)
                    {
                        BuySpaServicePriceText.Attributes["disabled"] = "disabled";
                    }

                    IsBuyAdditionalServiceCheckBox.Checked = PublicSurveys.IsBuyAdditionalService;
                    BuyAdditionalServicePriceText.Text = PublicSurveys.AdditionalServicePrice.HasValue
                        ? PublicSurveys.AdditionalServicePrice.Value.ToString("0.##")
                        : "";
                    if (!PublicSurveys.IsBuyAdditionalService)
                    {
                        BuyAdditionalServicePriceText.Attributes["disabled"] = "disabled";
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.FeedbackList);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            _surveyRepository.Delete(PublicSurveys);

            _surveyRepository.ResetCache();

            Response.Redirect(Constant.FeedbackList);
        }

        protected void SaveClick(object sender, EventArgs e)
        {
            double rating;
            double.TryParse(SurveyRating.Text, out rating);
            PublicSurveys.Rating = rating;

            PublicSurveys.RateCommend = CommentText.Text;
            PublicSurveys.UsePool = UsePoolCheckBox.Checked;
            PublicSurveys.UseGym = UseGymCheckBox.Checked;
            PublicSurveys.UseSpa = UseSpaCheckBox.Checked;
            PublicSurveys.UseBusinessCenter = UseBusinessCenterCheckBox.Checked;

            PublicSurveys.IsBuyFoodAndDrink = IsBuyFoodAndDrinkCheckBox.Checked;
            if (IsBuyFoodAndDrinkCheckBox.Checked)
            {
                double foodandDrinkPrice;
                if (double.TryParse(FoodAndDrinkPriceTextBox.Text, out foodandDrinkPrice))
                {
                    PublicSurveys.FoodAndDrinkPrice = foodandDrinkPrice;
                }
                else
                {
                    ErrorMessage.Text = "Food and Drink is required.";
                    return;
                }
            }
            else
            {
                PublicSurveys.FoodAndDrinkPrice = null;
                FoodAndDrinkPriceTextBox.Text = string.Empty;
            }

            PublicSurveys.IsPayForParking = IsPayForParkingCheckBox.Checked;

            PublicSurveys.IsBuySpaService = IsBuySpaServiceCheckBox.Checked;
            if (IsBuySpaServiceCheckBox.Checked)
            {
                double servicePrice;
                if (double.TryParse(BuySpaServicePriceText.Text, out servicePrice))
                {
                    PublicSurveys.SpaServicePrice = servicePrice;
                }
                else
                {
                    ErrorMessage.Text = "Spa Service Price is required.";
                    return;
                }
            }
            else
            {
                PublicSurveys.SpaServicePrice = null;
                BuySpaServicePriceText.Text = string.Empty;
            }

            PublicSurveys.IsBuyAdditionalService = IsBuyAdditionalServiceCheckBox.Checked;
            if (IsBuyAdditionalServiceCheckBox.Checked)
            {
                double additionalPrice;
                if (double.TryParse(BuyAdditionalServicePriceText.Text, out additionalPrice))
                {
                    PublicSurveys.AdditionalServicePrice = additionalPrice;
                }
                else
                {
                    ErrorMessage.Text = "Additional Price is required.";
                    return;
                }
            }
            else
            {
                PublicSurveys.AdditionalServicePrice = null;
                BuyAdditionalServicePriceText.Text = string.Empty;
            }

            _surveyRepository.Update(PublicSurveys);
            _surveyRepository.ResetCache();
            ErrorMessage.Text = "Review successfully updated";
        }
    }
}