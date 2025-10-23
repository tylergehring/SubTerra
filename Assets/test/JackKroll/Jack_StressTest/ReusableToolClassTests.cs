
//Jack Kroll

using UnityEngine;
using UnityEngine.SceneManagement;//lets me load the MVP sean before the test
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
//these are needed for my tests and often used in others 
public class ReusableToolClassPlayModeTests
{//temp game objet created durring the test sean 
    private GameObject testObj;
    private ReusableToolClass tool;
    private Light lightComponent;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Loadads the MVP sean 
        SceneManager.LoadScene("MVP");
        yield return null; // wait a frame for scene load to complete


        tool = GameObject.FindObjectOfType<ReusableToolClass>();


        lightComponent = tool.flashlight;
        //sets flashligh off before test
        lightComponent.enabled = false;
    }
    //destroys the temporyry test game objects if one was created
    //this is more of a extra safty mesure then anything else
    [UnityTearDown]
    public IEnumerator Teardown()
    {
        if (testObj != null)
            Object.Destroy(testObj);
        yield return null;
    }
    // This is the stress test that simply turns/toggles the flashlight
    //Untel we reach less then 100FPS to symulate very hevay usage 
    //this makes shure that the flashlight works proporly over
    //extended gameplay with no major erors It also helps 
    // rule out or conform if this is a problem for game latncy 
    // for normal results you should reach slightly less then 1oo FPS 
    // at about 4000 toggles also keep in mind that anything more then 
    // 10000 toggles is a time constraint prmiter that could be changed

    [UnityTest]
    public IEnumerator Stress_ToggleFlashlightRapidly()
    {
        float deltaTime = 0f;
        float fps = 999f; // start high so loop runs initially
        int i = 0;

        Debug.Log("Starting Stress test");

        // loop until fps < 100 OR i > 10000 just becuse its posible a PC is just that good. 
        while (fps > 100f && i < 10000)
        {
            if (i == 0)
            {
                Debug.Log($" Intal FPS is {fps:F2}");
            }

            tool.UseTool(); // toggle flashlight on/off
            yield return null; // wait a frame

            // Calculate FPS (smoothed average)
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1f / Mathf.Max(deltaTime, 0.0001f);

            // Log every 500 iterations this is so we dont get erorrs in the consal log
            if (i % 500 == 0)
                Debug.Log($"Iteration {i} | Current FPS: {fps:F2}");

            // Verify flashlight state
            Assert.AreEqual(tool.flashlight.enabled, i % 2 == 0);
            
            i++;
        }

        Debug.Log($" FPS dropped below 100 after {i} toggles L was pressed. Final FPS is {fps:F2}");

<<<<<<< HEAD
        // fail the test if final FPS is below 60, the reson for this is because if the 
        // FPS is below 60 (highly unlikly without other conncurent process runing) then
        // The Flashlight toggiling is contributing to much to the games speed or lack there of.
        Assert.GreaterOrEqual(fps, 60f, $"Final FPS {fps:F2} is below 60");

        Debug.Log("The test is done");
=======
            Vector3 direction = mousePos - tool.transform.position;
            tool.transform.rotation = Quaternion.LookRotation(direction);
            yield return null;
 
            // Validate rotations 
            Quaternion rot = tool.transform.rotation;
            Assert.IsFalse(float.IsNaN(rot.x) || float.IsNaN(rot.y) || float.IsNaN(rot.z),
                $"Invalid rotation detected at position {pos}");
        }
>>>>>>> ad3e370307f02836e8d383bda4a18e45106dda24
    }



    //this is my 1st boundry test witch tests 
    //The lowest bound state psoible for this item ie off or 0 
    //The purpous for tetsing the lower bound is 
    // to catch improper initialization.
    [UnityTest]
    public IEnumerator Flashlight_StartsOff()
    {
        // Wait one frame for initialization
        yield return null;

        // Flashlight should start off if the flashing starts on then this is a improper initialization error. 
        Assert.IsFalse(tool.flashlight.enabled, "Flashlight should start OFF if not test will fail.");
    }


    //This last boundry test tests the flashlight proporly tuning off and on 
    //with 2 second intervols witch automates checking the bounds of normal use.
    // with a uper bound of 20 uses
    [UnityTest]
    public IEnumerator Flashlight_ToggleOnOff_10Times_WithDelay()
    {
        yield return null; // wait a frame for initialization

        int toggleCount = 20; // we can change this to however meny interations we want 
        float interval = 2f; // 2-second interval

        for (int i = 0; i < toggleCount; i++)
        {
            tool.UseTool(); // toggle flashlight
            yield return null;

            // Check flashlight state is correct 
              bool expectedState = (i % 2 == 0) ? true : false;
                 Assert.AreEqual(tool.flashlight.enabled, expectedState,
                $"Flashlight state mismatch at toggle {i + 1}");

            // wait a few seconds before the next toggle
            yield return new WaitForSecondsRealtime(interval);
        }
    }

}
