using MBody;
using System.IO;
using UnityEditor;
using UnityEngine;
using Google.Protobuf;
using System;

namespace MBody
{
    /// <summary>
    /// Represents a node responsible for importing generated animation asset within the Unity editor.
    /// </summary>
    public class AssetImporterNode : BaseNode
    {
        public string assetPath = "";
        public string assetDestinationFolder = "Assets/Imported/";
        /// <summary>
        /// Initializes a new instanse of the AssetImporterNode.
        /// </summary>
        /// <param name="nodeId">Unique node ID.</param>
        public AssetImporterNode(string nodeId) : base(nodeId)
        {
        }
        /// <summary>
        /// Handles incoming frame data, processes it to import an asset, and sends a response message.
        /// </summary>
        /// <param name="data">The data received by the node.</param>
        /// <param name="inputIndex">The index of the input stream.</param>
        public override void HandleFrameData(byte[] data, int inputIndex)
        {
            FrameCollectionMessage message = FrameCollectionMessage.Parser.ParseFrom(data);
            StringData testData = StringData.Parser.ParseFrom(message.Data);
            string path = testData.StringData_;
            if (!AssetDatabase.IsValidFolder(assetDestinationFolder))
            {
                assetDestinationFolder = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder("Assets", "Imported")) + "/";
            }

            
            assetPath = assetDestinationFolder + Path.GetFileName(path);
            try
            {
                if (File.Exists(assetPath))
                {
                    FileUtil.ReplaceFile(path, assetPath);
                }
                else
                {
                    FileUtil.CopyFileOrDirectory(path, assetPath);
                }

                Debug.Log("Asset is imported at this location: " + assetPath);

                AssetDatabase.ImportAsset(assetPath);

                StringData sendData = new StringData
                {
                    StringData_ = assetPath
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
                //communicator.SendData(serializedMessage.ToStringUtf8(), 0);
            }
            catch (IOException ex)
            {
                Debug.Log(ex.Message);
                Debug.Log("The asset couldn't import.");
            }
        }
        /// <summary>
        /// Starts the node.
        /// </summary>
        public override void StartNode()
        {

        }
    }
}