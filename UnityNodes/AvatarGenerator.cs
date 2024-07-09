using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBody
{

    /// <summary>
    /// Class is responsible for generating Avatar
    /// </summary>

    public class AvatarGenerator : MonoBehaviour
    {
        public GameObject avatarPrefab; // Reference to the avatar prefab with the FBX model.
        private GameObject currentAvatar; // Reference to the currently instantiated avatar.

        // Customize avatar appearance 
        public Material bodyMaterial;
        public Material headMaterial;

        /// <summary>
        /// start method 
        /// </summary>
        void Start()
        {
            GenerateAvatar();
        }
        /// <summary>
        /// Destroy the currentAvator and Instantiates a new avatarPrefab 
        /// </summary>
        public void GenerateAvatar()
        {
            if (currentAvatar != null)
            {
                Destroy(currentAvatar);
            }

            currentAvatar = Instantiate(avatarPrefab, transform.position, transform.rotation);

            // Customize the avatar's appearance.
            CustomizeAvatar(currentAvatar);
        }


        /// <summary>
        /// Customize the avatar's appearance by changing materials or textures.
        /// </summary>
        /// <param name="avatar"></param>
        private void CustomizeAvatar(GameObject avatar)
        {
            Renderer[] renderers = avatar.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                if (renderer.name.Contains("Body"))
                {
                    renderer.material = bodyMaterial;
                }
                else if (renderer.name.Contains("Head"))
                {
                    renderer.material = headMaterial;
                }
            }
        }
    }

}