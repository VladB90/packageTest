using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MBody
{
    /// <summary>
    /// Interface for communication between nodes.
    /// </summary>
    public interface INodeCommunicator
    {
        public event Action<byte[], int> OnDataReceived;

        /// <summary>
        /// Sends data to a specified target index.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="targetIndex">Index of the target.</param>
        void SendData(string data, int targetIndex);
    }
}