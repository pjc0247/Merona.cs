Session
====

������ ������ Ŭ���̾�Ʈ�� ���� ������ �ӽ� ������Դϴ�.<br>
�⺻������, ���ǿ� ����� ������ ���� �ð� ���ȸ� ��ȿ�ϸ�, ������ ����Ǹ� ��� ���ư��ϴ�.

```c#
// ������ ����Ͽ� �����ϸ� �߰� Ŀ���� �����͸� �����ϰ�
// OnConnect, OnDisconnect���� �̺�Ʈ�� �������� �� �ֽ��ϴ�.
public class MySession : Session {
  // ���ǿ� �г����� �����մϴ�.
  public String nickname {get;set;}

  protected override void OnConnect() {
    // ���ο� Ŭ���̾�Ʈ�� ����Ǿ�, ���� ������ ���ε� �Ǿ��� �� ȣ��˴ϴ�.
  }
  protected override void OnDisconnect() {
    // Ŭ���̾�Ʈ���� ������ �����Ǿ�, ���� ���ǿ��� ����ε� �Ǿ��� �� ȣ��˴ϴ�.
  }
}
```

�̷��� ����� Ŀ���� ������ ������ ������ �����ؾ� ���������� �ݿ��˴ϴ�.
```c#
var config = Config.defaults;
config.sessionType = typeof(MySession);
```

������ �ڵ鷯���� Session ��� Ŀ���� ������ ���� �Ѱ� ���� �� �ֽ��ϴ�.
```c#
class TestService : Service {
  [Handler(typeof(TestPacket))]
  public void OnTestPacket(MySession session, TestPacket packet) {
    /* ... */
  }
}
```
