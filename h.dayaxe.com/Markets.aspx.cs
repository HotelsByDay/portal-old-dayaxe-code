using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class MarketPage : BasePage
    {
        private readonly HotelRepository _hotelRepository = new HotelRepository();
        private readonly MarketRepository _marketRepositoty = new MarketRepository();
        private readonly MarketHotelRepository _marketHotelRepository = new MarketHotelRepository();
        private Markets _markets;
        protected Cloudinary Cloudinary;

        protected void Page_Init(object sender, EventArgs e)
        {
            Account account = new Account(
                "vietluyen",
                "385557456569739",
                "Si2Q2D3dxjqgya-Rl7-lZ0cy99Q");

            Cloudinary = new Cloudinary(account);

            Session["Active"] = "Markets";
            if (!IsPostBack)
            {
                Session["UserHotel"] = null;
                Session["CurrentPage"] = 1;

                if (Request.Params["id"] == null)
                {
                    MarketMultiView.ActiveViewIndex = 0;
                    MarketRepeater.DataSource = _marketRepositoty.GetAll();
                    MarketRepeater.DataBind();
                }
                else
                {
                    MarketMultiView.ActiveViewIndex = 1;
                    int id = int.Parse(Request.Params["id"]);
                    if (id == 0) // Add new
                    {
                        DdlHotels.Visible = false;
                        AddHotelMarketButton.Visible = false;
                        Deactivebutton.Visible = false;
                        ActiveButton.Visible = false;
                        MarketImage.Visible = false;
                        UploadImage.Visible = false;
                    }
                    else
                    {
                        _markets = _marketRepositoty.GetById(id);
                        if (_markets != null)
                        {
                            LocationNameText.Text = _markets.LocationName;
                            MarketText.Text = _markets.MarketCode;
                            PermalinkText.Text = _markets.Permalink;
                            DdlState.SelectedValue = _markets.State;
                            LatitudeText.Text = _markets.Latitude;
                            LongtitudeText.Text = _markets.Longitude;
                            IsCollectTax.Checked = _markets.IsCalculateTax;

                            if (_markets.IsActive)
                            {
                                Deactivebutton.Visible = true;
                                ActiveButton.Visible = false;
                            }
                            else
                            {
                                Deactivebutton.Visible = false;
                                ActiveButton.Visible = true;
                            }

                            if (!string.IsNullOrEmpty(_markets.PublicId) && !string.IsNullOrEmpty(_markets.Format))
                            {
                                var url = Cloudinary.Api.UrlImgUp.Secure()
                                    .Transform(new Transformation().Width(500).Height(500).Crop("fill"))
                                    .BuildUrl(string.Format("{0}.{1}", _markets.PublicId, _markets.Format));
                                MarketImageControl.ImageUrl = url;
                            }
                        }
                        RebindHotelsByMarket();
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void RebindHotelsByMarket()
        {
            var allHotels = _hotelRepository.SearchHotelsByCode();

            var allHotelsMarkets = _marketHotelRepository.GetAll();
            var hotelExistsInMarkets = allHotels
                .Where(x => allHotelsMarkets.Select(y => y.HotelId).Contains(x.HotelId))
                .ToList();
            int id = int.Parse(Request.Params["id"]);

            var marketHotels = _hotelRepository.SearchHotelsByMarketId(id);
            DdlHotels.DataSource = allHotels.Except(marketHotels).Except(hotelExistsInMarkets);
            DdlHotels.DataTextField = "HotelInfo";
            DdlHotels.DataValueField = "HotelId";
            DdlHotels.DataBind();
        
            RptHotelListings.DataSource = marketHotels;
            RptHotelListings.DataBind();
        }

        protected void MarketRepeaterOnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var currentMarket = (Markets) e.Item.DataItem;
                var litMarkets = (Literal) e.Item.FindControl("HotelsOfMarketLiteral");
                litMarkets.Text = _hotelRepository.SearchHotelsByMarketId(currentMarket.Id).Count().ToString("N0");
            }
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlTableRow)e.Item.FindControl("rowHotel");
                rowHistory.Attributes.Add("class", "alternative");
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                var litPage = (Literal)e.Item.FindControl("LitPage");
                var litTotal = (Literal)e.Item.FindControl("LitTotal");
                var totaluser = _marketRepositoty.GetAll().Count();
                var totalPage = totaluser / Constant.ItemPerPage + (totaluser % Constant.ItemPerPage != 0 ? 1 : 0);
                litPage.Text = string.Format("Page {0} of {1}", Session["CurrentPage"], totalPage);
                litTotal.Text = totaluser + " Users";
            }
        }

        protected void RptHotelListingsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rowHistory = (HtmlGenericControl)e.Item.FindControl("liAlternatie");
                rowHistory.Attributes.Add("class", "alternative");
            }
        }

        protected void AddHotelMarketClick(object sender, EventArgs e)
        {
            int hotelId;
            int.TryParse(DdlHotels.SelectedValue, out hotelId);
            if (hotelId != 0)
            {
                int marketId = int.Parse(Request.Params["id"]);
                var marketHotels = new MarketHotels
                {
                    MarketId = marketId,
                    HotelId = hotelId
                };
                _marketHotelRepository.Add(marketHotels);

                _marketHotelRepository.ResetCache();

                RebindHotelsByMarket();
            }
        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.MarketList);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            int id = int.Parse(Request.Params["id"]);
            if (id != 0)
            {
                _marketRepositoty.Delete(id);

                _marketRepositoty.ResetCache();
            }
            Response.Redirect(Constant.MarketList);
        }

        protected void DeactiveClick(object sender, EventArgs e)
        {
            int id = int.Parse(Request.Params["id"]);
            if (id != 0)
            {
                _markets = _marketRepositoty.GetById(id);
                _markets.IsActive = false;
                _marketRepositoty.Update(_markets);

                _marketRepositoty.ResetCache();
            }

            Deactivebutton.Visible = false;
            ActiveButton.Visible = true;
        }

        protected void ActiveClick(object sender, EventArgs e)
        {
            int id = int.Parse(Request.Params["id"]);
            if (id != 0)
            {
                _markets = _marketRepositoty.GetById(id);
                _markets.IsActive = true;
                _marketRepositoty.Update(_markets);

                _marketRepositoty.ResetCache();
            }

            Deactivebutton.Visible = true;
            ActiveButton.Visible = false;
        }

        protected void SaveHotelMarketClick(object sender, EventArgs e)
        {
            int marketId = int.Parse(Request.Params["id"]);
            if (marketId == 0)
            {
                if (string.IsNullOrEmpty(LocationNameText.Text))
                {
                    LblMessage.Visible = true;
                    LblMessage.Text = "Location Name is required";
                    return;
                }
                _markets = new Markets
                {
                    LocationName = LocationNameText.Text,
                    MarketCode = MarketText.Text,
                    Permalink = GetPermalink(LocationNameText.Text),
                    State = DdlState.SelectedValue,
                    IsActive = true,
                    Latitude = LatitudeText.Text,
                    Longitude = LongtitudeText.Text,
                    IsCalculateTax = IsCollectTax.Checked
                };
                try
                {
                    marketId = _marketRepositoty.Add(_markets);
                }
                catch (Exception ex)
                {
                    LblMessage.Visible = true;
                    LblMessage.Text = ex.Message;
                    return;
                }
            }
            else
            {
                _markets = _marketRepositoty.GetById(marketId);
                _markets.LocationName = LocationNameText.Text;
                _markets.MarketCode = MarketText.Text;
                _markets.Permalink = GetPermalink(LocationNameText.Text);
                _markets.State = DdlState.SelectedValue;
                _markets.Latitude = LatitudeText.Text;
                _markets.Longitude = LongtitudeText.Text;
                _markets.IsCalculateTax = IsCollectTax.Checked;

                _marketRepositoty.Update(_markets);
            }

            _marketRepositoty.ResetCache();

            Response.Redirect(Constant.MarketList + "?id=" + marketId);
        }

        protected void RemoveHotelMarketClick(object sender, EventArgs e)
        {
            if (Request.Params["id"] != "0")
            {
                List<MarketHotels> marketHotelses = new List<MarketHotels>();
                foreach (RepeaterItem item in RptHotelListings.Items)
                {
                    //to get the dropdown of each line
                    CheckBox chkAmentyList = (CheckBox)item.FindControl("ChkRemove");

                    if (chkAmentyList.Checked)
                    {
                        HiddenField hidHotelId = (HiddenField)item.FindControl("HidHotelId");
                        marketHotelses.Add(new MarketHotels
                        {
                            MarketId = int.Parse(Request.Params["id"]),
                            HotelId = int.Parse(hidHotelId.Value)
                        });
                    }
                }
                _marketHotelRepository.Delete(marketHotelses);

                _marketHotelRepository.ResetCache();

                RebindHotelsByMarket();
            }
        }

        protected void Previous_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _marketRepositoty.GetAll().Skip((currentPage - 2) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any() && currentPage - 2 >= 0)
            {
                Session["CurrentPage"] = currentPage - 1;
                RptHotelListings.DataSource = hotels;
                RptHotelListings.DataBind();
            }
        }

        protected void Next_OnClick(object sender, EventArgs e)
        {
            int currentPage = int.Parse(Session["CurrentPage"].ToString());
            var hotels = _marketRepositoty.GetAll().Skip(currentPage * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();
            if (hotels.Any())
            {
                Session["CurrentPage"] = currentPage + 1;
                RptHotelListings.DataSource = hotels;
                RptHotelListings.DataBind();
            }
        }

        private string GetPermalink(string str)
        {
            return str.ToLower().Replace(" ", "-").Trim();
        }

        protected void UploadImageClick(object sender, EventArgs e)
        {
            if (MarketImage.HasFile)
            {
                var file = MarketImage.PostedFile;
                if (Request.Params["id"] != "0")
                {
                    _markets = _marketRepositoty.GetById(int.Parse(Request.Params["id"]));
                    string pathString = Server.MapPath("/HotelImage/markets/");
                    string localImageFile = string.Format("/HotelImage/markets/{0}", file.FileName);

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

                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(string.Format("{0}{1}", pathString, file.FileName)),
                        PublicId = "markets_" + _markets.Id
                    };
                    var uploadResult = Cloudinary.Upload(uploadParams);

                    _markets.ImageUrl = localImageFile;
                    _markets.PublicId = uploadResult.PublicId;
                    _markets.Version = uploadResult.Version;
                    _markets.Format = uploadResult.Format;

                    _marketRepositoty.Update(_markets);

                    _hotelRepository.ResetCache();

                    Response.Redirect(Request.Url.AbsoluteUri, true);
                }
            }

        }
    }
}