using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirstMessage_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[1];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 2427);

            Socket client = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            byte[] bytes = new byte[1024];                          // Буфер данных

            try
            {
                // Соединяем сокет с удаленной точкой
                client.Connect(ipEndPoint);
                Console.WriteLine("Клиент соединяется с {0}", client.RemoteEndPoint.ToString());

                // Получаем ответ от сервера
                int bytesRec = client.Receive(bytes);
                string answer = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                Console.WriteLine("\nСообщение от сервера: {0}\n", answer);

                if (answer == "Вы являетесь первым клиентом, пожалуйста отправьте сообщение для отправки другим клиентам.")
                {
                    Console.Write("Сообщение: ");
                    string message = Console.ReadLine();
                    Console.WriteLine();
                    client.Send(Encoding.UTF8.GetBytes(message));
                }

                // Закрываем соединение
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.Write("Соединение успешно завершено.\n\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
