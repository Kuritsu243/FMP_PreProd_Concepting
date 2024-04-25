using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class FloatingWallController : MonoBehaviour
    {

        [SerializeField] private List<GameObject> childObjs;

        public void TriggerWallMovement()
        {
            StartCoroutine(MoveObjectsY());
        }

        private IEnumerator MoveObjectsY()
        {
            foreach (var childObj in childObjs)
            {
                yield return new WaitForSeconds(0.35f);
                LeanTween.moveLocalY(childObj, 0, 3);
            }
        }
    }
}
