
namespace DnKR.AmeliaBot;

internal class Program
{
    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("Write the bot token");
            return;
        }
        MainAsync(args[0]).GetAwaiter().GetResult();
    }

    static async Task MainAsync(string token)
    {
        var bot = new Bot(token);
        await bot.RunAsync();
        await Task.Delay(-1);
    }
}
