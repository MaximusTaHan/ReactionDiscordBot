using Remora.Discord.API;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace ReactionDiscordBot
{
    public class ReactionHandler : IResponder<IMessageReactionAdd>, IResponder<IMessageReactionRemove>
    {
        private readonly IDiscordRestGuildAPI _guildAPI;

        //There is surely a beter place to store a dictionary of preset server roles and their associated emojis
        private readonly Dictionary<string, ulong> _serverRoles = new() { { "✅", 1001464786015502356 }, { "👏", 1002183685359009872 } };
        public ReactionHandler(IDiscordRestGuildAPI guildAPI)
        {
            _guildAPI = guildAPI;
        }

        public async Task<Result> RespondAsync(IMessageReactionAdd gatewayEvent, CancellationToken ct = default)
        {
            var roleMessage = DiscordSnowflake.New(1002163233190322236);
            string? emoji = gatewayEvent.Emoji.Name.Value;

            if (gatewayEvent.MessageID != roleMessage || !_serverRoles.ContainsKey(emoji))
            {
                return Result.FromSuccess();
            }

            //Check if emoji exists and fetches assosiated roleId
            _serverRoles.TryGetValue(emoji, out ulong roleId);

            var role = DiscordSnowflake.New(roleId);

            //Sets role on Server member 
            return await _guildAPI.AddGuildMemberRoleAsync
                (
                gatewayEvent.GuildID.Value,
                gatewayEvent.UserID,
                role,
                ct: ct
                );
        }

        public async Task<Result> RespondAsync(IMessageReactionRemove gatewayEvent, CancellationToken ct = default)
        {
            var roleMessage = DiscordSnowflake.New(1002163233190322236);
            var emoji = gatewayEvent.Emoji.Name.Value;

            if (gatewayEvent.MessageID != roleMessage)
            {
                return Result.FromSuccess();
            }

            _serverRoles.TryGetValue(emoji, out ulong roleId);

            var role = DiscordSnowflake.New(roleId);

            return await _guildAPI.RemoveGuildMemberRoleAsync
                (gatewayEvent.GuildID.Value,
                gatewayEvent.UserID,
                role,
                ct: ct
                );
        }
    }
}