using System;
using System.Diagnostics;
using System.Threading;
using VRTrackingLib;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Waiting for Train Simulator to start...");

            Process? gameProcess = null;
            while (gameProcess == null)
            {
                var processes = Process.GetProcessesByName("RailWorks64");
                if (processes.Length > 0)
                    gameProcess = processes[0];
                else
                {
                    Console.WriteLine("Train Simulator not detected yet...");
                    Thread.Sleep(1000);
                }
            }

            Console.WriteLine("Game started! Initializing VR tracking...");

            var tracker = new VRHeadTracker();
            if (!tracker.Initialize())
            {
                Console.WriteLine("Failed to initialize OpenVR.");
                return;
            }

            Console.WriteLine("VR tracking started. Press Ctrl+C to exit.");

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Shutting down VR tracking...");
                tracker.Shutdown();
                Environment.Exit(0);
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var pos = tracker.GetHeadPosition();
                        Console.WriteLine($"Head Position: X={pos.X:F2} Y={pos.Y:F2} Z={pos.Z:F2}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting head position: {ex.Message}");
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during tracking: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Cleaning up VR tracking.");
                tracker.Shutdown();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex.Message}");
        }

        // Add this at the very end of Main so console stays open
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}