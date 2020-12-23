using System;

namespace MOS_Lab_Sockets_Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            AsynchronousSocketListener.StartListening();
            MOS_Sync_Server.StartListening();
        }
    }
}
