using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace GameNetworkProgramming
{
    class Program
    {
        static string serverIp = "172.30.1.42";
        static void Main(string[] args)
        {
            IPAddress ip1 = IPAddress.Parse(serverIp);
            IPAddress ip2 = new IPAddress(new byte[] { 192,168,0,1 });
            //IPAddress의 Long값
            IPAddress ip3 = new IPAddress(16820416);
            byte[] b = ip1.GetAddressBytes();
            Console.WriteLine("Address : " + b[0] + "." + b[1] + "." + b[2] + "." + b[3]);
            byte[] b2 = ip2.GetAddressBytes();
            Console.WriteLine("Address : " + b2[0] + "." + b2[1] + "." + b2[2] + "." + b2[3]);
            //b2를 long값으로 변경
            int ip = BitConverter.ToInt32(b2, 0);
            Console.WriteLine("ip = " + ip.ToString());
            byte[] b3 = ip3.GetAddressBytes();
            Console.WriteLine("Address : " + b3[0] + "." + b3[1] + "." + b3[2] + "." + b3[3]);

            //DNS
            //호스트/도메인명에서 IP알아내기
            IPHostEntry hostEntry = Dns.GetHostEntry("www.google.com");
            Console.WriteLine(hostEntry.HostName);
            foreach(IPAddress one in hostEntry.AddressList)
            {
                Console.WriteLine(one);
                Console.WriteLine(one.MapToIPv6());
            }
            //로컬호스트명 정보얻기(내 PC의 호스트명)
            string hostname = Dns.GetHostName();
            Console.WriteLine("로컬 호스트 = "+hostname);
            IPHostEntry localhost = Dns.GetHostEntry(hostname);
            foreach(IPAddress one in localhost.AddressList)
            {
                Console.WriteLine("로컬IP = "+ip);
            }
            //이하는 참고
            IPAddress[] address = Dns.GetHostAddresses(hostname);
            for(int i=0; i < address.Length; i++)
            {
                byte[] byaddress = address[i].GetAddressBytes();
                string ipstring = BitConverter.ToString(byaddress);
                Console.WriteLine("로컬IP = "+ ipstring);
            }
        }
    }
}
