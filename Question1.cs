using System;
using System.Net.Sockets;
using System.Text;

namespace BiampTesiraControl
{
    class Program
    {
        static void Main(string[] args)
        {
            // Replace these with your Biamp Tesira Unit's IP address and port
            string ipAddress = "192.168.1.100"; // Example IP address
            int port = 23; // Example port number

            TcpClient client = new TcpClient();

            try
            {
                client.Connect(ipAddress, port);
                Console.WriteLine("Connected to Biamp Tesira Unit.");

                NetworkStream stream = client.GetStream();

                while (true)
                {
                    Console.WriteLine("Enter the action (UP/DOWN):");
                    string action = Console.ReadLine().ToUpper();

                    if (action != "UP" && action != "DOWN")
                    {
                        Console.WriteLine("Invalid action. Please enter UP or DOWN.");
                        continue;
                    }

                    Console.WriteLine("Enter the channel number:");
                    if (!int.TryParse(Console.ReadLine(), out int channel) || channel < 0)
                    {
                        Console.WriteLine("Invalid channel number. Channel should be >= 0.");
                        continue;
                    }

                    Console.WriteLine("Enter the increment/decrement value:");
                    if (!int.TryParse(Console.ReadLine(), out int increment) || increment < 0)
                    {
                        Console.WriteLine("Invalid increment value. Increment should be >= 0.");
                        continue;
                    }

                    Console.WriteLine("Enter the instance tag of the block (should start with 'Level' followed by a value):");
                    string instanceTag = Console.ReadLine();

                    if (!instanceTag.StartsWith("Level"))
                    {
                        Console.WriteLine("Invalid instance tag. It should start with 'Level'.");
                        continue;
                    }

                    string commandString = $"{instanceTag} {action.ToLower()} level {channel} {increment}\r\n";

                    try
                    {
                        // Convert the command string to bytes
                        byte[] commandBytes = Encoding.ASCII.GetBytes(commandString);

                        // Send the command to the Tesira Unit
                        stream.Write(commandBytes, 0, commandBytes.Length);

                        Console.WriteLine("Command sent.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
