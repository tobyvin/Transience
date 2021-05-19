using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LiteDB;
using Transience.Data;

namespace Transience.Services
{
    public class TransientHandler
    {
        private IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly LiteDatabase _database;
        private readonly OverwritePermissions _allowPerm = new OverwritePermissions(
            addReactions: PermValue.Allow, viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
            embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow,
            mentionEveryone: PermValue.Allow, useExternalEmojis: PermValue.Allow);

        public TransientHandler(IServiceProvider provider, DiscordSocketClient discord, LiteDatabase database)
        {
            _discord = discord;
            _provider = provider;
            _database = database;

            _discord.UserVoiceStateUpdated += VoiceStateUpdated;
        }

        private async Task VoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            Console.WriteLine($"VoiceStateUpdate: {user} - {before.VoiceChannel?.Name ?? "null"} -> {after.VoiceChannel?.Name ?? "null"}");
            var voiceChannel = after.VoiceChannel ?? before.VoiceChannel.Guild.GetVoiceChannel(before.VoiceChannel.Id);
            _ = UpdateTransientChannel(user, voiceChannel);
        }

        private async Task UpdateTransientChannel(SocketUser user, SocketVoiceChannel voiceChannel)
        {
            var transientChannels = _database.GetCollection<TransientChannel>("channels");
            var transientChannel = transientChannels.FindOne(tc => tc.VoiceId == voiceChannel.Id
                                                                   && tc.GuildId == voiceChannel.Guild.Id);

            if (transientChannel == null)
                return;

            var textChannel = (ITextChannel)voiceChannel.Guild.GetTextChannel(transientChannel.TextId);

            if (voiceChannel.Users.Count == 0)
            {
                if (textChannel != null)
                    await textChannel.DeleteAsync();
                transientChannel.TextId = 0;
            }
            else
            {
                if (textChannel == null)
                {
                    textChannel = await voiceChannel.Guild.CreateTextChannelAsync(transientChannel.Name, properties =>
                    {
                        properties.Topic = $"Temporary text chat for {voiceChannel.Name}";
                        properties.CategoryId = voiceChannel.CategoryId;
                    });
                    OverwritePermissions.DenyAll(textChannel);

                    transientChannel.TextId = textChannel.Id;
                }

                if (voiceChannel.GetUser(user.Id) == null)
                    await textChannel.RemovePermissionOverwriteAsync(user);
                else
                    await textChannel.AddPermissionOverwriteAsync(user, _allowPerm);
            }


            transientChannels.Update(transientChannel);
        }
    }
}