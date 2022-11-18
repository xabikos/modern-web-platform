//using Duende.Bff.EntityFramework;
using NextjsStaticHosting.AspNetCore;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Step 1: Add Next.js hosting support
builder.Services.Configure<NextjsStaticHostingOptions>(builder.Configuration.GetSection("NextjsStaticHosting"));
builder.Services.AddNextjsStaticHosting();

// Add BFF services to DI - also add server-side session management
builder.Services.AddBff(options =>
{
    options.ManagementBasePath = "/bff";
});
// Stores the session in a peramanent store instead of the cookie https://docs.duendesoftware.com/identityserver/v6/bff/session/server_side_sessions/  
//.AddEntityFrameworkServerSideSessions(options => { });

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultScheme = "cookie";
		options.DefaultChallengeScheme = "oidc";
		options.DefaultSignOutScheme = "oidc";
	})
	.AddCookie("cookie", options =>
	{
		// set session lifetime
		options.ExpireTimeSpan = TimeSpan.FromHours(24);
		// sliding or absolute
		options.SlidingExpiration = false;
		// host prefixed cookie name
		options.Cookie.Name = "__PublicWebApp-spa";
		// strict SameSite handling
		options.Cookie.SameSite = SameSiteMode.Strict;
	})
	.AddOpenIdConnect("oidc", options =>
	{
		// The URL of the identity server
		options.Authority = "https://localhost:44342";
		// confidential client using code flow + PKCE
		options.ClientId = "publicwebapp";
		options.ClientSecret = "secret";
		options.ResponseType = "code";

		// query response type is compatible with strict SameSite mode
		options.ResponseMode = "query";

		// get claims without mappings
		options.MapInboundClaims = false;
		options.GetClaimsFromUserInfoEndpoint = true;

		// save tokens into authentication session to enable automatic token management
		options.SaveTokens = true;

		// request scopes
		options.Scope.Clear();
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.Scope.Add("core_domain_API");

		// and refresh token
		options.Scope.Add("offline_access");
	});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0#serve-default-documents
//app.UseDefaultFiles();
//app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseBff();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapBffManagementEndpoints();
	endpoints.MapControllers().AsBffApiEndpoint().SkipAntiforgery().RequireAuthorization();

	// Step 2: Register dynamic endpoints to serve the correct HTML files at the right request paths.
	// Endpoints are created dynamically based on HTML files found under the specified RootPath during startup.
	// Endpoints are currently NOT refreshed if the files later change on disk.
    endpoints.MapNextjsStaticHtmls();
});

// Step 3: Serve other required files (e.g. js, css files in the exported next.js app).
app.UseNextjsStaticHosting();

app.Run();
