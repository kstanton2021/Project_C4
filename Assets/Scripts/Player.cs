﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    // Creates Rigidbody2D
        Rigidbody2D rb;
    // Acceses the managers
        LevelManager managerScript;
        SettingsManager settingsScript;
        public SoundManager soundScript;
    // Left and right move speed;
        public float moveSpeed;
    // Height of jump
        public float jumpForce;
    // bool for left facing
        public bool isLeft;
        public int leftScale;
    // bool for above player
        public bool isUp;
    // Empty game object that holds the location of the weapon on the player
        Vector2 weaponSpawn = Vector2.zero;
    // Creates inventory
         GameObject[] inventory = new GameObject[3];
    // Current inventory slot;
         public int slot;

    // Checks how many equpments have been used
        public int[] usedEquipment = new int[3];
    // Checks how many jumps have been used
        public int usedJump;

    // Raycast to determine if you can move left and right
        RaycastHit2D left;
        RaycastHit2D right;
    // Raycast to determine if you can jump
        RaycastHit2D downLeft;
        RaycastHit2D downRight;

    // Checks if the player will be destroyed by an explosive
        public GameObject warning;
    // Creates a cursor
        GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        // Gets the cursor GameObject
            cursor = GameObject.FindGameObjectWithTag("Cursor");
        // Gets the Rigidbody2D on the player
            rb = GetComponent<Rigidbody2D>();
        // Gets the script on the Level Manager
            managerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<LevelManager>();
            settingsScript = GameObject.FindGameObjectWithTag("Settings").GetComponent<SettingsManager>();
        // Equips weapons from the level manager
            inventory[0] = managerScript.weapons[0];
            inventory[1] = managerScript.weapons[1];
            inventory[2] = managerScript.weapons[2];
        // Defaults to the first inventory slot
            slot = 0; 
        // Defaults to facing right
            isLeft = false;
            leftScale = 1;

        // defaults explosive warning to false
        warning.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Sets raycast length on the left and right of the player
            left = Physics2D.Raycast(transform.position, Vector2.left, 0.45f);
            right = Physics2D.Raycast(transform.position, Vector2.right, 0.45f);

        // Sets raycast length on the bottom of the player to the left and right
            downLeft = Physics2D.Raycast(new Vector2(transform.position.x - 0.4f, transform.position.y), Vector2.down, 1.0f);
            downRight = Physics2D.Raycast(new Vector2(transform.position.x + 0.4f, transform.position.y), Vector2.down, 1.0f);

        // Function that moves the character right and left
        if (GameObject.FindGameObjectWithTag("FuseBox") != null && managerScript.pausedGame == false && Camera.main.GetComponent<CameraScript>().cameraPanned == true)
        {
            PlayerMove();


            SwitchWeapon();

            // Throws current weapon
            if (Input.GetMouseButtonDown(0) && managerScript.inventory[slot] > 0 && !EventSystem.current.IsPointerOverGameObject())
            {
                // Spawns the equipment  
                SpawnEqipment(inventory[slot]);
                // Increases the used equipment
                usedEquipment[slot] += 1;
            }

            // Cheks to see what direction the player is facing
            if (cursor.transform.position.x >= transform.position.x)
            {
                // If the cursor is right of the player, the player faces right
                isLeft = false;
                leftScale = 1;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                // If the cursor is left of the player, the player faces left
                isLeft = true;
                leftScale = -1;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            // Changes the IsUp variable
            if (cursor.transform.position.y >= transform.position.y)
                isUp = true;
            else
                isUp = false;
        }

    }

    void PlayerMove()
    {
        // Player moves right with D and left with A
        if (right.collider == null && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[1])))
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);


        if (left.collider == null && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[0])))
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[1])) || Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[0])))
            gameObject.GetComponent<Animator>().SetBool("Moving", true);
        else
            gameObject.GetComponent<Animator>().SetBool("Moving", false);

        // Function that makes sure you are standing on something before you jump
        if ((downLeft.collider != null || downRight.collider != null) && (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[2]))))
        {
            // Jumps with force
            rb.AddForce(Vector2.up * jumpForce);
            gameObject.GetComponent<Animator>().SetTrigger("Jump");
            // Adds one to the jump count only if you are standing on something
            usedJump += 1;
            // plays jump sound
            soundScript.PlaySoundFile(soundScript.jump);
        }
    }

    // Switches between the grenade, c4, and rocket
    void SwitchWeapon()
    {
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[3])) && !Input.GetMouseButton(0))
        {
            slot = 0;
            soundScript.PlaySoundFile(soundScript.click);
        }
        else if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[4])) && !Input.GetMouseButton(0))
        {
            slot = 1;
            soundScript.PlaySoundFile(soundScript.click);
        }
        else if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), settingsScript.controls[5])) && !Input.GetMouseButton(0))
        {
            slot = 2;
            soundScript.PlaySoundFile(soundScript.click);
        }
    }

    //Spawns each equipment at a given instantiation point
    void SpawnEqipment(GameObject equipment)
    {
        // Checks what equipment is being used and uses the equipment acordingly
        managerScript.inventory[slot]--;

        if (equipment.GetComponent<Grenade>() != null)
            Instantiate(equipment, (cursor.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
        else if (equipment.GetComponent<C4>() != null)
            Instantiate(equipment,(cursor.transform.position - transform.position).normalized * 1.25f + transform.position, Quaternion.identity);
        else if (equipment.GetComponent<Rocket_Launcher>() != null)
            Instantiate(equipment, new Vector2(transform.position.x + (equipment.GetComponent<Rocket_Launcher>().offset.x * leftScale), transform.position.y + equipment.GetComponent<Rocket_Launcher>().offset.y), Quaternion.Euler(0, 0, 90));

    }
}
