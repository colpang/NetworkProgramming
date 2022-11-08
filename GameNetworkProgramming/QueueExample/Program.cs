using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<int> queue = new Queue<int>();
            //큐에 데이터를 추가
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);
            queue.Enqueue(5);
            //큐에서 데이터를 삭제
            int result = queue.Dequeue();
            Console.WriteLine(result);
            //대이터를 제거하지 않고 반환
            result = queue.Peek();
            Console.WriteLine(result);
            //Queue 데이터 배열로 저장
            int[] array = queue.ToArray();
        }
    }
}
