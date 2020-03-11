using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class CustomerInfoHotelRepository : BaseRepository
    {
        public int Add(CustomerInfosHotels entity)
        {
            DayaxeDbContext.CustomerInfosHotels.InsertOnSubmit(entity);
            Commit();
            return entity.CustomerId;
        }

        public void Delete(List<CustomerInfosHotels> userHotelses)
        {
            var removeList = DayaxeDbContext.CustomerInfosHotels.Where(x => userHotelses.Contains(x)).ToList();
            DayaxeDbContext.CustomerInfosHotels.DeleteAllOnSubmit(removeList);
            Commit();
        }

        #region Custom

        public List<CustomerInfosHotels> GetByUserId(int userId)
        {
            var userHotels = (from p in UserHotelList
                              join p1 in HotelList
                                  on p.HotelId equals p1.HotelId
                              where !p1.IsDelete && p.CustomerId == userId
                              select p).ToList();
            return userHotels;
        }

        #endregion
    }
}
