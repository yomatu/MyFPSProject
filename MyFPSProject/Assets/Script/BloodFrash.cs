using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BloodFrash : MonoBehaviour
{
    public GameObject uiGameObject;

    private Image uiImage;

    private bool isusingDmgImg;

    private bool isCoroutineRunning = false;

    void Start()
    {
        uiImage= uiGameObject.GetComponent<Image>();
       
      //  StartCoroutine(FadeInAndOut(uiImage,1f));

    }

    void Update()
    {
        isusingDmgImg = uiGameObject.activeSelf;

        if (!isusingDmgImg && !isCoroutineRunning)
        {
            StartCoroutine(FadeInAndOut(uiImage, 1f));
        }
    }

    //协程来处理alpha值的渐变
    IEnumerator FadeInAndOut(Image image,float duration)
    {
        isCoroutineRunning = true;
        //从0渐变到1
        float counter = 0f;

        while (counter <duration) 
        { 
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(0f,1f,counter/duration);
            
            image.color= new Color(image.color.r,image.color.g,image.color.b,alpha);
        
            yield return null;
        }

        //等待1s
        yield return new WaitForSeconds(1f);

        //再从1渐变到0
        counter = 0f;
        while (counter<duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1f,0f,counter/duration);

            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        
            yield return null;
        }


        //自己隐藏或者委托隔壁代码隐藏
        uiGameObject.SetActive(false);
        isCoroutineRunning = false;
    }
}
