using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transience.Services;

namespace Transience
{
    class Transience
    {
        static void Main(string[] args)
            => new Transience().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml")
                .Build();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            await services.GetRequiredService<CommandHandler>().InstallCommandsAsync(services);

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(_config)
                .AddSingleton(new LiteDatabase("bot.db"))
                // Add additional services here...
                .BuildServiceProvider();
        }
    }
}
