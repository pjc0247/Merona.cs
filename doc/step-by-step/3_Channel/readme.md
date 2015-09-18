Channel
====

ä���� Session���� �����Դϴ�. ���(path)�� ������ ������ �ν��Ͻ� ��� ��η� ǥ���� �� �ֽ��ϴ�.
<br><br>
Channel.Pool�� �̿��Ͽ� ������ ����� ä�ο� Session�� ����/Ż���Ű��, Ư�� ä�ο� ��Ŷ�� ����� �� �ֽ��ϴ�.

```c#
class JoinRoom : Packet {
  public String name;
}
class LeaveRoom : Packet {
  public String name;
}
class Broadcast : Packet {
  public String name;
  public String message;
}
```
```c#
class TestService : Service {
  [Handler(typeof(JoinRoom))]
  public void OnJoinRoom(Session session, JoinRoom packet) {
    // Channel.Pool�� ����Ϸ��� current ������Ƽ�� �̿��� �ν��Ͻ��� ���ɴϴ�.
    // room.ROOM_NAME ä�ο� ���� ������ ���Խ�ŵ�ϴ�.
    Channel.Pool.current.Join(
      $"room.{packet.name}", session);
  }

  [Handler(typeof(LeaveRoom))]
  public void OnLeaveRoom(Session session, LeaveRoom packet) {
    // room.ROOM_NAME ä�ο��� Ż���ŵ�ϴ�.
    Channel.Pool.current.Leave(
      $"room.{packet.name}", session);
  }

  [Handler(typeof(Broadcast))]
  public void OnBroadcast(Session session, Broadcast packet) {
    // room.ROOM_NAME ä�ο� ��Ŷ�� ����մϴ�.
    // �ش� ä�ο� Join�� ��� Ŭ���̾�Ʈ���� ���۵˴ϴ�.
    Channel.Pool.current.Broadcast(
      $"room.{packet.name}", packet);
  }
}
```
ä�� ���
----
```c#
"area7.chatting"
// ���� ä���� ǥ���� �� ���� ������� �Ʒ��� ����� ����˴ϴ�.
"area.7.chatting"
```

���ϵ�ī�� ����ϱ�
----
��Ŷ ���(path)���� ���ϵ�ī�带 ����� �� �ֽ��ϴ�.<br>
�Ʒ��� �ڵ�� �� �̸��� ���� ���� ������ ��� �濡 ��ε�ĳ�����մϴ�.
```c#
Channel.Pool.current.Broadcast(
  "room.*", packet);
```
���ϵ�ī��� ���� �� ���� �� �ֽ��ϴ�.
```c#
�Ʒ��� ��δ� ����� ��� ����� �����ϴ� ��� ��Ƽ���� ����ŵ�ϴ�.
"worldmap.area.*.party.*"
```

Ư�� ä�� ��θ� �����ϴ� �ڵ鷯 �ۼ��ϱ�
----
������ �ڵ鷯������ ��Ŷ Ÿ�� ��� Ư�� ä�η� ������ ��� ��Ŷ�� �ڵ鸵�� �� �ֽ��ϴ�.
```c#
// ��Ŷ Ÿ�� ��� ������ ä�� ��θ� �ֽ��ϴ�.
// 
[Handler("room.*")]
public void OnRoom(Session session, Packet packet){
}
```

Raw Channel ����ϱ�
---
�Ʒ��� ������ Channel �ν��Ͻ��� ���� ����� �����ϴ� �����Դϴ�.<br>
�� ����� ������� �ʽ��ϴ�.
```c#
var channel = new Channel("CHANNEL_PATH");

channel.Join(session);
channel.Leave(session);
channel.Broadcast(packet);
```