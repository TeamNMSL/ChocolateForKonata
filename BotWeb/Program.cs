namespace BotWeb {
    public class BWB
    {
        static public void Main(string[] args) {
            
        }
        static public void start()
        {
            try
            {
                var builder = WebApplication.CreateBuilder();

                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                //app.UseHttpsRedirection();
                app.Urls.Add("http://192.168.0.114:11451/");
                app.UseAuthorization();
                
                app.MapControllers();

                app.Run();
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }

        }

    }

}