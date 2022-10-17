using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NeoContact.Data;
using NeoContact.Helpers;
using NeoContact.Models;
using NeoContact.Services;
using NeoContact.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Lesson #04 PostgreSQL Database Setup
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//ADD Lesson #04 PostgreSQL Database Setup
//var connectionString = builder.Configuration.GetSection("pgSettings")["pgConnection"];

//UPDATE Lesson #55 Connection Helper (Local vs Production)
var connectionString = ConnectionHelper.GetConnectionstring(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseSqlServer(connectionString));
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//ADD SERVICES
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IAddressBookService, AddressBookService>();
builder.Services.AddScoped<IEmailSender, EmailService > ();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

//ADD Lesson #56 Data Helper
var scope = app.Services.CreateScope();
//get database update with the latest migration
await DataHelper.ManageDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//ADD Lesson #51 Custom Error Page
app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
