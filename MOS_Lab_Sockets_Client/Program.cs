﻿using System;

namespace MOS_Lab_Sockets_Client
{
    class Program
    {
        static void Main(string[] args)
        {
                Console.Read();
                AsynchronousClient.StartClient();
                MOS_Sync_Client.StartClient();
        }
    }
}
