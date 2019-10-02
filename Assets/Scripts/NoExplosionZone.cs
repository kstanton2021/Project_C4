﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoExplosionZone : MonoBehaviour
{
    // Box that will disable the Disable
        public GameObject disableBox;
    // List of Objects the laser destroys
        public List<string> destroyList = new List<string>();

    void Update()
    {
        // If someone has destroyed the Disable Box, destroy the No-Explosion Zone
            if (disableBox == null)
                Destroy(gameObject);
    }

    // If an explosive enters the No-Explosion Zone it is destroyed
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (destroyList.Contains(other.gameObject.tag))
            Destroy(other.gameObject);
    }



}
