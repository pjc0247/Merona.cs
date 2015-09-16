using System;
using System.Collections.Generic;
using System.Text;

namespace Merona
{
	public partial class Packet
	{
        /// <summary>
        /// 이 클래스를 상속받아 커스텀 패킷 디스크립터를 구현한다.
        /// </summary>
        /// <example>
        /// public class PlusOne : CustomDescriptor {
        ///     internal protected override void OnPostProcess(ref object target) {
        ///         target = (int)target + 1;
        ///     }
        /// }
        /// </example>
        [AttributeUsage(AttributeTargets.Field)]
        public class CustomDescriptor : Attribute
        {
            protected enum AttachTarget
            {
                PreProcess,
                PostProcess
            }

            /// <summary>
            /// 이 메소드를 상속받아 패킷의 필드에 대한 선처리를 수행한다.
            /// </summary>
            /// <param name="target">패킷 필드의 값 (ref)</param>
            internal protected virtual void OnPreProcess(ref object target)
            {
            }

            /// <summary>
            /// 이 메소드를 상속받아 패킷의 필드에 대한 후처리를 수행한다.
            /// </summary>
            /// <param name="target">패킷 필드의 값 (ref)</param>
            internal protected virtual void OnPostProcess(ref object target)
            {
            }
        }
	}
}
