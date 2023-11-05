using System;
using Camera;
using input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public static class TransformExtensions
    {
        public static GameObject FindGameObjectInChildWithTag(this Transform parent, string tag)
        {
            GameObject foundChild = null;
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.CompareTag(tag))
                    foundChild = child.gameObject;
            }

            if (foundChild != null)
            {
                return foundChild;
            }
            else
            {
                throw new Exception("No child object with tag!");
            }

        }
    }
    
    
    public class PlayerController : MonoBehaviour
    {
        public GameObject eventSystem;
        public inputSystem inputSystem;
        public mainCamera mainCamera;
        public CharacterController characterController;
        public PlayerMovement playerMovement;
        public PlayerStamina playerStamina;
        public GameObject playerMesh;
        public PlayerWallRunning playerWallRunning;
        
        public void Awake()
        {
            characterController = GetComponentInChildren<CharacterController>();
            playerMovement = GetComponent<PlayerMovement>();
            eventSystem = GameObject.FindGameObjectWithTag("EventSystem");
            mainCamera = eventSystem.GetComponent<mainCamera>();
            inputSystem = eventSystem.GetComponent<inputSystem>();
            playerStamina = GetComponent<PlayerStamina>();
            playerMesh = transform.FindGameObjectInChildWithTag("PlayerMesh");
            playerWallRunning = GetComponent<PlayerWallRunning>();
        }

        private void FixedUpdate()
        {
            // transform.position = playerMesh.transform.position;
        }
    }
}
