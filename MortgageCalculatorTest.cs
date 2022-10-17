using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MortgageCalculator
{
    public class MortgageCalculatorTest
    {
        public void TestOne()
        {
            MortgageCalculator calc = new MortgageCalculator();

            MortgageAccount account = new MortgageAccount() { InitialBalance = 500000m };
            account.ProductSchedule = new List<MortgageProduct>();
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 4.7m, Years = 25 });

            var months = calc.GetRepaymentSchedule(account);
            OutputPayments(months);
        }

        public void TestTwo()
        {
            MortgageCalculator calc = new MortgageCalculator();

            MortgageAccount account = new MortgageAccount() { InitialBalance = 500000m };
            account.ProductSchedule = new List<MortgageProduct>();
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 3, Years = 5 });
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 5, Years = 20 });

            var months = calc.GetRepaymentSchedule(account);
            OutputPayments(months);
        }

        private void OutputPayments(List<RepaymentScheduleMonth> months)
        {
            for (int i = 0; i < months.Count; i++)
            {
                var x = months[i];
                Console.WriteLine($"Year {x.Year} - Month {x.Month} - Payment { x.Payment } - Balance {x.RemainingBalance_Total} - Balance Contrib - {x.AmountPaid_Balance} - Interest Contrib - {x.AmountPaid_Interest}");
            }

            Console.WriteLine($"Total Payment - {months.Sum(x => x.Payment)}");
            Console.WriteLine();
        }
    }
}
