using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

string authority = GetConfigurationValue("Authentication:Authority");
string audience = GetConfigurationValue("Authentication:Audience");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            ValidAudience = audience,
            ValidIssuer = authority
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated for: " + context.Principal.Identity.Name);
                return Task.CompletedTask;
            }
        };
    })
    ;
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("WeatherSecured", policy =>
    {
        policy.RequireRole("WeatherSecuredTools");
    });
});
builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly()
    .AddAuthorizationFilters();
var app = builder.Build();

app.MapGet("/health", () => "MCP Server is running");
app.MapMcp();
app.Run();
string GetConfigurationValue(string key)
{
    var configurationValue = builder.Configuration[key];
    if (string.IsNullOrEmpty(configurationValue))
    {
        throw new InvalidOperationException($"Configuration value for '{key}' is missing.");
    }
    return configurationValue;
}
