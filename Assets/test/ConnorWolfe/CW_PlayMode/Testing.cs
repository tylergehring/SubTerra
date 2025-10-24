using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System;
using System.Collections;
using System.Collections.Generic;


/* This script handles testing Scripts made by Connor Wolfe
 * Or Specifically
        - Player Controller
        - QuickAccess Inventory
 * This script tests:
 * ! Boundary tests:
   - Test the bounds of inventory space and checks its behaviour
   - Test health at variety of numbers
 * ! Stress tests:
   - Test the inventory by using Tab() to quickly load and unload object and observe how it affects FPS
 */


public class Testing
{
    // My testing objects
    private PlayerController _controller;
    private QuickAccess _inventory;
    private GameObject _badObj; // this object should never be added to player inventory in this test
                                // might be used in future testing systems, currently a placeholder
                                //    private List<GameObject> _extraObjs; // extra objects to use for testing 
    [UnitySetUp] public IEnumerator Setup()
    {
        SceneManager.LoadScene("MVP");
        yield return null; // give Unity time to actually load the Scene

        _inventory = new QuickAccess(); // inventory with 4 slots
        for (int i = 0; i < 4; i++)
        {
            GameObject temp = new GameObject($"TestingObj{i}");
            _inventory.SetItem(temp);
            _inventory.Tab(); // move to the next slot
        }

        _badObj = new GameObject("BadObject");

        GameObject _playerObj = GameObject.FindGameObjectWithTag("Player");
        if (_playerObj)
            _controller = _playerObj.GetComponent<PlayerController>();

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
     * The health system I made is important for many teamates scripts
        So, I want to test and ensure it works
      - this test simulates many invalid numbers to change health by and tracks failure
     */
    [UnityTest]
    public IEnumerator BoundaryHealthTests()
    {
        int testRuns = 100;
        // complicated, but essientially its how to read private data in functions
        // this way we can track how health changes
        var healthField = typeof(PlayerController).GetField("_health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(healthField, "_health was not found in PlayerController script");

        byte currentHealth = 0;

        // generates values likely to be beyond 
        int minRange = -10000;
        int maxRange = 10000;
        int temp;

        byte minExpected = 0;
        byte maxExpected = 255;

        for (int i = 0; i < 100; i++)
        {
            temp = 0;
            while (temp >= 0 && temp <= 255) // we only want out of bounds values
                temp = Random.Range(minRange, maxRange);
            _controller.ChangeHealth(temp);
            currentHealth = (byte)healthField.GetValue(_controller); // how to actually get the value from the private var
            if (temp < minExpected)
                Assert.AreEqual(currentHealth, minExpected, "-! Health Test Failure!:\n" +
                $" -> health should be {minExpected}\n" +
                $" -> health is instead {currentHealth}");
            else if (temp > maxExpected)
                Assert.AreEqual(currentHealth, maxExpected, "-! Health Test Failure!:\n" +
                $" -> health should be {maxExpected}\n" +
                $" -> health is instead {currentHealth}");
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
        int testRuns = 10000;
        float totalTime = 0.0f;
        int framesProcessed = 0;
        float minFPS = 0.0f;
        float maxFPS = 0.0f;
        float averageFPS = 0;

        var breakPoint = (brokeAt: 0, fps: 0f);
        bool broke = false;

        for (int i = 0; i < testRuns; i++)
        {

            _inventory.Tab();
            yield return null; // give unity a moment render, and updated deltatime
            float deltaTime = Time.deltaTime;
            totalTime += deltaTime;

            float currentFps = 1f / deltaTime;
            minFPS = minFPS == 0 ? currentFps : minFPS;
            minFPS = currentFps < minFPS ? currentFps : minFPS;
            maxFPS = currentFps > maxFPS ? currentFps : maxFPS;

            framesProcessed++;

            if (!broke && currentFps <= 40)
                breakPoint = (i, currentFps);
            // I don't report inside as a debug log here might impact the stress test
        }

        if (broke)
            Debug.LogWarning($"-! Tab Change Stress Test (BreakPoint)\n - broke at run {breakPoint.brokeAt} reaching an fps of {breakPoint.fps}");

        averageFPS = framesProcessed / totalTime;
        Debug.Log($"-! Tab Change Stress Test ({testRuns} tabs over {totalTime}s):\n" +
                  $" -> Average FPS: {averageFPS:F2}\n" +
                  $" -> Min FPS: {minFPS:F2}\n" +
                  $" -> Max FPS: {maxFPS:F2}");
    }


}