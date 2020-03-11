using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace DayaxeDal.Repositories
{
    public class UserRepository : BaseRepository, IRepository<CustomerInfos>
    {
        public IEnumerable<CustomerInfos> Get(Func<CustomerInfos, bool> criteria)
        {
            return GetAll().Where(criteria);
        }

        public int Add(CustomerInfos entity)
        {
            using (var transaction = new TransactionScope())
            {
                DayaxeDbContext.CustomerInfos.InsertOnSubmit(entity);
                Commit();

                var customerCredits = new CustomerCredits
                {
                    Amount = 0,
                    CreatedDate = DateTime.UtcNow,
                    CustomerId = entity.CustomerId,
                    IsActive = false,
                    IsDelete = false,
                    FirstRewardForOwner = 5,
                    FirstRewardForReferral = 5,
                    LastUpdatedDate = DateTime.UtcNow,
                    ReferralCode = Helper.RandomString(8),
                    ReferralCustomerId = 0
                };

                DayaxeDbContext.CustomerCredits.InsertOnSubmit(customerCredits);
                Commit();
                transaction.Complete();

                return entity.CustomerId;
            }
        }

        public void Update(CustomerInfos entity)
        {
            using (var transaction = new TransactionScope())
            {
                var update = DayaxeDbContext.CustomerInfos.SingleOrDefault(x => x.CustomerId == entity.CustomerId);
                if (update != null)
                {
                    update.IsActive = entity.IsActive;
                    update.IsAdmin = entity.IsAdmin;
                    update.IsDelete = entity.IsDelete;
                    update.FirstName = entity.FirstName;
                    update.LastName = entity.LastName;
                    update.Password = entity.Password;
                    update.EmailAddress = entity.EmailAddress;
                    update.IsCheckInOnly = entity.IsCheckInOnly;

                    var customerCredit = DayaxeDbContext.CustomerCredits.SingleOrDefault(cc => cc.CustomerId == entity.CustomerId);
                    if (customerCredit == null)
                    {
                        var customerCredits = new CustomerCredits
                        {
                            Amount = 0,
                            CreatedDate = DateTime.UtcNow,
                            CustomerId = entity.CustomerId,
                            IsActive = false,
                            IsDelete = false,
                            FirstRewardForOwner = 5,
                            FirstRewardForReferral = 5,
                            LastUpdatedDate = DateTime.UtcNow,
                            ReferralCode = Helper.RandomString(8),
                            ReferralCustomerId = 0
                        };

                        DayaxeDbContext.CustomerCredits.InsertOnSubmit(customerCredits);
                    }
                }
                Commit();
                transaction.Complete();
            }
        }

        public void Delete(CustomerInfos entity)
        {
            var hotels = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.CustomerId == entity.CustomerId);
            if (hotels != null)
            {
                hotels.IsDelete = true;
            }
            Commit();
        }

        public void Delete(Func<CustomerInfos, bool> predicate)
        {
            var entities = DayaxeDbContext.CustomerInfos.Where(predicate).AsEnumerable();
            entities.ToList().ForEach(hotels =>
            {
                hotels.IsDelete = true;
            });
            Commit();
        }

        public CustomerInfos GetById(long id)
        {
            return GetAll().FirstOrDefault(h => h.CustomerId == id && !h.IsDelete && (h.IsAdmin || h.IsSuperAdmin));
        }

        public IEnumerable<CustomerInfos> GetAll()
        {
            var entities = CacheLayer.Get<List<CustomerInfos>>(CacheKeys.CustomerInfosCacheKey);
            if (entities != null)
            {
                return entities.Where(u => !u.IsDelete && (u.IsAdmin || u.IsSuperAdmin)).AsEnumerable();
            }
            entities = DayaxeDbContext.CustomerInfos.Where(u => !u.IsDelete).ToList();
            CacheLayer.Add(entities, CacheKeys.CustomerInfosCacheKey);
            return entities.Where(u => !u.IsDelete && (u.IsAdmin || u.IsSuperAdmin)).AsEnumerable();
        }

        public CustomerInfos Refresh(CustomerInfos entity)
        {
            throw new NotImplementedException();
        }

        #region Custom

        public void Delete(int userId)
        {
            var hotels = DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.CustomerId == userId);
            if (hotels != null)
            {
                hotels.IsDelete = true;
            }
            Commit();
        }

        public CustomerInfos GetUsersByUsernameAndPassword(string userName, string password)
        {
            return DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.EmailAddress.ToUpper() == userName.Trim().ToUpper() && 
                x.Password.ToUpper() == password.Trim().ToUpper() && 
                (x.IsAdmin || x.IsSuperAdmin || x.IsCheckInOnly) &&
                !x.IsDelete);
        }

        public CustomerInfos GetUsersByEmail(string email)
        {
            return DayaxeDbContext.CustomerInfos.FirstOrDefault(x => x.EmailAddress.ToUpper() == email &&
                                                                     (x.IsAdmin || x.IsSuperAdmin || x.IsCheckInOnly) &&
                                                                     !x.IsDelete);
        }

        #endregion
    }
}
