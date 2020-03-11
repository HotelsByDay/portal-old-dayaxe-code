using System;
using System.Linq;
using DayaxeDal;
using DayaxeDal.Repositories;
using Stripe;

namespace Dayaxe.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set Stripe Api Key
            StripeConfiguration.SetApiKey(AppConfiguration.StripeApiKey);

            using (var subscriptionBookingRepository = new SubscriptionBookingRepository())
            {
                var customerInfoses = subscriptionBookingRepository.CustomerInfoList.ToList();

                var customerService = new StripeCustomerService();
                var customerItems = customerService.List(
                    new StripeCustomerListOptions
                    {
                        Limit = 100,
                        StartingAfter = "cus_B300tC1ruEyck9"
                    }
                ).ToList();

                string lastId = customerItems.Last().Id;
                customerItems.ForEach(customer =>
                {
                    var subscription = customerInfoses
                        .FirstOrDefault(sb => sb.StripeCustomerId != null && sb.StripeCustomerId.Equals(customer.Id, StringComparison.OrdinalIgnoreCase));
                    if (subscription == null)
                    {
                        try
                        {
                            customerService.Delete(customer.Id);
                            Console.WriteLine("Delete Customer - " + customer.Id);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error - " + ex.Message);
                        }
                    }
                });

                Console.WriteLine("Done!!!");
                Console.ReadLine();
            }
        }
    }
}
