using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

Console.WriteLine("csRemote client started!");

while(true)
{
    try
    {
        IPEndPoint endPoint = new IPEndPoint(Config.address, Config.port);

        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        while (!socket.Connected)
        {
            try
            {
                Console.WriteLine("Connecting...");
                socket.Connect(endPoint);
            }
            catch(Exception e)
            {
                    Console.WriteLine($"Error: {e.Message} Retrying...");
                    Thread.Sleep(5000);
            }
        }

        Console.WriteLine("Connected!");
        socket.Send(Encoding.UTF8.GetBytes(Config.clientName + "\\" + Config.userName + "\\" + Config.deviceName));
        int bytesRecorded;
        byte[] buffer = new byte[8196];

        while(socket.Connected)
        {
            Console.WriteLine("Waiting password...");
            bytesRecorded = socket.Receive(buffer);
            string password = Encoding.UTF8.GetString(buffer, 0, bytesRecorded);

            Console.WriteLine("Checking password...");
            if(password == Config.password)
            {
                Console.WriteLine("Password accepted");
                socket.Send(Encoding.UTF8.GetBytes("Connected"));
                    
                while (socket.Connected)
                {
                    try {
                        Console.WriteLine("Waiting for command...");
                        bytesRecorded = socket.Receive(buffer);
                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRecorded);
                        Console.WriteLine("Command running...");

                        Process process;

                        try {
                            string[] commands = command.Split(" @ ");
                            process = Process.Start(commands[0], commands[1]);
                        }
                        catch {
                            process = Process.Start(command);
                        }

                        try {
                        StreamReader reader = process.StandardOutput;
                        string output = reader.ReadToEnd();
                        process.WaitForExit();
                        socket.Send(Encoding.UTF8.GetBytes(output));
                        }
                        catch {
                            socket.Send(Encoding.UTF8.GetBytes("[PROCESS STARTED]"));
                        }

                        process.Close();
                        Console.WriteLine("Command ended.");
                    }
                    catch(Exception e) {
                        Console.WriteLine($"{e.StackTrace} ERROR: {e.Message}");
                        socket.Send(Encoding.UTF8.GetBytes($"{e.StackTrace} ERROR: {e.Message}"));
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong password!");
                socket.Send(Encoding.UTF8.GetBytes("Wrong password"));
            }
        }
        
    }
    catch(Exception e)
    {
        Thread.Sleep(5000);
        Console.WriteLine($"{e.StackTrace} ERROR: {e.Message}");
    }
}