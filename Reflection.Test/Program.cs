using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Reflection.Test
{
    public class Program
    {
        public Program(string nice)
        {
            n = nice;
        }
        public string n;
        static Program() => Swap.Init();
        static void Main(string[] args)
        {
            Program prog = new Program("fuck");
            Console.WriteLine(prog.n);
        }
        [Swap(typeof(Program), ConstructorType.Instance, typeof(string))]
        public static void C(Program prog, string nice)
        {
            prog.n = nice;
            Console.WriteLine(Type.GetType("Reflection.Test.Program") + nice);
        }
    }
}
