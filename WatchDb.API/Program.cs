using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WatchDb.DataAccess;
using WatchDb.DataAccess.Repositories;
using WatchDb.DataAccess.Repositories.SQL;
using WatchUILibrary;
using ConfigurationManager = WatchDb.API.ConfigurationManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = ConfigurationManager.Configuration["JWT:ValidIssuer"],

            ValidateAudience = true,
            ValidAudience = ConfigurationManager.Configuration["JWT:ValidAudience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.Configuration["JWT:Secret"]))
        };
    });

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

app.UseCors(x => x
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true)
                  .AllowCredentials());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
