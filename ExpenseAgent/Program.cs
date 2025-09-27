using Common.Abstractions;
using Data;
using ExpenseAgent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

var builder = Host.CreateApplicationBuilder(args);

// Register ExpensesDbContext with PostgreSQL provider
builder.Services.AddDbContext<ExpensesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITransactionRepository, EfTransactionRepository>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
