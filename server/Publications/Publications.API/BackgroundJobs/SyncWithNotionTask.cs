using Coravel.Invocable;
using Publications.API.Repositories;

namespace Publications.API.BackgroundJobs;

public class SyncWithNotionTask: IInvocable
{
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IPublicationsSourceRepository _sourceRepository;

    public SyncWithNotionTask(
        IPublicationsRepository publicationsRepository, 
        IPublicationsSourceRepository sourceRepository)
    {
        _publicationsRepository = publicationsRepository;
        _sourceRepository = sourceRepository;
    }

    public async Task Invoke()
    {
        var publications = await _sourceRepository.GetPublicationsAsync();
        await _publicationsRepository.InsertOrUpdateAsync(publications);
    }
}