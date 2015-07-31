using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Merona
{
    public partial class Packet
    {
        private void DumpComplex()
        {
            Console.WriteLine(GetType().Name);
            foreach (var field in GetType().GetFields())
            {
                Console.WriteLine("   {0} : {1}", field.Name, field.GetValue(this));

                var attrs = field.GetCustomAttributes();
                foreach (var attr in attrs)
                {
                    Console.Write("      ");
                    Console.Write(attr.GetType().Name);

                    if (attr is MemberOf)
                        Console.Write("({0})", ((MemberOf)attr).type.Name);
                    else if (attr is KeyOf)
                        Console.Write("({0})", ((KeyOf)attr).type.Name);
                    else if (attr is Bind)
                        Console.Write("(\"{0}\")", ((Bind)attr).format);

                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }
        private void DumpSimple()
        {
            Console.WriteLine(GetType().Name);
            foreach (var field in GetType().GetFields())
            {
                Console.WriteLine("  {0} : {1}", field.Name, field.GetValue(this));
            }
        }
        public void Dump(bool detailed = false)
        {
            if (detailed) DumpComplex();
            else DumpSimple();
        }
    }
}
