using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MOS_Lab_Sockets_Client
{
    class MOS_Sync_Client
    {
        public static void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    var conWatch = System.Diagnostics.Stopwatch.StartNew();
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());
                    conWatch.Stop();
                    Console.WriteLine("Sync socket connection response time is {0} milliseconds.", conWatch.ElapsedMilliseconds);

                    // Encode the data string into a byte array.  
                    //byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    byte[] msg = Encoding.ASCII.GetBytes("Water is green. The other thing is that it is quite expensive. So when you find yourself at a disadvantage, put a little bit of money somewhere on an edge. The amount I know I can afford to buy a house or apartment — a place that feels competitive, a place I can be proud of, a home that is affordable if you are looking to buy a house — you really dont want to have no problems. You only have a chance to build and make money. And, more importantly, you dont want to be a bully in your community as a result of the current status quo. Let's say you want a large house, that is, where at a minimum you should live.<EOF>");
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    watch.Stop();
                    Console.WriteLine("Sync socket message response time is {0} milliseconds.", watch.ElapsedMilliseconds);

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }


}
