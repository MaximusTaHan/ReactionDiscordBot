# ReactionDiscordBot

This is a demo app for a discord bot that handles roles on a server. It does so by listening to user reactions on a specific message.
The emojis and their corresponding roles are hard coded into the ReactionHandler for this demo.

The app is entierly built on the Remora.Discord library: https://github.com/Nihlus/Remora.Discord

The library gets you as close to the actual discord API as possible which makes it very easy to implement your own functionality with just some understanding of Discords documentation.

## Configurations and setup
In ConfigureServices you need to register the intents of your bot, this tells Discords end what events to send to you. 
(List of intents and what they cover: https://discord.com/developers/docs/topics/gateway#gateway-intents)

For this bot we need the "GuildMessageReactions" and "GuildMembers" intent. 
The first gets sent an event when a server member reacts to any message, this means you have to make conditions for when to send back a response.
Discord provides almost everything with a unique id, so we can look for when a reaction happened on a chosen message.
The native emotes are one of the things that doesnt have an id so we have to use its Unicode value (Discord name: `:white_check_mark:` Unicode: ✅).

Then we register the ReactionHandler with the AddResponder method. 
This adds a responder to the service collection which in turn allows you to use all the IResponder implementations it supports.

## ReactionHandler
This class implements two IResponder interfaces, IMessageReactionAdd and IMessageReactionRemove. These two methods recieve the gatewayEvents when a reaction is added or removed.
From these events we can check if they happened on the expected message with a valid emote. If not then we discard the event with a Result.FromSuccess to keep the app from crashing.

We inject an interface IDiscordRestGuildAPI which represents several endpoints for affecting Guild related objects. (A Guild is what is usually refered to as a Server. https://discord.com/developers/docs/resources/guild)
"AddGuildMemberRoleAsync" and "RemoveGuildMemberRoleAsync" are used here passing in the required parameters and Remora.Discord takes care of the rest.

I also put the dictionary here that holds the association between emoji and role.

## Shoutouts
Shoutout to the Remora.Discord discord server for helping me understand their library. It was really a treat to use and the level of detail gone into this library really shows.

Specifically thanks to Nihlus (aka Jax on discord) for updating the getting started guide within minutes of me bumping into an issue. 
