using WatchDb.DataAccess;
using WatchDb.DataAccess.Repositories;
using WatchDb.DataAccess.Repositories.SQL;
using WatchUILibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DBConfig>(services =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    return new DBConfig
    {
        //ConnectionString = configuration.GetConnectionString("DefaultConnection")
        ConnectionString = configuration.GetConnectionString("SQLiteConnection")
    };
});


//builder.Services.AddScoped<DbContext, SQLDbContext>();
builder.Services.AddScoped<DbContext, SQLiteDbContext>();
builder.Services.AddScoped<ShopDbContext>();



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
