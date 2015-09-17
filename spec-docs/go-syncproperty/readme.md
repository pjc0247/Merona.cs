
변경에 대한 pubsub은 Channel 구현을 이용한다.

```
SyncProperty.Reqest 패킷을 go.sync.#{objectId} 채널에 송신

ServerObject는 go.sync.#{objectId} 패킷을 구독한다.
```
