using Player;
using TMPro;
using UnityEngine;

namespace Debugging
{
    public class ShowMoveState : MonoBehaviour
    {

        private TextMeshProUGUI _moveStateText;
        private PlayerMovement _playerMovement;

        private void Start()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _moveStateText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            _moveStateText.text = _playerMovement.GetPlayerMovementState().ToString();
        }
    }
}
