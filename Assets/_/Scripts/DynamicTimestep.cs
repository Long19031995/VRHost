using System.Collections;
using UnityEngine;

public class DynamicTimestep : MonoBehaviour
{
    private float slowestTimestep = 1 / 50f;
    private float fastestTimestep = 1 / 144f;

    private float[] deltaTimes = new float[10];
    private int index = 0;

    private Coroutine coroutine = null;

    private void OnEnable()
    {
        coroutine = StartCoroutine(GetTimestep());
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    IEnumerator GetTimestep()
    {
        while (gameObject.activeSelf)
        {
            deltaTimes[index++] = Time.deltaTime;
            index %= deltaTimes.Length;
            Time.fixedDeltaTime = Mathf.Clamp(GetAverage(), fastestTimestep, slowestTimestep);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    public float GetAverage()
    {
        var average = 0f;
        foreach (var deltaTime in deltaTimes) average += deltaTime;
        return deltaTimes.Length > 0 ? average / deltaTimes.Length : 0;
    }
}
