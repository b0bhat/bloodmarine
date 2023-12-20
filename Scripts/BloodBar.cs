using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodBar : MonoBehaviour
{
    [SerializeField] Image bloodBar;
    Player player;
    // Start is called before the first frame update
    void Start() {
        player = Player.instance;
    }

    // Update is called once per frame
    void Update() {
        bloodBar.fillAmount = Mathf.Clamp(player.blood / player.maxBlood, 0, 1f);

    }
}
