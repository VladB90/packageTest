using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace MBody
{

    /// <summary>
    /// Base class for nodes in a communication system.
    /// </summary>
    public abstract class BaseNode
    {
        public string nodeId = "";
        public INodeCommunicator communicator;


        /// <summary>
        /// Action invoked when data is ready to be added for a Node output. Fields are Node ID, Output Index, and data.
        /// </summary>
        /// 
        public event Action<string, int, byte[]> OnDataProcessed;

        /// <summary>
        ///  Constructor for BaseNode class.
        /// </summary>
        /// <param name="nodeId">Identifier for the node. </param>
        public BaseNode(string nodeid) //INodeCommunicator communicator
        {
            this.nodeId = nodeid;
            // comminacator = communicator;
        }

        /// <summary>
        /// Destructor for BaseNode class.
        /// </summary>
        ~BaseNode()
        {
            communicator.OnDataReceived -= HandleFrameData;
        }

        /// <summary>
        ///  Abstract method to start the node.
        /// </summary>
        public abstract void StartNode();

        /// <summary>
        /// Abstract method to handle frame data.
        /// </summary>
        /// <param name="data">Data received.</param>
        /// <param name="inputIndex">Index of the input.</param>
        public abstract void HandleFrameData(byte[] data, int inputIndex);

        /// <summary>
        /// Overloaded method to handle frame data as string.
        /// </summary>
        /// <param name="data">String data received.</param>
        /// <param name="inputIndex">Index of the input.</param>
        public void HandleFrameData(string data, int inputIndex)
        {
            HandleFrameData(Encoding.UTF8.GetBytes(data), inputIndex);
        }

        //public void RegisterOnDataReceived(Action<string> callback)
        //{
        //   comminacator.OnDataReceived += callback;
        //}

        //public void SendData(string data)
        //{ 
        //    comminacator.SendData(data);
        //}

        //TODO: it should be removed 
        // public event Action<string, int, byte[]> OnDataProcessed; 


        /// <summary>
        /// Sets the communicator for the node.
        /// </summary>
        /// <param name="communicator">Communicator instance.</param>
        public void SetCommunicator(INodeCommunicator communicator)
        {
            this.communicator = communicator;
            communicator.OnDataReceived += HandleFrameData;
        }

        /// <summary>
        /// Registers a callback for data processed event.
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public void RegisterOnDataProcessed(Action<string, int, byte[]> callback)
        {
            OnDataProcessed += callback;
        }


        /// <summary>
        /// Deregisters a callback for data processed event.
        /// </summary>
        /// <param name="callback">Callback to deregister.</param>
        public void DeregisterOnDataProcessed(Action<string, int, byte[]> callback)
        {
            OnDataProcessed -= callback;
        }

        /// <summary>
        /// Invokes the data processed event.
        /// </summary>
        /// <param name="outputIndex">Index of the output.</param>
        /// <param name="data">Processed data.</param>
        protected void InvokeDataProcessed(int outputIndex, byte[] data)
        {
            OnDataProcessed?.Invoke(nodeId, outputIndex, data);
        }
    }
}