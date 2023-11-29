using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class WoodenSaw : MonoBehaviour
{
    [Header("Saw Management")]
    public float sawRotateSpeed;
    public Transform sawTop;
    public Transform sawBottom;
    public Transform sawPoint;
    public ParticleSystem sawFireEffect;

    [Header("Conveyer Management")]
    public float conveyorSpeed;
    public Transform conveyor;

    [Header("UI Management")]
    public Slider objectiveValueSlider;
    public GameObject minigameNotificationPrefab;
    public Transform uiContainer;
    public Image indicatorImage;

    [Header("Indicator Color")]
    public Color perfectColor;
    public Color goodColor;
    public Color badColor;
    public Color nothingColor;

    [Header("Event Management")]
    public UnityEvent winEvent;
    public UnityEvent gameOverEvent;

    [Header("Wood Management")]
    public WoodenSaw_Type[] types;
    Transform[] wood;
    Transform[] ironGap;

    [Header("Newest Score")]
    public List<string> score = new List<string>();

    float incrementAmount = 1.0f;
    float incrementDuration = 1.0f;

    bool isIncrementing = false;

    bool isSawEnable = true;
    bool isGameOver;
    bool isGameStart = false;
    WoodenSaw_Wood onCutting;

    int successCutting = 0;
    int objectiveValue;


    Animator sawAnimTop;
    Animator sawAnimBottom;

    WoodenSawItemPosition startPosition;
    WoodenSaw_Type typeinScene;
    private void Start()
    {
        isGameOver = true;
        sawAnimTop = sawTop.GetComponent<Animator>();
        sawAnimBottom = sawBottom.GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartMinigame();
            }
        }



        if (!isGameOver && isGameStart)
        {
            sawAnimTop.SetBool("isSawEnable", isSawEnable);
            sawAnimBottom.SetBool("isSawEnable", isSawEnable);

            foreach (Transform w in wood)
            {
                w.Translate(Vector2.left * conveyorSpeed * Time.deltaTime);
            }
            foreach (Transform g in ironGap)
            {
                g.Translate(Vector2.left * conveyorSpeed * Time.deltaTime);
            }


            //LISTENER KEYBOARD KEY
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSawEnable = false;

                if (onCutting)
                {
                    float dist = Vector3.Distance(onCutting.rightPoint.position, sawPoint.position);
                    Debug.Log(sawPoint.position.x + "  -  " + onCutting.rightPoint.position.x);

                    if (dist < 8)
                    {
                        SuccessSawWood("PERFECT");
                    }
                    else if (sawPoint.position.x > onCutting.full.transform.position.x)
                    {
                        SuccessSawWood("GOOD");
                    }
                    else
                    {
                        SuccessSawWood("BAD");
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isSawEnable = true;


            }

            if (onCutting)
            {
                if (onCutting.rightPoint.position.x < sawPoint.position.x)
                {
                    SuccessSawWood("GOOD");
                }
            }

            //CHECKING IF SAW IS ENABLE
            if (isSawEnable)
            {
                //CHECKING IF NOT ON CUTTING WOOD
                if (!onCutting)
                {
                    sawFireEffect.Stop();

                    //CHECKING DISTANCE OF NEARER WOOD
                    foreach (Transform w in wood)
                    {
                        WoodenSaw_Wood nearWood = w.GetComponent<WoodenSaw_Wood>();
                        if (nearWood)
                        {
                            if (nearWood.alreadyCut)
                                continue;

                            float dist = Vector3.Distance(nearWood.leftPoint.position, sawPoint.position);

                            //CHECK SAW STILL OUTSIDE WOOD
                            if (sawPoint.position.x <= nearWood.leftPoint.position.x)
                            {
                                //CHECK DISTANCE OF POINT SAW WITH WOOD LEFT POINT
                                if (dist < 3)
                                {
                                    if (nearWood.isTrap)
                                        LoseGame();

                                    onCutting = nearWood;
                                    onCutting.trail.enabled = true;


                                    nearWood.isCutting = true;
                                    Debug.Log("Is Cutting");
                                    sawFireEffect.Play();

                                }
                            }
                            else //IF SAW INSIDE THE WOOD
                            {
                                //CHECK DISTANCE IF SAW STILL PROBABILITY CAN CUT THE WOOD
                                if (dist < 5)
                                {
                                    if (nearWood.isTrap)
                                        LoseGame();

                                    onCutting = nearWood;
                                    onCutting.trail.enabled = true;

                                    nearWood.isCutting = true;
                                    Debug.Log("Is Cutting");
                                    sawFireEffect.Play();

                                }
                                else //IF SAW IS ENOUGH FAR TO MIDDLE OF WOOD - FAILED TO CUT THE WOOD
                                {
                                    //IF SAW INSIDE OR MIDDLE OF WOOD - FAILED
                                    if (sawPoint.position.x > nearWood.leftPoint.position.x && sawPoint.position.x < nearWood.rightPoint.position.x)
                                    {
                                        FailedSawWood("Wood is the middle of saw", nearWood);
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    Debug.Log("tes");

                    //CHECK DISTANCE OF THE END OF WOOD - FOR FINISH THE CUTTING WOOD

                }

                //CHECKING IF TOUCHING IRON GAP
                foreach (Transform gap in ironGap)
                {
                    //float dist = Vector3.Distance(sawPoint.position, gap.position);
                    if (gap.GetComponent<WoodenSaw_IronGap>().hasPass)
                        continue;

                    if (gap.position.x < sawPoint.position.x + 1)
                    {
                        LoseGame();
                    }

                }
            }
            else
            {
                sawFireEffect.Stop();

                if (!onCutting)
                {
                    foreach (Transform w in wood)
                    {
                        WoodenSaw_Wood nearWood = w.GetComponent<WoodenSaw_Wood>();

                        if (nearWood)
                        {
                            if (nearWood.alreadyCut)
                                continue;

                            float dist = Vector3.Distance(nearWood.leftPoint.position, sawPoint.position);

                            if (sawPoint.position.x > nearWood.leftPoint.position.x)
                            {
                                if (dist < 5)
                                {
                                    LoseGame();
                                }

                            }

                        }
                    }

                }

                //CHECKING IF NOT TOUCHING IRON GAP
                foreach (Transform gap in ironGap)
                {
                    //float dist = Vector3.Distance(sawPoint.position, gap.position);
                    if (gap.position.x < sawPoint.position.x + 3)
                    {
                        gap.GetComponent<WoodenSaw_IronGap>().hasPass = true;
                        gap.gameObject.SetActive(false);
                    }
                }

            }

            if (onCutting)
            {
                float dist = Vector3.Distance(onCutting.rightPoint.position, sawPoint.position);

                if (dist < 8) // dari ujung kanan hingga "8"
                {
                    indicatorImage.color = perfectColor;
                }
                else if (sawPoint.position.x > onCutting.full.transform.position.x) // dari ujung kanan hingga tengah
                {
                    indicatorImage.color = goodColor;
                }
                else // sisanya dari tengah hingga kiri
                {
                    indicatorImage.color = badColor;

                }
            }
            else
            {
                indicatorImage.color = nothingColor;
            }
        }
    }
    IEnumerator StartCountdown()
    {
        isGameOver = false;
        isGameStart = false;
        objectiveValueSlider.gameObject.SetActive(false);
        objectiveValue = 0;

        yield return new WaitForSeconds(1);
        StartCoroutine(SpawnNotification("Wood Cutting | MiniGame", 1.4f));
        yield return new WaitForSeconds(2);
        StartCoroutine(SpawnNotification("3"));
        yield return new WaitForSeconds(1);
        StartCoroutine(SpawnNotification("2"));
        yield return new WaitForSeconds(1);
        StartCoroutine(SpawnNotification("1"));
        yield return new WaitForSeconds(1);

        foreach (Transform w in wood)
        {
            if (!w.GetComponent<WoodenSaw_Wood>().isTrap)
                objectiveValue++;
        }
        successCutting = 0;
        objectiveValueSlider.gameObject.SetActive(true);

        isGameStart = true;

        objectiveValueSlider.maxValue = objectiveValue;
        objectiveValueSlider.value = 0;
        StartCoroutine(SpawnNotification("GAME START"));

    }

    public void FailedSawWood(string message = null, WoodenSaw_Wood onCut = null)
    {
        if (onCut)
            onCutting = onCut;

        onCutting.isCutting = false;
        onCutting.alreadyCut = true;
        onCutting.full.enabled = false;
        onCutting.trail.enabled = false;
        onCutting = null;
        Debug.Log(message);

        StartCoroutine(SpawnNotification(message));
    }

    public void SuccessSawWood(string message = null)
    {
        onCutting.isCutting = false;
        onCutting.alreadyCut = true;
        //onCutting.full.enabled = false;
        onCutting.GetComponent<Animator>().SetTrigger("Splice");

        onCutting.trail.enabled = false;
        onCutting = null;

        successCutting++;
        StartCoroutine(IncrementSliderSmoothly());

        Debug.Log(message);

        score.Add(message);


        if (successCutting >= objectiveValue)
        {
            WinGame();
        } else
        {
            StartCoroutine(SpawnNotification(message));
        }
    }

    public void WinGame()
    {
        isGameOver = true;
        isSawEnable = false;
        sawAnimTop.SetBool("isSawEnable", false);
        sawAnimBottom.SetBool("isSawEnable", false);

        StartCoroutine(SpawnNotification("GAME FINISH", 2));

        winEvent.Invoke();
    }

    public void LoseGame()
    {
        isGameOver = true;
        isGameStart = false;
        isSawEnable = false;

        StartCoroutine(SpawnNotification("YOU LOSE", 3));

        gameOverEvent.Invoke();


        sawAnimTop.SetBool("isSawEnable", false);
        sawAnimBottom.SetBool("isSawEnable", false);
    }


    private IEnumerator IncrementSliderSmoothly()
    {
        isIncrementing = true;

        float startValue = objectiveValueSlider.value;
        float targetValue = Mathf.Clamp(startValue + incrementAmount, 0, objectiveValueSlider.maxValue);
        float currentTime = 0;

        while (currentTime < incrementDuration)
        {
            currentTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, targetValue, currentTime / incrementDuration);
            objectiveValueSlider.value = newValue;

            yield return null;
        }

        objectiveValueSlider.value = targetValue;
        isIncrementing = false;
    }

    public IEnumerator SpawnNotification(string message, float delay = 0.4f)
    {
        GameObject notif = Instantiate(minigameNotificationPrefab, uiContainer);
        notif.GetComponent<TMP_Text>().text = message;

        yield return new WaitForSeconds(delay);
        notif.GetComponent<Animator>().SetTrigger("Hide");

        Destroy(notif, 2);

    }

    public void StartMinigame()
    {
        score.Clear();
        onCutting = null;

        if(typeinScene)
            Destroy(typeinScene.gameObject);

        WoodenSaw_Type type = Instantiate(types[Random.Range(0, types.Length)].gameObject, conveyor.transform).GetComponent<WoodenSaw_Type>();
        typeinScene = type;

        wood = type.wood;
        ironGap = type.ironGap;
        isSawEnable = true;

        StartCoroutine(StartCountdown());

    }

}

[System.Serializable]
public class WoodenSawItemPosition{
    public List<Vector3> startPositionWood = new List<Vector3>();
    public List<Vector3> startPositionIronGap = new List<Vector3>();
}
