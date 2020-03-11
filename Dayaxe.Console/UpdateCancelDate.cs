using System;
using System.Collections.Generic;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Repositories;
using Stripe;

namespace Dayaxe.ConsoleApp
{
    class Program
    {
        #region Subscription BookingId
        //static readonly List<int> ids = new List<int>
        //{
        //    194 ,
        //    193,
        //    146,
        //    129,
        //    122,
        //    120,
        //    110,
        //    1239,
        //    1230,
        //    1220,
        //    1218,
        //    215,
        //    208,
        //    192,
        //    172,
        //    171,
        //    170,
        //    164,
        //    156,
        //    151,
        //    150,
        //    148,
        //    142,
        //    134,
        //    123,
        //    121,
        //    62,
        //    107,
        //    100,
        //    97,
        //    95,
        //    93,
        //    1253
        //};
        static readonly List<int> ids = new List<int>
        {
            3,
            183,
            209,
            210,
            211
        };

        #endregion

        static void Main(string[] args)
        {

            // Set Stripe Api Key
            StripeConfiguration.SetApiKey(AppConfiguration.StripeApiKey);

            using (var subscriptionBookingRepository = new SubscriptionBookingRepository())
            {
                var subscriptionBookingList = subscriptionBookingRepository.SubscriptionBookingsList.Where(sb => ids.Contains(sb.Id)).ToList();

                // Each Subscription Bookings
                subscriptionBookingList.ForEach(subscriptionBookings =>
                {
                    var customerInfos = subscriptionBookingRepository.CustomerInfoList
                        .FirstOrDefault(ci => ci.CustomerId == subscriptionBookings.CustomerId);

                    // Customer Infos
                    if (customerInfos != null)
                    {
                        var subscriptionService = new StripeSubscriptionService();
                        StripeSubscription subscription = subscriptionService.Get(subscriptionBookings.StripeSubscriptionId);
                        DateTime? canceledDate = subscription.CanceledAt;

                        if (canceledDate.HasValue)
                        {
                            subscriptionBookings.CancelDate = canceledDate;
                        }
                    }
                });

                subscriptionBookingRepository.UpdateSubscriptionBookingCanceledDate(subscriptionBookingList);

                Console.WriteLine("Done!!!");
                Console.ReadLine();
            }
        }
    }
}
