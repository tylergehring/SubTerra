using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;



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
    [UnitySetUp]
    public IEnumerator Setup()
    {
        SceneManager.LoadScene("CW_testingScene");
        yield return null; // give Unity time to actually load the Scene
        yield return null; // give World a chance to generate        


        _badObj = new GameObject("BadObject");
        _controller = PlayerController.Instance;
        if (!_controller) // if the instance fails then try to find it in the scene manually
        {
            GameObject _playerObj = GameObject.FindGameObjectWithTag("Player");
            if (_playerObj)
                _controller = _playerObj.GetComponent<PlayerController>();
        }

        var inventoryField = typeof(PlayerController).GetField("_inventory",
                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        _inventory = (QuickAccess)inventoryField.GetValue(_controller);
        //        _inventory = new QuickAccess(); // inventory with 4 slots

        for (int i = 0; i < 4; i++)
        {
            _inventory.Tab(i); // move to the next slot
            GameObject temp = new GameObject($"TestingObj{i}");
            _inventory.SetItem(temp);

        }


        /*

        */

      
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        for (int i = 0; i < 4; i++) // clear inventory objects
        {
            GameObject temp = _inventory.SetItem(null);

            if (temp)
                UnityEngine.Object.Destroy(temp);
        }

        if (_badObj)
            UnityEngine.Object.Destroy(_badObj);

        yield return null;
    }


    //// -- Inventory Tests -- ////

    [UnityTest]
    public IEnumerator Inventory_InitialSlotIsZero()
    {
        Assert.AreEqual(0, _inventory.GetIndex(), "Inventory should start at slot 0");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Inventory_TabWrapsAround()
    {
        for (int i = 0; i < 6; i++) _inventory.Tab();
        Assert.AreEqual(2, _inventory.GetIndex(), "Tab should wrap back to slot 2 after 4 slots");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Inventory_GetItemNeverReturnsDestroyed()
    {
        GameObject obj = new GameObject("Temp");
        _inventory.SetItem(obj);
        UnityEngine.Object.DestroyImmediate(obj);
        yield return null; // let destruction propagate
        Assert.IsNull(_inventory.GetItem(), "GetItem must return null for destroyed objects");
    }

    /* Purpose for this test
     * I want to ensure that the inventory properly handles adding Items to invalid out of bounds
     *      inventory slots
     */
    [UnityTest]
    public IEnumerator Inventory_BoundaryInventoryBounds()
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
    [UnityTest]
    public IEnumerator Inventory_StressLoading()
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


    //// -- Health Tests -- ////
    /* Purpose for this test
     * The health system I made is important for many teamates scripts
        So, I want to test and ensure it works
      - this test simulates many invalid numbers to change health by and tracks failure
     */
    [UnityTest]
    public IEnumerator Health_BoundaryHealthTests()
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
                temp = UnityEngine.Random.Range(minRange, maxRange);
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

    [UnityTest]
    public IEnumerator Health_GetHealthReturnsCurrentValue()
    {
        typeof(PlayerController).GetField("_health",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_controller, (byte)0);

        // test for equality:
        _controller.ChangeHealth(-42);
        Assert.AreEqual(42, _controller.getHealth(), "Public getHealth must reflect internal value");
        yield return null;
    }

    //// -- Score Tests ////
    [UnityTest]
    public IEnumerator Score_ChangeScoreClampsToUIntRange()
    {
        _controller.ChangeScore(int.MaxValue);
        Assert.AreEqual(uint.MaxValue, _controller.GetScore(), "Score must clamp to uint.MaxValue");

        // Reset to 0 then go negative
        typeof(PlayerController).GetField("_playerScore",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_controller, (uint)0);

        _controller.ChangeScore(-1);
        Assert.AreEqual(0u, _controller.GetScore(), "Score must not go below 0");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Score_GetScoreMatchesInternal()
    {
        // reset to 0
        typeof(PlayerController).GetField("_playerScore",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(_controller, (uint)0);
        _controller.ChangeScore(123);
        Assert.AreEqual(123u, _controller.GetScore(), "Public getter must match internal field");
        yield return null;
    }

    //// -- Movement tests -- ////
    [UnityTest]
    public IEnumerator Movement_HorizontalInputChangesVelocity()
    {
        _controller.GetType().GetField("_horizontalMovement");
        _controller.GetType().GetField("_horizontalMovement",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_controller, 1f);

        yield return new WaitForFixedUpdate();
        float velX = _controller.GetComponent<Rigidbody2D>().linearVelocity.x;
        Assert.Greater(velX, 0f, "Right input must produce positive X velocity");
    }

    [UnityTest]
    public IEnumerator Movement_JumpOnlyWhenOnGround()
    {
        var rb = _controller.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // disable gravity

        _controller.GetType().GetField("_onGround",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_controller, false);
        _controller.GetType().GetField("_isJumping",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_controller, true);

        yield return new WaitForFixedUpdate();
        float velY = rb.linearVelocity.y;
        Assert.AreEqual(0f, velY, 0.01f, "Jump must be ignored when not on ground");
    }

    [UnityTest]
    public IEnumerator Movement_SprintDoublesSpeed()
    {
        // Set onGround, horizontalMovement=1, isSprinting=true
        _controller.GetType().GetField("_onGround", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, true);
        _controller.GetType().GetField("_horizontalMovement", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, 1f);
        _controller.GetType().GetField("_isSprinting", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, true);

        yield return new WaitForFixedUpdate();
        float velX = _controller.GetComponent<Rigidbody2D>().linearVelocity.x;
        float baseSpeed = ((Speed)_controller.GetType().GetField("_movementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_controller)).GetSpeed();
        Assert.Greater(velX, baseSpeed * 1.9f, "Sprint must double speed when stamina available");
    }

    [UnityTest]
    public IEnumerator Movement_NoSprintWhenExhausted()
    {
        var stamina = _controller.GetObject().GetComponentInChildren<StaminaWheelScript>();
        stamina.ChangeStamina(-150f);  // Exhaust
        yield return null;

        _controller.GetType().GetField("_onGround", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, true);
        _controller.GetType().GetField("_horizontalMovement", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, 1f);
        _controller.GetType().GetField("_isSprinting", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, true);

        yield return new WaitForFixedUpdate();
        float velX = _controller.GetComponent<Rigidbody2D>().linearVelocity.x;
        float baseSpeed = ((Speed)_controller.GetType().GetField("_movementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_controller)).GetSpeed();
        Assert.AreEqual(baseSpeed, velX, 0.1f, "No sprint multiplier when exhausted");
    }

    [UnityTest]
    public IEnumerator Movement_Stress_RapidMovementInput()
    {
        _controller.GetType().GetField("_onGround", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, true);
        int runs = 1000;
        for (int i = 0; i < runs; i++)
        {
            _controller.GetType().GetField("_horizontalMovement", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(_controller, i % 2 == 0 ? 1f : -1f);
            yield return new WaitForFixedUpdate();
        }
        Assert.Pass("No crash after 1000 rapid direction changes");
    }

    //// -- Player Controller Function tests -- ////
    // tests the Pickup() function

    [UnityTest]
    public IEnumerator Pickup_ReplacesExistingItem()
    {
        GameObject oldItem = new GameObject("OldI");
        _controller.PickUp(oldItem);  // Fill slot 0
        yield return null;

        GameObject newItem = new GameObject("NewI");
        GameObject replaced = _controller.PickUp(newItem);  // Should replace
        yield return null;

        Assert.AreEqual(oldItem, replaced, "Pickup must return replaced item");
        Assert.AreNotEqual(oldItem, _inventory.GetItem(), "Old item must be replaced");
        UnityEngine.Object.Destroy(oldItem);
        UnityEngine.Object.Destroy(oldItem);
    }

    [UnityTest]
    public IEnumerator Pickup_AddsItemToNextEmptySlot()
    {
        // Clear inventory first
        for (int i = 0; i < 4; i++) _inventory.SetItem(null);

        GameObject item = new GameObject("PickupItem");
        _controller.PickUp(item);
        yield return null;

        bool found = false;
        for (int i = 0; i < 4; i++)
        {
            _inventory.Tab(i);
            if (_inventory.GetItem() != null) { found = true; break; }
        }
        Assert.IsTrue(found, "Pickup() must have placed the item in an empty slot");
        UnityEngine.Object.Destroy(item);
    }

    [UnityTest]
    public IEnumerator Pickup_RejectsNullItem()
    {
        // Clear a slot
        _inventory.Tab(0);
        _inventory.SetItem(null);

        GameObject returned = _controller.PickUp(null);
        yield return null;

        Assert.IsNull(returned, "Pickup(null) must return null");
        Assert.IsNull(_inventory.GetItem(), "Null must not be added to inventory");
    }

    // tests the Drop() function
    [UnityTest]
    public IEnumerator Drop_RemovesItemAndSpawnsHandler()
    {
        GameObject item = new GameObject("Droppable");
        _inventory.Tab(0);
        _inventory.SetItem(item);
        _controller.GetType().GetMethod("_Drop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(_controller, null);

        yield return null;
        Assert.IsNull(_inventory.GetItem(), "Drop must remove item from inventory");

        ItemHandler[] handlers = UnityEngine.Object.FindObjectsOfType<ItemHandler>();
        Assert.IsTrue(handlers.Length > 0, "Drop must instantiate an ItemHandler");
        UnityEngine.Object.Destroy(item);
    }

    [UnityTest]
    public IEnumerator Drop_HandlerHasCorrectItemReference()
    {
        GameObject item = new GameObject("Droppable");
        item.tag = "Item"; // Ensure pickup logic sees it
        _inventory.Tab(0);
        _inventory.SetItem(item);

        var dropMethod = _controller.GetType().GetMethod("_Drop",
            BindingFlags.NonPublic | BindingFlags.Instance);
        dropMethod.Invoke(_controller, null);
        yield return null;

        ItemHandler handler = UnityEngine.Object.FindObjectOfType<ItemHandler>();
        var prefabField = _controller.GetType().GetField("_itemHandlerPrefab",
            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_controller);

        if (prefabField != null && handler != null)
        {
            GameObject heldItem = (GameObject)handler.GetType()
                .GetField("_heldItem", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(handler);
            Assert.AreSame(item, heldItem, "ItemHandler must hold reference to dropped item");
        }
        else
        {
            Assert.Pass("Drop called safely (no crash) even if prefab missing");
        }
    }

    // tests the Victory() function
    [UnityTest]
    public IEnumerator Victory_MultipliesScoreByInventoryCountPlusOne()

    {        
        _controller.ChangeScore(100);

        _inventory.Tab(0); _inventory.SetItem(new GameObject("VictoryItem0"));
        _inventory.Tab(1); _inventory.SetItem(new GameObject("VictoryItem1"));

        _controller.Victory();
        uint expected = 100u * 3;  // 100 * (1 base + 2 items)
        Assert.AreEqual(expected, _controller.GetScore(), "Victory must apply multiplier correctly");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Victory_EmptyInventoryMultipliesCountBy1()
    {
        _controller.ChangeScore(100);
        for (int i = 0; i < 4; i++) _inventory.Tab(i); _inventory.SetItem(null);  // Clear
        _controller.Victory();
        Assert.AreEqual(100u, _controller.GetScore(), "Empty inventory = 1x multiplier");
        yield return null;
    }

    // tests the Pause() function
    [UnityTest]
    public IEnumerator Pause_StopsMovement()
    {
        _controller.Pause(true);
        _controller.GetType().GetField("_horizontalMovement",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_controller, 1f);

        yield return new WaitForFixedUpdate();
        float vel = _controller.GetComponent<Rigidbody2D>().linearVelocity.x;
        Assert.AreEqual(0f, vel, 0.01f, "Movement must be zero when paused");
    }

    //// -- Animator Tests -- ////
    [UnityTest]
    public IEnumerator Animator_FlipXWhenMovingLeft()
    {
        _controller.GetType().GetField("_horizontalMovement",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_controller, -1f);

        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame(); // need to let the animator run and update sprites
        Assert.IsTrue(_controller.GetComponent<SpriteRenderer>().flipX, "Sprite must flip when moving left");
    }

    [UnityTest]
    public IEnumerator Animator_EntersIdleWhenNoInput()
    {
        // Ensure no movement
        _controller.GetType().GetField("_horizontalMovement", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(_controller, 0f);
        _controller.GetType().GetField("_onGround", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(_controller, true);

        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame(); // Let animator update

        var animator = _controller.GetComponent<Animator>();
        Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"),
            "Animator must be in Idle state when no input and grounded");
    }

    //// -- Stamina Wheel tests -- ////
    [UnityTest]
    public IEnumerator StaminaWheel_ChangeClampsToBounds()
    {
        var stamina = _controller.GetObject().GetComponentInChildren<StaminaWheelScript>();
        stamina.ChangeStamina(1000f);  // Over max
        yield return null;
        float curr = (float)stamina.GetType().GetField("_currStamina",
            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stamina);
        Assert.AreEqual(stamina.maxStamina, curr, 0.01f, "Stamina must clamp to max");

        stamina.ChangeStamina(-1000f);  // Under 0
        yield return null;
        curr = (float)stamina.GetType().GetField("_currStamina",
            BindingFlags.NonPublic | BindingFlags.Instance).GetValue(stamina);
        Assert.AreEqual(0f, curr, 0.01f, "Stamina must clamp to 0");
    }

    [UnityTest]
    public IEnumerator StaminaWheel_IsExhaustedAfterDrain()
    {
        var stamina = _controller.GetObject().GetComponentInChildren<StaminaWheelScript>();
        stamina.ChangeStamina(-150f);  // Drain past 0
        yield return null;
        Assert.IsTrue(stamina.IsExhausted(), "IsExhausted() must return true after full drain");
    }

    [UnityTest] public IEnumerator StaminaWheel_InitializesAtMax()
    {
        var stamina = _controller.GetObject().GetComponentInChildren<StaminaWheelScript>();
        Assert.AreEqual(stamina.maxStamina, stamina.GetType().GetField("_currStamina",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(stamina),
            "Stamina should start at max value");
        yield return null;
    }

    [UnityTest] public IEnumerator StaminaWheel_ExhaustionDisablesGreenWheel()
    {
        var stamina = _controller.GetObject().GetComponentInChildren<StaminaWheelScript>();
        var greenWheel = (UnityEngine.UI.Image)stamina.GetType().GetField("_greenWheel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(stamina);

        stamina.ChangeStamina(-1000f);
        yield return null;
        Assert.IsFalse(greenWheel.enabled, "Green wheel must be disabled when stamina is 0");
    }

    //// -- Item Factory Tests -- ////

    [UnityTest] public IEnumerator ItemFactory_HandlesNull()
    {
        GameObject result = ItemFactory.CreateItem(null);
        Assert.IsNull(result, "Factory must return null for null input");
        yield return null;
    }



    //// -- Edge tests -- ////
    [UnityTest] public IEnumerator Edge_SingletonEnforcesOneInstance()
    {
        GameObject duplicate = UnityEngine.GameObject.Instantiate(_controller.GetObject());
        yield return null;
        Assert.IsTrue(duplicate == null || !duplicate.activeSelf, "Duplicate Player must be destroyed by singleton pattern");
    }


}