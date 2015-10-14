auto_bootstrap
====

지금까지는 같은 어셈블리 내 서버 분리 기능으로 인해 부트스트랩 단계에 자동으로 서비스를 검색하여 추가하는 기능을 제공하지 않았지만,
옵션을 통해 켜고 끌 수 있는 기능을 제공한다.<br>
옵션을 on상태로 변경하고 서버를 시작하면 직접 서버에 서비스를 추가할 필요 없이 사용할 수 있는 서비스들을 자동으로 검색해 등록한다.

__manual_bootstrap__
```c#
var server = new Server();

server.AddService<ChatService>();
server.AddService<MatchingService>();
server.AddService<PingPongService>();

server.Start();
```

__auto_bootstrap__
```c#
var server = new Server();

server.Start(); // all-done!
```
