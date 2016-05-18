using Microsoft.AspNet.SignalR;
using OrDragon.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;

namespace OrDragon.Models
{
    public class Map
    {
        public volatile List<Noeud> noeuds = new List<Noeud>();
        public List<Chemin> chemins = new List<Chemin>();
        public MapListener mapListener;
        public Image background;

        public Map()
        {
            mapListener = new MapListener(this);
        }

        //public Image GetImage(string url)
        //{
        //    var request = WebRequest.Create(url);

        //    using (var response = request.GetResponse())
        //    using (var stream = response.GetResponseStream())
        //    {
        //        pictureBox1.Image = Bitmap.FromStream(stream);
        //    }
        //}

        public void GetFromServer()
        {
            noeuds.Clear();

                // Data buffer for incoming data.
            byte[] bytes = new byte[20480];
            String data = String.Empty;

            // Connect to a remote device.
            try {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPAddress ipAddress = IPAddress.Parse("149.56.47.97");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 51005);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork, 
                    SocketType.Stream, ProtocolType.Tcp );

                // Connect the socket to the remote endpoint. Catch any errors.
                try {
                    sender.Connect(remoteEP);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                } catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}",ane.ToString());
                } catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}",se.ToString());
                } catch (Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

                if (data != String.Empty) ParseData(data);

            } catch (Exception e) {
                Console.WriteLine( e.ToString());
            }
        }

        private void ParseData(String Data)
        {
            using (StringReader strRead = new StringReader(Data))
            {
                string strGetLineVal;
                bool NoeudDone = false;

                while ((strGetLineVal = strRead.ReadLine()) != null)
                {
                    if (strGetLineVal != "")
                    {
                        if (!NoeudDone)
                        {
                            string[] parameters = strGetLineVal.Split(' ');

                            int id = int.Parse(parameters[0]);
                            int x = int.Parse(parameters[1]);
                            int y = int.Parse(parameters[2]);
                            int tmpbool = int.Parse(parameters[3]);
                            bool boolean = tmpbool != 0;

                            noeuds.Add(new Noeud(id, new Point(x,y), boolean));
                        }
                        else
                        {
                            string[] parameters = strGetLineVal.Split(' ');

                            for (int i = 0; i < parameters.Length - 1; i++)
                            {
                                int id_noeud1 = int.Parse(parameters[0]);
                                int id_noeud2 = int.Parse(parameters[i + 1]);

                                Noeud noeud1 = noeuds.Find(x => x.Id == id_noeud1);
                                Noeud noeud2 = noeuds.Find(x => x.Id == id_noeud2);

                                if (noeud1 != null && noeud2 != null) chemins.Add(new Chemin(noeud1, noeud2));
                            } 
                        }
                    }
                    else NoeudDone = true;
                }
            }
        }

        public void StartListener()
        {
            Thread listenerThread = new Thread(mapListener.DoWork);
            listenerThread.Start();
        }

        public class MapListener
        {
            Map _map;
            List<Models.Game.Entity> entities = new List<Models.Game.Entity>();
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MapHub>();

            public MapListener(Map map)
            {
                _map = map;
            }

            // This method will be called when the thread is started.
            public void DoWork()
            {
                byte[] bytes = new byte[2048];
                String data = String.Empty;
                
                IPAddress ipAddress = null;
                IPEndPoint remoteEP = null;
                Socket sender = null;

                // Connect to a remote device.
                try
                {
                    // Establish the remote endpoint for the socket.
                    // This example uses port 11000 on the local computer.
                    ipAddress = IPAddress.Parse("149.56.47.97");
                    remoteEP = new IPEndPoint(ipAddress, 51006);

                    // Create a TCP/IP  socket.
                    sender = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);

                    sender.Connect(remoteEP);

                }
                catch (Exception e)
                {
                    this.RequestStop();
                }

                while (!_shouldStop)
                {
                    entities.Clear();
                    // Connect the socket to the remote endpoint. Catch any errors.
                    try
                    {
                        // Receive the response from the remote device.
                        int bytesRec = sender.Receive(bytes);
                        data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        ParseData(data);
                        context.Clients.All.sendEntities(entities);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unexpected exception : {0}", e.ToString());
                        RequestStop();
                    }
                }

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            public void RequestStop()
            {
                _shouldStop = true;
            }

            public void ParseData(String data)
            {
                data = data.Replace("\n", string.Empty);
                string[] parameters = data.Split(' ');

                foreach(var entity in parameters)
                {
                    if (!entity.Trim().Equals(String.Empty))
                    {
                        string[] ents = entity.Split(':');
                        Noeud tmpnoeud = _map.noeuds.Find(x => x.Id == int.Parse(ents[0]));

                        entities.Add(new Models.Game.Entity(ents[1], tmpnoeud));
                    }
                }
            }
            // Volatile is used as hint to the compiler that this data
            // member will be accessed by multiple threads.
            private volatile bool _shouldStop;
        }

        public class Noeud
        {
            public int Id { get; set; }
            public Point Coordonates { get; set; } // Relative to 1600 x 900
            public bool Buildable { get; set; }

            public Noeud(int id, Point coord, bool build)
            {
                Id = id;
                Coordonates = coord;
                Buildable = build;
            }
        }

        public class Chemin
        {
            public Noeud Noeud1 { get; set; }
            public Noeud Noeud2 { get; set; }

            public Chemin(Noeud noeud1, Noeud noeud2)
            {
                Noeud1 = noeud1;
                Noeud2 = noeud2;
            }
        }
    }
}