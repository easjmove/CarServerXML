using CarLibrary;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarServerXML
{
    class Program
    {
        static void Main(string[] args)
        {
            //Writes to the console what is running
            Console.WriteLine("Car server");

            //Creates a Listener that listens on all network adapters on port 10002
            TcpListener listener = new TcpListener(IPAddress.Any, 10002);
            //Starts the listener
            listener.Start();

            //Handles more clients
            while (true)
            {
                //Waits for a client to connect
                TcpClient socket = listener.AcceptTcpClient();
                //Makes the server concurrent
                //And sends the client to the method HandleClient
                Task.Run(() => HandleClient(socket));
            }
        }

        private static void HandleClient(TcpClient socket)
        {
            //Gets the stream object from the socket. The stream object is able to recieve and send data
            NetworkStream ns = socket.GetStream();
            //The StreamReader is an easier way to read data from a Stream, it uses the NetworkStream
            StreamReader reader = new StreamReader(ns);

            //no writer here, as it doesn't send data back to the client

            //Here it converts the message the client send to a Car object
            XmlSerializer serializer = new XmlSerializer(typeof(Car));

            //Reads the XML from the readers stream
            Car receivedCar = (Car) serializer.Deserialize(reader.BaseStream);

            //Notice it doesn't use ReadLine, as the XML it is receiving is in several lines

            //writes the 3 properties to the console, one property per line
            Console.WriteLine("Car Model: " + receivedCar.Model);
            Console.WriteLine("Car Color: " + receivedCar.Color);
            Console.WriteLine("Car RegistrationNumber: " + receivedCar.RegistrationNumber);

            //closes the socket, as it doesn't expect anything more from the client
            socket.Close();

            //XML example to test with
            //<Car>
            //<Model>BMW</Model>
            //<Color>Black</Color>
            //<RegistrationNumber>AB12345</RegistrationNumber>
            //</Car>
            //When testing this, it registers when you disconnect, it waits to deserialize until then
        }
    }
}
