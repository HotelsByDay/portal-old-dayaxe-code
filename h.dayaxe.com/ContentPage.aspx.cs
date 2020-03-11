using System;
using System.Linq;
using System.Web.UI.WebControls;
using DayaxeDal;
using DayaxeDal.Extensions;
using DayaxeDal.Repositories;
using Newtonsoft.Json;

namespace h.dayaxe.com
{
    public partial class ContentPage : BasePage
    {
        private readonly DalHelper _helper = new DalHelper();
        private readonly MarketRepository _marketRepositoty = new MarketRepository();
        private SiteMaps _siteMaps;
        private HtmlContents _htmlContents;
        protected void Page_Init(object sender, EventArgs e)
        {
            Session["Active"] = "ContentsPage";
            if (!IsPostBack)
            {
                Session["UserHotel"] = null;
                Session["CurrentPage"] = 1;

                if (Request.Params["id"] == null)
                {
                    MarketMultiView.ActiveViewIndex = 0;
                    ContentPageRpt.DataSource = _helper.GetAllSiteMaps();
                    ContentPageRpt.DataBind();
                }
                else
                {
                    MarketMultiView.ActiveViewIndex = 1;
                    int id = int.Parse(Request.Params["id"]);
                    // Add New
                    if (id > 0)
                    {
                        _siteMaps = _helper.GetSiteMapsById(id);
                        if (_siteMaps != null)
                        {
                            string json = JsonConvert.SerializeObject(_siteMaps, CustomSettings.SerializerSettings());
                            Session["SiteMaps"] = json;
                            PageNameText.Text = _siteMaps.Name;
                            UrlSegmentText.Text = _siteMaps.UrlSegment;
                            IsHomePageCheckBox.Checked = _siteMaps.IsHomePage.HasValue && _siteMaps.IsHomePage.Value;
                            IsActiveCheckBox.Checked = _siteMaps.IsActive;

                            _htmlContents = _helper.GetHtmlContentsBySiteMapsId(id);
                            if (_htmlContents != null)
                            {
                                json = JsonConvert.SerializeObject(_htmlContents, CustomSettings.SerializerSettings());
                                Session["HtmlContents"] = json;
                                PageTitleText.Text = _htmlContents.Title;
                                ScriptAnalyticsEditor.Text = _htmlContents.ScriptAnalyticsHeader;
                                ContentHtmlEditor.Text = _htmlContents.Data;
                                BodyClassText.Text = _htmlContents.BodyClass;
                                DescriptionText.Text = _htmlContents.MetaDescription;
                                KeywordText.Text = _htmlContents.MetaKeyword;
                                LandingImageDesktop.ImageUrl = _htmlContents.ImageLandingDesktop;
                                LandingImageMobile.ImageUrl = _htmlContents.ImageLandingMobile;
                            }
                        }
                    }
                    else
                    {
                        IsActiveCheckBox.Checked = true;
                    }
                }
            }
            else
            {
                string sessionSiteMaps = Session["SiteMaps"] != null ? Session["SiteMaps"].ToString() : string.Empty;
                _siteMaps = JsonConvert.DeserializeObject<SiteMaps>(sessionSiteMaps);
                string sessionHtmlContents = Session["HtmlContents"] != null ? Session["HtmlContents"].ToString() : string.Empty;
                _htmlContents = JsonConvert.DeserializeObject<HtmlContents>(sessionHtmlContents);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ContentPageRptOnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
        
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _marketRepositoty.GetAll().Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                ContentPageRpt.DataSource = hotels;
                ContentPageRpt.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _marketRepositoty.GetAll().Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                ContentPageRpt.DataSource = hotels;
                ContentPageRpt.DataBind();
            }
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.ContentPagePage);
        }

        protected void SaveHtmlContentsClick(object sender, EventArgs e)
        {
            LblMessage.Visible = false;
            if (Request.Params["id"] == "0")
            {
                _siteMaps = new SiteMaps
                {
                    Code = string.Empty,
                    IsActive = IsActiveCheckBox.Checked,
                    Name = PageNameText.Text.Trim(),
                    UrlSegment = GetPermalink(PageNameText.Text),
                    IsHomePage = IsHomePageCheckBox.Checked
                };
                _htmlContents = new HtmlContents
                {
                    Title = PageTitleText.Text,
                    Data = ContentHtmlEditor.Text,
                    ScriptAnalyticsHeader = ScriptAnalyticsEditor.Text,
                    BodyClass = BodyClassText.Text,
                    MetaDescription = DescriptionText.Text,
                    MetaKeyword = KeywordText.Text
                };
                int siteMapId;
                try
                {
                    siteMapId = _helper.AddSiteMaps(_siteMaps, _htmlContents);
                }
                catch (Exception ex)
                {
                    LblMessage.Text = ex.Message;
                    LblMessage.Visible = true;
                    return;
                }
                Response.Redirect(string.Format("/ContentPage.aspx?id=" + siteMapId));
            }

            _siteMaps.Name = PageNameText.Text.Trim();
            _siteMaps.UrlSegment = GetPermalink(PageNameText.Text);
            _siteMaps.IsHomePage = IsHomePageCheckBox.Checked;
            _siteMaps.IsActive = IsActiveCheckBox.Checked;

            _htmlContents.Title = PageTitleText.Text;
            _htmlContents.Data = ContentHtmlEditor.Text;
            _htmlContents.ScriptAnalyticsHeader = ScriptAnalyticsEditor.Text;
            _htmlContents.BodyClass = BodyClassText.Text;
            _htmlContents.MetaDescription = DescriptionText.Text;
            _htmlContents.MetaKeyword = KeywordText.Text;

            _helper.UpdateSiteMaps(_siteMaps, _htmlContents);

            _helper.ResetCache();
        }

        private string GetPermalink(string str)
        {
            return str.ToLower().Replace(" ", "-").Trim();
        }

        protected void UploadImageLandingDesktopClick(object sender, EventArgs e)
        {
            if (ImageLandingDesktopFileUpload.HasFile)
            {
                var file = ImageLandingDesktopFileUpload.PostedFile;
                if (Request.Params["id"] != "0")
                {
                    string pathString = Server.MapPath("/HotelImage/landing/");
                    string localImageFile = string.Format("/HotelImage/landing/{0}", file.FileName);

                    if (!System.IO.Directory.Exists(pathString))
                    {
                        System.IO.Directory.CreateDirectory(pathString);
                    }
                    try
                    {
                        file.SaveAs(Server.MapPath(localImageFile));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    _htmlContents.ImageLandingDesktop = localImageFile;
                    _helper.UpdateSiteMaps(_siteMaps, _htmlContents);

                    _helper.ResetCache();

                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
            }
        }

        protected void UploadImageLandingMobileClick(object sender, EventArgs e)
        {
            if (ImageLandingMobileFileUpload.HasFile)
            {
                var file = ImageLandingMobileFileUpload.PostedFile;
                if (Request.Params["id"] != "0")
                {
                    string pathString = Server.MapPath("/HotelImage/landing/");
                    string localImageFile = string.Format("/HotelImage/landing/{0}", file.FileName);

                    if (!System.IO.Directory.Exists(pathString))
                    {
                        System.IO.Directory.CreateDirectory(pathString);
                    }
                    try
                    {
                        file.SaveAs(Server.MapPath(localImageFile));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    _htmlContents.ImageLandingMobile = localImageFile;
                    _helper.UpdateSiteMaps(_siteMaps, _htmlContents);

                    _helper.ResetCache();

                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
            }
        }
    }
}