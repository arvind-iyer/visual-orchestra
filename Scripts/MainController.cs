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
    private GetHelper get;
    public Camera cam;
    private bool once = false;
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
        get = GameObject.Find("get").GetComponentInChildren<GetHelper>();
        M2XSchema res = get.m2x();

        Debug.Log("This is res " + res.ToString());

    }

    void LateUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit);
        Transform go_focus = hit.transform;
        for (int i = 0; i < items.Length; i++)
        {
            GvrAudioSource src = items[i].GetComponentInChildren<GvrAudioSource>();
            float updown = cam.transform.rotation.x;
            src.volume = 0.5f - updown;
            //Debug.Log(0.5 - updown);
            if (items[i].transform.position.Equals(initialPositions[i]))
                items[i].transform.position = initialPositions[i];
        }
        if (go_focus)
        {
            if (!once)
            {
                go_focus.position = Vector3.MoveTowards(go_focus.transform.position, cam.transform.position, 2f);
                GvrAudioSource src = go_focus.GetChild(0).GetComponent<GvrAudioSource>();
                src.Play();
                //Debug.Log(src.volume);
            }

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
