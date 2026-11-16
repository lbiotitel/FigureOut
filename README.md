# Figure Out

장비를 루팅하면서 적과 총격전을 벌이는 생존 액션 게임입니다.

| 항목 | 내용 |
|---|---|
| 인원 | 4인 팀 프로젝트 |
| 개발 기간 | 2026.05 ~ 2026.06 |
| 플랫폼 | PC |
| 사용 기술 | Unity, C#, Addressable, UniTask, ScriptableObject |
| 플레이 영상 | (링크 추가 예정) |

## 담당 파트

제가 작성한 코드는 [`Assets/03 Scripts/Biotite`](Assets/03%20Scripts/Biotite)에 있습니다.

| 파트 | 주요 파일 |
|---|---|
| 매니저 구조, 부트스트랩 | `Managers/Managers.cs`, `BootStrapper.cs`, `IManager.cs` |
| 씬 전환 | `Managers/SceneController.cs` |
| 데이터 관리 (Addressable) | `Managers/DataTableManager.cs`, `ResourceLoader.cs` |
| 이벤트 매니저 | `Managers/EventManager.cs`, `EventDatas.cs` |
| 퀘스트 | `Managers/QuestManager.cs`, `Core/Quest/*`, `Datas/Quests/*` |
| 인벤토리 | `Core/Inventory/*` |
| 장비 (무기, 방어구) | `Core/Equipment/*`, `Core/System/EquipmentController/*` |
| 기타 | 오브젝트 풀링, 아이템 효과, 장비 진화 레시피, 캐릭터 커스터마이즈 |

## 주요 구현

### 부트스트랩 씬과 Additive 씬 전환

- 매니저들은 BootStrap 씬에 두고, 게임 씬은 Additive로 올렸다 내리는 구조입니다. DontDestroyOnLoad를 쓰지 않아서 매니저 해제 시점을 따로 신경 쓰지 않아도 됩니다.
- `BootStrapper`가 `RuntimeInitializeOnLoadMethod`로 실행돼서, 에디터에서 어떤 씬을 바로 실행해도 BootStrap 씬과 리소스를 먼저 불러옵니다. 팀원이 자기 씬에서 바로 테스트할 수 있습니다.
- 씬 전환은 빌더 패턴으로 만들었습니다. 호출하는 쪽에서 필요한 단계만 이어 붙입니다.

```csharp
Managers.Scene
    .NewTransition()
    .Load(EScene.CharacterSelect.ToString())
    .LoadResources(EScene.CharacterSelect.ToString())
    .WithOverlay()
    .Perform();


```
실행 순서는 페이드 인 → 씬 언로드 → 리소스 언로드와 로드 → 씬 로드 → 페이드 아웃입니다.

### Addressable 기반 데이터 관리

이전 프로젝트에서 쓰던 Resources 로드를 Addressable로 바꿨습니다.

- 씬 이름을 라벨로 써서, 씬에 필요한 ScriptableObject, 프리팹, 오디오를 한 번에 불러옵니다.
- 불러온 데이터는 타입별 딕셔너리에 저장하고 `GetData<T>(key)`로 꺼내 씁니다.
- ScriptableObject는 복사본을 만들어 저장해서, 런타임에 값을 바꿔도 원본 에셋이 바뀌지 않습니다.
- 로드한 핸들은 딕셔너리로 관리하고, 씬을 바꿀 때 `Clear()`에서 모두 Release합니다.
- UniTask로 모든 리소스가 로드될 때까지 기다려서, 로드 전에 데이터에 접근하지 않게 했습니다.

### 박싱 없는 이벤트 매니저

- 이전 프로젝트의 이벤트는 매개변수가 object라서 박싱이 생기고 어떤 데이터가 오는지 알기 어려웠습니다.
- `IEventData` 제네릭 제약을 걸고 이벤트 데이터를 구조체로 만들어서 박싱을 없앴습니다.
- 이벤트 타입을 키로 델리게이트를 저장하기 때문에, 구독하는 쪽에서 타입이 바로 보입니다.

### 데이터 기반 퀘스트

- 퀘스트는 `QuestDataBase`(ScriptableObject)로 만들고, 처치 퀘스트와 재료 수집 퀘스트 두 종류를 지원합니다.
- 다음 퀘스트 ID 목록을 데이터에 넣어서, 퀘스트를 완료하면 연계 퀘스트가 자동으로 등록됩니다.
- 진행 상황은 이벤트로 받습니다. 처치 퀘스트는 수락할 때 적 사망 이벤트를 구독하고, 보상을 받으면 구독을 해제합니다.

팀원은 퀘스트 데이터를 만들어 Addressable에 등록하기만 하면 코드 수정 없이 새 퀘스트를 추가할 수 있습니다.

### 인벤토리 (데이터와 UI 분리)

- 아이템 데이터는 `InventorySystem`이, 화면 표시는 `InventoryUIBase`를 상속한 UI가 맡습니다. 데이터가 바뀌면 이벤트로 알려서 바뀐 슬롯만 다시 그립니다.
- 슬롯이 미리 배치된 UI는 `StaticInventoryUI`, 슬롯 개수만큼 프리팹을 만드는 UI는 `DynamicInventoryUI`로 나눴습니다. 플레이어 인벤토리, 창고, 장비 진화 슬롯이 같은 인벤토리 로직을 함께 씁니다.
- 같은 아이템은 스택에 쌓고, 정렬하면 슬롯 전체를 다시 연결합니다.

### 아이템 효과와 자동 설명

- 소모품 효과는 `ItemEffect`를 상속한 클래스(즉시 효과, 지속 효과)를 `[SerializeReference]` 리스트로 조합합니다.
- 효과마다 `BuildDescription()`을 구현해서, 툴팁 설명이 효과 데이터에서 자동으로 만들어집니다. 수치를 바꿔도 설명을 따로 고칠 필요가 없습니다.

### 장비와 팩토리

- 무기와 방어구는 `IEquipmentFactory`를 구현한 팩토리에서 데이터로 생성합니다.
- 같은 무기 데이터라도 플레이어용, 적용 무기 컴포넌트를 따로 붙여서 하나의 데이터를 양쪽에서 씁니다.
- 장착하면 장비 스탯을 수정자로 추가하고, 해제하면 그 장비가 준 수정자만 제거합니다.
- 방어구 내구도는 피격 시 데미지에 비례해 줄고, 바뀐 값을 이벤트로 UI에 알립니다.
- 총기 반동은 DOTween으로 무기 위치와 회전을 튕겨서 표현했습니다.

### 투사체와 오브젝트 풀링

- 투사체는 `IPoolingObject`를 구현해서 풀에서 꺼내 쓰고, 수명이 끝나거나 맞으면 풀로 돌아갑니다.
- 이동 방식은 `MotionBase`를 상속한 클래스로 분리해서, 투사체 데이터의 모션 타입에 따라 붙입니다. 지금은 직선 이동이 있고, 다른 궤적도 같은 방식으로 추가할 수 있습니다.

### 상호작용 오브젝트

- 프린터, 창고, 보물상자 같은 오브젝트는 `IInteractable`을 구현합니다.
- 플레이어가 범위에 들어오면 이벤트로 상호작용 안내 문구를 띄우고, 키를 누르면 `Interact()`를 호출합니다. UI는 이벤트만 받기 때문에 오브젝트 종류를 몰라도 됩니다.

## 저장소 안내

- 팀 프로젝트라 다른 팀원의 코드도 함께 있습니다. 제 코드는 위의 `Biotite` 폴더입니다.
- 에셋 스토어 리소스가 포함되어 있습니다.
