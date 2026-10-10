# dotnet-task-run-alternatives

**20 sposobów na to samo co `Task.Run`** — jeden przykład = jeden plik `.cs`, a runner odpala **wszystkie** i pokazuje wynik każdego.

Target: **.NET 10** (w konsoli: `net10.0`). Jedyna zależność zewnętrzna: `Microsoft.Extensions.Hosting` (przykład #19).

## Uruchomienie

```bash
dotnet build -c Release
dotnet run -c Release --project src/TaskRunAlternatives

# tylko wybrane przykłady / bez wyświetlania jednolinijkowców:
dotnet run -c Release --project src/TaskRunAlternatives -- 1 9 10 --no-code

# lista wszystkich przykładów:
dotnet run -c Release --project src/TaskRunAlternatives -- --list
```

Runner zbiera przykłady przez refleksję (`IExample`), więc dorzucenie pliku `Ex21_Cos.cs` automatycznie dopina nowy punkt do listy. Wyjście: `exit 0` gdy wszystkie OK, `1` gdy któryś padnie.

## Lista przykładów

| # | API | Plik |
|---|-----|------|
| 1 | `Task.Run` | [Ex01_TaskRun.cs](src/TaskRunAlternatives/Examples/Ex01_TaskRun.cs) |
| 2 | `Task.Factory.StartNew(Action)` | [Ex02_StartNewAction.cs](src/TaskRunAlternatives/Examples/Ex02_StartNewAction.cs) |
| 3 | `StartNew(async) + Unwrap` | [Ex03_StartNewAsyncUnwrap.cs](src/TaskRunAlternatives/Examples/Ex03_StartNewAsyncUnwrap.cs) |
| 4 | `StartNew(LongRunning)` | [Ex04_StartNewLongRunning.cs](src/TaskRunAlternatives/Examples/Ex04_StartNewLongRunning.cs) |
| 5 | `new Task(...).Start()` | [Ex05_ColdTaskStart.cs](src/TaskRunAlternatives/Examples/Ex05_ColdTaskStart.cs) |
| 6 | `ThreadPool.QueueUserWorkItem` | [Ex06_QueueUserWorkItem.cs](src/TaskRunAlternatives/Examples/Ex06_QueueUserWorkItem.cs) |
| 7 | `ThreadPool.UnsafeQueueUserWorkItem` | [Ex07_UnsafeQueueUserWorkItem.cs](src/TaskRunAlternatives/Examples/Ex07_UnsafeQueueUserWorkItem.cs) |
| 8 | `new Thread(...)` | [Ex08_NewThread.cs](src/TaskRunAlternatives/Examples/Ex08_NewThread.cs) |
| 9 | własny `TaskScheduler` (limit 3) | [Ex09_CustomTaskScheduler.cs](src/TaskRunAlternatives/Examples/Ex09_CustomTaskScheduler.cs) + [Scheduling/LimitedConcurrencyLevelTaskScheduler.cs](src/TaskRunAlternatives/Scheduling/LimitedConcurrencyLevelTaskScheduler.cs) |
| 10 | `ConcurrentExclusiveSchedulerPair(4)` | [Ex10_ConcurrentExclusivePair.cs](src/TaskRunAlternatives/Examples/Ex10_ConcurrentExclusivePair.cs) |
| 11 | `Parallel.Invoke` | [Ex11_ParallelInvoke.cs](src/TaskRunAlternatives/Examples/Ex11_ParallelInvoke.cs) |
| 12 | `Parallel.ForEachAsync` | [Ex12_ParallelForEachAsync.cs](src/TaskRunAlternatives/Examples/Ex12_ParallelForEachAsync.cs) |
| 13 | PLINQ `AsParallel()` | [Ex13_Plinq.cs](src/TaskRunAlternatives/Examples/Ex13_Plinq.cs) |
| 14 | `Task.WhenAll` + N x `Task.Run` | [Ex14_WhenAllTaskRun.cs](src/TaskRunAlternatives/Examples/Ex14_WhenAllTaskRun.cs) |
| 15 | `Task.Run` + `SemaphoreSlim` (throttling) | [Ex15_SemaphoreThrottle.cs](src/TaskRunAlternatives/Examples/Ex15_SemaphoreThrottle.cs) |
| 16 | `Channel<T>` + konsument w tle | [Ex16_Channel.cs](src/TaskRunAlternatives/Examples/Ex16_Channel.cs) |
| 17 | `System.Threading.Timer` | [Ex17_SystemTimer.cs](src/TaskRunAlternatives/Examples/Ex17_SystemTimer.cs) |
| 18 | `PeriodicTimer` w pętli async | [Ex18_PeriodicTimer.cs](src/TaskRunAlternatives/Examples/Ex18_PeriodicTimer.cs) |
| 19 | `BackgroundService` + Generic Host | [Ex19_BackgroundService.cs](src/TaskRunAlternatives/Examples/Ex19_BackgroundService.cs) |
| 20 | async I/O bez `Task.Run` | [Ex20_RealAsyncIo.cs](src/TaskRunAlternatives/Examples/Ex20_RealAsyncIo.cs) |

## Wynik uruchomienia (`dotnet run -c Release`)

```
=== 20 sposobów na to samo co Task.Run (.NET 10.0.10) ===

[01] Task.Run
     await Task.Run(() => Work.CpuBound(1), ct);
     OK  (   4 ms)  CpuBound(1)=2, wątek=4, pool=True

[02] Task.Factory.StartNew(Action)
     await Task.Factory.StartNew(() => Foo(), CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
     OK  (   1 ms)  CpuBound(2)=3, wątek=6, pool=True, DenyChildAttach + TaskScheduler.Default = Task.Run

[03] StartNew(async) + Unwrap
     await Task.Factory.StartNew(async () => await FooAsync(), ct, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
     OK  (  28 ms)  IoAsync(3)=4, wątek=4, bez Unwrap() byłby Task<Task<int>>

[04] StartNew(LongRunning)
     await Task.Factory.StartNew(() => Foo(), ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);
     OK  (   1 ms)  CpuBound(4)=5, wątek=9, pool=False, własny wątek, nie ThreadPool

[05] new Task(...).Start()
     var t = new Task(Foo); t.Start(); await t;
     OK  (   7 ms)  CpuBound(5)=6, wątek=4, pool=True, drugi Start() → InvalidOperationException (Start may not be called on a task that h...)

[06] ThreadPool.QueueUserWorkItem
     ThreadPool.QueueUserWorkItem(_ => Foo());
     OK  (   2 ms)  CpuBound(6)=7, wątek=6, pool=True, AsyncLocal w środku="kontekst-flowuje"

[07] ThreadPool.UnsafeQueueUserWorkItem
     ThreadPool.UnsafeQueueUserWorkItem(_ => Foo(), null);
     OK  (   1 ms)  CpuBound(7)=8, wątek=6, pool=True, AsyncLocal w środku=(null) → kontekst nie przepłynął

[08] new Thread(...).Start()
     new Thread(Foo) { IsBackground = true }.Start();
     OK  (   1 ms)  CpuBound(8)=9, wątek=10, pool=False, wątek nazwany=ex08-worker, join=True

[09] własny TaskScheduler (limit 3)
     await Task.Factory.StartNew(() => Foo(), CancellationToken.None, TaskCreationOptions.None, myScheduler);
     OK  ( 105 ms)  6 zadań na LimitedConcurrencyLevelTaskScheduler(3) → max równolegle=3, MaximumConcurrencyLevel=3, wyniki [10, 10, 10, 10, 10, 10]

[10] ConcurrentExclusiveSchedulerPair(4)
     var s = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 4).ConcurrentScheduler; await Task.Factory.StartNew(() => Foo(), ct, TaskCreationOptions.None, s);
     OK  (  82 ms)  8 zadań na ConcurrentScheduler(4) → max równolegle=4 (limit trzyma), wyniki [11, 11, 11, 11, 11, 11, 11, 11]; ExclusiveScheduler oddał CpuBound(10)=11 (wchodzi dopiero gdy concurrent jest puste)

[11] Parallel.Invoke
     Parallel.Invoke(Foo, Foo);
     OK  (   5 ms)  2x CpuBound(11) → [12@wątek7, 12@wątek4]

[12] Parallel.ForEachAsync(degree=4)
     await Parallel.ForEachAsync(Enumerable.Range(0, 10), new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (i, ct) => await FooAsync(ct));
     OK  (  57 ms)  6 iteracji, MaxDegreeOfParallelism=4 → wyniki [1, 2, 3, 4, 5, 6]

[13] PLINQ AsParallel()
     var r = Enumerable.Range(0, 10).AsParallel().WithDegreeOfParallelism(4).Select(i => Foo()).ToArray();
     OK  (   6 ms)  6 elementów, degree=4 → [1, 2, 3, 4, 5, 6] (kolejność zachowana przez AsOrdered)

[14] Task.WhenAll + N x Task.Run
     await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => Task.Run(Foo)));
     OK  (   1 ms)  5x Task.Run równolegle → [1, 2, 3, 4, 5]

[15] Task.Run + SemaphoreSlim(2)
     var sem = new SemaphoreSlim(2); await Task.WhenAll(items.Select(async i => { await sem.WaitAsync(); try { await Task.Run(Foo); } finally { sem.Release(); } }));
     OK  (   2 ms)  6x Task.Run z limitem 2 → max równolegle=2, wyniki [1, 2, 3, 4, 5, 6]

[16] Channel<T> + konsument w tle
     await channel.Writer.WriteAsync(item);
     OK  (   7 ms)  Bounded(4): producent zapisał 5, konsument odebrał [0, 1, 2, 3, 4]

[17] System.Threading.Timer
     _ = new System.Threading.Timer(_ => Foo(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
     OK  ( 121 ms)  callback odpalił się 3x (co 60 ms), wątek=4, pool=True

[18] PeriodicTimer w pętli async
     using var pt = new PeriodicTimer(TimeSpan.FromSeconds(1)); while (await pt.WaitForNextTickAsync()) Foo();
     OK  ( 153 ms)  3 ticki po ~50 ms → czasy [50 ms, 100 ms, 150 ms]

[19] BackgroundService + Generic Host
     protected override Task ExecuteAsync(CancellationToken ct) => Task.Run(Foo, ct);
     OK  ( 154 ms)  host wystartował, ExecuteAsync przetworzył 3 elementy (wątek=7), host zatrzymany

[20] async I/O bez Task.Run
     var text = await File.ReadAllTextAsync(path, ct);  // albo httpClient.GetStringAsync(url)
     OK  (   6 ms)  File.ReadAllTextAsync → "async I/O bez zajmowania wątku" (30 znaków); żaden wątek nie był zablokowany, wątek=6, pool=True

=== 20/20 OK, 0 FAIL ===
```

## Co z tego wynikło (i co łatwo pomylić)

- `Task.Run` to pod spodem dokładnie `Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default)`, a dla `Func<Task>` dodatkowo `.Unwrap()`.
- **Tego samego obiektu `Task` nie uruchomisz drugi raz.** `Start()` na odpalonym tasku rzuca `InvalidOperationException` (#5) — "restart" to ponowne wywołanie metody, która tworzy nowy `Task`.
- `LongRunning` (#4) daje `pool=False` — dedykowany wątek, nie ThreadPool. To jest właściwy wybór dla pracy blokującej, żeby nie zagłodzić poolu.
- `UnsafeQueueUserWorkItem` (#7) **nie flowuje `ExecutionContext`** — `AsyncLocal` w środku jest `null`, podczas gdy `QueueUserWorkItem` (#6) go widzi. Szybsze, ale tracisz ambient context (claims, culture, correlation id).
- Limit schedulera (#9, #10) ogranicza **wątki wykonujące zadania**, nie wiszące awaity. Z async ciałem zadania zwalniają pracownika na awaicie i scheduler odpala kolejne — limit widać tylko przy pracy blokującej (dlatego w przykładach jest `Thread.Sleep`).
- `Parallel.ForEachAsync` (#12) jest tym, czego zwykle szukasz zamiast `Task.Run` w pętli + własnego `SemaphoreSlim` — ma wbudowany `MaxDegreeOfParallelism`.
- `PeriodicTimer` (#18) trzyma stały interwał (50/100/150 ms), czego `while (true) { await Task.Delay(...) }` nie robi.
- Prawdziwe I/O (#20) nie potrzebuje żadnego wątku — `Task.Run` przy `HttpClient`/plikach/DB to tylko marnowanie wątku z poolu.

## CI

[.github/workflows/ci.yml](.github/workflows/ci.yml) — build + uruchomienie wszystkich 20 przykładów na `ubuntu-latest` przy każdym pushu.