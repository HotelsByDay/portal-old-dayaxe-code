using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Transactions;
using DayaxeDal.Extensions;

namespace DayaxeDal.Repositories
{
    public class CustomerCreditRepository : BaseRepository, IRepository<CustomerCredits>
    {
        public IEnumerable<CustomerCredits> Get(Func<CustomerCredits, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(CustomerCredits entity)
        {
            DayaxeDbContext.CustomerCredits.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public double AddReferral(CustomerCredits entity, CustomerCredits referralCustomer)
        {
            using (var transaction = new TransactionScope())
            {
                var currentCustomer = DayaxeDbContext.CustomerCredits.FirstOrDefault(cc => cc.Id == entity.Id);
                var referCustomer = DayaxeDbContext.CustomerCredits.FirstOrDefault(cc => cc.Id == referralCustomer.Id);
                if (currentCustomer != null && currentCustomer.ReferralCustomerId != 0)
                {
                    throw new Exception(ErrorMessage.CanNotReferralTwice);
                }

                if (currentCustomer != null && referCustomer != null)
                {
                    currentCustomer.ReferralCustomerId = referCustomer.CustomerId;

                    var logs = new CustomerCreditLogs
                    {
                        CreatedDate = DateTime.UtcNow,
                        CreditType = (int) Enums.CreditType.GiftCard,
                        CustomerId = currentCustomer.CustomerId,
                        ReferralId = referCustomer.CustomerId,
                        Amount = referCustomer.FirstRewardForReferral,
                        CreatedBy = currentCustomer.CustomerId,
                        Description = string.Format("{0} - {{0}} - {1}",
                            Enums.CreditType.Referral.ToDescription(),
                            referCustomer.ReferralCode),
                        BookingId = 0,
                        Status = false, // Pending
                        GiftCardId = 0
                    };

                    var logReferrals = new CustomerCreditLogs
                    {
                        CreatedDate = DateTime.UtcNow,
                        CreditType = (int)Enums.CreditType.Referral,
                        CustomerId = referCustomer.CustomerId,
                        ReferralId = currentCustomer.CustomerId,
                        Amount = referCustomer.FirstRewardForOwner,
                        CreatedBy = currentCustomer.CustomerId,
                        Description = string.Format("{0} - {{0}} - {1}", 
                            Enums.CreditType.Referral.ToDescription(),
                            referCustomer.ReferralCode),
                        BookingId = 0,
                        Status = false, // Pending
                        GiftCardId = 0
                    };

                    // Referred Customer has bookings
                    if (DayaxeDbContext.Bookings.Any(b => b.CustomerId == currentCustomer.CustomerId))
                    {
                        referCustomer.Amount += referCustomer.FirstRewardForOwner;
                        logReferrals.Status = true; // Complete
                    }

                    // Current Customer have bookings so referrer can use credit
                    if (DayaxeDbContext.Bookings.Any(b => b.CustomerId == referCustomer.CustomerId))
                    {
                        currentCustomer.Amount += referralCustomer.FirstRewardForReferral;
                        logs.Status = true; // Complete
                    }

                    DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(logs);
                    DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(logReferrals);
                }

                Commit();
                transaction.Complete();

                return currentCustomer != null ? currentCustomer.Amount : 0;
            }
        }

        public void Update(CustomerCredits entity)
        {
            var customerUpdate = DayaxeDbContext.CustomerCredits.SingleOrDefault(x => x.CustomerId == entity.CustomerId);
            if (customerUpdate != null)
            {
                if (!string.IsNullOrEmpty(entity.LogDescriptionCredits))
                {
                    var creditLogs = new CustomerCreditLogs
                    {
                        Amount = customerUpdate.Amount,
                        ProductId = 0,
                        BookingId = 0,
                        SubscriptionId = entity.SubscriptionId,
                        CreatedBy = 0,
                        CreatedDate = DateTime.UtcNow,
                        CreditType = (byte)Enums.CreditType.Charge,
                        Description = entity.LogDescriptionCredits,
                        CustomerId = entity.CustomerId,
                        ReferralId = customerUpdate.ReferralCustomerId,
                        SubscriptionBookingId = 0,
                        Status = true,
                        GiftCardId = 0
                    };

                    DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(creditLogs);
                }

                if (entity.LogsItem != null)
                {
                    AddLog(entity.LogsItem);
                }

                customerUpdate.Amount = entity.Amount;
                customerUpdate.LastUpdatedDate = entity.LastUpdatedDate;
                customerUpdate.FirstRewardForOwner = entity.FirstRewardForOwner;
                customerUpdate.FirstRewardForReferral = entity.FirstRewardForReferral;
                customerUpdate.IsDelete = entity.IsDelete;
            }
            Commit();
        }

        public void Delete(CustomerCredits entity)
        {
            var customer = DayaxeDbContext.CustomerCredits.FirstOrDefault(x => x.CustomerId == entity.CustomerId);
            if (customer != null)
            {
                customer.IsDelete = true;
            }
            Commit();
        }

        public void Delete(Func<CustomerCredits, bool> predicate)
        {
            IEnumerable<CustomerCredits> listHotels = DayaxeDbContext.CustomerCredits.Where(predicate).AsEnumerable();
            listHotels.ToList().ForEach(hotels =>
            {
                hotels.IsDelete = true;
            });
            Commit();
        }

        public CustomerCredits GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.CustomerId == id);
        }

        public CustomerCredits Refresh(CustomerCredits entity)
        {
            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues,
                DayaxeDbContext.CustomerCredits);

            DayaxeDbContext.Refresh(RefreshMode.OverwriteCurrentValues,
                DayaxeDbContext.GiftCards);

            return DayaxeDbContext.CustomerCredits.FirstOrDefault(x => x.Id == entity.Id);
        }

        public CustomerCredits GetByReferCode(string code)
        {
            return GetAll().FirstOrDefault(h => h.ReferralCode.ToUpper() == code.ToUpper());
        }

        public IEnumerable<CustomerCredits> GetAll()
        {
            var entities = CacheLayer.Get<List<CustomerCredits>>(CacheKeys.CustomerCreditsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerCredits.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerCreditsCacheKey);
            return entities.AsEnumerable();
        }

        #region 

        public IEnumerable<CustomerCreditLogs> GetAllLogs()
        {
            var entities = CacheLayer.Get<List<CustomerCreditLogs>>(CacheKeys.CustomerCreditLogsCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerCreditLogs.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerCreditLogsCacheKey);
            return entities.AsEnumerable();
        }

        public IEnumerable<CustomerCreditLogs> GetAllLogsByCustomerId(long customerId)
        {
            var logs = GetAllLogs().Where(h => h.CustomerId == customerId).OrderByDescending(l => l.CreatedDate).ToList();

            logs.ForEach(log =>
            {
                var customerInfos = CustomerInfoList.FirstOrDefault(ci => ci.CustomerId == log.ReferralId);
                string fullName = string.Empty;
                if (customerInfos != null)
                {
                    if (!string.IsNullOrEmpty(customerInfos.FirstName) && !string.IsNullOrEmpty(customerInfos.LastName))
                    {
                        fullName = string.Format("{0} {1}", customerInfos.FirstName, customerInfos.LastName);
                    }
                    else
                    {
                        fullName = customerInfos.EmailAddress;
                    }
                }
                log.Description = string.Format(log.Description, fullName);
            });

            return logs;
        }

        #endregion

        #region Gift Card

        public GiftCards GetGiftCardByCode(string code)
        {
            var giftCards = GiftCardList.FirstOrDefault(h => h.Code.ToUpper() == code.ToUpper());

            if (giftCards != null)
            {
                var isGiftForEmail = (from gb in GiftCardBookingList
                    join gc in GiftCardList on gb.GiftCardId equals gc.Id
                    where gc.Id == giftCards.Id
                    select gb).FirstOrDefault();
                if (isGiftForEmail != null)
                {
                    giftCards.EmailAddress = isGiftForEmail.RecipientEmail;
                }
            }

            return giftCards;
        }

        public double AddGiftCard(CustomerCredits customerCredits, string code)
        {
            using (var transaction = new TransactionScope())
            {

                var credits = DayaxeDbContext.CustomerCredits.FirstOrDefault(cc => cc.CustomerId == customerCredits.CustomerId);
                if (credits != null)
                {
                    var card = DayaxeDbContext.GiftCards.FirstOrDefault(g => g.Code.ToUpper() == code.ToUpper().Trim());
                    var logs = new CustomerCreditLogs
                    {
                        CreatedDate = DateTime.UtcNow,
                        CreditType = (int)Enums.CreditType.GiftCard,
                        CustomerId = credits.CustomerId,
                        ReferralId = credits.CustomerId,
                        Amount = card.Amount,
                        CreatedBy = credits.CustomerId,
                        Description = string.Format("{0} - {{0}} - {1}",
                            Enums.CreditType.GiftCard.ToDescription(),
                            code.ToUpper().Trim()),
                        BookingId = 0,
                        Status = true,
                        GiftCardId = card.Id
                    };

                    card.Status = (short) Enums.GiftCardType.Used;

                    DayaxeDbContext.CustomerCreditLogs.InsertOnSubmit(logs);
                    credits.Amount += card.Amount;

                    Commit();
                }

                transaction.Complete();

                return credits != null ? credits.Amount : 0;
            }
        }

        #endregion
    }
}
