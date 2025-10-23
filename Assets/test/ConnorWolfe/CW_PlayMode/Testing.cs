using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

/* This script handles testing Scripts made by Connor Wolfe
 * Or Specifically
        - Player Controller
        - QuickAccess Inventory
 * This script tests:
 * ! Boundary tests:
   - Test the bounds of inventory space and checks its behaviour
   - __
 * ! Stress tests:
   - Test the inventory by using Tab() to quickly load and unload object and observe how it affects FPS
 */


public class Testing
{
    // My testing objects
    private QuickAccess _inventory;
    private GameObject _badObj; // this object should never be added to player inventory in this test
                                // might be used in future testing systems, currently a placeholder
                                //    private List<GameObject> _extraObjs; // extra objects to use for testing 
    [UnitySetUp] public IEnumerator Setup()
    {
        SceneManager.LoadScene("CW_Testing Scene");
        yield return null; // give Unity time to actually load the Scene

        _inventory = new QuickAccess(); // inventory with 4 slots
        for (int i = 0; i < 4; i++)
        {
            GameObject temp = new GameObject($"TestingObj{i}");
            _inventory.SetItem(temp);
            _inventory.Tab(); // move to the next slot
        }

        _badObj = new GameObject("BadObject");

        yield return null;
    }

    [UnityTearDown] public IEnumerator Cleanup()
    {
        for (int i = 0; i < 4; i++) // clear inventory objects
        {
            GameObject temp = _inventory.SetItem(null);
            if (temp)
                Object.Destroy(temp);
        }

        if (_badObj)
            Object.Destroy(_badObj);

        /*        foreach (GameObject obj in _extraObjs)
                {
                    if (obj)
                        Object.Destroy(obj);
                }
        */
        yield return null;
    }

    /* Purpose for this test
     * I want to ensure that the inventory properly handles adding Items to invalid out of bounds
     *      inventory slots
     */
    [UnityTest] public IEnumerator BoundaryInventoryBounds()
    {
        if (!_badObj)
            _badObj = new GameObject("BadObject");
        _inventory.SetItem(-1, _badObj);
        // intial arbitrary test under
        Assert.AreNotEqual(_inventory.GetItem(), _badObj, $"-! Arbitrary Test Under\n -> Failure!: {_inventory.GetItem().name} is the bad object {_badObj}"); // checks that the current item is not the bad object
        // arbitrary test over
        _inventory.SetItem(10, _badObj);
        Assert.AreNotEqual(_inventory.GetItem(), _badObj, $"-! Arbitrary Test Under\n -> Failure!: {_inventory.GetItem().name} is the bad object {_badObj}"); // checks that the current item is not the bad object

        // test a bunch of numbers to ensure no failure occurs
        for (int i = 5; i < 20; i++)
        {
            _inventory.SetItem(i, _badObj);
            Assert.AreNotEqual(_inventory.GetItem(), _badObj, $"-! Test {i}\n -> Failure!: {_inventory.GetItem().name} is the bad object {_badObj}"); // checks that the current item is not the bad object
        }

        yield return null;
    }



    /* Purpose for this test
     * The inventory script loads and unloads objects when it does Tab()
     * So I want to stress test it's affect on Game performance
     * This test tabs through items quickly and reports the affect on FPS
     */
    [UnityTest] public IEnumerator StressInventoryLoading()
    {
        int testRuns = 1000;
        float averageFPS = 0;
        for (int i = 0; i < testRuns; i++)
        {
            _inventory.Tab();
            averageFPS += 1.0f / Time.deltaTime;
        }

        averageFPS = averageFPS / testRuns;

        Debug.Log($"- Tab Change Testing:\n -> Average Frames Per Second was \"{averageFPS}\"");
        yield return null;
    }


}