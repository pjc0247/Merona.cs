IStatusObservable & IStatusSubscriber
====

IStatusObservable
----
IStatusObservable은 객체의 상태 변화가 IStatusSubscriber에 의해 관찰이 가능하도록 합니다.
<br>
```c#
class Some : IStatusObservable<Some> {
  private List<IStatusSubscriber<Some>> subscribers;
  
  public void OnSubscribe(IStatusSubscriber<Some> obj) {
    subscribers.Add(obj);
  }
  public void OnUnsubscribe(IStatusSubscriber<Some> obj) {
    subscribers.Remove(obj);
  }
  
  public void PublishInvalidate() {
    foreach(var subscriber in subscribers) {
      subscriber.Invalidate(this);
    }
  }
}
```

IStatusSubscriber
----
IStatusSubscriber는 객체의 상태 변화를 감지하기 위한 구독자입니다.

```c#
class SomeCollection : List<Some>, IStatusSubscriber<Some> {
  public void Invalidate(Some item) {
    Remove(item);
  }
}
```
