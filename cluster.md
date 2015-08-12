```c#
void OnFooPacket(){
  // ...
  var response = await Server.current.cluster.Request(packet);
  // ...
}
```
