using DayaxeDal.Ultility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace DayaxeDal.Repositories
{
    public class CustomerInfoRepository:BaseRepository, IRepository<CustomerInfos>
    {
        public IEnumerable<CustomerInfos> Get(Func<CustomerInfos, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(CustomerInfos entity)
        {
            if (string.IsNullOrEmpty(entity.EmailAddress))
            {
                throw new Exception("Email Can Not Empty");
            }
            DayaxeDbContext.CustomerInfos.InsertOnSubmit(entity);
            Commit();
            return entity.CustomerId;
        }

        public void Update(CustomerInfos customer)
        {
            var customerUpdate = DayaxeDbContext.CustomerInfos.SingleOrDefault(x => x.CustomerId == customer.CustomerId);
            if (customerUpdate != null)
            {
                if (!string.IsNullOrWhiteSpace(customer.FirstName))
                {
                    customerUpdate.FirstName = customer.FirstName;
                }

                if (!string.IsNullOrWhiteSpace(customer.LastName))
                {
                    customerUpdate.LastName = customer.LastName;
                }

                if (!string.IsNullOrWhiteSpace(customer.EmailAddress))
                {
                    customerUpdate.EmailAddress = customer.EmailAddress;
                }

                if (!string.IsNullOrWhiteSpace(customer.ZipCode))
                {
                    customerUpdate.ZipCode = customer.ZipCode;
                }

                if (!string.IsNullOrWhiteSpace(customer.Password))
                {
                    customerUpdate.Password = customer.Password;
                }

                if (!string.IsNullOrWhiteSpace(customer.SessionId))
                {
                    customerUpdate.SessionId = customer.SessionId;
                }

                if (customer.LastSignInDate.HasValue)
                {
                    customerUpdate.LastSignInDate = customer.LastSignInDate;
                }

                if (!string.IsNullOrEmpty(customer.StripeTokenId))
                {
                    customerUpdate.StripeTokenId = customer.StripeTokenId;
                }

                if (!string.IsNullOrEmpty(customer.StripeCustomerId))
                {
                    customerUpdate.StripeCustomerId = customer.StripeCustomerId;
                }

                if (!string.IsNullOrEmpty(customer.StripeCardId))
                {
                    customerUpdate.StripeCardId = customer.StripeCardId;
                }

                if (!string.IsNullOrEmpty(customer.BankAccountLast4))
                {
                    customerUpdate.BankAccountLast4 = customer.BankAccountLast4;
                }

                if (!string.IsNullOrEmpty(customer.CardType))
                {
                    customerUpdate.CardType = customer.CardType;
                }

                if (!string.IsNullOrWhiteSpace(customer.BrowsePassUrl))
                {
                    customerUpdate.BrowsePassUrl = customer.BrowsePassUrl;
                }

                // Admin
                customerUpdate.IsAdmin = customer.IsAdmin;

                customerUpdate.IsConfirmed = customer.IsConfirmed;
                customerUpdate.IsActive = customer.IsActive;
                customerUpdate.IsDelete = customer.IsDelete;

                customerUpdate.IsCheckInOnly = customer.IsCheckInOnly;
            }
            Commit();
        }

        public void Delete(CustomerInfos entity)
        {
            var customer = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.CustomerId == entity.CustomerId);
            if (customer != null)
            {
                customer.IsDelete = true;
            }
            Commit();
        }

        public void Delete(Func<CustomerInfos, bool> predicate)
        {
            IEnumerable<CustomerInfos> listHotels = DayaxeDbContext.CustomerInfos.Where(predicate).AsEnumerable();
            listHotels.ToList().ForEach(hotels =>
            {
                hotels.IsDelete = true;
            });
            Commit();
        }

        public CustomerInfos GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.CustomerId == id);
        }

        public IEnumerable<CustomerInfos> GetAll()
        {
            var entities = CacheLayer.Get<List<CustomerInfos>>(CacheKeys.CustomerInfosCacheKey);
            if (entities != null)
            {
                return entities.AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerInfos.ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerInfosCacheKey);
            return entities.AsEnumerable();
        }

        public CustomerInfos Refresh(CustomerInfos entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public ResponseData SignIn(string username, string password)
        {
            var response = new ResponseData();
            // && x.IsConfirmed
            var user = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.EmailAddress.Equals(username.Trim()) && x.Password.Equals(password.Trim()));
            if (user != null)
            {
                user.SessionId = Helper.RandomString(20);
                user.LastSignInDate = DateTime.UtcNow;
                Commit();

                response.IsSuccessful = true;
                response.AccessKey = user.SessionId;
                response.SearchPageUrl = user.BrowsePassUrl;

                Update(user);
            }

            response.Message = new List<string>
            {
                "Please provide valid username and password"
            };

            return response;
        }

        public ResponseData ForgotPassword(string username)
        {
            var response = new ResponseData();
            var customerInfos = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.EmailAddress.Equals(username.Trim()));
            if (customerInfos != null)
            {
                customerInfos.ChangePasswordSessionId = Helper.RandomString(20);
                Commit();

                response.IsSuccessful = true;

                // Temp for current not send email
                response.AccessKey = customerInfos.ChangePasswordSessionId;
                response.Message = new List<string>
                {
                    customerInfos.FirstName
                };
            }
            else
            {
                response.Message = new List<string>
                {
                    "Please provide valid email"
                };
            }
            return response;
        }

        public ResponseData ChangePassword(string accessKey, string password)
        {
            var response = new ResponseData();
            var customerInfos = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.ChangePasswordSessionId.Equals(accessKey));
            if (customerInfos != null)
            {
                response.IsSuccessful = true;
                response.Message = new List<string>
                {
                    customerInfos.EmailAddress,
                    customerInfos.FirstName
                };

                customerInfos.IsConfirmed = true;
                // customerInfos.ChangePasswordSessionId = string.Empty;
                //customerInfos.Password = password;
                customerInfos.Password = Algoritma.EncryptHMACSHA512(password, customerInfos.Salt);

                DayaxeDbContext.SubmitChanges();
            }
            else
            {
                response.Message = new List<string>
                {
                    "Your session has been expired"
                };
            }

            return response;
        }

        public ResponseData GetVipAccess(string username, string browsePassUrl, string firstName = "", string lastName = "")
        {
            using (var transaction = new TransactionScope())
            {
                var response = new ResponseData();
                username = username.Trim().ToLower();

                var customerInfos = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.EmailAddress == username);
                if (customerInfos != null)
                {
                    customerInfos.CreateAccountSessionId = Helper.RandomString(20);
                    customerInfos.Password = Helper.RandomString(7);
                    customerInfos.ChangePasswordSessionId = Helper.RandomString(20);
                    customerInfos.IsConfirmed = false;

                    var customerCredits = DayaxeDbContext.CustomerCredits
                        .FirstOrDefault(x => x.CustomerId == customerInfos.CustomerId);
                    if (customerCredits != null)
                    {
                        response.ReferralCode = customerCredits.ReferralCode;
                    }
                }
                else
                {
                    customerInfos = new CustomerInfos
                    {
                        EmailAddress = username,
                        IsConfirmed = false,
                        CreateAccountSessionId = Helper.RandomString(20),
                        CreatedDate = DateTime.UtcNow,
                        BrowsePassUrl = browsePassUrl,
                        Password = Helper.RandomString(7),
                        ChangePasswordSessionId = Helper.RandomString(20),
                        FirstName = firstName,
                        LastName = lastName
                    };
                    DayaxeDbContext.CustomerInfos.InsertOnSubmit(customerInfos);

                    Commit();

                    var customerCredits = new CustomerCredits
                    {
                        Amount = 0,
                        CreatedDate = DateTime.UtcNow,
                        CustomerId = customerInfos.CustomerId,
                        IsActive = false,
                        IsDelete = false,
                        FirstRewardForOwner = 5,
                        FirstRewardForReferral = 5,
                        LastUpdatedDate = DateTime.UtcNow,
                        ReferralCode = Helper.RandomString(8),
                        ReferralCustomerId = 0
                    };

                    DayaxeDbContext.CustomerCredits.InsertOnSubmit(customerCredits);

                    response.ReferralCode = customerCredits.ReferralCode;
                }

                var url = string.Format(Constant.KlaviyoListApiUrl, AppConfiguration.KlaviyoListId);
                var addToListKlaviyoRes = Helper.Post(url, username);

                var logs = new Logs
                {
                    LogKey = "Klaviyo_Register_Response",
                    UpdatedBy = 1,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedContent = string.Format("{0} - {1}", username, addToListKlaviyoRes)
                };

                DayaxeDbContext.Logs.InsertOnSubmit(logs);

                Commit();
                transaction.Complete();

                response.IsSuccessful = true;
                response.AccessKey = customerInfos.CreateAccountSessionId.ToLower();
                response.Password = customerInfos.Password;
                response.PasswordKey = customerInfos.ChangePasswordSessionId.ToLower();
                response.CustomerId = customerInfos.CustomerId;
                response.StripeCustomerId = customerInfos.StripeCustomerId;

                return response;
            }
        }

        public CustomerInfos GetCustomerInfoBySessionChangePasswordId(string sessionId)
        {
            return GetAll().FirstOrDefault(x => x.ChangePasswordSessionId == sessionId.Trim());
        }

        #endregion

        #region Discount Subscription

        public Discounts GetSubscriptionDiscount(int customerId)
        {
            return (from d in DiscountOfSubscriptionList
                    join sdu in SubscriptionDiscountUsedList on d.Id equals sdu.DiscountId
                    where sdu.CustomerId == customerId
                        && d.PromoType == (byte)Enums.PromoType.SubscriptionPromo
                        && d.Status == Enums.DiscountStatus.Active
                    select d).FirstOrDefault();
        }

        public Discounts GetSubscriptionDiscountSuspended(int customerId)
        {
            return (from d in DiscountOfSubscriptionList
                join sdu in SubscriptionDiscountUsedList on d.Id equals sdu.DiscountId
                where sdu.CustomerId == customerId
                      && d.PromoType == (byte)Enums.PromoType.SubscriptionPromo
                select d).FirstOrDefault();
        }

        public Subscriptions GetSubscriptionByCustomerId(int customerId, int discountId)
        {
            return (from s in SubscriptionsList
                    join sb in SubscriptionBookingsList on s.Id equals sb.SubscriptionId
                    join sdu in SubscriptionDiscountUsedList on sb.Id equals sdu.SubscriptionBookingId
                    where sdu.CustomerId == customerId && sdu.DiscountId == discountId
                    select s).FirstOrDefault();
        }

        public Subscriptions GetSubscriptionActiveByDiscountId(int discountId)
        {
            return (from sdu in SubscriptionDiscountUsedList
                join sb in SubscriptionBookingsList on sdu.SubscriptionBookingId equals sb.Id
                join s in SubscriptionsList on sb.SubscriptionId equals s.Id
                join sd in DiscountOfSubscriptionList on sdu.DiscountId equals sd.Id
                where sdu.DiscountId == discountId && sd.Status == Enums.DiscountStatus.Active
                select s).FirstOrDefault();
        }

        #endregion
    }
}
