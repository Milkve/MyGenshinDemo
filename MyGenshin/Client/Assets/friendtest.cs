using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class friendtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddfriendTest()
    {
        FriendManager.Instance.SendAddFriend(2);
    }
}
