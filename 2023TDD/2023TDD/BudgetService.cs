﻿using System;
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
            return GetDailyBudget(start, budgets) * queryDays;
        }

        while (current < new DateTime(end.Year, end.Month, 1).AddMonths(1))
        {
            if (current.ToString("yyyyMM") == start.ToString("yyyyMM"))
            {
                var queryDays = DateTime.DaysInMonth(current.Year, current.Month) - current.Day + 1;

                totalBudget += GetDailyBudget(current, budgets) * queryDays;
            }
            else if (current.ToString("yyyyMM") == end.ToString("yyyyMM"))
            {
                totalBudget += GetDailyBudget(end, budgets) * end.Day;
            }
            else
            {
                totalBudget += GetDailyBudget(current, budgets) *
                               DateTime.DaysInMonth(current.Year, current.Month);
            }

            current = current.AddMonths(1);
        }

        return totalBudget;
    }

    private static int GetDailyBudget(DateTime budgetTime, List<Budget> budgets)
    {
        var budget = budgets.FirstOrDefault(x => x.YearMonth == $"{budgetTime.Year}{budgetTime.Month.ToString("00")}");
        if (budget == null) return 0;
        var dailyBudget = budget.Amount / DateTime.DaysInMonth(budgetTime.Year, budgetTime.Month);
        return dailyBudget;
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
}