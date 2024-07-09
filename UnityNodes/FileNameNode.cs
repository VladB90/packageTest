using MBody;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace MBody
{
    /// <summary>
    /// Represents a node that processes file names.
    /// </summary>
    public class FileNameNode : BaseNode
    {
        public string filePath = "";
        /// <summary>
        /// Initializes a new instance of the FileNameNode class with the specified node ID.
        /// </summary>
        /// <param name="nodeId">The unique identifier for the node.</param>
        public FileNameNode(string nodeId) : base(nodeId)
        {

        }
        /// <summary>
        /// Handles incoming frame data, extracts the file path, and sends a response message.
        /// </summary>
        /// <param name="data">The incoming data as a byte array.</param>
        /// <param name="inputIndex">The index of the input stream.</param>
        public override void HandleFrameData(byte[] data, int inputIndex)
        {
            FrameCollectionMessage message = FrameCollectionMessage.Parser.ParseFrom(data);
            StringData testData = StringData.Parser.ParseFrom(message.Data);
            string path = testData.StringData_;

            filePath = path;

            Debug.Log("File Name node received this: " + filePath);

            StringData sendData = new StringData
            {
                StringData_ = filePath
            };

            ByteString serializedData = sendData.ToByteString();

            FrameCollectionMessage sendMessage = new FrameCollectionMessage()
            {
                Data = serializedData,
                DataTypeName = "StringData",
                StartFrame = 7,
                EndFrame = 15
            };

            byte[] serializedMessage = sendMessage.ToByteArray();
            InvokeDataProcessed(0, serializedMessage);
        }
        /// <summary>
        /// Starts the node.
        /// </summary>
        public override void StartNode()
        {

        }
        /// <summary>
        /// Prepares and sends the file name as a serialized message.
        /// </summary>
        /// <returns>The serialized message containing the file name.</returns>
        public byte[] SendFileName()
        {
            StringData data = new StringData
            {
                StringData_ = filePath
            };

            ByteString serializedData = data.ToByteString();

            FrameCollectionMessage message = new FrameCollectionMessage()
            {
                Data = serializedData,
                DataTypeName = "StringData",
                StartFrame = 7,
                EndFrame = 15
            };

            Debug.Log(message.Data.ToStringUtf8());
            byte[] serializedMessage = message.ToByteArray();
            //ByteString serializedMessage = message.ToByteString();
            //communicator.SendData(serializedMessage.ToStringUtf8(), 0);
            return serializedMessage;

        }
    }
}
