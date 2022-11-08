using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryCatchExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string userInput = Console.ReadLine();
            if (userInput.Contains("!"))
                throw new Exception("사용자 입력에!가 있습니다.");
            int[] array = new int[4];
            try
            {
            array[0] = 100;
            array[4] = 200;
            Console.WriteLine(array[4]);
            Console.WriteLine("try구분 실행");//앞에서 예외가 발생하여 catch로 넘어감
            }
            catch (Exception e)
            {
                //exception은 모든 예외에 대한 처리
                Console.WriteLine("catch 실행");
                Console.WriteLine(e.Message);
            }
            finally
            {   //예외가 발생하지 않아도 finally 구문은 실행된다.
                Console.WriteLine("finally구문 실행");
            }
        }
    }
}
