using MBody;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using static Codice.Client.BaseCommands.Import.Commit;


namespace MBody

    
{
    /// <summary>
    /// enum that has different models
    /// </summary>
    public enum MODEL
    {
        DSG_ZEROEGGS_WIN = 0,
        DSG_ZEROEGGS_WSL = 1,
        DSG_TWH_WSL = 2,
    }

    /// <summary>
    /// Editor window for converting audio to gestures.
    /// </summary>

    public class AudioToGestureEditor : EditorWindow
    {
        public string audioFileLocation;
        public string blenderLocation = "C:/Users/SIRTCoop/Documents/Blender/blender.exe";
        public string pythonScriptLocation = "C:/Users/SIRTCoop/Documents/Blender_BVH_to_FBX.py";
        public string workingDirectory = "C:/Users/SIRTCoop/Downloads/DiffuseStyleGesture-master/DiffuseStyleGesture-master/main/mydiffusion_zeggs";
        public string directoryToScan = "C:/Users/SIRTCoop/Downloads/DiffuseStyleGesture-master/DiffuseStyleGesture-master/main/mydiffusion_zeggs/sample_dir";
        public string recordedClipName = "015_Happy_"; //The recorded audio clip should be named xxx_[style]_xx.wav

        public string clientIP = "172.30.224.87";//
        public int clientPort = 18889;
        public string serverIP = "172.30.224.1";
        public int serverPort = 18801;

       // public BVHAnimationLoader loader;
        public AudioSource audioSource;
        private FileNameNode fileNameNode;
        private AssetImporterNode assetImporterNode;
        private AvatarGeneratorNode avatarGeneratorNode;
        private AnimationClip animClip;
        private AudioClip audioClip;
        private TimelineCreationEditor timelineWindow;
        private string audioName;
        private AudioClip recordedClip;
        private static int count = 0;


        private FileNameNode fileNameNodeTxt;
        string transcriptPath = "";
        MODEL model = MODEL.DSG_ZEROEGGS_WSL;

        string fileName = "";
        private bool isPipelineStarted = false;
        GameObject character;
        Avatar avatar;


        /// <summary>
        /// Show the Audio To Gesture window in the Unity Editor. 
        /// </summary>
        [MenuItem("MBody/Audio To Gesture")]
        public static void ShowWindow()
        {
            AudioToGestureEditor window = GetWindow<AudioToGestureEditor>();
            window.titleContent = new GUIContent("Audio To Gesture");
        }

        /// <summary>
        /// Unity Editor GUI Method.
        /// </summary>
        [System.Obsolete]
        void OnGUI()
        {
            //To clean up any selection
            TimelineEditor.selectedClips = null;

            GUILayout.Label("Pipeline Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.Label("Enter the Blender Location(.exe):", EditorStyles.boldLabel);
            blenderLocation = GUILayout.TextField(blenderLocation);

            GUILayout.Label("Enter the Python Script Location:", EditorStyles.boldLabel);
            pythonScriptLocation = GUILayout.TextField(pythonScriptLocation);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Create a timeline"))
            {
                timelineWindow = ScriptableObject.CreateInstance<TimelineCreationEditor>();
                timelineWindow.CreateTimeline();
            }

            if (GUILayout.Button("Start audio recording"))
            {
                StartRecording();
            }
            if (GUILayout.Button("Stop audio recording"))
            {
                StopRecording();
                SaveRecord(recordedClipName, recordedClip);
                AddRecordToTimeline();
            }
            GUILayout.Label("Choose an audio clip: ", EditorStyles.boldLabel);
            audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true) as AudioClip;

            if (GUILayout.Button("Add the audio clip to the timeline"))
            {
                AddAudioClip();
            }

            GUILayout.Label("Choose a character:", EditorStyles.boldLabel);
            character = EditorGUILayout.ObjectField(character, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("Play the animation"))
            {
                double audioStartTime = 0;
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

                foreach (TrackAsset asset in TimelineEditor.timelineAsset.GetRootTracks())
                {
                    if (asset is AudioTrack)
                    {
                        foreach (TimelineClip timelineClip in asset.GetClips())
                        {
                            if (timelineClip.displayName == audioName)
                            {
                                audioStartTime = timelineClip.start;
                            }
                        }
                    }
                }

                animTrack.CreateClip(animClip);

                foreach (TimelineClip timelineclip in animTrack.GetClips())
                {
                    if (timelineclip.displayName == animClip.name)
                    {
                        timelineclip.start = audioStartTime;
                    }
                }
                animClip = null;
                TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
                TimelineEditor.playableDirector.Play();
            }

            GUILayout.Label("Choose an avatar:", EditorStyles.boldLabel);
            avatar = EditorGUILayout.ObjectField(avatar, typeof(Avatar), true) as Avatar;
            GUILayout.Space(5);
            model = (MODEL)EditorGUILayout.EnumPopup("Model", model, EditorStyles.popup);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Model Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);



            DisplayModelConfig();


            if (audioClip == null)
            {
                EditorGUILayout.HelpBox("Audio clip isn't selected", MessageType.Warning);
            }

            if (TimelineEditor.playableDirector == null)
            {
                EditorGUILayout.HelpBox("Timeline isn't created", MessageType.Warning);
            }

            if (avatar == null)
            {
                EditorGUILayout.HelpBox("Avatar isn't selected", MessageType.Warning);
            }





        }

        /// <summary>
        /// Method to display configuration based on the model selected. 
        /// </summary>
        void DisplayModelConfig()
        {
            switch (model)
            {
                case MODEL.DSG_ZEROEGGS_WIN:
                    //Debug.Log("Model: ZeroEggs Windows version");
                    break;

                case MODEL.DSG_ZEROEGGS_WSL:
                    //Debug.Log("Model: ZeroEggs Linux version");
                    break;

                case MODEL.DSG_TWH_WSL:
                    //Debug.Log("Model: TWH Linux version");
                    DSG_TWH_GUI();
                    break;

                default:
                    //Debug.Log("Should never be displayed");
                    break;
            }
        }


        /// <summary>
        /// GUI for DSG_TWH model settings
        /// </summary>
        void DSG_TWH_GUI()
        {
            GUILayout.Label("Enter the Transcript Location(.txt):", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            transcriptPath = GUILayout.TextField(transcriptPath);
            if (GUILayout.Button("Choose txt", GUILayout.Width(80)))
            {
                transcriptPath = EditorUtility.OpenFilePanel("Choose file ", "", "txt");
            }
            GUILayout.EndHorizontal();

            clientIP = EditorGUILayout.TextField("ClientIP", clientIP);
            clientPort = EditorGUILayout.IntField("ClientPort", clientPort);
            serverIP = EditorGUILayout.TextField("ServerIP", serverIP);
            serverPort = EditorGUILayout.IntField("ServerPort", serverPort);

            if (transcriptPath == "")
            {
                EditorGUILayout.HelpBox("Transcript isn't selected", MessageType.Warning);
            }
        }

        /// <summary>
        /// Sets up the pipeline configuration for the WSL_TWH model.
        /// </summary>

        void WSL_TWH()
        {

            Dictionary<string, string> communicatorConfig = new Dictionary<string, string>()
        {
            { "shouldListen", "true" },
            { "shouldSend", "false" },
            { "communicatorName", "comm1" }
        };
            NodeManager.Instance.ConfigurePipelineNode("commNode", communicatorConfig);//

            Dictionary<string, string> communicatorConfig1 = new Dictionary<string, string>()
        {
            { "shouldListen", "false" },
            { "shouldSend", "true" },
            { "communicatorName", "comm" }
        };
            NodeManager.Instance.ConfigurePipelineNode("commNode1", communicatorConfig1);//

            Dictionary<string, string> communicatorConfig2 = new Dictionary<string, string>()
        {
            { "shouldListen", "false" },
            { "shouldSend", "true" },
            { "communicatorName", "comm2" }
        };
            NodeManager.Instance.ConfigurePipelineNode("commNode2", communicatorConfig2);

            Dictionary<string, string> fileSaveConfig = new Dictionary<string, string>()
        {
            { "saveLocation", "C:/Users/SIRTCoop/Documents/" },
            { "saveFileName", "WSLresult.bvh" }
        };
            NodeManager.Instance.ConfigurePipelineNode("fileSave", fileSaveConfig);

            Dictionary<string, string> blenderNodeConfig = new Dictionary<string, string>()
        {
            { "blenderLocation", blenderLocation },
            { "pythonScriptLocation", pythonScriptLocation}
        };
            NodeManager.Instance.ConfigurePipelineNode("blenderBvhToFbxNode", blenderNodeConfig);

            NodeManager.Instance.BindNodeStreams("fileNameNode", 0, "fileRead", 0);
            NodeManager.Instance.BindNodeStreams("fileNameNodeTxt", 0, "fileReadTxt", 0);

            NodeManager.Instance.BindNodeStreams("fileRead", 0, "commNode2", 0);
            NodeManager.Instance.BindNodeStreams("fileReadTxt", 0, "commNode1", 0);

            NodeManager.Instance.BindNodeStreams("commNode", 0, "fileSave", 0);
            NodeManager.Instance.BindNodeStreams("fileSave", 0, "blenderBvhToFbxNode", 0);
            NodeManager.Instance.BindNodeStreams("blenderBvhToFbxNode", 0, "assetImporterNode", 0);
            NodeManager.Instance.BindNodeStreams("assetImporterNode", 0, "avatarGeneratorNode", 0);
        }

        /// <summary>
        /// Sets up the pipeline configuration for the DSG_WIN model.
        /// </summary>
        void DSG_WIN()
        {


            NodeManager.Instance.CreateNodeInPipeline("cmdStringNode", "cmdStringNode");
            NodeManager.Instance.CreateNodeInPipeline("dsgZeroEggsExecute", "dsgZeroEggsExecute");
            NodeManager.Instance.CreateNodeInPipeline("directoryMonitor", "directoryMonitor");



            Dictionary<string, string> dsgNodeConfig = new Dictionary<string, string>()
        {
            { "workingDirectory", workingDirectory }
        };
            NodeManager.Instance.ConfigurePipelineNode("dsgZeroEggsExecute", dsgNodeConfig);

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

            NodeManager.Instance.BindNodeStreams("fileNameNode", 0, "dsgZeroEggsExecute", 0);
            NodeManager.Instance.BindNodeStreams("dsgZeroEggsExecute", 0, "cmdStringNode", 0);
            NodeManager.Instance.BindNodeStreams("cmdStringNode", 0, "directoryMonitor", 0);
            NodeManager.Instance.BindNodeStreams("directoryMonitor", 0, "blenderBvhToFbxNode", 0);
            NodeManager.Instance.BindNodeStreams("blenderBvhToFbxNode", 0, "assetImporterNode", 0);
            NodeManager.Instance.BindNodeStreams("assetImporterNode", 0, "avatarGeneratorNode", 0);

        }

        /// <summary>
        /// start method
        /// </summary>

        void Start()
        {
            NodeManager.Instance.InitializeManager();

            // Load the audio file location
            fileNameNode = new FileNameNode("fileNameNode");
            fileNameNode.filePath = audioFileLocation;

            fileNameNodeTxt = new FileNameNode("fileNameNodeTxt");
            fileNameNodeTxt.filePath = transcriptPath;

            // Load the asset
            assetImporterNode = new AssetImporterNode("assetImporterNode");
            //Avatar setting 
            avatarGeneratorNode = new AvatarGeneratorNode("avatarGeneratorNode");
            avatarGeneratorNode.avatar = avatar;
            // Set up Communication


            // Register as nodes
            NodeManager.Instance.RegisterUnityNode(fileNameNode);
            NodeManager.Instance.RegisterUnityNode(fileNameNodeTxt);
            NodeManager.Instance.RegisterUnityNode(assetImporterNode);
            NodeManager.Instance.RegisterUnityNode(avatarGeneratorNode);

            NodeManager.Instance.CreateOutputForNode("fileNameNode", "frameCollection");
            NodeManager.Instance.CreateOutputForNode("fileNameNodeTxt", "frameCollection");

            NodeManager.Instance.CreateOutputForNode("assetImporterNode", "frameCollection");

            NodeManager.Instance.CreateNodeInPipeline("blenderBvhToFbxNode", "blenderBvhToFbxNode");

            NodeManager.Instance.CreateNodeInPipeline("fileRead", "fileRead");
            NodeManager.Instance.CreateNodeInPipeline("fileReadTxt", "fileRead");

            NodeManager.Instance.CreateNodeInPipeline("fileSave", "fileSave");

            NodeManager.Instance.CreateUDPCommunicator("comm1", clientIP, serverPort, serverIP, serverPort);
            NodeManager.Instance.CreateCommunicatorNode("commNode");

            NodeManager.Instance.CreateUDPCommunicator("comm", clientIP, 18802, serverIP, 18802);
            NodeManager.Instance.CreateCommunicatorNode("commNode1");

            NodeManager.Instance.CreateUDPCommunicator("comm2", clientIP, clientPort, serverIP, clientPort);
            NodeManager.Instance.CreateCommunicatorNode("commNode2");

            if (model == MODEL.DSG_TWH_WSL)
            {
                WSL_TWH();
            }
            else if (model == MODEL.DSG_ZEROEGGS_WIN)
            {
                DSG_WIN();
            }

            NodeManager.Instance.StartPipeline();

            Debug.Log("[WrapperTestComponent] Done startup!");
        }

        /// <summary>
        /// Sends the new file name for the transcript file
        /// </summary>
        /// <param name="fileName">Name of the new transcript file</param>

        public void SendNewFileNameTxt(string fileName)
        {
            fileNameNodeTxt.filePath = fileName;
            NodeManager.Instance.PushData("fileNameNodeTxt", 0, fileNameNodeTxt.SendFileName());
        }

        /// <summary>
        ///  Sends the new file name for the audio file.
        /// </summary>
        /// <param name="fileName">Name of the new audio file.</param>

        public void SendNewFileName(string fileName)
        {
            fileNameNode.filePath = fileName;
            NodeManager.Instance.PushData("fileNameNode", 0, fileNameNode.SendFileName());
        }

        /// <summary>
        /// starts recording audio
        /// </summary>
        public void StartRecording()
        {
            //TODO: 40 is fixed time for the record. Find a way to set it properly or trim the audio clip.
            recordedClip = Microphone.Start(Microphone.devices[0], false, 40, 44100);
        }

        /// <summary>
        /// stops recording audio
        /// </summary>
        public void StopRecording()
        {
            Microphone.End(Microphone.devices[0]);
        }

        /// <summary>
        /// saves the recorded audio clip
        /// </summary>
        /// <param name="recordedClipName"></param>
        /// <param name="recordedClip"></param>
        public void SaveRecord(string recordedClipName, AudioClip recordedClip)
        {
            //TODO: Find a way to set proper name for setting clip name. To more information to set it, check the DSG github page. 
            recordedClipName += count;
            count++;
            recordedClip.name = recordedClipName;
            string name = SavWav.Save(recordedClipName, recordedClip);
            Debug.Log("Clip saved at this location: " + name);

            //Refresh the assets database
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Adds the recorded audio clip to the timeline
        /// </summary>
        public void AddRecordToTimeline()
        {
            audioClip = recordedClip;
            AddAudioClip();
            audioClip = null;

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }
        /// <summary>
        /// Adds the selected audio clip to the timeline
        /// </summary>
        public void AddAudioClip()
        {
            AudioTrack audioTrack = TimelineEditor.timelineAsset.CreateTrack<AudioTrack>(null, "Audio Track");
            audioTrack.CreateClip(audioClip);

            TimelineEditor.playableDirector.SetGenericBinding(audioTrack, null);
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }

        /// <summary>
        /// update method
        /// </summary>
        public void Update()
        {
            //UdpCommunicatorFactory.UpdateCommunicators();
            if (TimelineEditor.selectedClip != null && TimelineEditor.selectedClip.animationClip == null)
            {
                if (!isPipelineStarted)
                {
                    Debug.Log("here");
                    Start();
                    isPipelineStarted = true;
                }
                string projectPath = Application.dataPath.Replace("/Assets", "");
                string assetGUID = AssetDatabase.FindAssets(TimelineEditor.selectedClip.displayName)[0];

                audioName = TimelineEditor.selectedClip.displayName;
                fileName = projectPath + "/" + AssetDatabase.GUIDToAssetPath(assetGUID);
                audioFileLocation = fileName;
                TimelineEditor.selectedClips = null;

                SendNewFileName(fileName);
                SendNewFileNameTxt(transcriptPath);
            }

            NodeManager.Instance.CheckForData();
        }

        /// <summary>
        /// stops the pipeline and destroy communicators.
        /// </summary>
        public void OnDestroy()
        {
            NodeManager.Instance.StopPipeline();
            NodeManager.Instance.DeinitializeManager();
            //UdpCommunicatorFactory.DestroyCommunicators();
            Debug.Log("Communicators destroyed.");
            isPipelineStarted = false;
        }

    }
}