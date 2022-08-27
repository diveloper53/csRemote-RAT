using System.Net;
using System.Security.Principal;

public static class Config
{
    // Client Info config
    public static string clientName = "Client";
    public static string userName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
    public static string deviceName = WindowsIdentity.GetCurrent().Name.Split('\\')[0];

    // IP and Port config
    public static IPAddress address = IPAddress.Parse("127.0.0.1");
    public static int port = 7777;

    // Additional protection
    public static string password = "csRemote is the best! (No.)";
}