using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public Vector3 force;
    public Rigidbody2D rb;
    public GameObject GatePrefab;
    public GameObject CubePrefab;
    

    bool isDead = false;

    public Text placarTexto;
    public int score = 0;
    

    public AudioClip moveSound;


    Vector3 camPos;


    // Use this for initialization
    void Start () {
        
        PlayerPrefs.SetInt("pontuacao", score);
        
        rb = GetComponent<Rigidbody2D>();
        float halfScreenWidthInUnits = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0)).x - 2f;
        Vector3 deltaPos = Vector3.up * 10f;
        Vector3 prevPos = Vector3.zero;
        Vector3 curPos = Vector3.zero;



        for (int i=1; i < 100; i++)
        {
            curPos = prevPos + deltaPos;
            curPos = new Vector3(Random.Range(-halfScreenWidthInUnits, halfScreenWidthInUnits), curPos.y, 0);
            Instantiate(GatePrefab, curPos, Quaternion.identity);

            Instantiate(CubePrefab, new Vector3(curPos.x + Random.Range(-2f, 2f), curPos.y+Random.Range(1f,5f)),Quaternion.identity);
            Instantiate(CubePrefab, new Vector3(curPos.x + Random.Range(-3f, 3f), curPos.y + Random.Range(2f,5f)), Quaternion.identity);

            prevPos = new Vector3(0, curPos.y);
        }
        
    }
  
    public void Score()
    {

        if (score > PlayerPrefs.GetInt("pontuacao"))
        {
            score = 0;
        }

        placarTexto.text = PlayerPrefs.GetInt("pontuacao", score).ToString();
        


    }

    // Update is called once per frame
    void Update () {
        
        Score();
        

        Vector3 curPos = Camera.main.WorldToScreenPoint(transform.position);

        if (curPos.x > Screen.width - 10 || curPos.x < 10)
        {
            rb.velocity = new Vector3(
                Mathf.Abs(rb.velocity.x) * ((curPos.x < 10)? 1: -1), rb.velocity.y);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (rb.isKinematic)
                rb.isKinematic = false;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;

            if(Input.mousePosition.x > Screen.width / 2)
            {
                rb.AddForce(force, ForceMode2D.Impulse);

            }
            else
            {
                rb.AddForce(new Vector3(-force.x, force.y),ForceMode2D.Impulse);
                
                
            }
        }
        if (curPos.y <10)
        {
            Die();
            
            
        }
        MoveCamera();
    }

 

    void Die()
    {
        
        isDead = true;
        GetComponent<Collider2D>().enabled = false;
        placarTexto.text = PlayerPrefs.GetInt("pontuacao", score).ToString();

        if (score > PlayerPrefs.GetInt("recorde"))
        {
            PlayerPrefs.SetInt("recorde", score);
        }
        
        Application.LoadLevel("Restart");
        
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        Die();
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        
        col.enabled = false;
        score++;

        if (score > PlayerPrefs.GetInt("pontuacao"))
        {
            PlayerPrefs.SetInt("pontuacao", score);
        }
        if (score > PlayerPrefs.GetInt("recorde"))
        {
            PlayerPrefs.SetInt("recorde", score);
        }

    }
    
  

    void MoveCamera()
    {
        camPos = Camera.main.transform.position;
        if(transform.position.y > camPos.y)
        {
            Camera.main.transform.position = new Vector3(camPos.x, transform.position.y, camPos.z);
        }
    }
}
