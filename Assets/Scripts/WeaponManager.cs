using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject bulletPrefab;

    public int clips = 3;
    public int bulletsPerClip = 10;

    public int bullets = 0;
    public float reloadTime = 1f;
    public float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        bullets = bulletsPerClip;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Shoot(){
        if(bullets > 0){
            bullets--;
            Debug.Log("Shooting");
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            // bullet.GetComponent<Bullet>().damage = damage;
        }
    }

    public void Reload(){
        if(bullets == bulletsPerClip){
            return;
        }

        if(clips == 0){
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    public IEnumerator ReloadCoroutine() {
        Debug.Log("Reloading");
        yield return new WaitForSeconds(reloadTime);
        bullets = bulletsPerClip;
        clips--;
    }

}

