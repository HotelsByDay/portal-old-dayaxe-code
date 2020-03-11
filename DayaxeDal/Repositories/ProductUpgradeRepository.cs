using System.Collections.Generic;
using System.Linq;

namespace DayaxeDal.Repositories
{
    public class ProductUpgradeRepository : BaseRepository
    {
        public int Add(ProductUpgrades entity)
        {
            DayaxeDbContext.ProductUpgrades.InsertOnSubmit(entity);
            Commit();
            return entity.Id;
        }

        public void Delete(List<ProductUpgrades> productUpgrades)
        {
            if (productUpgrades.Any())
            {
                var hotelIds = productUpgrades.Select(x => x.UpgradeId).ToList();

                var removeList = DayaxeDbContext.ProductUpgrades
                    .Where(x => x.ProductId == productUpgrades.First().ProductId
                        && hotelIds.Contains(x.UpgradeId));
                DayaxeDbContext.ProductUpgrades.DeleteAllOnSubmit(removeList);
            }
            Commit();
        }
    }
}
