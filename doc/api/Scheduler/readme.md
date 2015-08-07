Scheduler
====

Yield (int time)
----

```c#
[Handle(typeof(FooPacket))]
async void OnFooPacket(){
  Console.WriteLine("hello world");
  
  yield Scheduler.current.Yield(1000);
  
  Console.WriteLine("after 1000 ms");
}
```

Schedule (Action callback, long interval, long after, long count)
----

Unschedule (CancellationTokenSource cts)
----
