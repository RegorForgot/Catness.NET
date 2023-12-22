using Catness.Utilities;
using Discord;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Catness.Extensions;

public static class PaginatorExtensions
{
    public static StaticPaginator BuildStaticPaginator(
        this IList<PageBuilder> pageBuilder,
        IInteractionContext context,
        bool userOnly = true,
        ActionOnStop actionOnTimeout = ActionOnStop.DeleteInput)
    {
        StaticPaginatorBuilder builder = new StaticPaginatorBuilder()
            .WithPages(pageBuilder)
            .WithActionOnTimeout(actionOnTimeout)
            .WithOptions(EmoteCollection.PaginatorButtons);

        if (userOnly)
        {
            builder = builder.WithUsers(context.User);
        }

        return builder.Build();
    }
}