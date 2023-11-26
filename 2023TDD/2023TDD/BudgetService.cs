using System;
using System.Collections.Generic;
using System.Linq;

namespace _2023TDD;

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
        var budgets = _budgetRepo.GetAll();

        var current = start;

        var totalBudget = 0;


        if (start.ToString("yyyyMM") == end.ToString("yyyyMM"))
        {
            var queryDays = end.Day - start.Day + 1;
            return budgets.FirstOrDefault(x => x.YearMonth == $"{start.Year}{start.Month.ToString("00")}")
                .GetDailyBudget() * queryDays;
        }

        while (current < new DateTime(end.Year, end.Month, 1).AddMonths(1))
        {
            int queryDays;
            var budget = budgets.FirstOrDefault(x => x.YearMonth == current.ToString("yyyyMM"));

            if (budget != null)
            {
                if (budget.YearMonth == start.ToString("yyyyMM"))
                {
                    var last = budget.LastDay();
                    var first = current;
                    queryDays = (last - first).Days + 1;
                }
                else if (budget.YearMonth == end.ToString("yyyyMM"))
                {
                    queryDays = end.Day;
                }
                else
                {
                    queryDays = DateTime.DaysInMonth(current.Year, current.Month);
                }

                totalBudget += budget.GetDailyBudget() * queryDays;
            }

            current = current.AddMonths(1);
        }

        return totalBudget;
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

    private DateTime FirstDay()
    {
        var firstDay = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        return firstDay;
    }

    public DateTime LastDay()
    {
        return new DateTime(FirstDay().Year, FirstDay().Month, Days());
    }
}