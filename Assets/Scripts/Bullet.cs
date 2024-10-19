using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public delegate void BulletAnnouncements(GameObject GO);
    public static event BulletAnnouncements BulletDeactivated;

    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * speed * new Vector3(0, 1, 0);
        if(transform.position.y > 15) Deactivate();
    }

    private void Deactivate(){
        transform.position = new Vector3(100f, 100f, 100f);
        BulletDeactivated(gameObject);
        gameObject.SetActive(false);
    }

    public void Activate(Vector3 position){
        transform.position = position;
        gameObject.SetActive(true);
    }
}
