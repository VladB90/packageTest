using MBody;
using Google.Protobuf;
using System.Collections.Generic;
using UnityEngine;

namespace MBody
{
    /// <summary>
    /// Manages the creation, configuration, and execution of nodes in a pipeline.
    /// Singleton class to ensure only one instance of NodeManager exists.
    /// </summary>
    public sealed class NodeManager
    {
        private static NodeManager nodeManager = null;

        private Dictionary<string, BaseNode> nodeMapping = new Dictionary<string, BaseNode>();

        private byte[] receiveMemoryBuffer;
        private byte[] sendMemoryBuffer;

        private const int BUFFER_SIZE = 5000000;

        private int contextId = -1;

        //public int getContextId()
        //{
        //    return contextId;
        //}

        /// <summary>
        /// Initializes the manager and allocates memory buffers.
        /// </summary>
        private NodeManager()
        {
            InitializeManager();
            Debug.Log("Initialized context with ID: " + contextId);
            receiveMemoryBuffer = new byte[BUFFER_SIZE];
            sendMemoryBuffer = new byte[BUFFER_SIZE];
        }
        /// <summary>
        /// Cleans up resources and deinitializes the manager.
        /// </summary>
        ~NodeManager()
        {
            StopPipeline();
            DeinitializeManager();
            receiveMemoryBuffer = null;
            sendMemoryBuffer = null;
        }
        /// <summary>
        /// Gets the singleton instance of NodeManager.
        /// Creates a new instance if one does not exist.
        /// </summary>
        public static NodeManager Instance
        {
            get
            {
                if (nodeManager == null)
                {
                    nodeManager = new NodeManager();
                }
                return nodeManager;
            }
        }
        /// <summary>
        /// Initializes the manager by deinitializing any existing context and creating a new one.
        /// </summary>
        public void InitializeManager()
        {
            // Deinitialize existing manager ID, initialize new one
            DeinitializeManager();

            contextId = UnityNodeWrapper.InitializeManager();
        }
        /// <summary>
        /// Deinitializes the manager by clearing node mappings and deinitializing the context.
        /// </summary>
        public void DeinitializeManager()
        {
            nodeMapping.Clear();
            UnityNodeWrapper.DeinitializeManager(contextId);
            Debug.Log("Deinitialized context with ID: " + contextId);
        }
        /// <summary>
        /// Adds a node to the node mappings
        /// </summary>
        /// <param name="node">A node to register</param>
        /// <returns>True if the node was successfully registered, false if a node with the same ID already exists.</returns>
        public bool RegisterUnityNode(BaseNode node)
        {
            if (nodeMapping.ContainsKey(node.nodeId))
            {
                return false;
            }

            nodeMapping.Add(node.nodeId, node);

            node.RegisterOnDataProcessed(PushData);

            int result = UnityNodeWrapper.CreateNodeOfType(contextId, node.nodeId, "toolNode");
            return true;
        }

        /// <summary>
        /// Creates a new node of a specified type in the pipeline.
        /// </summary>
        /// <param name="nodeId">The ID of the node to create.</param>
        /// <param name="nodeTypeName">The type of the node to create.</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int CreateNodeInPipeline(string nodeId, string nodeTypeName)
        {
            int result = UnityNodeWrapper.CreateNodeOfType(contextId, nodeId, nodeTypeName);
            return result;
        }
        /// <summary>
        /// Starts the pipeline, starting all registered nodes.
        /// </summary>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int StartPipeline()
        {
            int result = UnityNodeWrapper.StartPipeline(contextId);
            if (result == 1)
            {
                foreach (var node in nodeMapping.Values)
                {
                    node.StartNode();
                }
            }
            return result;
        }
        /// <summary>
        /// Stops the pipeline.
        /// </summary>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int StopPipeline()
        {
            int result = UnityNodeWrapper.StopPipeline(contextId);
            return result;
        }
        /// <summary>
        /// Binds the output stream of one node to the input stream of another node.
        /// </summary>
        /// <param name="outputNodeId">The ID of the output node.</param>
        /// <param name="outputIndex">The output index of the output node.</param>
        /// <param name="inputNodeId">The ID of the input node.</param>
        /// <param name="inputIndex">The input index of the input node.</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int BindNodeStreams(string outputNodeId, int outputIndex, string inputNodeId, int inputIndex)
        {
            int result = UnityNodeWrapper.BindNodeStreams(contextId, outputNodeId, outputIndex, inputNodeId, inputIndex);
            return result;
        }
        /// <summary>
        /// Creates an input stream for a node.
        /// </summary>
        /// <param name="nodeId">The ID of the node.</param>
        /// <param name="inputStreamType">The type of input stream to create.</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int CreateInputForNode(string nodeId, string inputStreamType)
        {
            int result = UnityNodeWrapper.CreateInputForNode(contextId, nodeId, inputStreamType);
            return result;
        }
        /// <summary>
        /// Creates an output stream for a node.
        /// </summary>
        /// <param name="nodeId">The ID of the node.</param>
        /// <param name="outputStreamType">The type of output stream to create.</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int CreateOutputForNode(string nodeId, string outputStreamType)
        {
            int result = UnityNodeWrapper.CreateOutputForNode(contextId, nodeId, outputStreamType);
            return result;
        }
        /// <summary>
        /// Configures a node in the pipeline with the specified configuration.
        /// </summary>
        /// <param name="nodeId">The ID of the node to configure.</param>
        /// <param name="config">A dictionary containing the configuration parameters.</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int ConfigurePipelineNode(string nodeId, Dictionary<string, string> config)
        {
            NodeConfigurationMessage configMessage = new NodeConfigurationMessage();
            configMessage.ConfgurationMap.Add(config);

            byte[] serializedMessage = configMessage.ToByteArray();

            serializedMessage.CopyTo(sendMemoryBuffer, 0);

            int result = UnityNodeWrapper.ProvideNodeConfiguration(contextId, nodeId, serializedMessage.Length, sendMemoryBuffer);
            return result;
        }
        /// <summary>
        /// Pushes data to a specified node's output stream.
        /// </summary>
        /// <param name="nodeId">The ID of the node.</param>
        /// <param name="outputId">The output stream ID of the node.</param>
        /// <param name="data">The data to push.</param>
        public void PushData(string nodeId, int outputId, byte[] data)
        {
            int dataLength = data.Length;
            data.CopyTo(sendMemoryBuffer, 0);

            UnityNodeWrapper.AddDataToStream(contextId, nodeId, outputId, dataLength, sendMemoryBuffer);
        }
        /// <summary>
        /// Checks for data from all registered nodes and processes it if it exists.
        /// </summary>
        [ContextMenu("Check Data")]
        public void CheckForData()
        {
            foreach (var nodeId in nodeMapping.Keys)
            {

                //Debug.Log("Reading data from node " + nodeId);
                int dataLength = UnityNodeWrapper.CheckNodeForData(contextId, nodeId, 0, 0, 1, receiveMemoryBuffer);

                if (dataLength > 0)
                {
                    Debug.Log("Data exists");
                    byte[] receivedData = new byte[dataLength];
                    for (int i = 0; i < dataLength; i++)
                    {
                        receivedData[i] = receiveMemoryBuffer[i];
                    }
                    nodeMapping[nodeId].HandleFrameData(receivedData, 0);
                }
                else
                {
                    //Debug.Log("No Data Found");
                }



            }

        }
        /// <summary>
        /// Creates a UDP communicator for the specified client and server endpoints.
        /// </summary>
        /// <param name="communicatorName">The name of the communicator.</param>
        /// <param name="clientIp">The client IP address.</param>
        /// <param name="clientPort">The client port.</param>
        /// <param name="serverIp">The server IP address.</param>
        /// <param name="serverPort">The server port.</param>
        /// <returns>Returns 1 on success, 0 on failure.</returns>
        public int CreateUDPCommunicator(string communicatorName, string clientIp, int clientPort, string serverIp, int serverPort)
        {
            int result = UnityNodeWrapper.CreateUDPCommunicator(contextId, communicatorName, clientIp, clientPort, serverIp, serverPort);
            return result;
        }
        /// <summary>
        /// Creates a communicator node in the pipeline.
        /// </summary>
        /// <param name="nodeId">The ID of the node.</param>
        public void CreateCommunicatorNode(string nodeId)
        {
            UnityNodeWrapper.CreateCommunicatorNode(contextId, nodeId);
        }
    }
}

