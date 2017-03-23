using UnityEngine;
using System.Collections;

public class AddAnimationClip : MonoBehaviour
{
    public Animation anim;
    void Start()
    {
        //Animator animator = GetComponent<Animator>();
        anim = GetComponent<Animation>();
        anim.AddClip(anim.clip, "shoot", 0, 10);
        anim.AddClip(anim.clip, "walk", 11, 20, true);
        anim.AddClip(anim.clip, "idle", 21, 30, true);
    }
}
