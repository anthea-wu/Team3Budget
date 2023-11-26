using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace _2023TDD;

public class BudgetServiceTests
{
    private IBudgetRepo _budgetRepo;
    private BudgetService _budgetService;

    [Test]
    public void query_single_day()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            }
        });

        var queryDate = new DateTime(2023, 11, 25);
        Assert.AreEqual(10, _budgetService.Query(queryDate, queryDate));
    }

    [Test]
    public void query_multi_days_in_same_month()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            }
        });

        var start = new DateTime(2023, 11, 25);
        var end = new DateTime(2023, 11, 27);
        Assert.AreEqual(30, _budgetService.Query(start, end));
    }

    [Test]
    public void query_multi_days_in_different_month()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            },
            new()
            {
                YearMonth = "202310",
                Amount = 620
            }
        });

        var start = new DateTime(2023, 10, 30);
        var end = new DateTime(2023, 11, 3);
        Assert.AreEqual(70, _budgetService.Query(start, end));
    }

    [Test]
    public void query_multi_days_across_three_month()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            },
            new()
            {
                YearMonth = "202310",
                Amount = 620
            },
            new()
            {
                YearMonth = "202309",
                Amount = 900
            }
        });

        var start = new DateTime(2023, 09, 30);
        var end = new DateTime(2023, 11, 3);
        Assert.AreEqual(680, _budgetService.Query(start, end));
    }

    [Test]
    public void query_multi_days_with_null_budget_setting()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202311",
                Amount = 300
            },
            new()
            {
                YearMonth = "202309",
                Amount = 900
            }
        });

        var start = new DateTime(2023, 09, 30);
        var end = new DateTime(2023, 11, 3);
        Assert.AreEqual(60, _budgetService.Query(start, end));
    }

    [Test]
    public void query_multi_days_across_year()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202312",
                Amount = 310
            },
            new()
            {
                YearMonth = "202401",
                Amount = 620
            }
        });

        var start = new DateTime(2023, 12, 30);
        var end = new DateTime(2024, 01, 1);
        Assert.AreEqual(40, _budgetService.Query(start, end));
    }

    [Test]
    public void no_overlap()
    {
        GivenBudgetFromRepo(new List<Budget>
        {
            new()
            {
                YearMonth = "202312",
                Amount = 310
            },
            new()
            {
                YearMonth = "202401",
                Amount = 620
            }
        });

        var start = new DateTime(2024, 3, 30);
        var end = new DateTime(2024, 4, 1);
        Assert.AreEqual(0, _budgetService.Query(start, end));
    }


    [Test]
    public void query_invalid_query_day()
    {
        var start = new DateTime(2023, 11, 4);
        var end = new DateTime(2023, 11, 3);
        Assert.AreEqual(0, _budgetService.Query(start, end));
    }

    [SetUp]
    public void SetUp()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _budgetService = new BudgetService(_budgetRepo);
    }

    private void GivenBudgetFromRepo(List<Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }
}