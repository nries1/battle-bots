using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text playerNameText;
    void Start()
    {
        HandlePlayerNameChange(string.Empty, player.PlayerName.Value);
        player.PlayerName.OnValueChanged += HandlePlayerNameChange;
    }

    private void HandlePlayerNameChange(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerNameText.text = newValue.ToString();
    }

    void OnDestroy()
    {
        player.PlayerName.OnValueChanged -= HandlePlayerNameChange;
    }
}
