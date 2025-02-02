using CharginAssignment.WithTests.Application.Common.Contracts;
using Microsoft.Extensions.Caching.Distributed;

namespace CharginAssignment.WithTests.Infrastructure.Caching;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
}