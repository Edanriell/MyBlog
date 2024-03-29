using Data.Models.Interfaces;
using Microsoft.AspNetCore.ResponseCompression;
using Data;
using BlazorWebAssembly.Server.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BlazorWebAssembly.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services
    .AddOptions<BlogApiJsonDirectAccessSetting>()
    .Configure(options =>
    {
        options.DataPath = @"..\Data\";
        options.BlogPostsFolder = "Blogposts";
        options.TagsFolder = "Tags";
        options.CategoriesFolder = "Categories";
    });
builder.Services.AddScoped<IBlogApi, BlogApiJsonDirectAccess>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        JwtBearerDefaults.AuthenticationScheme,
        c =>
        {
            c.Authority = builder.Configuration["Auth0:Authority"];
            c.TokenValidationParameters =
                new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudience = builder.Configuration["Auth0:Audience1"],
                    ValidIssuer = builder.Configuration["Auth0:Authority1"]
                };
        }
    );
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapCategoryApi();
app.MapBlogPostApi();
app.MapTagApi();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHub<BlogNotificationHub>("/BlogNotificationHub");
app.Run();
