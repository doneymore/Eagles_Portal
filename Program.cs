using Eagles_Portal.contracts.Interface;
using Eagles_Portal.contracts.services;
using Eagles_Portal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // ✅ This line avoids schemaId collisions
    c.CustomSchemaIds(type => type.FullName);

    // Other Swagger config
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eagles Portal", Version = "v1" });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<SmsConfiguration>(
    builder.Configuration.GetSection("SmsConfiguration"));

builder.Services.AddHttpClient<INgBulk, NgBulkService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ITwilioBulkSms, TwilioBulkSms>();
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
