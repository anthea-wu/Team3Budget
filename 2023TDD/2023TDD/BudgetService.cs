using System;
using System.Collections.Generic;
using System.Linq;

namespace _2023TDD;

public class Period
{
    public Period(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public DateTime Start { get; }
    public DateTime End { get; }

    public int OverlappingDays(Period another)
    {
        var last = End < another.End ? End : another.End;
        var first = Start > another.Start ? Start : another.Start;
        return (last - first).Days + 1;
    }
}

public class BudgetService
{
    private readonly IBudgetRepo _budgetRepo;

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end) return 0;

        var period = new Period(start, end);
        return _budgetRepo.GetAll().Sum(budget => budget.TotalAmount(period));
    }
}

public interface IBudgetRepo
{
    List<Budget> GetAll();
}

public class Budget
{
    public string YearMonth { get; set; }
    public int Amount { get; set; }

    public int GetDailyBudget()
    {
        return Amount / Days();
    }

    private int Days()
    {
        var daysInMonth = DateTime.DaysInMonth(FirstDay().Year, FirstDay().Month);
        return daysInMonth;
    }

    public DateTime FirstDay()
    {
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        return firstDay;
    }

    public DateTime LastDay()
    {
        return new DateTime(FirstDay().Year, FirstDay().Month, Days());
    }

    public int TotalAmount(Period period)
    {
        var queryDays =
            period.OverlappingDays(new Period(FirstDay(), LastDay()));
        var totalAmount = GetDailyBudget() * queryDays;
        return totalAmount;
    }
}