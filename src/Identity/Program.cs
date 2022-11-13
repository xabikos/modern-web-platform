using Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddIdentityServer(options => 
	{
		// https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
		options.EmitStaticAudienceClaim = true;
		//options.UserInteraction.LoginUrl = ""
	})
	.AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
	.AddInMemoryApiScopes(IdentityConfig.ApiScopes)
	.AddInMemoryClients(IdentityConfig.Clients);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();


//app.MapGet("/", () => "Hello World!");

app.Run();
