using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;
using System.Threading;

public class TcpTest : MonoBehaviour
{

    public GameObject ghost;

    bool start = false;

    string uuid = null;

    private Thread clientReceiveThread;

    private TcpClient socketConnection;

    List<string> clientsToSpawn;
    List<NetworkGhostTransform> ghostsTransforms;

    bool debugTcp = false;
    int tickInterval = 20;

    Vector3 ghostPos;
    Quaternion qRot;
    public GameObject ghostTracker;
    public bool trackerMethod = false;

    class NetworkGhostTransform
    {
        public Vector3 pos;
        public Quaternion rot;
        public string reference;

        public NetworkGhostTransform(Vector3 v, Quaternion q, string r)
        {
            pos = v;
            rot = q;
            reference = r;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        clientsToSpawn = new List<string>();
        ghostsTransforms = new List<NetworkGhostTransform>();


        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }


    /// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("miseosolutions.com", 8080);
            Byte[] bytes = new Byte[1024];

            // socketConnection


            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        if (debugTcp) Debug.Log("server message received as: " + serverMessage);

                        HandleMessageReceived(serverMessage);



                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    void HandleMessageReceived(string messageReceived)
    {


        string[] splMessageReceived = messageReceived.Split('_');
        string messageLabel = splMessageReceived[0];

        if (messageLabel == "Hello")
        {
            uuid = splMessageReceived[1];
        }

        if (messageLabel == "NewClient")
        {
            ghostsTransforms.Add(new NetworkGhostTransform(new Vector3(0, 0, 0), Quaternion.identity, "Ghost_" + splMessageReceived[1]));
        }

        if (messageLabel == "GetPos")
        {

            string reference = "Ghost_" + splMessageReceived[1];
            // GameObject ghostReference = GameObject.Find(reference);

            Debug.Log("Seeking reference " + reference);

            Vector3 pos = new Vector3(float.Parse(splMessageReceived[2]), float.Parse(splMessageReceived[3]), float.Parse(splMessageReceived[4]));

            Quaternion q = Quaternion.Euler(
                new Vector3(float.Parse(splMessageReceived[5]), float.Parse(splMessageReceived[6]), float.Parse(splMessageReceived[7]))
                );

            foreach (NetworkGhostTransform ngt in ghostsTransforms)
            {
                if (ngt.reference == reference)
                {
                    ngt.pos = pos;
                    ngt.rot = q;

                    Debug.Log("Update ngt vals");
                }
            }

            // networkPlayerTransformQueue.Add(new NetworkPlayerTransformQueueObj(pos, q, reference));

        }

    }

    // Update is called once per frame
    void Update()
    {


        foreach (string uuid in clientsToSpawn)
        {
            GameObject newGhost = GameObject.Instantiate(ghost);
            newGhost.transform.name = "Ghost_" + uuid;
            // Debug.Log(uuid);

            clientsToSpawn.Remove(uuid);
        }

        foreach (NetworkGhostTransform nptq in ghostsTransforms)
        {
            GameObject reference = GameObject.Find(nptq.reference);

            // Debug.Log(nptq.reference);
            if (reference)
            {
                reference.transform.position = nptq.pos;
                reference.transform.rotation = nptq.rot;

                Debug.Log("update ref transf");

                // networkPlayerTransformQueue.Remove(nptq);
            }

            else
            {
                GameObject createReference = GameObject.Instantiate(ghost);
                createReference.transform.name = nptq.reference;

                if (trackerMethod)
                {
                    GameObject newTracker = GameObject.Instantiate(ghostTracker);
                    newTracker.transform.name = "Tracker_" + nptq.reference;

                    newTracker.transform.position = createReference.transform.position;

                    TrackerFollow tf = newTracker.GetComponent<TrackerFollow>();
                    tf.setTarget(createReference.transform);
                }

            }


        }



        if (Time.frameCount % tickInterval == 0)
        {

            Vector3 p = transform.position;
            Vector3 r = transform.rotation.eulerAngles;

            byte[] msg = Encoding.ASCII.GetBytes("GT_" + uuid + "_" + p.x + "_" + p.y + "_" + p.z + "_" + r.x + "_" + r.y + "_" + r.z);

            socketConnection.GetStream().Write(msg, 0, msg.Length);





        }
    }
}
