using System;
using System.Collections.Generic;
using System.Linq;

public interface IFinancialReportStrategy
{
    Func<Loan, bool> GetPredicate();
    IEnumerable<Tuple<string, string, decimal>> GetMetrics(Func<Loan, bool> predicate);
    IEnumerable<Tuple<decimal, string>> GetStatistics(Func<Loan, bool> predicate);
}

public class FinancialReport
{
    private IFinancialReportStrategy strategy;

    public FinancialReport(IFinancialReportStrategy strategy)
    {
        this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public IEnumerable<Tuple<string, string, decimal>> GetMetrics()
    {
        return strategy.GetMetrics(strategy.GetPredicate());
    }

    public IEnumerable<Tuple<decimal, string>> GetStatistics()
    {
        return strategy.GetStatistics(strategy.GetPredicate());
    }
}

public class FirstBankFinancialReportStrategy : IFinancialReportStrategy
{
    public Func<Loan, bool> GetPredicate() => loan => loan.LoanStartDate.Year >= 2021 && loan.Account.Customer.IsActive;

    public IEnumerable<Tuple<string, string, decimal>> GetMetrics(Func<Loan, bool> predicate)
    {
        string[] cities = { "Warszawa", "Kraków", "Gdańsk" };
        var loansLimited = FirstDatabase.GetData().Where(predicate).ToList();

        var metrics = new List<Tuple<string, string, decimal>>();
        foreach (var city in cities)
        {
            metrics.Add(new Tuple<string, string, decimal>("18-30", city, CalculateAverageInterestRate(loansLimited, GetMetricPredicate(city, 18, 30))));
            metrics.Add(new Tuple<string, string, decimal>("31-50", city, CalculateAverageInterestRate(loansLimited, GetMetricPredicate(city, 31, 50))));
            metrics.Add(new Tuple<string, string, decimal>("51-70", city, CalculateAverageInterestRate(loansLimited, GetMetricPredicate(city, 51, 70))));
        }

        return metrics;
    }

    public IEnumerable<Tuple<decimal, string>> GetStatistics(Func<Loan, bool> predicate)
    {
        var data = FirstDatabase.GetData().Where(predicate).ToList();

        return data.Select(l => new Tuple<decimal, string>(l.LoanAmount, l.InterestRate.ToString()));
    }

    private decimal CalculateAverageInterestRate(IEnumerable<Loan> loans, Func<Loan, bool> predicate = null)
    {
        if (predicate != null)
            loans = loans.Where(predicate);

        return loans.Average(l => l.InterestRate);
    }

    private static Func<Loan, bool> GetMetricPredicate(string city, int minAge, int maxAge) =>
        l => l.Account.Customer.CustomerAddress.Contains(city) &&
            DateTime.UtcNow.Year - l.Account.Customer.DateOfBirth.Year >= minAge &&
            DateTime.UtcNow.Year - l.Account.Customer.DateOfBirth.Year <= maxAge;
}

public class SecondBankFinancialReportStrategy : IFinancialReportStrategy
{
    public Func<Loan, bool> GetPredicate() => loan => loan.LoanStartDate.Year >= 2020 && loan.Account.Customer.IsActive && loan.InterestRate <= 20;

    public IEnumerable<Tuple<string, string, decimal>> GetMetrics(Func<Loan, bool> predicate)
    {
        string[] cities = { "Warszawa", "Kraków", "Gdańsk" };
        var loansLimited = SecondDatabase.GetData().Where(predicate).ToList();

        var metrics = new List<Tuple<string, string, decimal>>();
        foreach (var city in cities)
        {
            metrics.Add(new Tuple<string, string, decimal>("2020", city, CalculateAverageInterestRate(loansLimited, GetMetricPredicate(city, 2020))));
            metrics.Add(new Tuple<string, string, decimal>("2021", city, CalculateAverageInterestRate(loansLimited, GetMetricPredicate(city, 2021))));
            metrics.Add(new Tuple<string, string, decimal>("2022", city, CalculateAverageInterestRate(loansLimited, GetMetricPredicate(city, 2022))));
        }

        return metrics;
    }

    public IEnumerable<Tuple<decimal, string>> GetStatistics(Func<Loan, bool> predicate)
    {
        var data = SecondDatabase.GetData().Where(predicate).ToList();

        return data.Select(l => new Tuple<decimal, string>(l.LoanAmount, $"{l.BankEmployee.EmployeeName}"));
    }

    private decimal CalculateAverageInterestRate(IEnumerable<Loan> loans, Func<Loan, bool> predicate = null)
    {
        if (predicate != null)
            loans = loans.Where(predicate);

        return loans.Average(l => l.InterestRate);
    }

    private static Func<Loan, bool> GetMetricPredicate(string city, int year) =>
        l => l.BankEmployee.BankDepartment.PlacementCity.Contains(city) &&
            l.LoanStartDate.Year == year;
}
