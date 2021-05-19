using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LiteDB;
using Transience.Data;

// Keep in mind your module **must** be public and inherit ModuleBase.
// If it isn't, it will not be discovered by AddModulesAsync!
// Create a module with no prefix
public class ControlModule : ModuleBase<SocketCommandContext>
{
    public LiteDatabase Database { get; set; }
    // The following example only requires the user to either have the
    // Administrator permission in this guild or own the bot application.
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    [Command("enable")]
    [Summary("Enables a Transient channel for a guild")]
    public Task EnableAsync()
    {
        foreach (var voiceChannel in Context.Guild.VoiceChannels)
        {
            var transientChannels = Database.GetCollection<TransientChannel>("channels");
            
            var transientChannel = new TransientChannel
            {
                Name = voiceChannel.Name,
                GuildId = voiceChannel.Guild.Id,
                VoiceId = voiceChannel.Id,
                TextId = 0
            };
            transientChannels.Insert(transientChannel);
        }
        return ReplyAsync($"Enabled for all channels.");
    }

    // TODO Impliment enable voice channel parameters
    public Task EnableAsync(
        [Summary("Name of the voice channel to enable")] long voiceChannel)
        => ReplyAsync($"Feature not yet implimented");

    // TODO Impliment enable voice channel by id
    private Task EnableTransience(long voiceChannel)
        => ReplyAsync($"Enabled {voiceChannel}");

    // TODO Impliment enable voice channel by name
    private Task EnableTransience(string voiceChannelName)
        => EnableTransience(0);

}