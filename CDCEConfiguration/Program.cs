using System;
using System.IO.Ports;
public class Program {
    public static void Main()
    {
        Console.WriteLine("CDCE Configurator\n");
        List<string> ports = GetAllPorts();
        Console.WriteLine("Select port:");
        foreach (var listItem in ports)
        {
            Console.WriteLine("\t" + listItem);
        }

        string? selectedPort = Console.ReadLine();
        Console.WriteLine($"Port selected: {selectedPort}");

        var port = new SerialPort(selectedPort, 9600, Parity.None);
        port.Open();

        ReadAllRegisters(port);
    }

    public static List<string> GetAllPorts()
    {
        return new List<string>(SerialPort.GetPortNames());
    }

    public static List<string> ReadAllRegisters(SerialPort port)
    {
        for(int i = 0; i < 8; i++)
        {
            Byte address = (byte)i;
            Byte instruction = 0b1110;
            uint data = ((uint)address << 4) | ((uint)instruction);

            Byte[] sendBytes = BitConverter.GetBytes(data);
            Array.Reverse(sendBytes);

            port.Write(sendBytes, 0, sendBytes.Length);

            Byte[] receivedBytes = new Byte[4];

            port.Read(receivedBytes, 0, 4);

            string received = address.ToString("D1") + ": ";
            foreach(byte b in receivedBytes)
            {
                received += b.ToString();
            }

            Console.WriteLine(received);
        }
        return new List<string>();
    }
}