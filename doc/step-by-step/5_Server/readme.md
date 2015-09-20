Server
====

���� ����&�����ϱ�
----
```c#
var config = Config.defaults;
var server = new Server(config);

server.Start();
```

������ �����ϱ�
----
������ �����忡���� �׻� `Server.current`�� �̿��Ͽ� ���� �ν��Ͻ��� ������ �� �ֽ��ϴ�.

���� ���� �����ϱ�
----
������ ��� �������� Config ������Ʈ�� ��� ������, ���� �����ÿ� Config�� �Ѱܼ� �����ϰ� �˴ϴ�.<br>
���� ������ �Ѱ��ִ� Config�� �����Ͽ� ������ ���������� ������ �� �ֽ��ϴ�. (������ ��Ÿ�� ���߿� ������ �ٲٴ� ���� ������ �ʽ��ϴ�)

```c#
// �⺻ ������ �����ɴϴ�.
var config = Config.defaults; 

config.host = "0.0.0.0";
config.port = 9916;

// ������ �ѹ��� recv �ϴ� ����Ʈ ũ�⸦ �����մϴ�.
config.sessionRecvBufferSize = 128;
// ������ ������ ũ�⸦ �����մϴ�.
// �Ϲ������� ������ ������� ū ��Ŷ�� ó���� �� �����ϴ�,
config.sessionRingBufferSize = 1024;

// ���� Ǯ ����� �����մϴ�.
// �Ϲ������� ���� Ǯ ���� ū ������ ������ ���� �� �����ϴ�.
config.sessionPoolSize = 1024;
```

Config ��ü�� ����Ͽ� �ܺ� ���� ����(json, xml)��� ������ �ε��ϵ��� �ۼ��� �� �ֽ��ϴ�.

```c#
class MyConfig : Config {
  public static Config LoadFromFile(String path) {
    /* ... */
  }
}
```
```c#
var config = MyConfig.LoadFromFile("config.json");
```