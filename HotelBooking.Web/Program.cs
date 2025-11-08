using HotelBooking.Web.Components;
using HotelBooking.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configuration spécifique pour Blazor Server avec authentification
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
    });

// Configurer le HttpClient pour l'API
builder.Services.AddHttpClient("HotelBookingAPI", client => 
{
    client.BaseAddress = new Uri("https://localhost:7186/");
});

// Enregistrer les services
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHotelAdminService, HotelAdminService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Ajouter l'authentification
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Ajouter l'authentification middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
