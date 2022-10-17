using System;
using System.Collections.Generic;
using System.Linq;

namespace MortgageCalculator
{
    public class MortgageAccount
    {
        public decimal InitialBalance { get; set; } 

        public List<MortgageProduct> ProductSchedule { get; set; }
    }

    public class MortgageProduct
    { 
        public short Years { get; set; }

        public decimal APRRate { get; set; }

        public decimal MonthlyRate { get { return APRRate / (decimal)1200; } }
    }

    public class RepaymentScheduleMonth
    {
        public short Year { get; set; }

        public short Month { get; set; }

        public MortgageProduct Product { get; set; }

        public decimal Payment { get; set; }

        public decimal RemainingBalance_Total { get; set; }

        public decimal RemainingBalance_Account { get; set; }

        public decimal AmountPaid_Balance { get; set; }

        public decimal AmountPaid_Interest { get; set; }
    }

    public class MortgageCalculator
    {
        public List<RepaymentScheduleMonth> GetRepaymentSchedule(MortgageAccount account)
        {
            return CalculatePaymentSchedule(account);
        }

        private List<RepaymentScheduleMonth> CalculatePaymentSchedule(MortgageAccount account)
        {
            var schedule = new List<RepaymentScheduleMonth>();
            var monthsRemaining = account.ProductSchedule.Sum(x => x.Years) * 12;
            decimal totalAccountBalance = account.InitialBalance;

            short year = 1;
            short month = 1;

            foreach (var product in account.ProductSchedule)
            {
                RepaymentScheduleMonth[] months = new RepaymentScheduleMonth[product.Years * 12];
                decimal payment = Math.Round((decimal)CalculateMonthlyPayment((double)account.InitialBalance, (double)product.MonthlyRate, monthsRemaining), 2);

                var totalPaymentDue = (decimal)Math.Round(CalculateTotalPayment((double)totalAccountBalance, (double)product.MonthlyRate, monthsRemaining), 2);
                Console.WriteLine($"Rate - {product.APRRate} : Total Payment Calculated - {totalPaymentDue} : Total Account Balance - {totalAccountBalance}");

                for (int i = 0; i < product.Years * 12; i++)
                {
                    decimal monthlyInterest = (decimal)Math.Round(CalculateMonthlyInterest((double)totalAccountBalance, (double)product.MonthlyRate), 2);
                    totalPaymentDue -= payment;
                    totalAccountBalance -= payment - monthlyInterest;

                    months[i] = new RepaymentScheduleMonth() { Product = product, Payment = payment, Year = year, Month = month, RemainingBalance_Total = totalPaymentDue, AmountPaid_Balance = payment - monthlyInterest, AmountPaid_Interest = monthlyInterest, RemainingBalance_Account = totalAccountBalance };
                    month++;

                    if (month >= 13)
                    {
                        year++;
                        month = 1;
                    }

                    monthsRemaining--;
                }

                schedule.AddRange(months.ToList());
            }

            return schedule;
        }

        private double CalculateMonthlyPayment(double initialBalance, double monthlyRate, double totalMonths)
        {
            double topline = monthlyRate * Math.Pow(1 + monthlyRate, totalMonths);
            double bottomline = Math.Pow(1 + monthlyRate, totalMonths) - 1;
            return initialBalance * (topline / bottomline);
        }

        private double CalculateTotalPayment(double initialBalance, double monthlyRate, double totalMonths)
        {
            double topline = monthlyRate * initialBalance;
            double bottomline = 1 - Math.Pow(1 + monthlyRate, 0 - totalMonths);
            return (topline / bottomline) * totalMonths;
        }

        private double CalculateMonthlyInterest(double previousBalance, double monthlyRate)
        {
            return previousBalance * monthlyRate;
        }
    }
}
