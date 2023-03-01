﻿using DSharpPlus;
using DSharpPlus.EventArgs;


namespace DnKR.AmeliaBot.BotCommands.ChatCommands;

public static class ChatEvents
{
    private static string[] censor =
    {
        "шлюха",
        "тварь",
        "блядь",
        "сука",
        "суки",
        "говноед",
        "говноеды",
        "обосрался",
        "обоссан",
        "обоссаны",
        "пидор",
        "пидоры",
        "соси",
        "быдло",
        "похуй",
        "нахуй",
        "пиздабол",
        "гандон",
        "ебать",
        "хуй",
        "пизда",
        "пидорас",
        "пидорасы",
        "чурка"
    };
    private static string[] shutup =
    {
        "заткнись",
        "заткнулся",
        "заткнулись",
        "завались",
        "закройся",
        "закрой пасть",
        "закрой рот"
    };

    public static async Task MessageCreated(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Author.IsBot)
            return;

        string content = args.Message.Content.ToLower();

        if (content.Split().Any(s => shutup.Contains(s)))
        {
            await args.Message.RespondAsync("Ты кого затыкаешь, сынок?");
            return;
        }

        if (content.Split().Any(s => censor.Contains(s)))
        {
            await args.Message.RespondAsync($"Слышь, {args.Author.Username}," +
                "что-то мне не нравится как ты со мной разговариваешь. " +
                "\nСкладывается впечатление что ты реально контуженный, обиженный жизнью имбицил )) " +
                "Могу тебе и в глаза сказать, готов приехать послушать? ) " +
                "\nВся та хуйня тобою написанное это простое пиздабольство, рембо ты комнатный) )" +
                "от того что ты много написал, жизнь твоя лучше не станет) ) " +
                "пиздеть не мешки ворочить, много вас таких по весне оттаяло )) " +
                "Про таких как ты говорят: Мама не хотела, папа не старался) " +
                "\nВникай в моё послание тебе< постарайся проанализировать и сделать выводы для себя)");
            return;
        }

        if (content.StartsWith("я"))
        {
            Random rnd = new(DateTime.Now.Millisecond);
            if (rnd.Next(100) > 60)
            {
                await args.Message.RespondAsync($"Привет, {args.Author.Username}, я Амелия)");
                return;
            }
        }

        if (content == "а" || content == "a")
        {
            await args.Message.RespondAsync("б");
            return;
        }

        if (content == "да" || content == "da")
        {
            Random rnd = new(DateTime.Now.Millisecond);
            if (rnd.Next(100) >= 65)
                await args.Message.RespondAsync("пизда)");
            return;
        }

        if (content == "нет" || content == "net")
        {
            Random rnd = new(DateTime.Now.Millisecond);
            if (rnd.Next(100) >= 65)
                await args.Message.RespondAsync("пидора ответ!");
            return;
        }

        if (content.Contains("люблю"))
        {
            Random rnd = new(DateTime.Now.Millisecond);
            if (rnd.Next(100) >= 30)
                await args.Message.RespondAsync("я тебя тоже люблю❤");
            return;
        }
    }
}
