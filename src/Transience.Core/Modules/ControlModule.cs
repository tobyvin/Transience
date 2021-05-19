using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LiteDB;
using Transience.Data;

namespace Transience.Modules
{
    public class ControlModule : ModuleBase<SocketCommandContext>
    {
        public LiteDatabase Database { get; set; }
    
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
}