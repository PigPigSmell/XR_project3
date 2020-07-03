using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hit_block : MonoBehaviour
{
    public GameObject cam;
    public GameObject All;
    public GameObject tutorial;

    [Header("Standard")]
    public GameObject note;
    
    [Header("Hand")]
    public GameObject leftHand;
    public GameObject rightHand;

    [Header("MoveCube")]
    public GameObject move0;
    public GameObject move1;

    public bool start, RisTrigger, LisTrigger, move, slow;
    private bool walk, jump;
    private int cnt, slide, walkStep, jumpStep, skipStep;
    private float startTime, baseLine, prevD;
    private SteamVR_Behaviour_Boolean steamVR_Behaviour_Boolean = new SteamVR_Behaviour_Boolean();
    private float x = 0, y = 0, z = 0;
    private Vector3 bias;

    Quaternion Q;

    AudioPlayer audioP;
    image_alpha img;
    GoCastle gc;

    // Start is called before the first frame update
    void Start()
    {
        audioP = GameObject.Find("AudioPlayer").GetComponent<AudioPlayer>();
        img = GameObject.Find("Image").GetComponent<image_alpha>();
        gc = GameObject.Find("留聲機").GetComponent<GoCastle>();

        Q = note.transform.rotation;

        walk = false;
        jump = false;
        slide = 0;
        cnt = 0;
        walkStep = -1;
        jumpStep = 0;
        skipStep = 0;
        move = false;
        start = false;
        slow = false;

        //bias = All.transform.position - standard_low.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(note.transform.position.y < -10)
        {
            img.close = 1;
            audioP.enabled = false;

            gc.SetScore(0);
            gc.WriteData();
        }

        RisTrigger = SteamVR_Input.GetState("Maze", "right_hold", steamVR_Behaviour_Boolean.inputSource);
        LisTrigger = SteamVR_Input.GetState("Maze", "left_hold", steamVR_Behaviour_Boolean.inputSource);

        //Move();

        //start = true;
        //prevD = Vector3.Distance(standard_low.transform.position, new Vector3(0, 0, 0));


        //Debug.Log(isTrigger);
        if (start)
        {
            Move();
            note.transform.rotation = Q;
        }
        else
        {
            if (RisTrigger)
            {
                //Debug.Log("innn");
                //All.transform.position = new Vector3(cam.transform.position.x + 2, cam.transform.position.y + 3f, cam.transform.position.z - 3);
                //All.transform.rotation = new Quaternion(0, 1, 0, -cam.transform.rotation.y - 0.2f);
                // note.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
                //All.transform.position = standard_low.transform.position + bias;
                x = note.transform.position.x - this.transform.position.x;
                //y = note.transform.position.y - this.transform.position.y;
                z = note.transform.position.z - this.transform.position.z;
                //this.transform.position = new Vector3(standard_low.transform.position.x, standard_low.transform.position.y, standard_low.transform.position.z);
                //All.transform.position = new Vector3(cam.transform.position.x + 2, cam.transform.position.y + 4f, cam.transform.position.z - 6);
                start = true;
                prevD = Vector3.Distance(note.transform.position, new Vector3(0, 0, 0));
                tutorial.SetActive(false);
            }
        }

        if (move){
            walkStep = -2;
            move0.transform.position += (move0.transform.forward * 1.25f + move0.transform.up) * Time.deltaTime * 2;
            note.transform.position += (move0.transform.forward * 1.3f + move0.transform.up) * Time.deltaTime * 2;
        }
        if(move && move0.transform.position.y >= move1.transform.position.y)
        {
            //move0.transform.position = move1.transform.position;
            //standard_low.transform.position = move0.transform.position + new Vector3(0, 0.5f, 0);
            move = false;
            walkStep = -1;
        }
    }


    private void Move()
    {
        //if(leftFoot.transform.position.y > 0.8f && rightFoot.transform.position.y > 0.8f && slide == 0)
        if (slide == 0 && LisTrigger)
        {
            if (!RisTrigger)
            {
                /* walk */
                if (walkStep == -1)
                {
                    if (rightHand.transform.position.y > leftHand.transform.position.y + 0.1f)
                    {
                        walkStep = 0;
                    }
                }
                else if (walkStep == 0)
                {
                    if (rightHand.transform.position.y < leftHand.transform.position.y)
                    {
                        walkStep = 1;
                        walk = true;
                    }
                }
                else if (walkStep == 1)
                {
                    if (rightHand.transform.position.y > leftHand.transform.position.y + 0.1f)
                    {
                        walkStep = 2;
                        walk = true;
                    }
                }
                if (walkStep == 2)
                {
                    if (!slow)
                    {
                        note.transform.position += new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z) * (0.6f);
                    }
                    else
                    {
                        note.transform.position += new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z) * (0.2f);
                    }
                    
                    audioP.WalkSound();
                    walkStep = 0;
                    walk = false;
                }
            }
            else if (RisTrigger)
            {
                /* jump */
                if (!jump && rightHand.transform.position.y > leftHand.transform.position.y + 0.4f)
                {
                    //walkStep = -2;
                    jump = true;
                    audioP.JumpSound();
                    note.transform.position += new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z) * (0.3f);
                    note.transform.position += new Vector3(0, 0.2f, 0);
                }

                if (jump && rightHand.transform.position.y < leftHand.transform.position.y + 0.2f)
                {
                    jump = false;
                    //walkStep = -1;
                }
            }
            else
            {
                walkStep = -1;
                jump = false;
            }

            if (walk || jump)
            {
                startTime = Time.time;
            }
            else if (Time.time - startTime > 2)
            {
                audioP.stopSound();
                startTime = Time.time;
            }
        }
        else if (slide == 1)
        {
            // slide up
            note.transform.position += new Vector3(cam.transform.forward.x * 0.3f, 0.4f, cam.transform.forward.z * 0.3f) * (Time.deltaTime) * 3;
        }
        else if (slide == -1)
        {
            // slide down
            note.transform.position += new Vector3(cam.transform.forward.x * 0.3f, 0f, cam.transform.forward.z * 0.3f) * (Time.deltaTime) * 3;
        }

        this.transform.position = new Vector3(note.transform.position.x - x, note.transform.position.y - 0.5f, note.transform.position.z - z);

        if(!walk && !jump)
        {
            float currentD = Vector3.Distance(note.transform.position, new Vector3(0, 0, 0));
            //Debug.Log(currentD);
            if (currentD < prevD)
            {
                skipStep++;
                if(skipStep > 10)
                {
                    skipStep = 0;
                    audioP.walkBack();
                }
            }
            prevD = currentD;
        }
    }

    public void setSlide(int s)
    {
        slide = s;
    }

    public void setMove(bool mark)
    {
        move = mark;
    }

    public void setSlow(bool s)
    {
        slow = s;
    }
}
