using MBody;
using Google.Protobuf;
using UnityEditor;
using UnityEngine;

namespace MBody
{
    /// <summary>
    /// Represents a node responsible for importing animations with specified avatar.
    /// </summary>
    public class AvatarGeneratorNode : BaseNode
    {
        //public Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>("Assets/Models/ZeroEggsModel.fbx"); // Assets/Animations/X Bot@Walking (1).fbx
        //public Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>("Assets/Models/GenevaModel_v2_Tpose_Final.fbx");

        public Avatar avatar;
        /// <summary>
        /// Initializes a new instance of the AvatarGeneratorNode class with the specified node ID.
        /// </summary>
        /// <param name="nodeId">The unique identifier for the node.</param>
        public AvatarGeneratorNode(string nodeId) : base(nodeId)
        {
        }
        /// <summary>
        /// Handles incoming frame data, processes it to set up the avatar rig, reimports the animation.
        /// </summary>
        /// <param name="data">The incoming data as a byte array.</param>
        /// <param name="inputIndex">The index of the input stream.</param>
        public override void HandleFrameData(byte[] data, int inputIndex)
        {
            FrameCollectionMessage message = FrameCollectionMessage.Parser.ParseFrom(data);
            StringData testData = StringData.Parser.ParseFrom(message.Data);
            string path = testData.StringData_;
            Debug.Log("Avatar received this path: " + path);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;



            StringData sendData = new StringData
            {
                StringData_ = path
            };

            ByteString serializedData = sendData.ToByteString();

            FrameCollectionMessage sendMessage = new FrameCollectionMessage()
            {
                Data = serializedData,
                DataTypeName = "StringData",
                StartFrame = 7,
                EndFrame = 15
            };

            //byte[] serializedMessage = sendMessage.ToByteArray();
            //InvokeDataProcessed(0, serializedMessage);


            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                //Create from this model - rig setup
                //importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

                //Copy from other avatar - Rig, use this 3 lines.
                importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                importer.sourceAvatar = avatar;

                //TODO: Animation-Motion-Root motion node should be set. 
                //importer.motionNodeName = "Hips";
                importer.SaveAndReimport();
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