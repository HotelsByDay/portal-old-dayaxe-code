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
                var subscriptionBookingList = subscriptionBookingRepository.SubscriptionBookingsList.ToList();

                var invoiceService = new StripeInvoiceService();
                var invoiceItems = invoiceService.List(
                    new StripeInvoiceListOptions
                    {
                        Limit = Int32.MaxValue
                    }
                ).ToList();

                invoiceItems.ForEach(invoice =>
                {
                    var subscription = subscriptionBookingList
                        .FirstOrDefault(sb => sb.StripeSubscriptionId.Equals(invoice.SubscriptionId, StringComparison.OrdinalIgnoreCase));
                    if (subscription == null)
                    {
                        try
                        {
                            var subscriptionService = new StripeSubscriptionService();
                            var stripeSubscription = subscriptionService.Cancel(invoice.SubscriptionId);
                            Console.WriteLine("Cancel - " + invoice.SubscriptionId);
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
