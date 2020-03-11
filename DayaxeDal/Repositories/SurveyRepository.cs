using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace DayaxeDal.Repositories
{
    public class SurveyRepository:BaseRepository, IRepository<Surveys>
    {
        public IEnumerable<Surveys> Get(Func<Surveys, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Surveys entity)
        {
            DayaxeDbContext.Surveys.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Update(Surveys surveys)
        {
            using (var transaction = new TransactionScope())
            {
                var surveyUpdate = DayaxeDbContext.Surveys.FirstOrDefault(x => x.Id == surveys.Id);
                if (surveyUpdate != null)
                {
                    surveyUpdate.Rating = surveys.Rating;
                    surveyUpdate.RateCommend = surveys.RateCommend;

                    surveyUpdate.UsePool = surveys.UsePool;
                    surveyUpdate.UseGym = surveys.UseGym;
                    surveyUpdate.UseSpa = surveys.UseSpa;
                    surveyUpdate.UseBusinessCenter = surveys.UseBusinessCenter;

                    surveyUpdate.IsBuyFoodAndDrink = surveys.IsBuyFoodAndDrink;
                    surveyUpdate.FoodAndDrinkPrice = surveys.FoodAndDrinkPrice;

                    surveyUpdate.IsPayForParking = surveys.IsPayForParking;
                    surveyUpdate.ParkingPrice = surveys.ParkingPrice;

                    surveyUpdate.IsBuySpaService = surveys.IsBuySpaService;
                    surveyUpdate.SpaServicePrice = surveys.SpaServicePrice;

                    surveyUpdate.IsBuyAdditionalService = surveys.IsBuyAdditionalService;
                    surveyUpdate.AdditionalServicePrice = surveys.AdditionalServicePrice;
                    surveyUpdate.IsFinish = surveys.IsFinish;

                    surveyUpdate.LastStep = surveys.LastStep;
                    surveyUpdate.LastUpdatedDate = surveys.LastUpdatedDate;

                    surveyUpdate.IsDelete = surveys.IsDelete;

                    // && surveys.Rating.HasValue && surveys.Rating.Value < 5
                    if (surveys.IsFinish)
                    {
                        var exists = DayaxeDbContext.Schedules
                            .Count(s => s.BookingId == surveys.BookingId &&
                                s.ScheduleSendType == (int) Enums.ScheduleSendType.IsNotifiedSurvey);

                        if (exists <= 0)
                        {
                            var schedules = new Schedules
                            {
                                Name = "Email Notified Survey",
                                ScheduleSendType = (int) Enums.ScheduleSendType.IsNotifiedSurvey,
                                Status = (int) Enums.ScheduleType.NotRun,
                                BookingId = surveys.BookingId
                            };
                            DayaxeDbContext.Schedules.InsertOnSubmit(schedules);
                        }
                    }

                    Commit();
                }
                transaction.Complete();
            }
        }

        public void Delete(Surveys entity)
        {
            var surveys = DayaxeDbContext.Surveys.SingleOrDefault(x => x.Id == entity.Id);
            if (surveys != null)
            {
                surveys.IsDelete = true;
            }
            Commit();
        }

        public void Delete(Func<Surveys, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Surveys GetById(long id)
        {
            var survey = GetAll().FirstOrDefault(h => h.Id == id);

            if (survey != null)
            {
                var customerId = BookingList.First(x => x.BookingId == survey.BookingId).CustomerId;
                var hotels = (from h in HotelList
                    join p in ProductList on h.HotelId equals p.HotelId
                    join b in BookingList on p.ProductId equals b.ProductId
                    where b.BookingId == survey.BookingId
                    select h).FirstOrDefault();
                var products = (from p in ProductList
                    join b in BookingList on p.ProductId equals b.ProductId
                    where b.BookingId == survey.BookingId
                    select p).FirstOrDefault();
                var bookings = BookingList.FirstOrDefault(b => b.BookingId == survey.BookingId);

                survey.TicketPurchased = BookingList.Where(b => b.CustomerId == customerId).Sum(b => b.Quantity);
                survey.HotelName = hotels != null ? hotels.HotelName : string.Empty;
                survey.City = hotels != null ? hotels.City : string.Empty;
                survey.ProductName = products != null ? products.ProductName : string.Empty;
                survey.CheckInDate = bookings != null ? bookings.CheckinDate : null;
                survey.PerTicketPrice = bookings != null ? bookings.HotelPrice : 0;
                survey.TotalPrice = bookings != null ? bookings.TotalPrice : 0;
            }

            return survey;
        }

        public IEnumerable<Surveys> GetAll()
        {
            var entities = CacheLayer.Get<List<Surveys>>(CacheKeys.SurveysCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Surveys.ToList();
            CacheLayer.Add(entities, CacheKeys.SurveysCacheKey);
            return entities.AsEnumerable();
        }

        public Surveys Refresh(Surveys entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public Surveys GetSurveysByCode(string code)
        {
            var surveys = GetAll().FirstOrDefault(x => x.Code == code);
            if (surveys != null)
            {
                var booking = BookingList.FirstOrDefault(b => b.BookingId == surveys.BookingId);
                if (booking != null)
                {
                    var product = ProductList.FirstOrDefault(p => p.ProductId == booking.ProductId);
                    surveys.RedeemedDate = booking.RedeemedDate;
                    surveys.HotelPrice = booking.HotelPrice;
                    if (product != null)
                    {
                        var hotel = HotelList.FirstOrDefault(h => h.HotelId == product.HotelId);
                        string imageUrl = Constant.ImageDefault;
                        var image = ProductImageList.FirstOrDefault(x => x.ProductId == product.ProductId && x.IsCover && x.IsActive);
                        if (image != null)
                        {
                            imageUrl = image.Url;
                        }
                        surveys.ImageUrl = imageUrl;
                        if (hotel != null)
                        {
                            surveys.HotelInfo = string.Format("{0} at {1}<br/> {2}, {3}",
                                product.ProductName,
                                hotel.HotelName,
                                hotel.Neighborhood,
                                hotel.City);
                        }
                    }
                }
            }
            return surveys;
        }

        public ListResult<Surveys> SearchSurveys(int hotelId, DateTime? startDate, DateTime? endDate, int pageNumber = 1)
        {
            var result = new ListResult<Surveys>();
            var surveys = (from p in SurveyList
                join p1 in BookingList on p.BookingId equals p1.BookingId
                join p2 in ProductList on p1.ProductId equals p2.ProductId
                join p3 in HotelList on p2.HotelId equals p3.HotelId
                where p3.HotelId == hotelId && p.IsFinish && !p.IsDelete
                orderby p1.RedeemedDate descending 
                select p).ToList();

            // Assign CheckInDate
            if (surveys.Any())
            {
                surveys.ForEach(item =>
                {
                    var bookings = BookingList.FirstOrDefault(b => b.BookingId == item.BookingId);
                    if (bookings != null)
                    {
                        item.CheckInDate = bookings.CheckinDate;
                    }
                });
            }

            if (startDate.HasValue)
            {
                surveys =
                    surveys.Where(x => x.CheckInDate.HasValue && x.CheckInDate.Value.Date >= startDate.Value.Date).ToList();
            }

            if (endDate.HasValue)
            {
                surveys =
                    surveys.Where(x => x.CheckInDate.HasValue && x.CheckInDate.Value.Date <= endDate.Value.Date).ToList();
            }

            result.TotalRecords = surveys.Count();

            surveys = pageNumber <= 1 ? surveys.Take(Constant.ItemPerPage).ToList() : surveys.Skip((pageNumber - 1) * Constant.ItemPerPage).Take(Constant.ItemPerPage).ToList();

            if (surveys.Any())
            {
                surveys.ForEach(item =>
                {
                    double spend = 0;
                    if (item.IsBuyFoodAndDrink && item.FoodAndDrinkPrice.HasValue)
                    {
                        spend += item.FoodAndDrinkPrice.Value;
                    }

                    if (item.IsPayForParking)
                    {
                        spend += Constant.ParkingPrice;
                    }

                    if (item.IsBuySpaService && item.SpaServicePrice.HasValue)
                    {
                        spend += item.SpaServicePrice.Value;
                    }

                    if (item.IsBuyAdditionalService && item.AdditionalServicePrice.HasValue)
                    {
                        spend += item.AdditionalServicePrice.Value;
                    }

                    item.EstSpend = spend;
                    item.RedeemedDate = item.Bookings.RedeemedDate;
                });
            }

            result.Items = surveys.ToList();

            return result;
        }

        public IEnumerable<Surveys> GetByHotelId(long hotelId)
        {
            return (from s in SurveyList
                join b in BookingList on s.BookingId equals b.BookingId
                join p in ProductList on b.ProductId equals p.ProductId
                join h in HotelList on p.HotelId equals h.HotelId
                where p.HotelId == hotelId && s.IsFinish &&
                      b.PassStatus == (int)Enums.BookingStatus.Redeemed &&
                      !s.IsDelete &&
                      h.IsActive && !h.IsDelete && h.IsPublished
                orderby b.RedeemedDate descending
                select s);
        }

        public IEnumerable<Surveys> GetAllReviews()
        {
            return (from s in SurveyList
                join b in BookingList on s.BookingId equals b.BookingId
                join p in ProductList on b.ProductId equals p.ProductId
                join h in HotelList on p.HotelId equals h.HotelId
                    where s.IsFinish &&
                        b.PassStatus == (int)Enums.BookingStatus.Redeemed &&
                        !s.IsDelete &&
                        h.IsActive && !h.IsDelete && h.IsPublished
                    orderby b.RedeemedDate descending
                select s);
        }

        public Hotels GetHotelBySurveyId(int id)
        {
            return (from h in HotelList
                join p in ProductList on h.HotelId equals p.HotelId
                join b in BookingList on p.ProductId equals b.ProductId
                join s in SurveyList on b.BookingId equals s.BookingId
                where s.Id == id
                select h).FirstOrDefault();
        }

        #endregion
    }
}
