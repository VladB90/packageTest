using UnityEditor.Timeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using Unity.VisualScripting;

namespace MBody
{
    /// <summary>
    /// Editor window for creating and managing timelines.
    /// </summary>
    public class TimelineCreationEditor : EditorWindow
    {
        public GameObject timelineObject;
        public GameObject character;
        public PlayableDirector playableDirector;
        public TimelineAsset timelineAsset;
        public AnimationClip animClip;
        public AudioClip audioClip;
        public string fileName;

        /// <summary>
        /// Displays the Timeline Creation window in Unity's editor menu under "MBody"
        /// </summary>
        [MenuItem("MBody/Timeline Creation")]
        public static void ShowWindow()
        {
            TimelineCreationEditor window = GetWindow<TimelineCreationEditor>();
            window.titleContent = new GUIContent("Timeline Creation");
        }

        /// <summary>
        ///  GUI method for displaying the Timeline Creation window and handling user interactions.
        /// </summary>
        void OnGUI()
        {
            if (GUILayout.Button("Create a timeline"))
            {
                CreateTimeline();
            }

            character = EditorGUILayout.ObjectField(character, typeof(GameObject), true) as GameObject;
            animClip = EditorGUILayout.ObjectField(animClip, typeof(AnimationClip), true) as AnimationClip;
            if (GUILayout.Button("Add animation clip to timeline"))
            {
                AddAnimationClip();
            }

            audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true) as AudioClip;
            if (GUILayout.Button("Add audio clip to timeline"))
            {
                AddAudioClip();
            }

            fileName = EditorGUILayout.TextField("Timeline File Name: ", fileName);
            if (GUILayout.Button("Save Timeline"))
            {
                fileName = "Assets/" + fileName + ".playable";
                AssetDatabase.CreateAsset(timelineAsset, fileName);
                timelineAsset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(fileName);
            }
        }

        /// <summary>
        /// Refreshes the Timeline Editor window after modifying the timeline structure.
        /// </summary>
        private void RefreshTimelineWindow()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }

        /// <summary>
        ///  Creates a new timeline GameObject, adds a PlayableDirector and a new TimelineAsset.
        /// </summary>
        public void CreateTimeline()
        {
            //playableDirector = timelineObject.GetComponent<PlayableDirector>();
            timelineObject = new GameObject("Timeline");

            //if there is a playable director remove it
            if (timelineObject.GetComponent<PlayableDirector>())
            {
                DestroyImmediate(playableDirector);
            }

            playableDirector = timelineObject.AddComponent<PlayableDirector>();

            //Create timeline asset
            timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();

            playableDirector.playableAsset = timelineAsset;
            playableDirector.RebuildGraph();

            //Open the Timeline window and display the timeline associated with the character
            EditorApplication.ExecuteMenuItem("Window/Sequencing/Timeline");
            EditorWindow.GetWindow(typeof(TimelineEditorWindow));
            Selection.activeGameObject = timelineObject;

        }

        /// <summary>
        /// Adds the currently selected AnimationClip to the timeline.
        /// </summary>
        public void AddAnimationClip()
        {
            Animator charAnimator = character.GetComponent<Animator>();
            AnimationTrack animationTrack = timelineAsset.CreateTrack<AnimationTrack>(null, "Animation Track");
            animationTrack.CreateClip(animClip);

            playableDirector.SetGenericBinding(animationTrack, charAnimator);

            RefreshTimelineWindow();
        }

        /// <summary>
        ///  Adds the currently selected AudioClip to the timeline.
        /// </summary>
        public void AddAudioClip()
        {
            AudioTrack audioTrack = timelineAsset.CreateTrack<AudioTrack>(null, "Audio Track");
            audioTrack.CreateClip(audioClip);

            AudioSource charAudioSource = character.GetComponent<AudioSource>();
            playableDirector.SetGenericBinding(audioTrack, charAudioSource);

            RefreshTimelineWindow();
        }
    }
}