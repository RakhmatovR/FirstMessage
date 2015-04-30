using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirstMessage_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[1];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 2427);

            // Создаем сокеты для клиента и сервера
            Socket server = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp), client;

            bool isFirstClient = true;                  // Флаг, для определения первого клиента
            byte[] message = new byte[1024];            // Сообщение, отправляемое первому клиенту, а затем пересылаемое остальным клиентам
            byte[] bytes = new byte[1024];              // Буфер данных
            string request = String.Empty;              // Запрос с сообщением, отправляемый первому клиенту
            string inputData = String.Empty;            // Данные, приходящие с первого клиента
            int bytesRec = 0;                           // Длина входящих данных от первого клиента

            try
            {
                // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
                server.Bind(ipEndPoint);
                server.Listen(10);

                // Начинаем слушать входящие соединения
                while (true)
                {
                    Console.WriteLine("Ожидание клиента через {0}", ipEndPoint);
                    // Ожидаем входящее соединение
                    client = server.Accept();
                    Console.WriteLine("Клиент успешно подключен.\n");

                    // Если клиент, подключенный на данный момент, является первым, то запрашиваем у него сообщение
                    if (isFirstClient)
                    {
                        request = "Вы являетесь первым клиентом, пожалуйста отправьте сообщение для отправки другим клиентам.";
                        message = Encoding.UTF8.GetBytes(request);
                        client.Send(message);

                        bytesRec = client.Receive(bytes);
                        inputData = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        Console.Write("Сообщение для пересылки другим клиентам: " + inputData + "\n\n");
                        message = Encoding.UTF8.GetBytes(inputData);
                        isFirstClient = false;
                    }

                    // Закрываем соединение с текущим клиентом
                    client.Send(message);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    Console.Write("Соединение успешно завершено.\n\n\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
