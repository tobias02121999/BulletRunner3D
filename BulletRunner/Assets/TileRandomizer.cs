using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizer : MonoBehaviour
{
    // Initialize the public variables
    public Material[] variations;

    // Start is called before the first frame update
    void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        var rand = Mathf.RoundToInt(Random.Range(0f, variations.Length - 1f));
        var target = variations[rand];
        renderer.material = target;
    }
}
