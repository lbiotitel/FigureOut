using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{


    // t는 기다릴 시간 action은 그 후 에 실행할 메서드 입니다.
    public static async void Delay(float t, Action action)
    {
        await UniTask.WaitForSeconds(t);
        action?.Invoke();
    }



    //플레이어가 상호작용하는 지 확인하고 메서드를 위임하여 실행합니다.
    public static void CheckPlayerInteract(Collider other, IInteractable interactableObject, Action<PlayerUnit> action)
    {
        if (other.CompareTag(KeyDefines.TagKey.PLAYER))
        {
            if (other.TryGetComponent<PlayerUnit>(out PlayerUnit playerUnit))
            {
                if (playerUnit.StateMachine.CurrentState != playerUnit.DashState)
                {
                    action?.Invoke(playerUnit);
                }
            }
        }
    }


    //재귀적으로 자식을 다 찾아 특정한 오브젝트의 위치를 찾습니다.
    public static Transform Find(string name, Transform transform)
    {
        if (name == transform.name)
        {
            return transform;
        }

        //자식을 재귀로 다 찾아봄
        Transform find = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            find = Find(name, child);
            if (find != null) return find;
        }

        return null;
    }


    //ammotype 과 itemId 매핑 
    public class AmmoTypeToItemId
    {
        private static Dictionary<EAmmoType, string> dict = new Dictionary<EAmmoType, string>()
        {
            { EAmmoType.Ammo9mm, "10020_AM_001" },
            { EAmmoType.Ammo762, "10020_AM_002" },
            { EAmmoType.Ammo556, "10020_AM_003" },
            { EAmmoType.Gaze12, "10020_AM_004" }
        };

        public static string GetItemId(EAmmoType ammoType)
        {
            if (dict.ContainsKey(ammoType))
            {
                return dict[ammoType];
            }
            else
            {
                Debug.LogError($"AmmoType {ammoType} not found in dictionary.");
                return null;
            }
        }
    }




    //클래스 타입에 매핑된 한글 이름을 반환하는 클래스 입니다.
    public class ClassTypeToKor
    {
        private static Dictionary<EClassType, string> dict = new Dictionary<EClassType, string>()
    {
        { EClassType.Security, "보안관" },
        { EClassType.Hunter, "헌터" },
        { EClassType.Observer, "감시자" },
        { EClassType.Farmer, "농부" },
        { EClassType.Fighter, "싸움꾼" }
    };

        public static string GetKor(EClassType classType)
        {
            if (dict.TryGetValue(classType, out string kor))
                return kor;
            else
                return "알 수 없는 클래스";
        }
    }

    public class ElementTypeToKor
    {
        private static Dictionary<EElementType, string> dict = new Dictionary<EElementType, string>()
        {
            {EElementType.Ignite, "화상" },
            {EElementType.Freeze, "빙결" },
            {EElementType.Irradiation, "쇠약" },
            {EElementType.Corrosion, "부식" },
            {EElementType.Marked, "표식" },
            {EElementType.Injury, "부상" }
        };

        public static string GetKor(EElementType elementType)
        {
            if (dict.TryGetValue(elementType, out string kor))
                return kor;
            else
                return "알 수 없는 원소";
        }
    }


    public class WayPoint
    {
        private static Dictionary<string, Vector3> wayPointDict = new Dictionary<string, Vector3>()
        {
            { "부랑아들의은신처", new Vector3(10.51f,0,186.81f) },
            { "우울한무기창고", new Vector3(186f, 0.4f, 133f) },
            { "호화로웠던마을", new Vector3(120,0,103) },
            { "황금향", new Vector3(233,0,12) },
        };

        public static Vector3 GetWayPointPos(string wayPointName)
        {
            if (wayPointDict.TryGetValue(wayPointName, out Vector3 pos))
                return pos;
            else
                return Vector3.zero;
        }
    }
}
