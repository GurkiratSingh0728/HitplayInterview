using System;
using System.Net.Sockets;
using System.Text;

namespace BiampTesiraControl
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the IP address and port for the Biamp Tesira Unit
            string ipAddress = "76.69.212.103"; // Example IP address
            int port = 80; // Example port number

            TcpClient? client = null;

            try
            {
                // Create a TcpClient and establish a connection to the Biamp Tesira Unit
                client = new TcpClient();
                client.Connect(ipAddress, port);
                Console.WriteLine("Connected to Biamp Tesira Unit.");

                // Get the network stream for communication
                NetworkStream? stream = client.GetStream();

                // Continuous loop to take user input and control the Tesira Unit
                while (true)
                {
                    // Prompt user to enter action (UP/DOWN)
                    Console.WriteLine("Enter the action (UP/DOWN):");
                    string? actionInput = Console.ReadLine()?.ToUpper();

                    string action;
                    // Set action based on user input (UP = increment, DOWN = decrement)
                    if (actionInput == "UP")
                    {
                        action = "increment";
                    }
                    else if (actionInput == "DOWN")
                    {
                        action = "decrement";
                    }
                    else
                    {
                        // Invalid input, prompt the user to enter UP or DOWN and continue loop
                        Console.WriteLine("Invalid action. Please enter UP or DOWN.");
                        continue;
                    }

                    // Prompt user to enter the channel number
                    Console.WriteLine("Enter the channel number:");
                    if (!int.TryParse(Console.ReadLine(), out int channel) || channel < 0)
                    {
                        // Invalid input for channel, prompt the user and continue loop
                        Console.WriteLine("Invalid channel number. Channel should be >= 0.");
                        continue;
                    }

                    // Prompt user to enter the increment/decrement value
                    Console.WriteLine("Enter the increment/decrement value:");
                    if (!int.TryParse(Console.ReadLine(), out int increment) || increment < 0)
                    {
                        // Invalid input for increment, prompt the user and continue loop
                        Console.WriteLine("Invalid increment value. Increment should be >= 0.");
                        continue;
                    }

                    // Prompt user to enter the instance tag of the block (should start with 'Level' followed by a value)
                    Console.WriteLine("Enter the instance tag of the block (should start with 'Level' followed by a value):");
                    string? instanceTag = Console.ReadLine();

                    if (instanceTag == null || !instanceTag.StartsWith("Level"))
                    {
                        // Invalid input for instance tag, prompt the user and continue loop
                        Console.WriteLine("Invalid instance tag. It should start with 'Level'.");
                        continue;
                    }

                    // Construct the command string based on user inputs
                    string commandString = $"{instanceTag} {action} level {channel} {increment}\r\n";

                    try
                    {
                        // Convert the command string to bytes and send it to the Tesira Unit
                        byte[] commandBytes = Encoding.ASCII.GetBytes(commandString);

                        if (stream != null)
                        {
                            stream.Write(commandBytes, 0, commandBytes.Length);
                        }

                        Console.WriteLine("Command sent.");
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions occurred during command sending
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }

                    // Check for 'exit' condition to break the loop
                    if (actionInput?.ToUpper() == "EXIT")
                    {
                        break;
                    }
                }

                // Close the network stream and TcpClient after the loop exits
                if (stream != null)
                {
                    stream.Close();
                }

                if (client != null)
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions occurred during the connection or operation
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Dispose the TcpClient resources in the end
                client?.Dispose();
            }
        }
    }
}
