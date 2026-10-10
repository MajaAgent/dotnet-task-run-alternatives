namespace TaskRunAlternatives.Scheduling;

/// <summary>
/// Scheduler ograniczający liczbę jednocześnie wykonywanych zadań
/// (klasyczny wzorzec z dokumentacji Parallel Extensions, pracownicy na ThreadPoolu).
/// </summary>
public sealed class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
{
    [ThreadStatic]
    private static bool _currentThreadIsProcessingItems;

    private readonly LinkedList<Task> _tasks = new();
    private readonly int _maxDegreeOfParallelism;
    private int _delegatesQueuedOrRunning;

    public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxDegreeOfParallelism, 1);
        _maxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    public override int MaximumConcurrencyLevel => _maxDegreeOfParallelism;

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        lock (_tasks) return _tasks.ToArray();
    }

    protected override void QueueTask(Task task)
    {
        lock (_tasks) _tasks.AddLast(task);
        NotifyThreadPoolOfPendingWork();
    }

    private void NotifyThreadPoolOfPendingWork()
    {
        lock (_tasks)
        {
            if (_delegatesQueuedOrRunning >= _maxDegreeOfParallelism) return;
            _delegatesQueuedOrRunning++;
            ThreadPool.UnsafeQueueUserWorkItem(_ => ProcessQueuedTasks(), null);
        }
    }

    private void ProcessQueuedTasks()
    {
        _currentThreadIsProcessingItems = true;
        try
        {
            while (true)
            {
                Task? task;
                lock (_tasks)
                {
                    if (_tasks.Count == 0)
                    {
                        _delegatesQueuedOrRunning--;
                        return;
                    }

                    task = _tasks.First!.Value;
                    _tasks.RemoveFirst();
                }

                TryExecuteTask(task);
            }
        }
        finally
        {
            _currentThreadIsProcessingItems = false;
        }
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        if (!_currentThreadIsProcessingItems) return false;
        if (taskWasPreviouslyQueued) lock (_tasks) _tasks.Remove(task);
        return TryExecuteTask(task);
    }
}
