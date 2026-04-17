using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuCamera : MonoBehaviour
{
    //[SerializeField] float speed;
    [SerializeField] Transform house;
    [SerializeField] Image panel;
    //float angle = 0;
    // Update is called once per frame
    //adjust this to change speed
    float speed = 3f;
    //adjust this to change how high it goes
    float height = 1f;
    bool goIn = false;

    Color newColor;
    float baseY;
    void Start()
    {
        newColor = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
        baseY = transform.position.y;
    }
    void Update()
    {
        transform.LookAt(house);

        if(goIn)
        {
            
            if (Vector3.Distance(transform.position, house.position) > .1f)
            {
                newColor.a += .025f;
                panel.color = newColor;
                transform.Translate(Vector3.forward * Time.deltaTime * 200);
            }

        }




        float newY = baseY + Mathf.Sin(Time.time * speed);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z) * height;
    }

    public async void GoIn()
    {
        goIn = true;
    }
}

