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
            if (current.ToString("yyyyMM") == start.ToString("yyyyMM"))
                queryDays = DateTime.DaysInMonth(current.Year, current.Month) - current.Day + 1;
            else if (current.ToString("yyyyMM") == end.ToString("yyyyMM"))
                queryDays = end.Day;
            else
                queryDays = DateTime.DaysInMonth(current.Year, current.Month);

            var budget = budgets.FirstOrDefault(x => x.YearMonth == current.ToString("yyyyMM"));
            if (budget != null) totalBudget += budget.GetDailyBudget() * queryDays;
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
        var budgetDate = DateTime.ParseExact(YearMonth, "yyyyMM", null);
        var dailyBudget = Amount / DateTime.DaysInMonth(budgetDate.Year, budgetDate.Month);
        return dailyBudget;
    }
}