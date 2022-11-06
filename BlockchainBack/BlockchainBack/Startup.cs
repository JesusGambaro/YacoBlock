namespace BlockchainBack;

public class Startup
{
    public void CreateHost(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //ALLOW FOR ALL ORIGINS, AUTHENTICATION, CERTIFICATES, HEADERS, METHODS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policyBuilder =>
                {
                    policyBuilder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(_ => _.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseMiddleware<UpdateMiddleware>();
        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}