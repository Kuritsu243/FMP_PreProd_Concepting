using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment
{
    public class Portal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.transform.parent.gameObject.CompareTag("Player")) return;
            SceneManager.LoadScene("mainScene");
        }
        private void OnTriggerStay(Collider other)
        {
            if (!other.transform.parent.gameObject.CompareTag("Player")) return;
            SceneManager.LoadScene("mainScene");
        }
    }
}
