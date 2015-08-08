스케쥴러
====
스케쥴러는 작업이 일정한 시간 후에 실행되어야 하거나, 일정한 시간 간격으로 반복되어 실행되어야 할 때 사용합니다.<br>
Merona에서 스케쥴러를 사용해 작업을 미루는 방법은 async/await 스타일과 Callback 스타일 두가지가 있습니다.<br>
스케쥴된 작업은 항상 안전한 스레드에서 실행됨이 보장됩니다.

Async/Await 스타일의 작업 미루기
----
```c#
[Handler(typeof(FooPacket)]
async void OnFooPacket(Session session, FooPacket packet){
  Console.WriteLine("hello world");
  
  awati Scheduler.current.Yield(1000); // 1000ms만큼 대기
  
  Console.WriteLine("after 1000ms");
}
```

Callback 스타일의 작업 미루기
----
```c#
[Handler(typeof(FooPacket)]
async void OnFooPacket(Session session, FooPacket packet){
  Console.WriteLine("hello world");
  
  Scheduler.current.Defer(()=>{
    Console.WriteLine("after 1000ms");
  }, 1000); // 1000ms 후에 콜백이 실행됨
}
```

콜백 방식으로 스케쥴된 작업은 도중에 취소가 가능합니다.
```c#
[Handler(typeof(FooPacket)]
async void OnFooPacket(Session session, FooPacket packet){
  Console.WriteLine("hello world");
  
  var cts = Scheduler.current.Defer(()=>{
    Console.WriteLine("after 1000ms");
  }, 1000);
  
  cts.Cancel(); // 실행을 취소합니다, 콜백은 실행되지 않습니다.
}
```
