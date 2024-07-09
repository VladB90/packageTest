using MBody;
using Google.Protobuf;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;


namespace MBody
{

    /// <summary>
    /// Editor window for converting text prompt to gestures.
    /// </summary>
    public class PromptToGestureEditor : EditorWindow
    {
        public string workingDirectory = "C:/Users/SIRTCoop/Downloads/momask-codes-main/momask-codes-main";
        public string directoryToScan = "C:/Users/SIRTCoop/Downloads/momask-codes-main/momask-codes-main/generation/exp1/animations/0";
        public string blenderLocation = "C:/Users/SIRTCoop/Documents/Blender/blender.exe";
        public string pythonScriptLocation = "C:/Users/SIRTCoop/Documents/Blender_BVH_to_FBX_Momask.py";

        public string textPrompt = "";

        GameObject character;
        private AssetImporterNode assetImporterNode;
        private AvatarGeneratorMomaskNode avatarGeneratorNode;
        private AnimationClip animClip;
        private TimelineCreationEditor timelineWindow;
        private bool isPipelineStarted = false;


        /// <summary>
        /// Show the Audio To Gesture window in the Unity Editor. 
        /// </summary>
        [MenuItem("MBody/Prompt To Gesture")]
        public static void ShowWindow()
        {
            PromptToGestureEditor window = GetWindow<PromptToGestureEditor>();
            window.titleContent = new GUIContent("Prompt To Gesture");
        }

        /// <summary>
        /// Unity Editor GUI Method.
        /// </summary>
        [System.Obsolete]
        void OnGUI()
        {
            GUILayout.Label("Pipeline Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.Label("Enter the Working Directory:", EditorStyles.boldLabel);
            workingDirectory = GUILayout.TextField(workingDirectory);

            GUILayout.Label("Enter the Directory Scan Location:", EditorStyles.boldLabel);
            directoryToScan = GUILayout.TextField(directoryToScan);

            GUILayout.Label("Enter the Blender Location(.exe):", EditorStyles.boldLabel);
            blenderLocation = GUILayout.TextField(blenderLocation);

            GUILayout.Label("Enter the Python Script Location:", EditorStyles.boldLabel);
            pythonScriptLocation = GUILayout.TextField(pythonScriptLocation);

            if (GUILayout.Button("Start"))
            {
                StartPipeline();
                isPipelineStarted = true;
            }

            GUILayout.Label("Enter a text prompt:", EditorStyles.boldLabel);
            textPrompt = GUILayout.TextField(textPrompt);

            if (GUILayout.Button("Generate"))
            {
                Generate(textPrompt);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Create a timeline"))
            {
                timelineWindow = ScriptableObject.CreateInstance<TimelineCreationEditor>();
                timelineWindow.CreateTimeline();
            }

            GUILayout.Label("Choose a character:", EditorStyles.boldLabel);
            character = EditorGUILayout.ObjectField(character, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Play the animation"))
            {
                animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetImporterNode.assetPath);

                Animator charAnimator = character.GetComponent<Animator>();
                TimelineAsset timelineAsset = TimelineEditor.timelineAsset;
                AnimationTrack animTrack = null;

                foreach (TrackAsset asset in TimelineEditor.timelineAsset.GetRootTracks())
                {
                    if (asset is AnimationTrack && TimelineEditor.playableDirector.GetGenericBinding(asset) == character.GetComponent<Animator>())
                    {
                        animTrack = (AnimationTrack)asset;
                        break;
                    }
                }

                if (animTrack == null)
                {
                    animTrack = timelineAsset.CreateTrack<AnimationTrack>(null, "Animation Track");
                    TimelineEditor.playableDirector.SetGenericBinding(animTrack, charAnimator);
                }

                animTrack.CreateClip(animClip);

                TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
                TimelineEditor.playableDirector.Play();
            }
        }

        /// <summary>
        /// Method with the pipeline configuration.
        /// </summary>
       
        void StartPipeline()
        {
            NodeManager.Instance.InitializeManager();
            // Load the asset
            assetImporterNode = new AssetImporterNode("assetImporterNode");
            //Avatar setting 
            avatarGeneratorNode = new AvatarGeneratorMomaskNode("avatarGeneratorNode");

            

            NodeManager.Instance.RegisterUnityNode(assetImporterNode);
            NodeManager.Instance.RegisterUnityNode(avatarGeneratorNode);

            NodeManager.Instance.CreateNodeInPipeline("toolNode", "toolNode");
            NodeManager.Instance.CreateNodeInPipeline("cmdStringNode", "cmdStringNode");
            NodeManager.Instance.CreateNodeInPipeline("momaskExecute", "momaskExecute");
            NodeManager.Instance.CreateNodeInPipeline("directoryMonitor", "directoryMonitor");
            NodeManager.Instance.CreateNodeInPipeline("blenderBvhToFbxNode", "blenderBvhToFbxNode");

            NodeManager.Instance.CreateOutputForNode("toolNode", "frameCollection");
            NodeManager.Instance.CreateOutputForNode("assetImporterNode", "frameCollection");

            Dictionary<string, string> config = new Dictionary<string, string>()
        {
            { "workingDirectory", workingDirectory },
        };
            NodeManager.Instance.ConfigurePipelineNode("momaskExecute", config);

            Dictionary<string, string> dirMonitorNodeConfig = new Dictionary<string, string>()
        {
            { "directory", directoryToScan }
        };
            NodeManager.Instance.ConfigurePipelineNode("directoryMonitor", dirMonitorNodeConfig);

            

            Dictionary<string, string> blenderNodeConfig = new Dictionary<string, string>()
        {
            { "blenderLocation", blenderLocation },
            { "pythonScriptLocation", pythonScriptLocation}
        };
            NodeManager.Instance.ConfigurePipelineNode("blenderBvhToFbxNode", blenderNodeConfig);

            

            NodeManager.Instance.BindNodeStreams("toolNode", 0, "momaskExecute", 0);
            NodeManager.Instance.BindNodeStreams("momaskExecute", 0, "cmdStringNode", 0);
            NodeManager.Instance.BindNodeStreams("cmdStringNode", 0, "directoryMonitor", 0);
            NodeManager.Instance.BindNodeStreams("directoryMonitor", 0, "blenderBvhToFbxNode", 0);
            NodeManager.Instance.BindNodeStreams("blenderBvhToFbxNode", 0, "assetImporterNode", 0);
            NodeManager.Instance.BindNodeStreams("assetImporterNode", 0, "avatarGeneratorNode", 0);

            NodeManager.Instance.StartPipeline();

            Debug.Log("[WrapperTestComponent] Done startup!");
        }

        /// <summary>
        /// Method to generate a FrameCollectionMessage with a string prompt.
        /// </summary>
        /// <param name="textPrompt">The text prompt to be encapsulated in the message.</param>
        void Generate(string textPrompt)
        {
            StringData data = new StringData
            {
                StringData_ = textPrompt
            };

            byte[] serializedData = data.ToByteArray();

            FrameCollectionMessage message = new FrameCollectionMessage()
            {
                Data = ByteString.CopyFrom(serializedData),
                DataTypeName = "StringData",
                StartFrame = 0,
                EndFrame = 15
            };

            Debug.Log(message.Data.ToStringUtf8());

            byte[] serializedMessage = message.ToByteArray();

            NodeManager.Instance.PushData("toolNode", 0, serializedMessage);
        }

        /// <summary>
        /// Update method is called every frame
        /// </summary>
        public void Update()
        {
            NodeManager.Instance.CheckForData();
            
        }

        /// <summary>
        ///  Method called when the script instance is being destroyed.
        /// </summary>
        public void OnDestroy()
        {
            NodeManager.Instance.DeinitializeManager();
            
            Debug.Log("Communicators destroyed.");
            isPipelineStarted = false;
        }
    }
}