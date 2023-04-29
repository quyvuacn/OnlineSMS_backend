using Microsoft.EntityFrameworkCore;
using OnlineSMS.Data;
using OnlineSMS;
using OnlineSMS.Controllers;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<OnlineSMSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineSMSContext") ?? throw new InvalidOperationException("Connection string 'OnlineSMSContext' not found.")));

// Add SignalR 
builder.Services.AddSignalR();

//My Services
builder.Services.AddServices(configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Add Cors
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .WithOrigins("http://localhost:3000")
           .AllowAnyHeader()
           .AllowCredentials();
}));




var app = builder.Build();


//UseCors
app.UseCors("MyPolicy");

//Seed data

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
});

app.UseHttpsRedirection();



app.MapControllers();



app.Run();
