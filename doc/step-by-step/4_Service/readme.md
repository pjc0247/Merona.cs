Service
====

서비스 초기화, 정리하기
----
서비스는 초기화, 정리를 위해 Setup, Cleanup 메소드를 제공합니다.<br>
이 메소드를 상속하여 커스텀 초기화, 정리 로직을 작성할 수 있습니다.<br>
이러한 Setup/Cleanup이 생성자/소멸자와 다른 점은 반드시 안전한 스레드에서 실행이 보장된다는 점 입니다.
따라서 서버의 모든 자원에 접근하는것이 가능합니다.
```c#
public TestService : Service {
  protected void Setup() {
  }
  protected void Cleanup() {
  }
}
```