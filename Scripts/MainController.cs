// Copyright 2015 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MainController : MonoBehaviour
{
    private GameObject[] items;
    private Vector3[] initialPositions;
    public Camera cam;
    private bool once = false;
    private float leapData;
    public Material[] materials;
    public Renderer rend;
    public float changeInterval = 0.33F;

    void Awake()
    {
        items = GameObject.FindGameObjectsWithTag("Item");
        initialPositions = new Vector3[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            initialPositions[i] = items[i].transform.position;
        }
    }

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (cam != null)
        {
            // Tie this to the camera, and do not keep the local orientation.
            transform.SetParent(cam.GetComponent<Transform>(), true);
        }
        rend = GetComponent<Renderer>();
        rend.enabled = true;

        InvokeRepeating("getLeapData", 1.5f, 0.5f);
    }

    void Update()
    {
        if (materials.Length == 0)
            return;

        // we want this material index now
        int index = Mathf.FloorToInt(Time.time / changeInterval);

        // take a modulo with materials count so that animation repeats
        index = index % materials.Length;

        // assign it to the renderer
        rend.sharedMaterial = materials[index];
    }

    void getLeapData()
    {
        GetHelper get = GameObject.Find("get").GetComponentInChildren<GetHelper>();
        M2XSchema m2xData = get.m2x();
        leapData = m2xData.Value;
        Debug.Log("Got value" + leapData);
    }

    void LateUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit);
        Transform go_focus = hit.transform;
        for (int i = 0; i < items.Length; i++)
        {
            GvrAudioSource src = items[i].GetComponentInChildren<GvrAudioSource>();
            //updown = cam.transform.rotation.x;
            //src.volume = 0.5f - updown;
            
         //   Debug.Log("volume: " + src.volume);

            if (items[i].transform.position.Equals(initialPositions[i]))
                items[i].transform.position = initialPositions[i];
        }
        if (go_focus)
        {
            GvrAudioSource src = go_focus.GetChild(0).GetComponent<GvrAudioSource>();

            if (!once)
            {
                go_focus.position = Vector3.MoveTowards(go_focus.transform.position, cam.transform.position, 2f);
                src.Play();
                //Debug.Log(src.volume);
            }
            if (leapData > 75 && leapData < 400)
            {
                src.volume = leapData / 300 > 1 ? 1 : leapData / 300;
            }
            if (leapData < 75)
                src.volume = 0;

            once = true;
        }
        else
        {

            for (int i = 0; i < items.Length; i++)
            {
                GvrAudioSource src = items[i].GetComponentInChildren<GvrAudioSource>();
                
                items[i].transform.position = initialPositions[i];
            }
            once = false;
        }
    }
}
