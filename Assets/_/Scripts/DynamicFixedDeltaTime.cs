using System.Collections;
using UnityEngine;

public class DynamicFixedDeltaTime : MonoBehaviour
{
    private float max = 1 / 50f;
    private float min = 1 / 144f;

    private float[] dts = new float[10];
    private int i = 0;

    private Coroutine coroutine = null;

    private void OnEnable()
    {
        coroutine = StartCoroutine(GetFixedDeltaTime());
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    IEnumerator GetFixedDeltaTime()
    {
        while (gameObject.activeSelf)
        {
            i %= dts.Length;
            dts[i++] = Time.deltaTime;

            var t = 0f;
            foreach (var dt in dts) t += dt;
            Time.fixedDeltaTime = Mathf.Clamp(t / dts.Length, min, max);

            yield return new WaitForSecondsRealtime(0.02f);
        }
    }
}
