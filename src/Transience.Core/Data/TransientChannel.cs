namespace Transience.Data
{
    public class TransientChannel
    {
        public string Name { get; set; }
        public ulong GuildId { get; set; }
        public ulong VoiceId { get; set; }
        public ulong TextId { get; set; } = 0;
    }
}
