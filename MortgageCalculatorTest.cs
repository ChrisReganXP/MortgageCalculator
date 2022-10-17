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
            OutputPayments(account, months);
        }

        public void TestTwo()
        {
            MortgageCalculator calc = new MortgageCalculator();

            MortgageAccount account = new MortgageAccount() { InitialBalance = 500000m };
            account.ProductSchedule = new List<MortgageProduct>();
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 3, Years = 5 });
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 5, Years = 20 });

            var months = calc.GetRepaymentSchedule(account);
            OutputPayments(account, months);
        }

        public void TestThree()
        {
            MortgageCalculator calc = new MortgageCalculator();

            MortgageAccount account = new MortgageAccount() { InitialBalance = 673857m };
            account.ProductSchedule = new List<MortgageProduct>();
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 5, Years = 20 });

            var months = calc.GetRepaymentSchedule(account);
            OutputPayments(account, months);
        }

        public void TestFour()
        {
            MortgageCalculator calc = new MortgageCalculator();

            MortgageAccount account = new MortgageAccount() { InitialBalance = 100000 };
            account.ProductSchedule = new List<MortgageProduct>();
            account.ProductSchedule.Add(new MortgageProduct() { APRRate = 5, Years = 20 });

            var months = calc.GetRepaymentSchedule(account);
            OutputPayments(account, months);
        }

        private void OutputPayments(MortgageAccount account, List<RepaymentScheduleMonth> months)
        {
            for (int i = 0; i < months.Count; i++)
            {
                var repayment = months[i];
                var monthText = repayment.Month >= 10 ? repayment.Month.ToString() : repayment.Month + " ";
                Console.WriteLine($"Month : {repayment.Year}/{monthText} Pay : { repayment.Payment } Remaining : {repayment.RemainingBalance_Total} CapRemaining : {repayment.RemainingBalance_Capital} CapContrib : {repayment.AmountPaid_Capital} IntContrib : {repayment.AmountPaid_Interest}");
            }

            Console.WriteLine($"Capital Amount : {account.InitialBalance} Term : {account.ProductSchedule.Sum(x => x.Years)} Total Payment : {months.Sum(x => x.Payment)}");
            Console.WriteLine();
        }
    }
}
