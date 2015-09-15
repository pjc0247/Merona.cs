using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

namespace Merona.Go
{
	[AttributeUsage(AttributeTargets.Method)]
    public class RPC : Attribute
    {
		public static Task<T> FromRemote<T>(params object[] args)
        {
            StackTrace traces = new StackTrace(1);

            return RawCall<T>(
                traces.GetFrame(0).GetMethod().Name,
                args);
        }
		public static Task<T> RawCall<T>(String method, params object[] args)
        {
            var packet = new RpcCall.Request();
            packet.method = method;
            packet.args = args;

            return Task.Factory.StartNew<T>(() =>
            {
                return default(T);
            });
        }
    }
}
