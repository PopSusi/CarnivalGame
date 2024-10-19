using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public delegate void PlayerAnnouncements();
    public static event PlayerAnnouncements PlayerDie;

    [SerializeField] private float speed;
    [SerializeField] private GameObject bulletPrefab;
    private List<GameObject> bulletsInactive = new List<GameObject>();
    private int iterator = 0;

    [SerializeField] private float bulletcooldown;
    private bool canFire = true;

    
    void Awake()
    {
        for(int i = 0; i < 10; i++){
            bulletsInactive.Add(Instantiate(bulletPrefab,
             new Vector3(100f, 100f, 100f),
             Quaternion.identity));
        }
        Bullet.BulletDeactivated += BulletReturnToInactive;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += Time.deltaTime * speed * moveInput;

        if (Input.GetButtonDown("Fire1"))
        {
            FireBullet();
        }
    }

    private void FireBullet(){
        if(canFire){
            bulletsInactive[iterator%10].GetComponent<Bullet>().Activate(transform.position);
            iterator++;
            Debug.Log("Removed from stack");
            canFire = false;
            StartCoroutine("BulletCooldownStart");
        }
    }

    private IEnumerator BulletCooldownStart(){
        yield return new WaitForSeconds(bulletcooldown);
        canFire = true;
    }

    private void OnCollision2DEnter(Collision collision){
        if(collision.gameObject.CompareTag("Bullet")) PlayerDie();
        if(collision.gameObject.CompareTag("Enemy")) PlayerDie();
    }

    private void BulletReturnToInactive(GameObject bullet){
        bulletsInactive.Add(bullet);
    }
}
