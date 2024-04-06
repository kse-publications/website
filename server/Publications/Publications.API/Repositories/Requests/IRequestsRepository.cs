using Publications.API.Models;

namespace Publications.API.Repositories.Requests;

public interface IRequestsRepository
{
    Task AddAsync(Request request);
}