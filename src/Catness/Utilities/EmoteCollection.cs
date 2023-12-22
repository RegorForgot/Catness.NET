using Discord;

namespace Catness.Utilities;

public static class EmoteCollection
{
    public static readonly IDictionary<string, IEmote> CatnessEmotes = new Dictionary<string, IEmote>
    {
        { "edit", Emote.Parse("<:edit:1062784953399648437>") },
        { "regen", Emote.Parse("<:regen:1062784969749045318>") },
        { "download", Emote.Parse("<:download:1062784992243105813>") },
        { "spotify_white", Emote.Parse("<:spotify_white:1062784981203689472>") },
        { "bookmark", Emote.Parse("<:bookmark:1062790109491114004>") },
        { "remove", Emote.Parse("<:remove:1062791085404999781>") },
        { "active_developer", Emote.Parse("<:Active_Developer:1078093417680224356>") },
        { "supports_commands", Emote.Parse("<:Supports_Commands:1078022481144729620>") },
        { "bug_hunter", Emote.Parse("<:Bug_Hunter:1078015408684142743>") },
        { "bug_hunter_2", Emote.Parse("<:Bug_Hunter_Level_2:1078015437285097542>") },
        { "discord_mod_alum", Emote.Parse("<:Discord_Mod_Alumni:1078091018886451281>") },
        { "early_supporter", Emote.Parse("<:Early_Supporter:1078015810909524149>") },
        { "verified_bot", Emote.Parse("<:Verified_Bot:1078090782118002719>") },
        { "verified_bot_dev", Emote.Parse("<:Verified_Bot_Developer:1078094815398461450>") },
        { "hypesquad_event", Emote.Parse("<:HypeSquad_Event:1078014370912686180>") },
        { "hypesquad_balance", Emote.Parse("<:HypeSquad_Balance:1078014406396485642>") },
        { "hypesquad_bravery", Emote.Parse("<:HypeSquad_Bravery:1078014366953242794>") },
        { "hypesquad_brilliance", Emote.Parse("<:HypeSquad_Brilliance:1078014368454820010>") },
        { "discord_partner", Emote.Parse("<:Discord_Partner:1078021591859986463>") },
        { "discord_staff", Emote.Parse("<:Discord_Staff:1078015590683390093>") },
        { "lastfm", Emote.Parse("<:lastfm:1080282189100482693>") },
        { "steam", Emote.Parse("<:steam:1080281878847832186>") },
        { "contributor", Emote.Parse("<:Contributor:1078661797185335398>") },
        { "back", Emote.Parse("<:back:1101510067880202301>") },
        { "page_left", Emote.Parse("<:page_left:1112880577390055544>") },
        { "page_right", Emote.Parse("<:page_right:1112880675067015188>") },
        { "close", Emote.Parse("<:close:1175843411576754277>") },
        { "special", Emote.Parse("<:Special:1078664371661713449>") },
        { "bot", Emote.Parse("<:bot:1078091845051088979>") },
        { "nitro", Emote.Parse("<:nitro:1078094211351584928>") },
        { "github", Emote.Parse("<:github:1114661850118897806>") },
        { "confirm", Emote.Parse("<:confirm:1175842297481547889>") }
    };
}