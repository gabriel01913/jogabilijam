using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    public GameObject torch;
    torchTests _scriptTorch;
    public Vector3 _respawn;

    private void Awake()
    {
        _scriptTorch = torch.GetComponent<torchTests>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.transform.position = _respawn;
            _scriptTorch.Restart();
        }
    }
}
