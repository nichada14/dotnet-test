var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "login",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.MapControllerRoute(
    name: "register",
    pattern: "{controller=Account}/{action=Register}/{id?}");
app.MapControllerRoute(
    name: "home",
    pattern: "{controller=Account}/{action=Home}/{id?}");
app.MapControllerRoute(
    name: "index",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
