using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands;
using System.ComponentModel;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;

namespace DSPTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            String discordToken = "";
            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(discordToken, DiscordIntents.All);
            builder.ConfigureEventHandlers
            (
                b => b.HandleMessageCreated(async (s, e) =>
                {
                    
                    //This outputs as expected.
                    
                    DiscordMember member = await e.Guild.GetMemberAsync(e.Author.Id);
                    DiscordPermissions perms = member.Permissions;

                    if (perms.HasPermission(DiscordPermission.BanMembers))
                    {
                        Console.WriteLine("You have ban permissions!");
                    }
                    else
                    {
                        Console.WriteLine("You do not have ban permissions!");
                    }
                })
            );

            builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
            {
                extension.AddCommands([typeof(Ban)]);
                TextCommandProcessor textCommandProcessor = new();
            });

            DiscordClient client = builder.Build();

            await builder.ConnectAsync();
            await Task.Delay(-1);
        }
    }

    public class Ban
    {

        //This also works as expected. I changed BanMembers to administrator and my alt no longer had the command. 
        //Changing it back made it re-appear.
        //I also removed and re-added the app. No integration overrides. 

        [RequirePermissions([DiscordPermission.BanMembers])]
        [Command("Ban"), Description("Bans the user"), ]
        public static async ValueTask ExecuteAsync(SlashCommandContext context,
            [Description("The user to ban")] DiscordMember target)
        {
            DiscordMember interactionUser = (DiscordMember)context.User;
            await context.DeferResponseAsync();

            //Just print debug message
            Console.WriteLine($"User {interactionUser.Username}#{interactionUser.Discriminator} is trying to ban {target.Username}#{target.Discriminator}");
            await context.FollowupAsync("Banned the user " + target.DisplayName);
        }
    }
}
