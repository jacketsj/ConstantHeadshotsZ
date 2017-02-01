using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using Lidgren.Network;

namespace ConstantHeadshotsZMP
{

    public class GameClient
    {
        static NetClient Client;
        static System.Timers.Timer update;



        public static string ip = "localhost";
        //public static string ip = "127.0.0.1";
        public static readonly object syncLock = new object();

        // The port number for the remote device.
        private const int port = 25565;

        // ManualResetEvent instances signal completion.
        //private static ManualResetEvent connectDone = new ManualResetEvent(false);
        //private static ManualResetEvent sendDone = new ManualResetEvent(false);
        //private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public static bool Stop = true;

        public static Game1 game;

        // The response from the remote device.
        //private static String response = String.Empty;

        public static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                NetPeerConfiguration Config = new NetPeerConfiguration("game");
                Client = new NetClient(Config);
                NetOutgoingMessage outmsg = Client.CreateMessage();
                Client.Start();
                outmsg.Write((byte)PacketTypes.LOGIN);
                outmsg.Write(game.clientInfo.player.name);
                Client.Connect(ip, 14242, outmsg);
                update = new System.Timers.Timer(10);
                update.Elapsed += new System.Timers.ElapsedEventHandler(update_Elapsed);
                WaitForStartingInfo();
                update.Start();


                /*
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[1];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                //Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.
                //Send(client, "This is a test<EOF>");
                sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                //Console.WriteLine("Response received : {0}", response);

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                */

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void update_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Check if server sent new messages
            CheckServerMessages();

            // Draw the world
            //DrawGameState();
        }
        
        private static void SendMessage()
        {
            if(Stop)
            {
                // Disconnect and give the reason
                Client.Disconnect("Leaving");

            }

            // Create new message
            NetOutgoingMessage outmsg = Client.CreateMessage();

            // Write byte = Set "MOVE" as packet type
            outmsg.Write((byte)PacketTypes.PLAYERSTATE);

            // Write byte = move direction
            String data = (new TypeConverter()).ConvertToString(game.clientInfo);
            outmsg.Write(Encoding.ASCII.GetBytes(data));

            // Send it to server
            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
        }

        private static void CheckServerMessages()
        {
            // Create new incoming message holder
            NetIncomingMessage inc;

            // While theres new messages
            //
            // THIS is exactly the same as in WaitForStartingInfo() function
            // Check if its Data message
            // If its WorldState, read all the characters to list
            while ((inc = Client.ReadMessage()) != null)
            {
                if (inc.MessageType == NetIncomingMessageType.Data)
                {
                    if (inc.ReadByte() == (byte)PacketTypes.SERVERSTATE)
                    {
                        byte[] readablebytes = new byte[inc.Data.Length - 1];
                        inc.ReadBytes(readablebytes, 1, inc.Data.Length - 1);
                        TypeConverter typeConverter = new TypeConverter();
                        ServerInfo clientInfo = (ServerInfo)typeConverter.ConvertFromString(Encoding.ASCII.GetString(readablebytes));
                        game.RecieveInfo(clientInfo);
                    }
                    break;
                }
                SendMessage();
            }
        }

        private static void WaitForStartingInfo()
        {
            // When this is set to true, we are approved and ready to go
            bool CanStart = false;

            // New incomgin message
            NetIncomingMessage inc;

            // Loop untill we are approved
            while (!CanStart)
            {

                // If new messages arrived
                if ((inc = Client.ReadMessage()) != null)
                {
                    // Switch based on the message types
                    switch (inc.MessageType)
                    {

                        // All manually sent messages are type of "Data"
                        case NetIncomingMessageType.Data:

                            // Read the first byte
                            // This way we can separate packets from each others
                            if (inc.ReadByte() == (byte)PacketTypes.SERVERSTATE)
                            {
                                byte[] readablebytes = new byte[inc.Data.Length - 1];
                                inc.ReadBytes(readablebytes, 1, inc.Data.Length - 1);
                                TypeConverter typeConverter = new TypeConverter();
                                ServerInfo clientInfo = (ServerInfo)typeConverter.ConvertFromString(Encoding.ASCII.GetString(readablebytes));
                                game.RecieveInfo(clientInfo);
                                // When all players are added to list, start the game
                                CanStart = true;
                            }
                            break;

                        default:
                            // Should not happen and if happens, don't care
                            Console.WriteLine(inc.ReadString() + " Strange message");
                            break;
                    }
                }
            }
        }

        /*
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                //Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

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
            if (!Stop)
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
                        // There  might be more data, so store the data received so far.
                        try
                        {
                            //ClientInfo clientInfo = state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString();
                            TypeConverter typeConverter = new TypeConverter();
                            ServerInfo clientInfo = (ServerInfo)typeConverter.ConvertFromString(state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString());
                            game.RecieveInfo(clientInfo);
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            SendClientInfo(game.clientInfo, state);
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

                    if (bytesRead > 0)
                    {
                        // There might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                        // Get the rest of the data.
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // All the data has arrived; put it in response.
                        if (state.sb.Length > 1)
                        {
                            response = state.sb.ToString();
                        }
                        // Signal that all bytes have been received.
                        receiveDone.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void SendClientInfo(ClientInfo serverInfo, StateObject state)
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
                        new AsyncCallback(ReceiveCallback), state);
                    }
                }
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
            if (!Stop)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket client = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = client.EndSend(ar);
                    //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                    // Signal that all bytes have been sent.
                    sendDone.Set();
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
    public class GameClient
    {
        public static string ip = "localhost";
        //public static string ip = "127.0.0.1";
        public static readonly object syncLock = new object();

        // The port number for the remote device.
        private const int port = 25565;

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public static bool Stop = true;

        public static Game1 game;

        // The response from the remote device.
        private static String response = String.Empty;

        public static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[1];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                //Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.
                //Send(client, "This is a test<EOF>");
                sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                //Console.WriteLine("Response received : {0}", response);

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

                //Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

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
            if (!Stop)
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
                        // There  might be more data, so store the data received so far.
                        try
                        {
                            //ClientInfo clientInfo = state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString();
                            TypeConverter typeConverter = new TypeConverter();
                            ServerInfo clientInfo = (ServerInfo)typeConverter.ConvertFromString(state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead)).ToString());
                            game.RecieveInfo(clientInfo);
                        }
                        catch (Exception e)
                        {

                        }

                        try
                        {
                            SendClientInfo(game.clientInfo, state);
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

                    if (bytesRead > 0)
                    {
                        // There might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                        // Get the rest of the data.
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                    else
                    {
                        // All the data has arrived; put it in response.
                        if (state.sb.Length > 1)
                        {
                            response = state.sb.ToString();
                        }
                        // Signal that all bytes have been received.
                        receiveDone.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void SendClientInfo(ClientInfo serverInfo, StateObject state)
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
                        new AsyncCallback(ReceiveCallback), state);
                    }
                }
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
            if (!Stop)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket client = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = client.EndSend(ar);
                    //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                    // Signal that all bytes have been sent.
                    sendDone.Set();
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
