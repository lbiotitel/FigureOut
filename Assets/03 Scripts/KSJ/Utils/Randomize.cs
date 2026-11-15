using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize
{
    // maxCount 안에서 겹치지 않는 n개의 난수 생성
    public static int[] RandomNumbers(int maxCount, int n)
    {
        // 0 ~ maxCount까지 순서대로 저장하는 배열
        int[] defaults = new int[maxCount];
        // 결과값을 저장하는 배열
        int[] results = new int[n];

        // 배열 전체에 0부터 maxCount의 값을 순서대로 저장한다.
        for(int i = 0; i < maxCount; ++i)
        {
            defaults[i] = i;
        }

        // 필요한 n개의 난수를 생성한다
        for(int i = 0; i < n; ++i)
        {
            int index = Random.Range(0, maxCount);

            results[i] = defaults[index];
            defaults[index] = defaults[maxCount - 1];

            maxCount--;
        }

        return results;
    }
}
