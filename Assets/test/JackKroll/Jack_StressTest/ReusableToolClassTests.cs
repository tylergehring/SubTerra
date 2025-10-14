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
    //1000 times to symulate very hevay uses 
    //this makes shure that the flashlight works proporly over
    //extended gameplay with no major erors 
    [UnityTest]
    public IEnumerator Stress_ToggleFlashlightRapidly()
    {
        for (int i = 0; i < 1000; i++)
        {
            tool.UseTool();//tuns light on off
            yield return null;

            Assert.AreEqual(tool.flashlight.enabled, i % 2 == 0);
        }
    }
    //this is my 1st boundry test witch tests the rotation stability
    //of the flashlight tool in relation to the position of the 
    //mouse. Testing the games responce to the mouse being at the 
    //screens extrems
    [UnityTest]
    public IEnumerator Boundary_RotationStaysValidAtScreenEdges()
    { 
      // smulates extream mouse psitions
        Vector3[] edgePositions = new Vector3[]
        {
        // bottom-left
        new Vector3(0, 0, 10f), 
        // bottom-right
        new Vector3(Screen.width, 0, 10f), 
        // top-left
        new Vector3(0, Screen.height, 10f), 
        // top-right
        new Vector3(Screen.width, Screen.height, 10f) 
        };

        foreach (var pos in edgePositions)
        {
            // this simulates what update will do
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(pos);

            Vector3 direction = mousePos - tool.transform.position;
            tool.transform.rotation = Quaternion.LookRotation(direction);
            yield return null;

            // Validate rotations 
            Quaternion rot = tool.transform.rotation;
            Assert.IsFalse(float.IsNaN(rot.x) || float.IsNaN(rot.y) || float.IsNaN(rot.z),
                $"Invalid rotation detected at position {pos}");
        }
    }

//this last boundry test tests the Max intensity handling 
//this is simply done by seting unitys max limit for the light

    [UnityTest]
    public IEnumerator Boundary_MaxIntensityHandled()
    {
        tool.flashlight.intensity = float.MaxValue;
        tool.UseTool();
        yield return null;

        Assert.IsTrue(tool.flashlight.enabled);
        Assert.IsTrue(tool.flashlight.intensity > 0);
    }
}
