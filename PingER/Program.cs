using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;

namespace PingER;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Invalid args");
            return;
        }

        var address = string.Empty;

        var command = args[0];

        switch (command)
        {
            case "--a" when args.Length == 2:
                address = args[1];
                break;
            default:
                Console.WriteLine("Invalid command");
                break;
        }

        var fileName = address.Replace(".", "_");

        using Ping myPing = new();

        do
        {
            while (!Console.KeyAvailable)
            {
                DateTime dt = DateTime.Now;

                long ms = 5000;

                try
                {
                    PingReply reply = myPing.Send(address, 5000);
                    if (reply != null)
                    {
                        Console.WriteLine($"Status : {reply.Status}; Time={reply.RoundtripTime}ms; Address {reply.Address}");

                        ms = reply.RoundtripTime;
                    }
                }
                catch
                {
                    Console.WriteLine("ERROR: You have Some TIMEOUT issue");
                }

                string capt = dt.ToString("yyyy-MM-dd HH:mm:ss");

                DateTime dt2 = DateTime.Now;
                TimeSpan ts = (dt2 - dt);

                if (ts.TotalMilliseconds < 1000)
                {
                    Thread.Sleep(Convert.ToInt32(1000 - ts.TotalMilliseconds));
                }

                try
                {
                    using (StreamWriter sw = File.AppendText($"{fileName}.txt"))
                    {
                        sw.WriteLine($"{capt};{ms}");

                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            }
        } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
    }
}
