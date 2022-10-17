using System;
using System.Collections.Generic;
using System.Linq;

namespace MortgageCalculator
{
    /// <summary>
    /// Covers entirely of mortgate term
    /// </summary>
    public class MortgageAccount
    {
        /// <summary>
        /// Balance to be borrowed
        /// </summary>
        public decimal InitialBalance { get; set; } 

        /// <summary>
        /// List of products used to cover term
        /// </summary>
        public List<MortgageProduct> ProductSchedule { get; set; }
    }

    /// <summary>
    /// List of mortgage products to cover a period of time
    /// </summary>
    public class MortgageProduct
    { 
        /// <summary>
        /// How many years product is used for
        /// </summary>
        public short Years { get; set; }

        /// <summary>
        /// Quote APR rate of loan
        /// </summary>
        public decimal APRRate { get; set; }

        /// <summary>
        /// Calculated monthly interest rate as a decimal percentage of 1
        /// </summary>
        public decimal MonthlyRate { get { return APRRate / (decimal)1200; } }
    }

    /// <summary>
    /// Monthly Payment Schedule
    /// </summary>
    public class RepaymentScheduleMonth
    {
        /// <summary>
        /// Which year this payment is for
        /// </summary>
        public short Year { get; set; }

        /// <summary>
        /// Which month this payment is for
        /// </summary>
        public short Month { get; set; }

        /// <summary>
        /// Which mortgage product is being repaid
        /// </summary>
        public MortgageProduct Product { get; set; }

        /// <summary>
        /// Amount to be repaid
        /// </summary>
        public decimal Payment { get; set; }

        /// <summary>
        /// Total amount still to be repaid after this payment
        /// </summary>
        public decimal RemainingBalance_Total { get; set; }

        /// <summary>
        /// Capital amount still to be repaid after this payment
        /// </summary>
        public decimal RemainingBalance_Capital { get; set; }

        /// <summary>
        /// The amount of capital being repaid
        /// </summary>
        public decimal AmountPaid_Capital { get; set; }

        /// <summary>
        /// The amount of interest being repaid
        /// </summary>
        public decimal AmountPaid_Interest { get; set; }
    }

    public class MortgageCalculator
    {
        /// <summary>
        /// Gets the repayment schedule for a mortgage
        /// </summary>
        /// <param name="account">list of mortgage products</param>
        /// <returns>the repayment schedule</returns>
        public List<RepaymentScheduleMonth> GetRepaymentSchedule(MortgageAccount account)
        {
            return CalculatePaymentSchedule(account);
        }

        /// <summary>
        /// Calculates the repayment schedule for a mortgage
        /// </summary>
        /// <param name="account">list of mortgag</param>
        /// <returns></returns>
        private List<RepaymentScheduleMonth> CalculatePaymentSchedule(MortgageAccount account)
        {
            var schedule = new List<RepaymentScheduleMonth>();
            var monthsRemaining = account.ProductSchedule.Sum(x => x.Years) * 12;
            decimal capitalBalance = account.InitialBalance;

            short year = 1;
            short month = 1;

            foreach (var product in account.ProductSchedule)
            {
                RepaymentScheduleMonth[] productSchedule = new RepaymentScheduleMonth[product.Years * 12];

                // Get the total amount due on this new product. i.e. loan value for this new agreement.
                // this is based on the assumptions around balance calculations for new agreements shown by the capitalBalance reduction below
                var totalPaymentDue = GetTotalPaymentDue(capitalBalance, product.MonthlyRate, monthsRemaining);
                
                // get new monthly amount for the new agreement.
                decimal payment = GetMonthlyPayment(capitalBalance, product.MonthlyRate, monthsRemaining);

                for (int i = 0; i < product.Years * 12; i++)
                {
                    // reduce the total amount due by this month's payment
                    totalPaymentDue -= payment;

                    // calculate how much interest is being repaid this month
                    decimal monthlyInterest = GetMonthlyInterest(capitalBalance, product.MonthlyRate);

                    // calculate how much being repaid is not interest
                    var capitalRepayment = payment - monthlyInterest;

                    // reduce the capital balance by the amount of this month payment for capital
                    capitalBalance -= capitalRepayment;

                    productSchedule[i] = new RepaymentScheduleMonth() { Product = product, Payment = payment, Year = year, Month = month, RemainingBalance_Total = totalPaymentDue, AmountPaid_Capital = capitalRepayment, AmountPaid_Interest = monthlyInterest, RemainingBalance_Capital = capitalBalance };

                    IncrementMonths(ref year, ref month, ref monthsRemaining);
                }

                schedule.AddRange(productSchedule.ToList());
            }

            return schedule;
        }

        /// <summary>
        /// Gets the total payment due if this produce was used until loan repaid
        /// </summary>
        /// <param name="capitalBalance">Capital outstanding</param>
        /// <param name="monthlyRate">monthly interest rate</param>
        /// <param name="months">length of term</param>
        /// <returns>decimal of total outstanding cost</returns>
        private decimal GetTotalPaymentDue(decimal capitalBalance, decimal monthlyRate, int months)
        {
            return Math.Round((decimal)CalculateTotalPayment((double)capitalBalance, (double)monthlyRate, months), 2);
        }

        /// <summary>
        /// Calculates the total payment due using standard formula
        /// </summary>
        /// <param name="initialBalance">total capital required</param>
        /// <param name="monthlyRate">monthly interest rate</param>
        /// <param name="months">length of term</param>
        /// <returns>decimal of total outstanding cost</returns>
        private double CalculateTotalPayment(double initialBalance, double monthlyRate, double months)
        {
            double topline = monthlyRate * initialBalance;
            double bottomline = 1 - Math.Pow(1 + monthlyRate, 0 - months);
            return (topline / bottomline) * months;
        }

        /// <summary>
        /// Gets the expected monthly payment for the rate and months
        /// </summary>
        /// <param name="capitalBalance">amount of capital outstanding</param>
        /// <param name="monthlyRate">monthly interest rate</param>
        /// <param name="monthsRemaining">length of term</param>
        /// <returns>decimal of expected monthly repayment</returns>
        private decimal GetMonthlyPayment(decimal capitalBalance, decimal monthlyRate, int months)
        {
            return Math.Round((decimal)CalculateMonthlyPayment((double)capitalBalance, (double)monthlyRate, months), 2);
        }

        /// <summary>
        /// Calculates the expected monthly payment
        /// </summary>
        /// <param name="initialBalance">amount of capital outstanding</param>
        /// <param name="monthlyRate">monthly interest rate</param>
        /// <param name="months">length of term</param>
        /// <returns>decimal of expected monthly repayment</returns>
        private double CalculateMonthlyPayment(double initialBalance, double monthlyRate, double months)
        {
            double topline = monthlyRate * Math.Pow(1 + monthlyRate, months);
            double bottomline = Math.Pow(1 + monthlyRate, months) - 1;
            return initialBalance * (topline / bottomline);
        }

        /// <summary>
        /// Gets the month interest on balance
        /// </summary>
        /// <param name="capitalBalance">the capital outstanding</param>
        /// <param name="monthlyRate">the monthly rate</param>
        /// <returns>the amount of interest to be paid in a month</returns>
        private decimal GetMonthlyInterest(decimal capitalBalance, decimal monthlyRate)
        {
            return (decimal)Math.Round(CalculateMonthlyInterest((double)capitalBalance, (double)monthlyRate), 2);
        }

        /// <summary>
        /// Calculates the month interest on balance
        /// </summary>
        /// <param name="capitalBalance">the capital outstanding</param>
        /// <param name="monthlyRate">the monthly rate</param>
        /// <returns></returns>
        private double CalculateMonthlyInterest(double capitalBalance, double monthlyRate)
        {
            return capitalBalance * monthlyRate;
        }

        /// <summary>
        /// Handles the year and month counters
        /// </summary>
        /// <param name="year">year for print out</param>
        /// <param name="month">month for print out</param>
        /// <param name="monthsRemaining">months remaining for calculations</param>
        private void IncrementMonths(ref short year, ref short month, ref int monthsRemaining)
        {
            month++;

            if (month >= 13)
            {
                year++;
                month = 1;
            }

            monthsRemaining--;
        }
    }
}
