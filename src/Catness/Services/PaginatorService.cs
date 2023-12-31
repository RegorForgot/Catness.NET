using Catness.Extensions;
using Discord;
using Fergun.Interactive;

namespace Catness.Services;

public class PaginatorService
{
    private readonly InteractiveService _interactiveService;

    public PaginatorService(
        InteractiveService interactiveService)
    {
        _interactiveService = interactiveService;
    }

    public async Task SendPaginator(IList<PageBuilder> pageBuilders, IInteractionContext context)
    {
        await _interactiveService.SendPaginatorAsync(pageBuilders.BuildStaticPaginator(context), 
            context.Interaction);
    }
}