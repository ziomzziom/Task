using System.Collections.Generic;
using System.Linq;

public class FinancialReport
{
    private string strategy;

    public FinancialReport(string strategy)
    {
        
    }
    
    public IEnumerable<Tuple<string, string, decimal>> GetMetrics()
    {
    		// Should be replaced by strategy method
        var data = FirstDatabase.GetData().Where(l => l.LoanStartDate.Year >= 2021).ToList();

        return data.Select(l => new Tuple<string, string, decimal>(
            (DateTime.UtcNow.Year - l.Account.Customer.DateOfBirth.Year).ToString(),
            l.Account.Customer.CustomerAddress.Contains("Kraków") ? "Kraków" : "Warszawa",
            l.InterestRate));
    }

    public IEnumerable<Tuple<decimal, string>> GetStatistics()
    {
    		// here specific strategy method should be used
        var data = FirstDatabase.GetData().ToList();

        return data.Select(l => new Tuple<decimal, string>(l.LoanAmount, l.InterestRate.ToString()));
    }
}

public class FirstBankFinancialReportStrategy
{
    public Func<Loan, bool> GetPredicate() => loan => loan.LoanStartDate.Month >= 2021;

    public IEnumerable<Tuple<string, string, decimal>> GetMetrics(Func<Loan, bool> predicate)
    {
        string[] cities = { "Warszawa", "Kraków", "Gdañsk" };
        var loansLimited = FirstDatabase.GetData().Where(predicate).ToList();

        return new List<Tuple<string, string, decimal>>
        {
            new Tuple<string, string, decimal>("18-30", "Warszawa", loansLimited
                .Where(GetWarsawMetricPredicateForYoung())
                .Average(l => l.InterestRate)),
            new Tuple<string, string, decimal>("31-50", "Warszawa", loansLimited
                .Average(l => l.InterestRate)),
            new Tuple<string, string, decimal>("51-70", "Warszawa", loansLimited
                .Average(l => l.InterestRate))
        };
    }

    public IQueryable<Tuple<decimal, string>> GetStatistics(Func<Loan, bool> predicate)
    {
        var data = FirstDatabase.GetData().Where(predicate).ToList();

        return data.Select(l => new Tuple<decimal, string>(l.LoanAmount, l.InterestRate.ToString())).AsQueryable();
    }

    private static Func<Loan, bool> GetWarsawMetricPredicateForYoung() =>
        l => l.Account.Customer.CustomerAddress.Contains("Warszawa") &&
            DateTime.UtcNow.Year - l.Account.Customer.DateOfBirth.Year >= 18 &&
            DateTime.UtcNow.Year - l.Account.Customer.DateOfBirth.Year <= 30;
}

public class SecondBankFinancialReportStrategy
{
    public Func<Loan, bool> GetPredicate() => loan => loan.LoanStartDate.Month >= 2021;

    public IEnumerable<Tuple<string, string, decimal>> GetMetrics(Func<Loan, bool> predicate)
    {
        string[] cities = { "Warszawa", "Kraków", "Gdañsk" };
        var loansLimited = SecondDatabase.GetData().Where(predicate).Where(l => l.InterestRate <= 20).AsQueryable();

        return cities.SelectMany(c => new List<Tuple<string, string, decimal>>
        {
            new Tuple<string, string, decimal>("2020", c, loansLimited
                .Where(GetMetricPredicate(c, 2020))
                .Average(l => l.InterestRate)),
            new Tuple<string, string, decimal>("2021", c, loansLimited
                .Where(GetMetricPredicate(c, 2021))
                .Average(l => l.InterestRate)),
            new Tuple<string, string, decimal>("2022", c, loansLimited
                .Where(GetMetricPredicate(c, 2022))
                .Average(l => l.InterestRate))
        });
    }

    public IEnumerable<Tuple<decimal, string>> GetStatistics(Func<Loan, bool> predicate)
        => SecondDatabase.GetData().Where(predicate).AsQueryable();

    private static Func<Loan, bool> GetMetricPredicate() =>
        l => l.BankEmployee.BankDepartment.PlacementCity.Contains("Warszawa") &&
            l.LoanStartDate.Year == 2022;
}