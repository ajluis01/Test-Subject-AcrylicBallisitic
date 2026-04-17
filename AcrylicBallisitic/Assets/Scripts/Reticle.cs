using Unity.VisualScripting;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField] Movement player;
    [SerializeField] float MinSize = .2f;

    float size;
    // Update is called once per frame
    void Update()
    {
        Vector3 reticlePos = SceneCamera.cursorPos;

        size = player.MultiShotPenalty * player.penaltyLevel * 2;

        Vector3 sizeVector;
        if (size >  MinSize)
        {
            sizeVector = new Vector3(size, 0.01f, size);
        }
        else
        {
            sizeVector = new Vector3(MinSize, 0.01f, MinSize);
        }
        transform.localScale = sizeVector;


        transform.position = reticlePos;
    }
}
