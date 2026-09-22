using System.Collections;
using UnityEngine;

namespace _Project.Scripts
{
    public class CoroutineRunner : MonoBehaviour
    {
        public void Run(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }
    }
}