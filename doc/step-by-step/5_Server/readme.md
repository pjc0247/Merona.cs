Server
====

서버 생성&실행하기
----
```c#
var config = Config.defaults;
var server = new Server(config);

server.Start();
```

서버에 접근하기
----
안전한 스레드에서는 항상 `Server.current`를 이용하여 서버 인스턴스에 접근할 수 있습니다.

서버 설정 변경하기
----
서버는 모든 설정값을 Config 오브젝트에 담고 있으며, 서버 생성시에 Config를 넘겨서 생성하게 됩니다.<br>
서버 생성시 넘겨주는 Config를 변경하여 서버의 설정값들을 변경할 수 있습니다. (서버의 런타임 도중에 설정을 바꾸는 것은 허용되지 않습니다)

```c#
// 기본 설정을 가져옵니다.
var config = Config.defaults; 

config.host = "0.0.0.0";
config.port = 9916;

// 세션이 한번에 recv 하는 바이트 크기를 설정합니다.
config.sessionRecvBufferSize = 128;
// 세션이 링버퍼 크기를 설정합니다.
// 일반적으로 링버퍼 사이즈보다 큰 패킷은 처리할 수 없습니다,
config.sessionRingBufferSize = 1024;

// 세션 풀 사이즈를 설정합니다.
// 일반적으로 세션 풀 보다 큰 숫자의 연결은 받을 수 없습니다.
config.sessionPoolSize = 1024;
```

Config 객체를 상속하여 외부 설정 파일(json, xml)등에서 설정을 로드하도록 작성할 수 있습니다.

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