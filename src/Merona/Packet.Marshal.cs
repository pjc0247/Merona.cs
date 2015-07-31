using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merona
{
    public partial class Packet
    {
		public static T Deserialize<T>(byte[] buffer) where T : Packet, new()
        {
            return new T();
        }
		public byte[] Serialize()
        {
            Console.WriteLine("Serialize" + this.GetType().Name);

            var targetFields = GetType().GetFields().ToList();
            var exceptFields = Packet.GetC2SFields(this.GetType());
			if(exceptFields != null)
            {
                targetFields = targetFields.Except(exceptFields).ToList();
            }

			foreach(var field in targetFields)
            {
                Console.WriteLine(field.Name);
            }

            return new byte[1];
        }
    }
}
