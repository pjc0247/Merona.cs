SafeCollection 사용하기
====

SafeCollection에 들어갈 오브젝트는 반드시 ISafeCollectionContainable인터페이스를 구현해야 합니다.<br>
```c#
class SomeObject : ISafeCollectionContainable<SomeObject> {
  private List<SafeCollection<SomeObject>> subscribers;
  
  public void OnAdded(SafeCollection<SomeObject> safeCollection) {
    subscribers.Add(safeCollection);
  }
  public void OnRemoved(SafeCollection<SomeObject> safeCollection) {
    subscribers.Remove(safeCollection);
  }
  
  public void PublishInvalidated() {
    foreach(var sc in subscribers) {
      sc.Invalidate(this);
    }
  }
}
```
