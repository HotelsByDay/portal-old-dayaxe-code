using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DayaxeDal.Custom;
using DayaxeDal.Extensions;

namespace DayaxeDal.Repositories
{
    public class HotelRepository: BaseRepository, IRepository<Hotels>
    {
        public IEnumerable<Hotels> Get(Func<Hotels, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(Hotels entity)
        {
            DayaxeDbContext.Hotels.InsertOnSubmit(entity);
            Commit();
            return entity.HotelId;
        }

        public void Update(Hotels hotel)
        {
            var hotelUpdate = DayaxeDbContext.Hotels.SingleOrDefault(x => x.HotelId == hotel.HotelId);
            if (hotelUpdate != null)
            {
                hotelUpdate.IsActive = hotel.IsActive;
                hotelUpdate.IsDelete = hotel.IsDelete;

                hotelUpdate.IsComingSoon = hotel.IsComingSoon;
                hotelUpdate.IsPublished = hotel.IsPublished;

                hotelUpdate.HotelName = hotel.HotelName;
                hotelUpdate.Neighborhood = hotel.Neighborhood;
                hotelUpdate.TripAdvisorRating = hotel.TripAdvisorRating;
                hotelUpdate.HoteltypeId = hotel.HoteltypeId;
                hotelUpdate.HotelPinCode = hotel.HotelPinCode;
                hotelUpdate.HotelDiscountCode = hotel.HotelDiscountCode;
                hotelUpdate.HotelDiscountPercent = hotel.HotelDiscountPercent;
                hotelUpdate.HotelDiscountDisclaimer = hotel.HotelDiscountDisclaimer;
                hotelUpdate.HotelParking = hotel.HotelParking;
                hotelUpdate.StreetAddress = hotel.StreetAddress;
                hotelUpdate.City = hotel.City;
                hotelUpdate.State = hotel.State;
                hotelUpdate.ZipCode = hotel.ZipCode;
                hotelUpdate.PhoneNumber = hotel.PhoneNumber;
                hotelUpdate.GeneralHours = hotel.GeneralHours;
                hotelUpdate.Recommendation = hotel.Recommendation;
                hotelUpdate.Order = hotel.Order;
                hotelUpdate.TargetGroups = hotel.TargetGroups;
                hotelUpdate.Gender = hotel.Gender;
                hotelUpdate.Education = hotel.Education;
                hotelUpdate.Income = hotel.Income;
                hotelUpdate.Distance = hotel.Distance;
                hotelUpdate.AgeFrom = hotel.AgeFrom;
                hotelUpdate.AgeTo = hotel.AgeTo;
                hotelUpdate.BookingConfirmationEmail = hotel.BookingConfirmationEmail;
                if (!string.IsNullOrWhiteSpace(hotel.BookingConfirmationCabanaEmail))
                {
                    hotelUpdate.BookingConfirmationCabanaEmail = hotel.BookingConfirmationCabanaEmail;
                }
                if (!string.IsNullOrWhiteSpace(hotel.BookingConfirmationSpaEmail))
                {
                    hotelUpdate.BookingConfirmationSpaEmail = hotel.BookingConfirmationSpaEmail;
                }
                hotelUpdate.Latitude = hotel.Latitude;
                hotelUpdate.Longitude = hotel.Longitude;
                hotelUpdate.CheckInPlace = hotel.CheckInPlace;
                hotelUpdate.DailyPassSale = hotel.DailyPassSale;
                hotelUpdate.PassCapacity = hotel.PassCapacity;
                hotelUpdate.ReportSubscribers = hotel.ReportSubscribers;

                hotelUpdate.TimeZoneId = hotel.TimeZoneId;
                hotelUpdate.TimeZoneOffset = hotel.TimeZoneOffset;

                hotelUpdate.TripAdvisorScript = hotel.TripAdvisorScript;
                Commit();
            }
        }

        public void Update(List<Hotels> entities)
        {
            entities.ForEach(entity =>
            {
                var hotelUpdate = DayaxeDbContext.Hotels.SingleOrDefault(x => x.HotelId == entity.HotelId);
                if (hotelUpdate != null && !string.IsNullOrEmpty(entity.TimeZoneId))
                {
                    hotelUpdate.TimeZoneId = entity.TimeZoneId;
                    hotelUpdate.TimeZoneOffset = entity.TimeZoneOffset;
                }
                Commit();
            });
        }

        public void Delete(Hotels entity)
        {
            var hotels = DayaxeDbContext.Hotels.FirstOrDefault(x => x.HotelId == entity.HotelId);
            if (hotels != null)
            {
                hotels.IsDelete = true;
                Commit();
            }
        }

        public void Delete(Func<Hotels, bool> predicate)
        {
            IEnumerable<Hotels> listHotels = DayaxeDbContext.Hotels.Where(predicate).AsEnumerable();
            listHotels.ToList().ForEach(hotels =>
            {
                hotels.IsDelete = true;
            });
            Commit();
        }

        public Hotels GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.HotelId == id);
        }

        public IEnumerable<Hotels> GetAll()
        {
            var entities = CacheLayer.Get<List<Hotels>>(CacheKeys.HotelsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.Hotels.ToList();
            SetTotalTickets(ref entities);
            CacheLayer.Add(entities, CacheKeys.HotelsCacheKey);
            return entities.AsEnumerable();
        }

        public Hotels Refresh(Hotels entity)
        {
            return DayaxeDbContext.Hotels.FirstOrDefault(x => x.HotelId == entity.HotelId);
        }

        #region Custom

        public DailyPassLimit GetDailyPassLimit(int hotelId)
        {
            var hotels = GetAll().FirstOrDefault(h => h.HotelId == hotelId);
            if (hotels == null)
            {
                return new DailyPassLimit
                {
                    HotelId = hotelId
                };
            }
            var products = ProductList.Where(p => p.HotelId == hotelId && p.IsActive && !p.IsDelete).ToList();
            var passLimit = new DailyPassLimit
            {
                HotelId = hotelId,
                DailyPass = products.Where(h => h.ProductType == (int)Enums.ProductType.DayPass).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0),
                CabanaPass = products.Where(h => h.ProductType == (int)Enums.ProductType.Cabana).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0),
                SpaPass = products.Where(h => h.ProductType == (int)Enums.ProductType.SpaPass).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0),
                DaybedPass = products.Where(h => h.ProductType == (int)Enums.ProductType.Daybed).DefaultIfEmpty().Max(h => h != null ? h.PassCapacity : 0)
            };
            return passLimit;
        }

        public void UpdateDailyPassLimit(List<Products> listProducts, string destinationTimeZoneId)
        {
            var updateQuantityObjects = new List<UpdateQuantityObject>();
            listProducts.ForEach(products =>
            {
                var updateProducts = DayaxeDbContext.Products.FirstOrDefault(p => p.ProductId == products.ProductId);
                if (updateProducts != null)
                {
                    var defaultPrice = Mapper.Map<Products, DefaultPrices>(updateProducts);
                    defaultPrice.UpdatedDate = DateTime.UtcNow;
                    defaultPrice.UpdatedBy = 1;
                    defaultPrice.EffectiveDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(destinationTimeZoneId).AddDays(1).Date;

                    // Delete old records if edit on same day
                    DeleteDefaultPrices(products.ProductId, DateTime.UtcNow);

                    if (updateProducts.PassCapacityMon < products.PassCapacityMon)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }

                    if (updateProducts.PassCapacityTue < products.PassCapacityTue)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }

                    if (updateProducts.PassCapacityWed < products.PassCapacityWed)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }

                    if (updateProducts.PassCapacityThu < products.PassCapacityThu)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }

                    if (updateProducts.PassCapacityFri < products.PassCapacityFri)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }

                    if (updateProducts.PassCapacitySat < products.PassCapacitySat)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }

                    if (updateProducts.PassCapacitySun < products.PassCapacitySun)
                    {
                        updateQuantityObjects.Add(new UpdateQuantityObject
                        {
                            ProductId = products.ProductId,
                            DayType = Enums.DayType.Monday
                        });
                    }
                    
                    updateProducts.PriceMon = products.PriceMon;
                    updateProducts.PriceTue = products.PriceTue;
                    updateProducts.PriceWed = products.PriceWed;
                    updateProducts.PriceThu = products.PriceThu;
                    updateProducts.PriceFri = products.PriceFri;
                    updateProducts.PriceSat = products.PriceSat;
                    updateProducts.PriceSun = products.PriceSun;

                    updateProducts.PassCapacity = products.PassCapacity;
                    updateProducts.PassCapacityMon = products.PassCapacityMon;
                    updateProducts.PassCapacityTue = products.PassCapacityTue;
                    updateProducts.PassCapacityWed = products.PassCapacityWed;
                    updateProducts.PassCapacityThu = products.PassCapacityThu;
                    updateProducts.PassCapacityFri = products.PassCapacityFri;
                    updateProducts.PassCapacitySat = products.PassCapacitySat;
                    updateProducts.PassCapacitySun = products.PassCapacitySun;

                    // Other type of products != Add-Ons

                    defaultPrice.PriceMon = products.PriceMon;
                    defaultPrice.PriceTue = products.PriceTue;
                    defaultPrice.PriceWed = products.PriceWed;
                    defaultPrice.PriceThu = products.PriceThu;
                    defaultPrice.PriceFri = products.PriceFri;
                    defaultPrice.PriceSat = products.PriceSat;
                    defaultPrice.PriceSun = products.PriceSun;

                    defaultPrice.PassCapacityMon = products.PassCapacityMon;
                    defaultPrice.PassCapacityTue = products.PassCapacityTue;
                    defaultPrice.PassCapacityWed = products.PassCapacityWed;
                    defaultPrice.PassCapacityThu = products.PassCapacityThu;
                    defaultPrice.PassCapacityFri = products.PassCapacityFri;
                    defaultPrice.PassCapacitySat = products.PassCapacitySat;
                    defaultPrice.PassCapacitySun = products.PassCapacitySun;
                    
                    // Add-Ons
                    switch (updateProducts.ProductType)
                    {
                        case (int) Enums.ProductType.DayPass:
                            defaultPrice.DailyDayPass = products.PassCapacity;
                            break;
                        case (int) Enums.ProductType.Cabana:
                            defaultPrice.DailyCabana = products.PassCapacity;
                            break;
                        case (int) Enums.ProductType.SpaPass:
                            defaultPrice.DailySpa = products.PassCapacity;
                            break;
                        case (int) Enums.ProductType.Daybed:
                            defaultPrice.DailyDaybed = products.PassCapacity;
                            break;
                    }

                    DayaxeDbContext.DefaultPrices.InsertOnSubmit(defaultPrice);

                    // Add Default Price
                    if (products.IsUpdateDefaultPrice)
                    {
                        defaultPrice.EffectiveDate = DateTime.UtcNow.ToLosAngerlesTimeWithTimeZone(destinationTimeZoneId).Date;
                        switch (products.ProductType)
                        {
                            case (int)Enums.ProductType.Cabana:
                                defaultPrice.DailyCabana = products.PassCapacity;
                                break;
                            case (int)Enums.ProductType.SpaPass:
                                defaultPrice.DailySpa = products.PassCapacity;
                                break;
                            case (int)Enums.ProductType.Daybed:
                                defaultPrice.DailyDaybed = products.PassCapacity;
                                break;
                            default:
                                defaultPrice.DailyDayPass = products.PassCapacity;
                                break;
                        }

                        DayaxeDbContext.DefaultPrices.InsertOnSubmit(defaultPrice);
                    }
                }

                Commit();
            });

            if (updateQuantityObjects.Any())
            {
                AddEmailToProductWaitingLists(updateQuantityObjects, destinationTimeZoneId);
            }
        }

        public List<Hotels> SearchHotelsByCode(bool isPublish = false, string code = "", int customerId = 0)
        {
            var hotels = GetAll().Where(x => !x.IsDelete).OrderByDescending(x => x.Order).ToList();

            // 
            if (!string.IsNullOrEmpty(code))
            {
                hotels = hotels.Where(x => x.IsActive).ToList();
            }

            // Has user login and admin
            if (customerId > 0)
            {
                var user = CustomerInfoList.FirstOrDefault(x => x.CustomerId == customerId && x.IsAdmin);
                if (user != null)
                {
                    return hotels.Where(x => x.IsActive).ToList();
                }
            }

            // Publish use on client
            if (isPublish)
            {
                hotels = hotels.Where(x => x.IsPublished && x.IsActive).ToList();
            }

            return hotels.OrderBy(h => h.Order).ToList();
        }

        public Hotels GetHotel(int id, string userName)
        {
            var users = CustomerInfoList.FirstOrDefault(x => x.EmailAddress == userName);

            Hotels hotels;
            if (users != null && users.IsSuperAdmin)
            {
                hotels = GetAll().FirstOrDefault(x => x.HotelId == id);
            }
            else
            {
                hotels = (from p in HotelList
                          join p1 in UserHotelList on p.HotelId equals p1.HotelId
                          join p2 in CustomerInfoList on p1.CustomerId equals p2.CustomerId
                          where p.HotelId == id && p2.EmailAddress == userName && p2.IsAdmin
                          select p).FirstOrDefault();
            }

            if (hotels != null)
            {
                SetHotelData(ref hotels, hotels.HotelId);
            }
            return hotels;
        }

        public int GetHotelOrder()
        {
            var hotels = GetAll().Where(x => x.Order.HasValue).ToList();
            var order = hotels.Any() ? hotels.Where(x => x.Order.HasValue).Max(x => x.Order.Value) : 1;
            if (order > 0)
            {
                return order;
            }
            return GetAll().Count();
        }

        public void Delete(int hotelId)
        {
            var hotels = DayaxeDbContext.Hotels.FirstOrDefault(x => x.HotelId == hotelId);
            if (hotels != null)
            {
                hotels.IsDelete = true;
            }
            Commit();
        }

        public List<Hotels> SearchHotelsByUser(string userName)
        {
            var users = CustomerInfoList.FirstOrDefault(x => x.EmailAddress == userName);
            if (users != null && users.IsSuperAdmin)
            {
                return (from p in HotelList where !p.IsDelete orderby p.Order descending select p).ToList();
            }

            var hotels = from p in HotelList
                         join p1 in UserHotelList on p.HotelId equals p1.HotelId
                         join p2 in CustomerInfoList on p1.CustomerId equals p2.CustomerId
                         where p2.EmailAddress == userName && !p.IsDelete && p2.IsAdmin
                         orderby p.Order
                         select p;

            return hotels.ToList();
        }

        public List<Hotels> SearchHotelsByUserId(int userId)
        {
            var users = CustomerInfoList.FirstOrDefault(x => x.CustomerId == userId);
            if (users != null && users.IsSuperAdmin)
            {
                return (from p in HotelList where !p.IsDelete orderby p.Order descending select p).ToList();
            }

            var hotels = from p in HotelList
                         join p1 in UserHotelList on p.HotelId equals p1.HotelId
                         join p2 in CustomerInfoList on p1.CustomerId equals p2.CustomerId
                         where p2.CustomerId == userId && !p.IsDelete && p2.IsAdmin
                         orderby p.Order
                         select p;

            return hotels.ToList();
        }

        public IEnumerable<Hotels> SearchHotelsByMarketId(int id)
        {
            return (from p in HotelList
                    join p1 in MarketHotelList
                        on p.HotelId equals p1.HotelId
                    where p1.MarketId == id
                    select p);
        }

        #endregion

        #region Policies

        public void UpdatePolicies(Policies entity)
        {
            var hotelUpdate = DayaxeDbContext.Policies.SingleOrDefault(x => x.Id == entity.Id);
            if (hotelUpdate != null)
            {
                hotelUpdate.Name = entity.Name;
                Commit();
            }
        }

        public int AddPolicies(Policies entity)
        {
            DayaxeDbContext.Policies.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void DeletePolicies(List<long> ids)
        {
            var policies = DayaxeDbContext.Policies.Where(p => ids.Contains(p.Id)).ToList();
            policies.ForEach(policy =>
            {
                policy.IsDelete = true;
            });

            var hotelPolicies = DayaxeDbContext.HotelPolicies.Where(hp => ids.Contains(hp.PolicyId)).ToList();
            DayaxeDbContext.HotelPolicies.DeleteAllOnSubmit(hotelPolicies);

            Commit();
        }

        public IEnumerable<HotelPolicies> GetAllHotelPolices(int hotelId)
        {
            return GetAllHotelPolicies()
                .Where(hp => hp.HotelId == hotelId)
                .OrderBy(hp => hp.Order);
        }

        public void AddHotelPolicies(HotelPolicies entity)
        {
            DayaxeDbContext.HotelPolicies.InsertOnSubmit(entity);
            Commit();
        }

        public void AddHotelPolicies(IEnumerable<HotelPolicies> entities)
        {
            DayaxeDbContext.HotelPolicies.InsertAllOnSubmit(entities);
            Commit();
        }

        public void DeleteHotelPolicies(List<long> ids, int hotelId)
        {
            var policies = DayaxeDbContext.HotelPolicies.Where(p => ids.Contains(p.PolicyId) && p.HotelId == hotelId).ToList();
            DayaxeDbContext.HotelPolicies.DeleteAllOnSubmit(policies);
            Commit();
        }

        public void UpdateHotelPolicies(List<HotelPolicies> entities, long hotelId)
        {
            var policies = DayaxeDbContext.HotelPolicies.Where(p => entities.Select(e => e.PolicyId).Contains(p.PolicyId) && p.HotelId == hotelId).ToList();
            if (policies.Any())
            {
                policies.ForEach(policy =>
                {
                    policy.Order = entities.First(e => e.HotelId == policy.HotelId && e.PolicyId == policy.PolicyId)
                        .Order;
                });
                Commit();
            }
        }

        public HotelPolicies GetHotelPolicesById(long policyId, int hotelId)
        {
            return GetAllHotelPolices(hotelId).FirstOrDefault(p => p.PolicyId == policyId);
        }

        public Policies GetPolicesById(long id)
        {
            return GetAllPolices().FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Policies> GetAllPolices()
        {
            var entities = CacheLayer.Get<List<Policies>>(CacheKeys.PoliciesCacheKey);
            if (entities != null)
            {
                return entities.Where(p => !p.IsDelete).AsEnumerable();
            }
            entities = DayaxeDbContext.Policies.Where(p => !p.IsDelete).ToList();
            CacheLayer.Add(entities, CacheKeys.PoliciesCacheKey);
            return entities.AsEnumerable();
        }

        public IEnumerable<HotelPolicies> GetAllHotelPolicies()
        {
            var entities = CacheLayer.Get<List<HotelPolicies>>(CacheKeys.HotelPoliciesCacheKey);
            if (entities != null)
            {
                entities.ForEach(entity =>
                {
                    entity.Name = GetPolicesById(entity.PolicyId).Name;
                });
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.HotelPolicies.ToList();
            entities.ForEach(entity =>
            {
                entity.Name = GetPolicesById(entity.PolicyId).Name;
            });
            CacheLayer.Add(entities, CacheKeys.HotelPoliciesCacheKey);
            return entities.AsEnumerable();
        }

        #endregion
    }
}
