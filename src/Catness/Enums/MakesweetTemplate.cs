using Discord.Interactions;

namespace Catness.Enums;

public enum MakesweetTemplate
{
    [ChoiceDisplay("Heart Locket")] HeartLocket,
    [ChoiceDisplay("Flying Bear")] FlyingBear,
    [ChoiceDisplay("Flag")] Flag,
    [ChoiceDisplay("Billboard City")] BillboardCity,
    [ChoiceDisplay("Nesting Doll")] NestingDoll,
    [ChoiceDisplay("Circuit Board")] CircuitBoard
}

public static class MakesweetTemplateExtensions
{
    public static string GetMakesweetURL(this MakesweetTemplate template)
    {
        string returnString = template switch
        {
            MakesweetTemplate.HeartLocket => "heart-locket",
            MakesweetTemplate.FlyingBear => "flying-bear",
            MakesweetTemplate.Flag => "flag",
            MakesweetTemplate.BillboardCity => "billboard-cityscape",
            MakesweetTemplate.NestingDoll => "nesting-doll",
            MakesweetTemplate.CircuitBoard => "circuit-board",
            _ => throw new ArgumentOutOfRangeException(nameof(template), template, null)
        };
        return returnString;
    }
}