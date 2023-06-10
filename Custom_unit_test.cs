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

    // Add custom metric and city
    metrics.Add(new Tuple<string, string, decimal>("CustomMetric", "CustomCity", 9));

    return metrics;
}
