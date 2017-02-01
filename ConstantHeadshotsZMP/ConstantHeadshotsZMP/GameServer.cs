using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.ComponentModel;
using Lidgren.Network;

namespace ConstantHeadshotsZMP
{
    // State object for reading client data asynchronously
    /*
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    */

    enum PacketTypes
    {
        LOGIN,
        PLAYERSTATE,
        SERVERSTATE
    }

    public class GameServer
    {
        static NetServer Server;
        static NetPeerConfiguration Config;
        static NetIncomingMessage inc;


        //public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static readonly object syncLock = new object();
        public static Game1 game;
        public static ServerInfo serverInfo = new ServerInfo();
        public static bool Stop = true;
        public static string ip = "localhost";
        //public static Socket listener;
        static DateTime time = DateTime.Now;
        static TimeSpan timetopass = new TimeSpan(0, 0, 0, 0, 10);

        public GameServer(Game1 game)
        {
            GameServer.game = game;
        }

        public static IPEndPoint localEndPoint;

        public static void StartListening()
        {
            Config = new NetPeerConfiguration("game");
            Config.Port = 25565;
            Config.MaximumConnections = 10;
            //Config.LocalAddress
            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            Server = new NetServer(Config);
            Server.Start();

            // Bind the socket to the local endpoint and listen for incoming connections.

            try
            {
                Thread updateThread = new Thread(new ThreadStart(Update));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            

            //Console.WriteLine("\nPress ENTER to continue...");
            //Console.Read();
        
        }


        public static void Update()
        {
            while (!Stop)
            {
                try
                {
                    if ((inc = Server.ReadMessage()) != null)
                    {
                        // Theres few different types of messages. To simplify this process, i left only 2 of em here
                        switch (inc.MessageType)
                        {
                            // If incoming message is Request for connection approval
                            // This is the very first packet/message that is sent from client
                            // Here you can do new player initialisation stuff
                            case NetIncomingMessageType.ConnectionApproval:

                                // Read the first byte of the packet
                                // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                                if (inc.ReadByte() == (byte)PacketTypes.LOGIN)
                                {
                                    //Console.WriteLine("Incoming LOGIN");

                                    // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                                    inc.SenderConnection.Approve();

                                    // Create message, that can be written and sent
                                    NetOutgoingMessage outmsg = Server.CreateMessage();

                                    outmsg.Write((byte)PacketTypes.SERVERSTATE);

                                    lock (syncLock)
                                    {
                                        String data = (new TypeConverter()).ConvertToString(serverInfo);
                                        outmsg.Write(Encoding.ASCII.GetBytes(data));
                                    }

                                    // Now, packet contains:
                                    // Byte = packet type
                                    // Int = how many players there is in game
                                    // character object * how many players is in game

                                    // Send message/packet to all connections, in reliably order, channel 0
                                    // Reliably means, that each packet arrives in same order they were sent. Its slower than unreliable, but easyest to understand
                                    Server.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                                    // Debug
                                    //Console.WriteLine("Approved new connection and updated the world status");
                                }

                                break;
                            // Data type is all messages manually sent from client
                            // ( Approval is automated process )
                            case NetIncomingMessageType.Data:

                                // Read first byte
                                if (inc.ReadByte() == (byte)PacketTypes.PLAYERSTATE)
                                {
                                    TypeConverter typeConverter = new TypeConverter();
                                    byte[] readablebytes = new byte[inc.Data.Length - 1];
                                    inc.ReadBytes(readablebytes, 1, inc.Data.Length - 1);
                                    ClientInfo clientInfo = (ClientInfo)typeConverter.ConvertFromString(Encoding.ASCII.GetString(readablebytes));
                                    game.RecieveInfo(clientInfo);
                                }
                                break;
                            default:
                                // As i statet previously, theres few other kind of messages also, but i dont cover those in this example
                                // Uncommenting next line, informs you, when ever some other kind of message is received
                                Console.WriteLine("Not Important Message");
                                break;
                        }
                        // if 30ms has passed
                        if ((time + timetopass) < DateTime.Now)
                        {
                            // If there is even 1 client
                            if (Server.ConnectionsCount != 0)
                            {
                                // Create new message
                                NetOutgoingMessage outmsg = Server.CreateMessage();

                                // Write byte
                                outmsg.Write((byte)PacketTypes.SERVERSTATE);

                                lock (syncLock)
                                {
                                    String data = (new TypeConverter()).ConvertToString(serverInfo);
                                    outmsg.Write(Encoding.ASCII.GetBytes(data));
                                }


                                // Write Int
                                //outmsg.Write(GameWorldState.Count);

                                // Iterate throught all the players in game
                                //foreach (Character ch2 in GameWorldState)
                                //{

                                // Write all properties of character, to the message
                                //outmsg.WriteAllProperties(ch2);
                                //}

                                // Message contains
                                // byte = Type
                                // Int = Player count
                                // Character obj * Player count

                                // Send messsage to clients ( All connections, in reliable order, channel 0)
                                Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            // Update current time
                            time = DateTime.Now;
                        }
                    }


                    //listener.Bind(localEndPoint);
                    //listener.Listen(100);
                    //while (!Stop)
                    //{
                    // Set the event to nonsignaled state.
                    //allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    //Console.WriteLine("Waiting for a connection...");
                    //listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    //allDone.WaitOne();
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /*

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            if (!Stop)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    try
                    {
                        //ClientInfo clientInfo = state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString();
                        TypeConverter typeConverter = new TypeConverter();
                        ClientInfo clientInfo = (ClientInfo)typeConverter.ConvertFromString(state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString());
                        game.RecieveInfo(clientInfo);
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        SendServerInfo(serverInfo, state);
                    }
                    catch (Exception e2)
                    {

                    }
                    //state.sb.Append(Encoding.ASCII.GetString( state.buffer,0,bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    /*
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1) {
                        // All the data has been read from the 
                        // client. Display it on the console.
                        //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content );
                        // Echo the data back to the client.
                        Send(handler, content);
                    } else {
                        // Not all data received. Get more.
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                    
                }
            }
        }

        public static void SendServerInfo(ServerInfo serverInfo, StateObject state)
        {
            if (!Stop)
            {
                lock (syncLock)
                {
                    String data = (new TypeConverter()).ConvertToString(serverInfo);
                    Socket handler = state.workSocket;
                    String content = String.Empty;
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        // All the data has been read from the 
                        // client. Display it on the console.
                        //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content );
                        // Echo the data back to the client.
                        Send(handler, content);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
            }
        }
    
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.ASCII.GetBytes(data);
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            if (!Stop)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = handler.EndSend(ar);
                    //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        */
    }
    /*
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class GameServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static readonly object syncLock = new object();
        public static Game1 game;
        public static ServerInfo serverInfo = new ServerInfo();
        public static bool Stop = true;
        public static string ip = "localhost";
        public static Socket listener;

        public GameServer(Game1 game)
        {
            GameServer.game = game;
        }

        public static IPEndPoint localEndPoint;

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            foreach (IPAddress ip in ipHostInfo.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    Console.WriteLine(ip.ToString());
                }
            }
            localEndPoint = new IPEndPoint(ipAddress, 25565);

            // Create a TCP/IP socket.
            //Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp )
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.

            try
            {
                Thread updateThread = new Thread(new ThreadStart(Update));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }



            //Console.WriteLine("\nPress ENTER to continue...");
            //Console.Read();

        }


        public static void Update()
        {
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (!Stop)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    //Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            if (!Stop)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    try
                    {
                        //ClientInfo clientInfo = state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString();
                        TypeConverter typeConverter = new TypeConverter();
                        ClientInfo clientInfo = (ClientInfo)typeConverter.ConvertFromString(state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString());
                        game.RecieveInfo(clientInfo);
                    }
                    catch (Exception e)
                    {

                    }

                    try
                    {
                        SendServerInfo(serverInfo, state);
                    }
                    catch (Exception e2)
                    {

                    }
                    //state.sb.Append(Encoding.ASCII.GetString( state.buffer,0,bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    /*
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1) {
                        // All the data has been read from the 
                        // client. Display it on the console.
                        //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content );
                        // Echo the data back to the client.
                        Send(handler, content);
                    } else {
                        // Not all data received. Get more.
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                   
                }
            }
        }

        public static void SendServerInfo(ServerInfo serverInfo, StateObject state)
        {
            if (!Stop)
            {
                lock (syncLock)
                {
                    String data = (new TypeConverter()).ConvertToString(serverInfo);
                    Socket handler = state.workSocket;
                    String content = String.Empty;
                    content = state.sb.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        // All the data has been read from the 
                        // client. Display it on the console.
                        //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content );
                        // Echo the data back to the client.
                        Send(handler, content);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.ASCII.GetBytes(data);
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            if (!Stop)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = handler.EndSend(ar);
                    //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
    */
}
