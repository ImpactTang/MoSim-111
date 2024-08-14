using System.Linq;
using UnityEngine;

public class ChainDetector : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private BoxCollider[] chainColliders;
    private BoxCollider[] _hookColliders;

    public static bool isRedTouchingChain;
    public static bool isBlueTouchingChain;
    
    private void Start()
    {
        var climberTriggers = GameObject.FindGameObjectsWithTag("hookCollider");
        _hookColliders = new BoxCollider[climberTriggers.Length];
        for (var i = 0; i < climberTriggers.Length; i++)
        {
            _hookColliders[i] = climberTriggers[i].GetComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        //Detect whether an alliances hooks are touching their chain
        foreach (var hookCollider in _hookColliders)
        {
            foreach (var chainCollider in chainColliders)
            {
                if (hookCollider.bounds.Intersects(chainCollider.bounds))
                {
                    if (alliance == Alliance.Blue)
                    {
                        isBlueTouchingChain = true;
                    }
                    else if (alliance == Alliance.Red)
                    {
                        isRedTouchingChain = true;
                    }
                    break;
                }
                else 
                {
                    if (alliance == Alliance.Blue)
                    {
                        isBlueTouchingChain = false;
                    }
                    else if (alliance == Alliance.Red)
                    {
                        isRedTouchingChain = false;
                    }
                }
            }

            if ((alliance == Alliance.Blue && isBlueTouchingChain) || (alliance == Alliance.Red && isRedTouchingChain))
            {
                break;
            }
        }
    }
}
