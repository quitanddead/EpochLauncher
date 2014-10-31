﻿using System;
using System.Collections.Generic;
using System.Threading;
using Launcher;

namespace Launcher.ConsoleTest
{
    class Program
    {
        static void Main()
        {
            var browser = new ServerBrowser();
            while (true)
            {
                foreach (var server in browser.Servers)
                {
                    Console.WriteLine(server.Value.Name);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
