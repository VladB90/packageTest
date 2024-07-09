using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MBody
{
    /// <summary>
    /// Wrapper class responsible for accessing functions within the MBody Library for running model inference
    /// </summary>
    /// 
    public class UnityNodeWrapper
    {

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddDataToStream(int contextId, string nodeId, int outputIndex, int dataLength, byte[] writeBuffer);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CheckNodeForData(int contextId, string nodeId, int inputIndex, int startFrame, int endFrame, byte[] writeBuffer);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateNodeOfType(int contextId, string nodeId, string nodeTypeName);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ProvideNodeConfiguration(int contextId, string nodeId, int configLength, byte[] writeBuffer);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartPipeline(int contextId);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int StopPipeline(int contextId);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindNodeStreams(int contextId, string outputNodeId, int outputIndex, string inputNodeId, int inputIndex);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateInputForNode(int contextId, string nodeId, string inputStreamType);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateOutputForNode(int contextId, string nodeId, string inputStreamType);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CreateUDPCommunicator(int contextId, string communicatorName, string clientIp, int clientPort, string serverIp, int serverPort);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitializeManager();

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DeinitializeManager(int contextId);

        [DllImport("MachineLearningARTP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateCommunicatorNode(int contextId, string nodeName);

        // TODO: Add call to AddOutputData(nodeId, outputIndex, data)

        //public static void PushDataToLibrary(string nodeId, int outputIndex, int dataLength, StringBuilder readBuffer)
        //{
        //    // TODO: Link with DLL
        //}



    }
}