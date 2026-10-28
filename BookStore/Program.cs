using BookStore.Business.Services;
using BookStore.Business.Services.IServices;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// This is the connection string for the database, which is stored in the appsettings.json file.
// The connection string is used to connect to the database.
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:SQLConnection").Value);
});
// Register the CategoryService and ProductService with the dependency injection container.
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// This is a temporary workaround to allow the use of the unload event in the browser,
// which is blocked by default due to security reasons. The unload event is used to perform
// cleanup tasks when a user navigates away from a page or closes a tab. By adding this header,
// it's allowing the unload event to be used in this application.
// https://learn.microsoft.com/en-ie/answers/questions/5975894/violation-permissions-policy-violation-unload-is-n
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Permissions-Policy", "unload=*");
    await next();
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
// 
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Customer" })
    .WithStaticAssets();


app.Run();
