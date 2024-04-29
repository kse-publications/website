using Microsoft.Extensions.Logging;
using Publications.Application.Repositories;
using Publications.BackgroundJobs.Abstractions;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Domain.Shared;

namespace Publications.BackgroundJobs;

public class UpdateResourceViewsTask: BaseLoggableTask<UpdateResourceViewsTask>
{
    private readonly IEntityRepository<Publication> _publicationsRepository;
    private readonly IEntityRepository<Publisher> _publishersRepository;
    private readonly IEntityRepository<Author> _authorsRepository;
    private readonly IRequestsRepository _requestsRepository;

    public UpdateResourceViewsTask(
        ILogger<UpdateResourceViewsTask> logger,
        IEntityRepository<Publication> publicationsRepository,
        IEntityRepository<Publisher> publishersRepository,
        IEntityRepository<Author> authorsRepository,
        IRequestsRepository requestsRepository)
        : base(logger)
    {
        _publicationsRepository = publicationsRepository;
        _publishersRepository = publishersRepository;
        _authorsRepository = authorsRepository;
        _requestsRepository = requestsRepository;
    }

    protected override async Task DoLoggedTaskAsync()
    {
        await UpdateResourceViews<Publication>();
        await UpdateResourceViews<Publisher>();
        await UpdateResourceViews<Author>();
    }
    
    private async Task UpdateResourceViews<T>() 
        where T : Entity<T>
    {
        Dictionary<int, int> views = await _requestsRepository.GetResourceViews<T>();
        IEntityRepository<T> resourceRepository = GetRepository<T>();
        
        IEnumerable<T> updatedResource = (await resourceRepository.GetAllAsync())
            .Select(resource => views.TryGetValue(resource.Id, out int resourceViews) 
                ? resource.UpdateViews(resourceViews) 
                : resource)
            .ToList();

        await resourceRepository.UpdateAsync(updatedResource);
    }
    
    private IEntityRepository<T> GetRepository<T>()
        where T : Entity<T>
    {
        return typeof(T) switch
        {
            { } publication when publication == typeof(Publication) => (IEntityRepository<T>)_publicationsRepository,
            { } author when author == typeof(Author) => (IEntityRepository<T>)_authorsRepository,
            { } publisher when publisher == typeof(Publisher) => (IEntityRepository<T>)_publishersRepository,
            _ => throw new ArgumentException("Invalid entity repository type")
        };
    }
}