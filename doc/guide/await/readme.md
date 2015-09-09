await
====

Worker 쓰레드에서 await 키워드를 사용하여 작업을 미룰 수 있습니다.

```c#
void OnTestPacket(Session session, TestPacket packet) {
  Console.WriteLoine("OnTestPacket");
  
  await Scheduler.current.Yield(1000);
  
  Console.WriteLine("After 1000ms");
}
```

Worker 쓰레드에 한해서 await이후의 동작은 동일한 Worker 스레드에서 실행됨이 보장됩니다.
<br>
http://stackoverflow.com/questions/21838651/i-thought-await-continued-on-the-same-thread-as-the-caller-but-it-seems-not-to
