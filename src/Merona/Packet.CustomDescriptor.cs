using System;
using System.Collections.Generic;
using System.Text;

namespace Merona
{
	public partial class Packet
	{
        [AttributeUsage(AttributeTargets.Field)]
        public class CustomDescriptor : Attribute
        {
            protected enum AttachTarget
            {
                PreProcess,
                PostProcess
            }

            internal protected void OnPreProcess(ref object target)
            {
            }
            internal protected void OnPostProcess(ref object target)
            {
            }
        }
	}
}
