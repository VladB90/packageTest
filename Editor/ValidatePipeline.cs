using MBody;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ValidatePipeline : EditorWindow
{
    [MenuItem("MBody/Util/Validate Pipeline")]
    public static void OpenWindow()
    {
        ValidatePipeline wnd = GetWindow<ValidatePipeline>();
        wnd.titleContent = new GUIContent("Pipeline Validation");
    }

    FileNameNode fileNameNode;
    AssetImporterNode assetImporterNode;
    bool started = false;

    public void InitPipeline()
    {

        // Start is called before the first frame update

        NodeManager.Instance.InitializeManager();

        // Load the audio file location
        fileNameNode = new FileNameNode("fileNameNode");
        fileNameNode.filePath = Path.GetFullPath("Packages/com.unity.mbodyplugin/Test/test.txt"); ;


        // Load the asset
        assetImporterNode = new AssetImporterNode("assetImporterNode");
        //Avatar setting 



        // Register as nodes
        NodeManager.Instance.RegisterUnityNode(fileNameNode);

        NodeManager.Instance.RegisterUnityNode(assetImporterNode);


        NodeManager.Instance.CreateOutputForNode("fileNameNode", "frameCollection");
        NodeManager.Instance.CreateOutputForNode("assetImporterNode", "frameCollection");



        NodeManager.Instance.CreateNodeInPipeline("fileRead", "fileRead");
        NodeManager.Instance.CreateNodeInPipeline("fileSave", "fileSave");


        Dictionary<string, string> fileSaveConfig = new Dictionary<string, string>()
        {
            { "saveLocation", "C:/Users/SIRTCoop/Documents/Receive/" },
            { "saveFileName", "msg.txt" }
        };
        NodeManager.Instance.ConfigurePipelineNode("fileSave", fileSaveConfig);


        NodeManager.Instance.BindNodeStreams("fileNameNode", 0, "fileRead", 0);
        NodeManager.Instance.BindNodeStreams("fileRead", 0, "fileSave", 0);
        NodeManager.Instance.BindNodeStreams("fileSave", 0, "assetImporterNode", 0);

        NodeManager.Instance.StartPipeline();

        Debug.Log("[WrapperTestComponent] Done startup!");
    }

    private void CreateGUI()
    {

        VisualElement root = rootVisualElement;
        InitPipeline();
        // Create button
        Button button = new Button();
        button.name = "Validate";
        button.text = "Validate";
        root.Add(button);
        button.clicked += OnClicked;
    }

    private void OnClicked()
    {
        started = true;
    }

    private void Update()
    {
        if (started)
        {
            InitPipeline();
            NodeManager.Instance.PushData("fileNameNode", 0, fileNameNode.SendFileName());
            started = false;
        }
        NodeManager.Instance.CheckForData();
    }
}