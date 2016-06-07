﻿/*
**  Bullet.cs: Moves the gameobject forward by a specified speed until its lifetime ends.
*/

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    public string bulletName = "Bullet";

    public int manaCost = 0;

    //How after this bullet  can be fired (in seconds)
    public float fireTime = 0.25f;

    //How fast the bullet will move
    public float moveSpeed = 10f;
    //The amount of seconds after spawning until the bullet will be destroyed
    public float lifeTime = 5f;

    //The amount of damage that will be applied when the bullet collides with a character
    public int damage = 10;

    //Rotation is a SyncVar so that it is synced when the bullet is first spawned, 
    //thus removing the need to sync bullet position since it will not change direction.
    [SyncVar]
    [HideInInspector] //Public because it is set externally, but should not show in inspector
    public Vector3 initialRotation;

    //The player who shot this bullet - they should not be damaged
    [SyncVar]
    public GameObject owner;

    void Start()
    {
        transform.eulerAngles = initialRotation;
        //Set object to be destroyed after its lifetime ends
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        //Move the object forward by speed (local forward, as bullet rotation is set when spawned)
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject != owner)
        {
            PlayerStats stats = col.gameObject.GetComponent<PlayerStats>();

            //if the object that was collided with has stats, and is alive
            if (stats && stats.isAlive)
            {
                //Apply damage (name of bullet owner is also sent to identify who killed who)
                stats.ApplyDamage(damage, owner.GetComponent<PlayerInfo>().username, bulletName);

                //destroy bullet
                Destroy(gameObject);
            }
        }
    }

    void SetOwner(GameObject obj)
    {
        owner = obj;
    }
}