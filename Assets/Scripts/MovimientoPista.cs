using UnityEngine;

public class MovimientoPista : MonoBehaviour
{
    public float velocidad = -5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       transform.Translate(-Camera.main.transform.forward * velocidad * Time.deltaTime, Space.World);
        
    }
}
