using System;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using FTD2XX_NET;
using static FTD2XX_NET.FTDI;
public class Program {

    private static FTDI ftdi;

    public Program()
    {
    }

    public static void Main()
    {
        Console.WriteLine("CDCE Configurator\n");

        FT_DEVICE_INFO_NODE[] deviceList = new FT_DEVICE_INFO_NODE[1];
        Program.ftdi = new();
        Program.ftdi.GetDeviceList(deviceList);
        if(deviceList.Length == 0)
        {
            Console.WriteLine("No FTDI device found. Quitting...");
            return;
        }

        foreach (var device in deviceList)
        {
            Console.WriteLine(device.ID + " " + device.Description + ", Serial: " + device.SerialNumber);
        }
        Program.ftdi.OpenBySerialNumber(deviceList.ElementAt(0).SerialNumber);

        string FTDIport = String.Empty;
        Program.ftdi.GetCOMPort(out FTDIport);
        Console.WriteLine("Device found on port: " + FTDIport);
        ReadAllRegisters();



        Program.ftdi.Close();

        //ReadAllRegisters(port);
    }

    public static List<string> GetAllPorts()
    {
        return new List<string>(SerialPort.GetPortNames());
    }

    public static List<string> ReadAllRegisters()
    {
        for(int i = 0; i < 8; i++)
        {
            Byte address = (byte)i;
            Byte instruction = 0b1110;
            uint data = ((uint)address << 4) | ((uint)instruction);

            Byte[] sendBytes = BitConverter.GetBytes(data);
            Array.Reverse(sendBytes);
            uint sent = 0;
            uint read = 0;

            ftdi.Write(sendBytes, 4, ref sent);

            Byte[] receivedBytes = new Byte[4];

            ftdi.Read(receivedBytes, 0, ref read);

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