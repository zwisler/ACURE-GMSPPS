
using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;
//
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using GMSPPStoGMS;
using GMSPPStoGMS.Model;
using System.Linq;

using System.Collections;
using System.Collections.Generic;



namespace ConsoleApplication1
{
    public class ESMission
    {
        public string CostumMissionID { get; set; }
        public string ID { get; set; }
    }

    public class User_Mission
    {
        public string CostumMissionID { get; set; }
        public string Name { get; set; }
    }

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    class Program
    {
        // The port number for the remote device.
        // private const int port = 777;

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private const string crlf = "\r\n";
        private const string backslash = "\\";
        private const string endpoint = "http://gmspps.azurewebsites.net";
        private static String Host;
        private static String ProviderName;
        public static String ProviderToken;
        private static int Port;
        private static Socket client;
        private GMSPPS_Client GMPS_Client;
        // DODO static Proble lösen
        public static ICollection<ESMission> Einsätze;
        public static ICollection<User_Mission> User_Einsätze;



        // The response from the remote device.
        private static String response = String.Empty;

        public static object ConfigurationManager { get; private set; }

        static void Main(string[] args)
        {
            Einsätze = new List<ESMission>();
            User_Einsätze = new List<User_Mission>();

            ReadSetting();

            OpenSocket();
            SendTestGmt();

            // Receive the response from the remote device.
            Receive(client);
            MainAsync().Wait();
            Console.ReadLine();
        }
        static void ReadSetting()
        {
            try
            {
                var appSettings = ConfigurationSettings.AppSettings;
                Host = appSettings["GmsTerminalHost"] ?? "localhost";
                ProviderName = appSettings["ProviderName"] ?? "EUROFUNK";
                ProviderToken = appSettings["ProviderTOKEN"] ?? "NO";
                Port = Int32.Parse(appSettings["GmsTerminalPort"]);
            }
            catch
            {
                Console.WriteLine("Error reading app settings");
            }
        }
        static async Task MainAsync()
        {
            try
            {

                var hubConnection = new HubConnection("http://gmspps.azurewebsites.net/signalr/");                
                IHubProxy hubProxy = hubConnection.CreateHubProxy("providerhub");
                hubProxy.On<float, float, string, int>("addPin", (LAT, LON, Name, Status) =>
                {
                    string ESID = "0";
                    Console.WriteLine("Incoming data: {0} {1} von {2} Status {3}", LAT, LON, Name, Status);
                    User_Mission mymission = User_Einsätze.Where(x => x.Name == Name).FirstOrDefault();
                    if (mymission != null)
                    {
                        // bei einem Einsatz dabei
                        ESID = mymission.CostumMissionID;
                        if(Status == 1 || Status == 2 || Status == 6)
                        {
                            //Vom Einsatz entfernen
                            User_Einsätze.Remove(mymission);
                        }
                    }
                FMS10GMT(Name, ESID, Status.ToString(), LAT.ToString(), LON.ToString());
                });
                await hubConnection.Start();
                hubProxy.Invoke("Subscribe", ProviderName).Wait();
            }
            catch (Exception ex)
            {

            }

        }
        private static void OpenSocket()
        {

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.                
                IPAddress ipAddress = IPAddress.Parse(Host);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

                // Create a TCP/IP socket.
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void SendTestGmt()
        {
            // Send test INFO/10 to the remote device.
            try
            {
                string GMT = "STX SEND //////" + "INFO" + "/" + "10" + "/" + "GMSPPS;3;Hallo From GMSPPS1 " + " ETX" + crlf; // + "STX QUIT ETX" + crlf;
                Send(client, GMT);
                sendDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void FMS10GMT(string von, string ESid, string status, string lat, string lon)
        {
            // Send test INFO/10 to the remote device.

            ///FMS/10/123;0;1;0;0;0;0;01.01.0001 00:00:00.000;0;0;0;0;-1;-1;
            ///FMS/10/123;0;1;0;0;0;0;21.10.2015 17.38.02.028;0;0;0;0;-1;-1; 
            string FMS10 = $@"FMS/10/{von};{ESid};{status};0;0;{lat.Replace(',', '.')};{lon.Replace(',', '.')};{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff")};0;0;0;0;-1;-1";
            try
            {
                //string[] param = new string[14];
                //param[0] = von;
                //param[1] = "0";
                //param[2] = status;
                //param[3] = "0";
                //param[4] = "0";
                //param[5] = lat.Replace(',', '.');
                //param[6] = lon.Replace(',', '.');
                //param[7] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
                //param[8] = "0";
                //param[9] = "0";
                //param[10] = "0";
                //param[11] = "0";
                //param[12] = "-1";
                //param[13] = "-1";
                //SendGMT("FMS/10", param);
                SendGMT(FMS10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void SendGMT(string gmt, string[] parameter)
        {
            // Send test INFO/10 to the remote device.
            try
            {
                string GMT = "STX SEND //////" + gmt + "/";
                for (int i = 0; i <= parameter.Count() - 1; i++)
                {
                    GMT = GMT + parameter[i] + ";";
                }
                GMT = GMT + " " + " ETX" + crlf; // 
                Send(client, GMT);
                sendDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void SendGMT(string gmt)
        {
            // Send test INFO/10 to the remote device.
            try
            {
                string GMT = "STX SEND //////" + gmt + " ETX" + crlf;
                Send(client, GMT);
                sendDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void SendGMT(string toProzess, string ToNr, string TrNummer, string TrString, string gmt)
        {
            // Send test INFO/10 to the remote device.
            try
            {
                string GMT = $"STX SEND //{toProzess}/{ToNr}/{TrString}/{TrNummer}/" + gmt + " ETX" + crlf;
                Send(client, GMT);
                sendDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void StartClientReciveandClose()
        {
            try
            {

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                Console.WriteLine("Response received : {0}", response);

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.GetEncoding("ISO-8859-1").GetString(state.buffer, 0, bytesRead));
                    response = state.sb.ToString();
                    string[] split = response.Split(' ');
                    String[] parts = response.Split(new char[] { '/' });
                    bool containsSenderAddress = parts[0] == "A";
                    int startIndex = containsSenderAddress ? 3 : 0;

                    int dataStartIndex = 0;
                    for (int i = 0; i <= startIndex + 7; i++)
                    {
                        dataStartIndex += parts[i].Length + 1; // 1 == '/'
                    }
                    string Typ = parts[startIndex + 6];
                    string Nummer = parts[startIndex + 7];
                    string Daten = response.Substring(dataStartIndex);
                    string DatenClean = Daten.Substring(0, Daten.Length - 9);



                    if (split[split.Count() - 1] == "ETX")
                    {
                        // All the data has arrived; put it in response.

                        string[] g = DatenClean.Split(';');
                        Console.WriteLine($" Juhu Receive {Typ} Nr: {Nummer} .....");


                        if (Typ == "FMS" && Nummer == "47")
                        {
                            Console.WriteLine($" FMS/47 erkannt");
                            //Antwor senden

                            string vorXml = @"<?xml version='1.0' encoding='utf - 8' ?>" + g[7];
                            XDocument xdoc = XDocument.Parse(vorXml);
                            XElement mission = xdoc.Descendants("mission").FirstOrDefault();
                            string Title = mission.Element("Title").Value.ToString();
                            string ID = mission.Element("ID").Value.ToString();
                            double Lat = (double)mission.Element("Lat");
                            double Lon = (double)mission.Element("Lon");
                            string Text = mission.Element("Text").Value.ToString().Replace("$r", "<br />");
                            SendMission(new GPMS_MissionModel()
                            {
                                Text = Text,
                                Title = Title,
                                CostumMissionID = ID,
                                Positions = new Position[] { new Position() { LAT = Lat, LON = Lon } }

                            }).Wait();
                            //Antwor senden
                            int Staus = (g[7].Length - 3) / 4;
                            String Antwort = $"FMS/48/{g[0]};{g[1]};{g[2]};{Staus.ToString()};{g[3]};{g[4]};{g[5]};{g[6]};-1;-1";
                            // SendGMT("FMS/48/;2512511;0;25;0;1;0;0;-1;-1");
                            SendGMT(parts[2], parts[3], parts[5], parts[4], Antwort);
                            Console.WriteLine($" FMS/48 gesendet");


                        }

                        // Signal that all bytes have been received.
                        receiveDone.Set();
                        // Receive the response from the remote device.
                        Receive(client);


                    }
                    else
                    {
                        // Get the rest of the data.
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                    }


                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    // receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to Eldis.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static async Task SendMission(GPMS_MissionModel Mission)
        {
            try
            {
                GMSPPS_Client GpmsClient = new GMSPPS_Client(endpoint, ProviderToken);
                string Mission_ID = await GpmsClient.SendMissioAsync(Mission);
                // Am Hub die Mission anmelden um Akk zu erhalten
                if (!Einsätze.Any(x => x.CostumMissionID == Mission.CostumMissionID))
                {
                    ESMission M = new ESMission() { CostumMissionID = Mission.CostumMissionID, ID = Mission_ID };
                    Einsätze.Add(M);
                    var hubConnection = new HubConnection("http://gmspps.azurewebsites.net/signalr/");                   
                    IHubProxy hubProxy = hubConnection.CreateHubProxy("missionhub");
                    hubProxy.On<string, int, int, string>("ClientAkk", (Name, Status, ID, CostMid) =>
                    {
                        Console.WriteLine($"Incoming data Mission von {Name} Status: {Status} ID: {ID} ");
                        ESMission CurendEinsatz = Einsätze.Where(x => x.CostumMissionID == CostMid).FirstOrDefault();
                        if (Status == 2)
                        {
                            User_Mission  m1 = new User_Mission() {CostumMissionID = CurendEinsatz.CostumMissionID, Name = Name };
                            User_Einsätze.Add(m1);

                        }
                            
                            //FMS10GMT(Name, Status.ToString(), LAT.ToString(), LON.ToString());
                    });                   
                    await hubConnection.Start();
                    hubProxy.Invoke("Subscribe", Mission_ID).Wait();

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


    }
}
