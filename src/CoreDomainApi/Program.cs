using Common;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Register the IOptions object
builder.Services.Configure<ServicesConfiguration>(
	builder.Configuration.GetSection("Services"));
// Explicitly register the settings object by delegating to the IOptions object
builder.Services.AddSingleton(resolver =>
		resolver.GetRequiredService<IOptions<ServicesConfiguration>>().Value);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders =
		ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer(options =>
	{
		options.Authority = builder.Configuration.GetRequiredSection("Services:Idendity:Url").Value;
		options.TokenValidationParameters.ValidateAudience = false;
	});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseForwardedHeaders();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
