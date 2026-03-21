using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DecalAnimation : MonoBehaviour
{
    public float rotationSpeed = 40f; //Degrees rotation
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float pulseSpeed = 5.0f;
    public float pulseAmount = 0.2f;
    private Image decalImage;
    private Vector3 baseScale;

    void Start()
    {
       decalImage = GetComponent<Image>();
       baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        //pulse
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount; //calculate how much to scale
        transform.localScale = baseScale + new Vector3(scaleOffset, scaleOffset, scaleOffset); //apply new scale to image
    }
}
