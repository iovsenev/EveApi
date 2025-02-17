using Eve.Domain.Interfaces.CacheProviders;
using Eve.Domain.Interfaces.ExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Application.AuthServices.AdminBotRgistreService;
public class BotRegistreService
{
    private IEveApiOpenClientProvider _eveApiProvider;
    private IRedisProvider _redisProvider;

    public BotRegistreService(
        IEveApiOpenClientProvider eveApiProvider, 
        IRedisProvider redisProvider)
    {
        _eveApiProvider = eveApiProvider;
        _redisProvider = redisProvider;
    }

    public async Task Registry(CancellationToken token)
    {

    }
}
