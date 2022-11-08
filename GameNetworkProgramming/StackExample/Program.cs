using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Stack<int> stack = new Stack<int>();
            //스택에 데이터 추가
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);
            stack.Push(5);
            //스택에서 데이터 삭제
            int result = stack.Pop();
            Console.WriteLine(result);
            result = stack.Peek();
            Console.WriteLine(result);
            //참고
            int[] array= stack.ToArray();
        }
    }
}
