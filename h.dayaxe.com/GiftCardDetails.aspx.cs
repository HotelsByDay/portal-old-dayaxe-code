using System;
using DayaxeDal;
using DayaxeDal.Repositories;

namespace h.dayaxe.com
{
    public partial class GiftCardDetails : BasePage
    {
        private GiftCards _giftCards;
        private readonly GiftCardRepository _giftCardRepository = new GiftCardRepository();

        protected void Page_Init(object sender, EventArgs e)
        {
            int id = int.Parse(Request.Params["id"]);
            if (id > 0)
            {
                _giftCards = _giftCardRepository.GetById(id);
                if (_giftCards == null)
                {
                    Response.Redirect(Constant.GiftCardListPage);
                }

                NameText.Text = _giftCards.Name;
                CodeText.Text = _giftCards.Code;
                AmountText.Text = string.Format("{0:0.00}", _giftCards.Amount);
            }
            else
            {
                if (!IsPostBack)
                {
                    var code = Helper.RandomString(7);
                    while (_giftCardRepository.IsCodeExists(code))
                    {
                        code = Helper.RandomString(7);
                    }
                    CodeText.Text = code;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelClick(object sender, EventArgs e)
        {
            Response.Redirect(Constant.GiftCardListPage);
        }

        protected void DeleteClick(object sender, EventArgs e)
        {
            int discountId = int.Parse(Request.Params["id"]);
            if (discountId != 0)
            {
                _giftCardRepository.Delete(_giftCards);
                _giftCardRepository.ResetCache();
            }
            Response.Redirect(Constant.GiftCardListPage);
        }

        protected void SaveDiscountClick(object sender, EventArgs e)
        {
            LblMessage.Visible = false;
            LblMessage.Text = "";

            if (string.IsNullOrEmpty(NameText.Text.Trim()))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Name is required";
                return;
            }

            if (string.IsNullOrEmpty(CodeText.Text.Trim()))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Code is required";
                return;
            }

            if (string.IsNullOrEmpty(AmountText.Text.Trim()))
            {
                LblMessage.Visible = true;
                LblMessage.Text = "Amount is required";
                return;
            }

            int gId = int.Parse(Request.Params["id"]);

            double amount;
            double.TryParse(AmountText.Text, out amount);

            if (gId == 0)
            {
                _giftCards = new GiftCards
                {
                    Name = NameText.Text.Trim(),
                    Code = CodeText.Text.ToUpper().Trim(),
                    Amount = amount,
                    IsDelete = false
                };
                try
                {
                    gId = _giftCardRepository.Add(_giftCards);
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
                _giftCards = _giftCardRepository.GetById(gId);
                _giftCards.Name = NameText.Text.Trim();
                _giftCards.Code = CodeText.Text.ToUpper().Trim();
                _giftCards.Amount = amount;

                _giftCardRepository.Update(_giftCards);
            }

            _giftCardRepository.ResetCache();

            Response.Redirect(Constant.GiftCardListPage);
        }
    }
}