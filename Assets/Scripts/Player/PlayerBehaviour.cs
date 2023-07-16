using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public bool isPlayer1 = true;
    public Animator animator;
    void Start() {
        animator = GetComponent<Animator>();
    }
    
    void Update() {
    }
}
