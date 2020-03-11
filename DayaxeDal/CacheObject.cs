using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using DayaxeDal.Extensions;

namespace DayaxeDal
{
    public class CacheLayer
    {
        static readonly ObjectCache Cache = MemoryCache.Default;

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)Cache[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="objectToCache">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Add<T>(T objectToCache, string key) where T : class
        {
            Cache.Add(key, objectToCache, DateTime.UtcNow.ToLosAngerlesTime().AddDays(7));
        }

        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <param name="objectToCache">Item to be cached</param>
        /// <param name="key">Name of item</param>
        /// <param name="minutes">Days Cache Of Item - Default 30 days</param>
        public static void Add(object objectToCache, string key, int minutes = 15)
        {
            Cache.Add(key, objectToCache, DateTime.UtcNow.ToLosAngerlesTime().AddMinutes(minutes));
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public static void Clear(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            return Cache.Get(key) != null;
        }

        /// <summary>
        /// Gets all cached items as a list by their key.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAll()
        {
            return Cache.Select(keyValuePair => keyValuePair.Key).ToList();
        }

        public static void ClearAll()
        {
            Cache.Remove(CacheKeys.HotelsCacheKey);
            Cache.Remove(CacheKeys.AmentiesCacheKey);
            Cache.Remove(CacheKeys.ProductsCacheKey);
            Cache.Remove(CacheKeys.CustomerInfosCacheKey);
            Cache.Remove(CacheKeys.CustomerInfosSearchCriteriaCacheKey);
            Cache.Remove(CacheKeys.CustomerCreditsCacheKey);
            Cache.Remove(CacheKeys.CustomerCreditLogsCacheKey);
            Cache.Remove(CacheKeys.MarketsCacheKey);
            Cache.Remove(CacheKeys.MarketHotelsCacheKey);
            //Cache.Remove(CacheKeys.BlockedDatesCacheKey);
            Cache.Remove(CacheKeys.ProductUpgradesCacheKey);
            Cache.Remove(CacheKeys.CustomerInfosHotelsCacheKey);
            Cache.Remove(CacheKeys.DiscountsCacheKey);
            Cache.Remove(CacheKeys.DiscountProductsCacheKey);
            Cache.Remove(CacheKeys.ProductImagesCacheKey);
            Cache.Remove(CacheKeys.PhotosCacheKey);
            Cache.Remove(CacheKeys.AmentyListsCacheKey);
            Cache.Remove(CacheKeys.BookingsCacheKey);
            Cache.Remove(CacheKeys.SurveysCacheKey);
            Cache.Remove(CacheKeys.BlockedDatesCustomPricesCacheKey);
            Cache.Remove(CacheKeys.InvoicesCacheKey);
            Cache.Remove(CacheKeys.DiscountBookingsCacheKey);
            Cache.Remove(CacheKeys.DefaultPricesCacheKey);
            Cache.Remove(CacheKeys.ProductAddOnsCacheKey);
            Cache.Remove(CacheKeys.GiftCardCacheKey);
            Cache.Remove(CacheKeys.GiftCardBookingCacheKey);
            Cache.Remove(CacheKeys.BookingHistoriesCacheKey);
            Cache.Remove(CacheKeys.SubscriptionsCacheKey);
            Cache.Remove(CacheKeys.SubscriptionImagesCacheKey);
            Cache.Remove(CacheKeys.SubscriptionBookingsCacheKey);
            Cache.Remove(CacheKeys.DiscountSubscriptionsCacheKey);
            Cache.Remove(CacheKeys.SubscriptionBookingDiscountsCacheKey);
            Cache.Remove(CacheKeys.SubsciptionDiscountUsedCacheKey);
            Cache.Remove(CacheKeys.SubscriptionDiscountsCacheKey);
            Cache.Remove(CacheKeys.LogsCacheKey);
            Cache.Remove(CacheKeys.HotelPoliciesCacheKey);
            Cache.Remove(CacheKeys.PoliciesCacheKey);
            Cache.Remove(CacheKeys.TaxesCacheKey);
        }
    }
}
