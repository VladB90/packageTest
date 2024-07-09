using MBody;
using UnityEditor;
using UnityEngine;

namespace MBody
{

    /// <summary>
    /// Specialized node for handling avatar generation and MoMask setup.
    /// </summary>

    public class AvatarGeneratorMomaskNode : BaseNode
    {

        /// <summary>
        /// Constructor for AvatarGeneratorMomaskNode.
        /// </summary>
        /// <param name="nodeId">Identifier for the node.</param>
        public AvatarGeneratorMomaskNode(string nodeId) : base(nodeId)
        {
        }

        /// <summary>
        /// Handles incoming frame data containing path to the model file.
        /// </summary>
        /// <param name="data">Byte array containing serialized data.</param>
        /// <param name="inputIndex">Index of the input.</param>
        public override void HandleFrameData(byte[] data, int inputIndex)
        {
            // Deserialize the incoming data
            FrameCollectionMessage message = FrameCollectionMessage.Parser.ParseFrom(data);
            StringData testData = StringData.Parser.ParseFrom(message.Data);
            string path = testData.StringData_;

            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null)
            {
                importer.animationType = ModelImporterAnimationType.Human;
                //Create from this model - rig setup
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

                //Copy from other avatar - Rig, use this 3 lines.
                //importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                //importer.sourceAvatar = avatar;

                //TODO: Animation-Motion-Root motion node should be set. 
                //importer.motionNodeName = "Hips";
                importer.SaveAndReimport();
            }
        }

        /// <summary>
        /// Placeholder method for starting the node.
        /// </summary>
        public override void StartNode()
        {

        }
    }
}