namespace Deepin.API; 
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDeepinChatAPI(builder.Configuration);

        var app = builder.Build();

        app.UseDeepinChatAPI();

        app.Run();
    }
}