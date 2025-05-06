using Eve.Infrastructure.ExternalServices.Interfaces;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Eve.Infrastructure.ExternalServices.Base
{
    public class EveGlobalRateLimit : IEveGlobalRateLimit
    {
        private const int MaxUserRequestsPerSecond = 10;
        private const int MaxServiceRequestsPerSecond = 20;

        private readonly SemaphoreSlim _userSemaphore = new SemaphoreSlim(MaxUserRequestsPerSecond);
        private readonly SemaphoreSlim _serviceSemaphore = new SemaphoreSlim(MaxServiceRequestsPerSecond);

        private readonly ConcurrentQueue<DateTime> _userRequestTimes = new();
        private readonly ConcurrentQueue<DateTime> _serviceRequestTimes = new();

        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(1);


        /// <summary>
        /// Выполняет задачу с учетом глобальных ограничений для пользовательских или служебных запросов.
        /// </summary>
        /// <param name="token">Токен отмены.</param>
        /// <param name="requestSender">Функция, представляющая запрос.</param>
        /// <param name="priority">Приоритет задачи. Чем меньше число, тем выше приоритет.</param>
        /// <param name="isServiceRequest">Флаг, указывающий, является ли запрос служебным.</param>
        public async Task<HttpResponseMessage> RunTaskAsync(
            Func<Task<HttpResponseMessage>> requestSender,
            bool isServiceRequest,
            CancellationToken token)
        {
            if (isServiceRequest)
            {
               return await ServiceRunTask(requestSender, token);
            }
            else
            {
                return await UserRunTask(requestSender, token);
            }

        }

        private async Task<HttpResponseMessage> ServiceRunTask(Func<Task<HttpResponseMessage>> taskToRun, CancellationToken token)
        {
            await _serviceSemaphore.WaitAsync(token);

            await EnforceRateLimitAsync(_serviceRequestTimes, MaxServiceRequestsPerSecond, token);


            try
            {
                var result = await taskToRun();
                return result;
            }
            finally
            {
                _serviceSemaphore.Release();
            }
        }

        private async Task<HttpResponseMessage> UserRunTask(Func<Task<HttpResponseMessage>> taskToRun, CancellationToken token)
        {
            await _userSemaphore.WaitAsync(token);

            await EnforceRateLimitAsync(_userRequestTimes, MaxUserRequestsPerSecond, token);

            try
            {
                return await taskToRun();
            }
            finally
            {
                _userSemaphore.Release();
            }
        }

        private async Task EnforceRateLimitAsync(
            ConcurrentQueue<DateTime> requestTimes,
            int maxRequestsPerSecond,
            CancellationToken token)
        {
            while (true)
            {
                DateTime oldestRequestTime;

                lock (requestTimes)
                {
                    // Удаляем устаревшие записи
                    while (requestTimes.TryPeek(out oldestRequestTime) &&
                           oldestRequestTime < DateTime.UtcNow - _rateLimitWindow)
                    {
                        requestTimes.TryDequeue(out _);
                    }

                    // Проверяем, можем ли добавить новый запрос
                    if (requestTimes.Count < maxRequestsPerSecond)
                    {
                        requestTimes.Enqueue(DateTime.UtcNow);
                        return; // Выход из метода, ограничение не превышено
                    }
                }

                // Если очередь полная, вычисляем задержку до следующего свободного слота
                var delay = _rateLimitWindow - (DateTime.UtcNow - oldestRequestTime);
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, token);
                }
            }
        }
    }
}
