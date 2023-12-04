using System.Diagnostics.CodeAnalysis;
using Hw11.Configuration;
using Hw11.Exceptions;
using Hw11.Services.ExpressionBuilder;
using Hw11.Services.GraphBuilder;
using Hw11.Services.Parser;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddMathCalculator();
builder.Services.AddTransient<IExceptionHandler, ExceptionHandler>();
builder.Services.AddScoped<IExpressionBuilder, ExpressionBuilder>();
builder.Services.AddScoped<IGraphBuilder, GraphBuilder>();
builder.Services.AddScoped<IStringExpressionParser, StringExpressionParser>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Calculator}/{id?}");

app.Run();

namespace Hw11
{
    [ExcludeFromCodeCoverage]
    public partial class Program { }
}