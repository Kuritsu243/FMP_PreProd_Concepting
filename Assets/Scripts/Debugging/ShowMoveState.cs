using System;
using Player;
using TMPro;
using UnityEngine;

namespace Debugging
{
    public class ShowMoveState : MonoBehaviour
    {

        private TextMeshProUGUI _moveStateText;
        private PlayerController _playerController;
        private PlayerMovement _playerMovement;

        private void Start()
        {
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _playerMovement = _playerController.playerMovement;
            _moveStateText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            _moveStateText.text = _playerMovement.GetPlayerMovementState().ToString();
        }
    }
}
