using System;
using System.Web.UI.HtmlControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using System.Linq;

namespace h.dayaxe.com
{
    public partial class TargetAudience : BasePageProduct
    {
        private readonly HotelRepository _hotelRepository = new HotelRepository();
        protected string UserName { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "TargetAudienceObject";
            UserName = PublicCustomerInfos != null ? PublicCustomerInfos.EmailAddress : string.Empty;
            HidHotelId.Value = PublicHotel.HotelId.ToString();
            BindTargetGroups();
            BindGender();
            BindEducation();
            BindIncome();
            BindAge();
        
            IncomCurrent.Text = PublicHotel.Income;
            DistanceCurrent.Text = PublicHotel.Distance;
            AgeFromCurrent.Text = PublicHotel.AgeFrom.ToString();
            AgeToCurrent.Text = PublicHotel.AgeTo.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        private void BindTargetGroups()
        {
            var targetSelected = PublicHotel.TargetGroups.Split('|');
            foreach (object targetGroups in Enum.GetValues(typeof(Enums.TargetGroups)))
            {
                Enums.TargetGroups currentTargetGroup = (Enums.TargetGroups)targetGroups;
                var divTag = new HtmlGenericControl("div");
                string objClass = "col-md-2 pointer";
                var isSelected = targetSelected.Contains(((int)currentTargetGroup).ToString());
                if (isSelected)
                {
                    objClass += " alter-target";
                }
                if (targetGroups.ToString().Length <= 10)
                {
                    objClass += " col-single";
                }
                divTag.Attributes.Add("class", objClass);
                divTag.Attributes.Add("data-value", ((int)currentTargetGroup).ToString());
                divTag.InnerHtml = currentTargetGroup.ToDescription();
                divTag.EnableViewState = true;
                TargetAudienceDiv.Controls.Add(divTag);
            }
        }

        private void BindGender()
        {
            var genderSelected = PublicHotel.Gender.Split('|');
            foreach (object gender in Enum.GetValues(typeof(Enums.Gender)))
            {
                Enums.Gender currentGender = (Enums.Gender)gender;
                var divTag = new HtmlGenericControl("div");
                string objClass = "col-md-4 pointer";
                var isSelected = genderSelected.Contains(((int)currentGender).ToString());
                if (isSelected)
                {
                    objClass += " alter-target";
                }
                if (gender.ToString().Length <= 10)
                {
                    objClass += " col-single";
                }
                divTag.Attributes.Add("class", objClass);
                divTag.InnerHtml = currentGender.ToDescription();
                divTag.Attributes.Add("data-value", ((int)currentGender).ToString());
                divTag.EnableViewState = true;
                GenderDiv.Controls.Add(divTag);
            }
        }

        private void BindEducation()
        {
            var educationSelected = PublicHotel.Education.Split('|');
            foreach (object education in Enum.GetValues(typeof(Enums.Education)))
            {
                Enums.Education currentEducation = (Enums.Education)education;
                var divTag = new HtmlGenericControl("div");
                string objClass = "col-md-2 pointer";
                var isSelected = educationSelected.Contains(((int)currentEducation).ToString());
                if (isSelected)
                {
                    objClass += " alter-target";
                }
                if (education.ToString().Length <= 10)
                {
                    objClass += " col-single";
                }
                divTag.Attributes.Add("class", objClass);
                divTag.InnerHtml = currentEducation.ToDescription();
                divTag.Attributes.Add("data-value", ((int)currentEducation).ToString());
                divTag.EnableViewState = true;
                EducationDiv.Controls.Add(divTag);
            }
        }

        private void BindIncome()
        {
            for (var i = 1; i <= 300; i++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor();
                link.HRef = "#";
                link.Attributes.Add("value", i.ToString());
                link.InnerText = i + " mi";
                liTag.Controls.Add(link);
                Distance.Controls.Add(liTag);
            }
            var incomeValue = new[]
            {
                "$ 30,000",
                "$ 40,000",
                "$ 50,000",
                "$ 75,000",
                "$ 100,000",
                "$ 150,000",
                "$ 200,000",
                "$ 250,000",
                "$ 300,000",
                "$ 350,000 +"
            };

            for (var j = 0; j < incomeValue.Length; j++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor();
                link.HRef = "#";
                link.InnerText = incomeValue[j];
                liTag.Controls.Add(link);
                DropdownIncome.Controls.Add(liTag);
            }
        }

        private void BindAge()
        {
            for (var k = 18; k <= 65; k++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor();
                link.HRef = "#";
                link.InnerText = k.ToString();
                liTag.Controls.Add(link);
                AgeFrom.Controls.Add(liTag);
            }
            for (var k = 18; k <= 65; k++)
            {
                var liTag = new HtmlGenericControl("li");
                var link = new HtmlAnchor();
                link.HRef = "#";
                link.InnerText = k.ToString();
                liTag.Controls.Add(link);
                AgeTo.Controls.Add(liTag);
            }
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            saving.InnerText = "Saved!";
            saving.Attributes["class"] = "saving";

            _hotelRepository.ResetCache();
        }
    }
}