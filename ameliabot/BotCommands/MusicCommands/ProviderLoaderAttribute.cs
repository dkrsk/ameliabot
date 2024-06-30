namespace DnKR.AmeliaBot.BotCommands.MusicCommands;


/// <summary>
/// Method with this attribute have to be <see cref="CommonContext"></see> -> <see cref="string"></see> -> <see cref="bool"></see>
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class ProviderLoaderAttribute : Attribute
{

}
