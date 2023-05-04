using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using AsyncIO;
using System.Threading;
using NetMQ.Sockets;
using System;
using System.Text;

public class SendXPath : MonoBehaviour
{
    public ZmqClientEntity testClient;

    string ipAddress = "192.168.31.138";
    string port = "1215";

    // Start is called before the first frame update
    void Start()
    {
        testClient = new ZmqClientEntity(ipAddress, port);
        testClient.Start();
        Debug.Log("Client.Start()");
    }

    // Update is called once per frame
    void Update()
    {
        if (testClient.isNeededReconnection)
        {
            print("ready to reconnect ......");
            testClient.Stop();
            testClient = null;
            testClient = new ZmqClientEntity(ipAddress, port);
            testClient.Start();
        }
    }

    private void OnDestroy()
    {
        testClient.Stop();
        testClient = null;
    }

    public class ZmqClientEntity : RunAbleThread
    {
        public string connect_ip;
        public string connect_port;

        public bool isNeededReconnection;

        // references
        public Queue<Tuple<string, float, float, int>> gazeQueue;

        //public object gazeQueueLock;

        public ZmqClientEntity(string ip, string port)
        {
            connect_ip = ip;
            connect_port = port;

            isNeededReconnection = false;
        }

        protected override void Run()
        {
            ForceDotNet.Force();

            //using (RequestSocket client = new RequestSocket())
            //{
            //    string connect_information = "tcp://" + connect_ip + ":" + connect_port;
            //    client.Connect(connect_information);
            //    Debug.Log("client.Connect");


            //    for (int i = 0; Running; i++)
            //    {
            //        Thread.Sleep(1000);

            //        client.SendFrame("hello");
            //        Debug.Log("client.SendFrame(\"hello\")");

            //        string message = null;
            //        bool gotMessage = false;

            //        while (Running)
            //        {
            //            gotMessage = client.TryReceiveFrameString(TimeSpan.FromSeconds(3), out message);
            //            if (gotMessage)
            //            {
            //                Debug.Log("Message : " + message);
            //                break;
            //            }
            //            else
            //            {
            //                Debug.Log("Reconnect!!!!!");
            //                isNeededReconnection = true;
            //                break;
            //            }
            //        }
            //        if (isNeededReconnection)
            //        {
            //            break;
            //        }
            //    }
            //}

            NetMQConfig.Cleanup();

        }

        public void SendString(string XPath)
        {
            // create a new socket to send to server
            using (RequestSocket socket = new RequestSocket())
            {
                string connect_information = "tcp://" + connect_ip + ":" + connect_port;
                socket.Connect(connect_information);
                socket.SendFrame(XPath);

                string message = null;
                bool gotMessage = false;

                gotMessage = socket.TryReceiveFrameString(TimeSpan.FromSeconds(4), out message);
                if (gotMessage)
                {
                    Debug.Log("Message: " + message);
                }
                else
                {
                    Debug.Log("No response");
                }
            }
        }
    }

    public abstract class RunAbleThread
    {
        private readonly Thread _runnerThread;

        protected RunAbleThread()
        {
            // we need to create a thread instead of calling Run() directly because it would block unity
            // from doing other tasks like drawing game scenes
            _runnerThread = new Thread(Run);
        }

        public bool Running { get; private set; }

        /// <summary>
        /// This method will get called when you call Start(). Programmer must implement this method while making sure that
        /// this method terminates in a finite time. You can use Running property (which will be set to false when Stop() is
        /// called) to determine when you should stop the method.
        /// </summary>
        protected abstract void Run();

        public void Start()
        {
            Running = true;
            _runnerThread.Start();
        }

        public void Stop()
        {
            Running = false;
            // block main thread, wait for _runnerThread to finish its job first, so we can be sure that 
            // _runnerThread will end before main thread end
            _runnerThread.Join();
        }
    }
}
