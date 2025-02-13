using Polly;
namespace AppCore.Services.Helpers;

public class PollyService<T, U>: IPollyService<T, U> where T : class where U : Exception
{
    public static IAsyncPolicy<T> GetRetryPolicy(int retryCount = 3)
    {
        return Policy<T>
            .Handle<U>()
            .WaitAndRetryAsync(retryCount, (int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static IAsyncPolicy<T> GetRetryPolicyOld(int retryCount = 3)
    {
        var policy = Policy<T>.Handle<U>().RetryAsync(retryCount, (ex, retryCount) =>
        {
            Console.WriteLine($"Retry count: {retryCount}, {ex}");
        });
        return policy;
    }

    /// <summary>
    /// Generic Resilience Pipeline Builder to handle retries using Polly
    /// </summary>
    /// <param name="retryCount">The number of times to retry after first failure.</param>
    /// <param name="retryDelayInSeconds">the interval in seconds to wait between retries.</param>
    /// <param name="fallbackAction">The action to take is all retries failed to execute successfully.</param>
    /// <returns></returns>
    public ResiliencePipeline<T> ResiliencePipeline(int retryCount = 3, int retryDelayInSeconds = 5, Func<Polly.Fallback.FallbackActionArguments<T>, ValueTask<Polly.Outcome<T>>>? fallbackAction = null)
    {
        ResiliencePipelineBuilder<T> policy = new ResiliencePipelineBuilder<T>().AddRetry(new Polly.Retry.RetryStrategyOptions<T>
        {
            MaxRetryAttempts = retryCount,
            BackoffType = DelayBackoffType.Constant,
            Delay = TimeSpan.FromSeconds(retryDelayInSeconds),
            ShouldHandle = new PredicateBuilder<T>().Handle<U>(),
            OnRetry = (retryArguments) =>
            {
                Console.WriteLine($"Current attempt: {retryArguments.AttemptNumber}, {retryArguments.Outcome}");
                return ValueTask.CompletedTask;
            }
        });
        if (fallbackAction != null)
        {
            policy.AddFallback(new Polly.Fallback.FallbackStrategyOptions<T>
            {
                FallbackAction = fallbackAction                
            });
        }
        return policy.Build();
    }
}


public interface IPollyService<T, U> where T : class where U : Exception
{
    ResiliencePipeline<T> ResiliencePipeline(int retryCount = 3, int retryDelayInSeconds = 5, Func<Polly.Fallback.FallbackActionArguments<T>, ValueTask<Polly.Outcome<T>>>? fallbackAction = null);
}
