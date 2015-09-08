SafeCollection 사용하기
====

SafeCollection에 들어갈 오브젝트는 반드시 IStatusObservable인터페이스를 구현해야 합니다.<br>
```c#
class SomeObject : IStatusObservable<SomeObject> {
  private List<SafeCollection<SomeObject>> subscribers;
  
  // 현재 오브젝트가 SafeCollection에 추가되었을 때 호출됩니다.
  public void OnSubscribe(SafeCollection<SomeObject> safeCollection) {
    subscribers.Add(safeCollection);
  }
  
  // 현재 오브젝트가 SafeCollection에서 제거되었을 때 호출됩니다.
  public void OnUnsubscribe(SafeCollection<SomeObject> safeCollection) {
    subscribers.Remove(safeCollection);
  }
  
  // 현재 오브젝트가 사용할 수 없는 상태가 되었음을 자신을 가지고있는 모든
  // SafeCollection들에게 알립니다.
  public void PublishInvalidated() {
    foreach(var sc in subscribers) {
      sc.Invalidate(this);
    }
  }
}
```
