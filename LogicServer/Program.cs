using LogicServer.DataBase;
using LogicServer.EnemyPull;
using LogicServer.FightCalc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DnDDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetRequiredSection("DataBase")["ConnectionString"]));
builder.Services.AddScoped<IEnemyPull, EnemyPull>();
builder.Services.AddScoped<IAttackAttemptCalculator, AttackAttemptCalculator>();
builder.Services.AddScoped<IFightCalculator, FightCalculator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
