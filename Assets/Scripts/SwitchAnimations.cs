using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAnimations : MonoBehaviour
{
    public PlayerMovement movement;
    public Animator playerAnim;
    private int switchIndex = 0;

    void Update()
    {
        Switch();
    }

    private void Switch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch(switchIndex)
            {
                case 0:
                    switchIndex++;
                    break;
                
                case 1:
                    switchIndex--;
                    break;
            }
        }

        if (switchIndex == 0)
        {
            // Hand Animations
            movement.Mode = PlayerMovement.ParkourMode.Jog;

            playerAnim.SetLayerWeight(1, 1);
            playerAnim.SetLayerWeight(2, 0);

            transform.GetChild(0).gameObject.SetActive(false);

            playerAnim.SetBool("WeaponSpawn", false);
        }

        if (switchIndex == 1)
        {
            // Weapon Animations
            movement.Mode = PlayerMovement.ParkourMode.Weapon;

            playerAnim.SetLayerWeight(1, 0);
            playerAnim.SetLayerWeight(2, 1);

            transform.GetChild(0).gameObject.SetActive(true);
            playerAnim.SetBool("WeaponSpawn", true);
        }
    }
}
