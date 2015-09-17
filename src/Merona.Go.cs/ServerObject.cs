using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Merona.Go
{
    using Merona;

    public class ServerObject
    {
        public int objectId { get; private set; }
        internal protected Server server { get; internal set; }
        private CancellationTokenSource cts { get; set; }   
        private List<String> dirtyProperties { get; set; }
        
        public ServerObject()
        {
            dirtyProperties = new List<String>();
        }

        public void ScheduleUpdate()
        {
            int ServerFrameInterval = 33; // FIXME
            var tick = Environment.TickCount;

            cts = Scheduler.current.Schedule(() => {
                Update(Environment.TickCount - tick);
                tick = Environment.TickCount;
            }, ServerFrameInterval);
        }
        public void Unschedule()
        {
            if (cts == null)
                throw new InvalidOperationException();

            cts.Cancel();
            cts = null;
        }

        internal protected virtual void OnPreKeyDown(int key)
        {
            OnKeyDown(key);
        }
        internal protected virtual void OnKeyDown(int key)
        {
        }
        internal protected virtual void OnPreKeyUp(int key)
        {
            OnKeyUp(key);
        }
        internal protected virtual void OnKeyUp(int key)
        {
        }
        internal protected virtual void OnPreMouseDown(float x, float y)
        {
            OnMouseDown(x, y);
        }
        internal protected virtual void OnMouseDown(float x, float y)
        {
        }
        internal protected virtual void OnPreMouseUp(float x, float y)
        {
            OnMouseUp(x, y);
        }
        internal protected virtual void OnMouseUp(float x, float y)
        {
        }
        
        internal protected virtual void Update(float dt)
        {
        }

        private void PostUpdate(float dt)
        {

        }

        internal void OnSyncProperty(SyncProperty.Request packet)
        {
            GetType().GetProperty(packet.key)
                .SetValue(this, packet.value);
        }

        protected void MakeDirty(String name)
        {
            dirtyProperties.Add(name);
        }
        internal void Sync()
        {
            foreach(var prop in dirtyProperties)
            {
                var packet = new SyncProperty.Request();
                packet.objectId = objectId;
                packet.key = prop;
                packet.value = 
                    GetType().GetProperty(prop).GetValue(this);

                Channel.Pool.current.Broadcast(
                    $"go.sync.{objectId}", packet);
            }

            dirtyProperties.Clear();
        }
    }
}
