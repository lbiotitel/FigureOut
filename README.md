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
