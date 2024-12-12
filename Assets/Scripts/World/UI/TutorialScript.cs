using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] List<Sprite> tutorial_images;
    [SerializeField] List<string> tutorial_strings;

    [SerializeField] TextMeshProUGUI pageNumber_text;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image timage;
    private int page_number;

    private void Start()
    {
        page_number = 0;
        pageNumber_text.text = (page_number + 1).ToString() + "/3";
        gameObject.SetActive(false);
    }

    private void Update()
    {
        pageNumber_text.text = (page_number + 1).ToString() + "/3";
        timage.sprite = tutorial_images[page_number];
        description.text = tutorial_strings[page_number];
    }

    public void NextPage()
    {
        if(page_number == tutorial_images.Count-1)
        {
            page_number = 0;
        }
        else
        {
            page_number++;
        }
    }

    public void LastPage()
    {
        if (page_number == 0)
        {
            page_number = tutorial_images.Count;
        }
        else
        {
            page_number--;
        }
    }

    public void ShowTutorial()
    {
        gameObject.SetActive(true);
    }
}
