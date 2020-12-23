using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MOS_Lab_Sockets_Server
{
    public class StateObject
    {
        public const int bufferSize = 1024;

        public byte[] buffer = new byte[bufferSize];

        public StringBuilder stringBuilder = new StringBuilder();

        public Socket workSocket = null;

    }

    public class AsynchronousSocketListener
    {

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {

        }

        public static void StartListening ()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while(true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback),
                        listener);

                    allDone.WaitOne();
                }
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.Read();
        }
    
        public static void AcceptCallback(IAsyncResult ar)
        {
            //Signal main thread to continue
            allDone.Set();

            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //Create state object.
            StateObject stateObject = new StateObject();
            stateObject.workSocket = handler;
            handler.BeginReceive(stateObject.buffer, 0,
                StateObject.bufferSize, 0,
                new AsyncCallback(ReadCallback), stateObject);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            //Retrueve SO and handler socket from ASO.
            StateObject stateObject = (StateObject)ar.AsyncState;
            Socket handler = stateObject.workSocket;

            //Read data from client.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                stateObject.stringBuilder.Append(Encoding.ASCII.GetString(
                    stateObject.buffer, 0, bytesRead));

                //Check endline tag or read for data.
                content = stateObject.stringBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1 )
                {
                    //All read.
                    Console.WriteLine("Read {0} bytes from socket. \n Data: {1}", content.Length, content);

                    //Echo to client.
                    Send(handler, content);
                }
                else
                {
                    //Get more data if not <EOF>.
                    handler.BeginReceive(stateObject.buffer, 0, StateObject.bufferSize, 0,
                        new AsyncCallback(ReadCallback), stateObject);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            //Convert string to byte (ASCII).
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            //Start sending data.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }

}
