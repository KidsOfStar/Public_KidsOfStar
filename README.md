# KidsOfStar
# 🧭 프로젝트 개요

<details>
<summary>클릭해서 열기</summary>

- **프로젝트 이름**: 별의 아이(Kids of Star)
- **개발 기간**: 2025.04.04 ~ 
- **팀 구성**: 강현아(팀장/기획리드), 윤동영(개발리드), 김자은(개발), 김태겸(개발), 김혜지(개발)
- **사용 기술**: Unity, C#, Git
- **장르/타겟 플랫폼**: 2D, 퍼즐, 플랫포머, 어드벤처 / 모바일, PC
- **한 줄 소개**: 퍼즐을 풀고 삶의 목적을 찾는 게임

</details>

---

# 🎮 게임 소개

<details>
<summary>클릭해서 열기</summary>

- **게임 설명**: 플레이어가 형태변화를 이용해 목표 지점으로 나아가는 힐링 게임
- **기획 배경**: 삶에 지친 현대인들에게 위로를 주기 위해 기획
- **주요 기능/콘텐츠**:
    - 퍼즐 : 삶에서 만나는 수많은 장애물을 상징. 플랫포머 퍼즐과 팝업 퍼즐로 나뉘며, 각각 점프 관련 기믹과 회전 퍼즐 기믹으로 이루어져 있음 
    - 형태 변화 : 삶에서 만나는 장애물을 돌파하기 위한 페르소나 상징. 점프력, 충돌체 크기, 패시브 스킬이 다른 다양한 형태로 변화해 플랫포머 퍼즐을 파훼 
    - 스토리 : 누구나 공감할 수 있는 성장 스토리로, 한번뿐인 삶을 어떻게 살아갈 것인지 질문하는 이야기. 

</details>

---

# ✨ 주요 특징 및 기술 구현

<details>
<summary>클릭해서 열기</summary>

### 🔧 기술 스택
- Unity 버전: 2022.3.17f1
- 주요 플러그인 / 외부 라이브러리: 
    - Cinemachine: 카메라 제어
    - TextMeshPro: 텍스트 렌더링
    - Unity Google Sheets: 구글 시트 연동
    - Ingame Debug Console: 디버깅 툴
    - Text Animator: 텍스트 애니메이션 툴

### 🧠 주요 기능 구현
<details>
<summary>🔧 이미지 퍼즐 시스템 (윤동영)</summary>

### 📝 기능 설명
회전 퍼즐 게임은 다채로운 컨텐츠를 위해서 만든 이미지 맞추기 형태의 미니게임

### ⚙️ 핵심 구현 포인트
“평탄화(flattened)”된 1차원 리스트를 인덱스 계산으로 2차원처럼 활용한 형태로
스크립터블 오브젝트를 활용한 데이터를 통해서 퍼즐의 이미지를 확장성 있게 구성하고,
퍼즐 조각을 프리펩화하여 그리드(1차원 리스트) 형태 + 그리드 너비(gridWidth)를 이용한 구조

</details>

<details>
<summary>🔧 패럴렉스 스크롤링 (윤동영)</summary>

### 📝 기능 설명
2D 또는 2.5D 게임에서 깊이감 있는 배경 연출을 위해 카메라의 이동에 따라 배경이 서로 다른 속도로 움직이는 패럴렉스 스크롤링 효과를 구현
카메라의 움직임에 따라 각 배경의 레이어가 Z값에 따라 배경 텍스처 오프셋을 다르게 적용함으로써 원근감 있는 화면을 연출

### ⚙️ 핵심 구현 포인트
- `Initialized()`메서드를 통해 카메라의 위치를 주입받아 시작 위치를 기록
- 자식 오브젝트들을 자동으로 배경으로 인식하고 각 배경의 Material을 저장
- `BackSpeedCalculate()`를 통해서 각 배경 오브젝트의 카메라로부터 z거리 기준으로 상대속도를 계산
- `FixedUpdate()`에서 카메라 이동 거리 기반으로 배경 텍스쳐의 Offset을 조정하여 스크롤 효과를 구현

### 🧩 구조 및 연동
- 여러 배경 레이어(예: 바다,산호초,물고기 등)를 빈 오브젝트에 자식 오브젝트로 부착.
- 외부에서 `Initialized()` 호출에 따라 카메라 참조를 연결해야 정상적으로 작동
- ParallaxSpeed 값을 통해서 전체의 속도 비율을 조정 가능

</details>

<details>
<summary>🔧 나뭇잎 트램펄린 기믹 (윤동영)</summary>

### 📝 기능 설명
나뭇잎 점프(Leaf Jump)는 게임 내에서 일종의 트램펄린 역할을 하는 오브젝트

### ⚙️ 핵심 구현 포인트
- 충돌감지를 통해 ILeafJumpable 인터페이스를 구현한다면 점프동작을 위임하고, 인터페이스 구현 객체에서 실제 물리 처리를 수행하는 구조
- 인터페이스의 분리 덕분에, 플레이어·박스·기타 오브젝트 모두 같은 메서드만 구현하면 잎사귀 점프에 연동 가능

</details>

<details>
<summary>🔧 상자 옮기기 기믹 (윤동영)</summary>

### 📝 기능 설명
플레이어의 형태변화에 따라 달라지는 힘과 박스라는 오브젝트를 통해 만든 플레이어 기믹 중 한 요소

### ⚙️ 핵심 구현 포인트
- 플레이어의 Form형태에 따라서 형태 별 PushPower에 따라 박스를 미는 힘이 달리지는 구조
- IWeightable의 구현을 통해 boxWeight를 반환하고 RigidBody를 통해 실제 물리를 적용
- 박스가 플레이어와 부딪히며 튕기는 현상을 방지하기 위해,  코루틴 기반 충돌 무시로 부자연스러운 반발 제거

</details>

<details>
<summary>🔧 컷씬 시스템 (김자은)</summary>

### 📝 기능 설명
스토리의 일부를 컷신 형태로 구성하여, 애니메이션과 연출을 통해 **스토리 몰입감을 높이고 게임의 서사를 풍부하게 전달**

### ⚙️ 핵심 구현 포인트
- Unity Timeline을 활용해 다양한 연출을 자연스럽게 구성
- Signal Asset을 사용하여 타임라인 내부에서 **대사 출력, Bgm 전환, 컷신 종료** 등 이벤트를 정확한 타이밍에 실행
- 기능 단위 컴포넌트(DialogPlayer, BgmPlayer 등)를 타임라인과 연동하여 **타이밍 기반 제어** 수행

### 🧩 구조 및 연동
- 각 컷신 프리팹은 공통 기반 클래스 `CutSceneBase`를 가지고 있으며, 자체 Timeline(PlayableDirector)을 실행하는 구조
- 컷신 프리팹은 필요한 기능만 선택적으로 추가하는 **조립식 구조**로 구성
- 전체 컷신의 생성 및 실행 흐름은 `CutSceneManager`가 담당하며, 씬 내 컷신의 시작과 종료를 제어

</details>

<details>
<summary>🔧 대사 시스템 (김자은)</summary>

### 📝 기능 설명
컷씬 진행 중이거나 플레이어의 자유 상호작용 중에 **대사를 출력하고, 그에 따른 게임 내 액션을 트리거하는 시스템**  
스토리 흐름과 상호작용을 자연스럽게 연결해주는 핵심 역할을 함

### ⚙️ 핵심 구현 포인트
- **Google Sheets 기반 데이터 테이블**을 Unity에 연동하여 대사 내용을 관리
- 각 대사는 `index`를 기준으로 불러오며, `nextIndex` 값을 통해 자동으로 다음 대사로 이어짐
- `ActionType` 필드를 통해 **대사 직후에 발생할 이벤트(선택지 표시, 컷씬 재생, 진행도 갱신 등)를 정의**

### 🧩 구조 및 연동
- 대사의 시작/종료, 분기 로직 관리는 `DialogueManager`에서 담당
- 출력은 `UITextBubble`이 담당하며, 대사 출력 자체에만 집중
- 각 대사 액션은 `IDialogActionHandler` 인터페이스를 구현한 클래스로 분리되어 있으며,  
  `Dictionary<DialogActionType, IDialogActionHandler>`를 통해 타입에 따라 동적으로 실행

</details>

<details>
<summary>🔧 엘리베이터 기믹 (김자은)</summary>

### 📝 기능 설명
- 지정 방향으로 왕복 이동하는 플랫폼
- 탑승 객체의 무게 합산 → `MaxWeight` 초과 시 경고·고장·자동 복구

### ⚙️ 핵심 구현 포인트
- 목표 위치 계산: `Direction` + `distance` (`GetTargetPosition()`)
- 이동 로직: `MoveRoutine` → `MoveWaitRoutine` → 반복
- 과부하 체크: `GetCurrentWeight()` → `BreakSequence()` 실행
- 탑승 판별: `IsOnElevator()` → `OnCollision`에서 부모 설정

### 🧩 구조 및 연동
- **Elevator**: `Collider2D`, `SpriteRenderer`
- **Managers**: SFX 재생, 경고 대사 호출
- **IWeightable**: `GetWeight()` 구현체

</details>

<details>
<summary>🔧 저장 시스템 (김자은)</summary>

### 📝 기능 설명
게임 진행 정보를 **JSON 파일** 형태로 저장하고 불러오는 시스템
- 여러 슬롯(index) 관리로 Save, Load, 에디터용 Delete, DeleteAll 기능 제공
- 저장 시점에 **인터넷 시간**을 조회하여 파일명에 반영

### ⚙️ 핵심 구현 포인트
- **저장 경로 확보:** `Application.persistentDataPath` + 슬롯 인덱스 기반 파일명(`SaveData{index}.json`)
- **직렬화/역직렬화:** `JsonUtility.ToJson`/`JsonUtility.FromJson`
- **비동기 인터넷 시간 조회:** `UnityWebRequest.Head("https://www.google.com")`로 응답 헤더의 data활용
- **파일 I/O:** `File.WriteAllText`,`File.ReadAllText`,`File.Delete`,`Directory.GetFiles`

### 🧩 구조 및 연동
**SaveManager**
- `Save(int index, Action<string> onComplete)`: 데이터 수집 → 인터넷 시간 조회 → JSON 저장
- `Load(int index)`:파일 존재 여부 검사 → JSON 파싱 → `SaveData.LoadData()`호출

**SaveData**
- `InitData()`: `GameManager`에서 게임 상태(난이도,장면,챕터,플레이어 위치,해금 폼 등) 수집
- `LoadData()`: 수집된 데이터를 `GameManager`에 적용
- `FetchInternetTime(Action)`: 서버 시간 조회 후 `saveName` 생성

**Managers.Instance.GameManager:** 저장 대상 데이터 제공 및 로드 데이터 반영

</details>

<details>
<summary>🔧 BGM Layered Fader (김자은)</summary>

### 📝 기능 설명
- 대사 진행에 따라 BGM 트랙을 순차적으로 쌓아 재생
- 메인 루프·이펙트 트랙 DSP 스케줄링 및 페이드 인/아웃
- 최종 선택 시 Rise 이펙트 재생 및 기존 루프 중단

### ⚙️ 핵심 구현 포인트
- **트랙 매핑**: `mainSources`/`loopEffects` + `MainBgmSourceType` → `audioDict`, `audioByIndexDict` 초기화
- **DSP 스케줄링**: `ScheduleLoop()` & `LoopScheduler()` 코루틴 → `AudioSettings.dspTime` 기반 `PlayScheduled` 호출
- **이벤트 처리**:
    - `DialogueManager.OnDialogStepStart/End` → `PlaySource()`로 레이어 재생
    - `UISelectionPanel.OnFinalSelect` → `OnPlayRiseEffect()`에서 루프 중단 및 Rise 이펙트
- **볼륨 관리**: `SoundManager.SetBgmVolumeCallback` 구독 → `SetVolume()`에서 메인·루프 볼륨 조정

### 🧩 구조 및 연동
- **BgmLayeredFader**: `AudioSource[]`, 코루틴, 이벤트 콜백
- **Managers**:
    - `SoundManager`: BGM/앰비언스 제어, 볼륨 콜백
    - `DialogueManager`: 대사 단계 이벤트 제공
    - `UIManager` → `UISelectionPanel`: 최종 선택 이벤트
- **코루틴 & 페이드**: `LoopScheduler`, `FadeInAudio`, `FadeOutAudio`

</details>

<details>
<summary>🔧 UI 구조 (김태겸)</summary>

### 📝 기능 설명
게임 내 모든 UI는 Canvas 기반으로 구성되며, UI를 **기본 UI / 팝업 / 최상위 알림**의 세 계층으로 구분하여 관리
이를 통해 사용자에게 지속적으로 보여져야 할 정보와, 특정 이벤트에 의한 임시 인터페이스를 **명확하게 분리**할 수 있음

- **UI**: 조이스틱, 타이머, 점수 등 항상 표시되는 기본 UI
- **Popup**: 설정창, 일시정지, 결과창 등 이벤트 기반 인터페이스
- **Top**: 경고창, 시스템 오류 등 최우선 처리 인터페이스

### ⚙️ 핵심 구현 포인트
- UIManager에서 **Canvas 계층을 관리**하며, 각 계층에 맞는 UI Prefab을 동적으로 생성 및 제거
- Popup 계층은 중첩 표시를 고려한 **레이어 구조**로 설계 (예: 스택 또는 큐 구조)
- 최상위 알림 UI는 항상 다른 팝업 위에 표시되도록 **Sorting Order** 또는 **Transform 계층 구조**를 명확히 분리

### 🧩 구조 및 연동
- UIManager는 각 UI 계층별 Transform을 미리 참조하고 있으며, 필요 시 해당 위치에 프리팹을 Instantiate하여 붙이는 방식으로 동작
- 각 UI는 **인터페이스나 공통 베이스 클래스**를 상속하여 열기/닫기 등의 동작을 통일
- 게임의 상태 변화(예: 게임 오버, 일시정지)는 UIManager를 통해 해당 UI를 요청하는 방식으로 일관성 있게 처리됨

### 📥사용된 주요 컴포넌트
- UIManager: UI 계층 구조(UI / Popup / Top) 관리 및 UI 요소 동적 생성

</details>

<details>
<summary>🔧 타임어택 미니게임 시스템 (김태겸)</summary>
    
### 📝 기능 설명 
타임어택 미니게임은 제한 시간 없이 플레이어가 가능한 한 빠르게 맵을 클리어하고, 그 기록을 바탕으로 NPC의 반응이 달라지는 콘텐츠
NPC와의 대화가 모두 끝나면 타임어택 맵이 화면에 표시되고, 5초간의 준비 타이머가 진행
준비 시간 이후 스톱워치가 자동으로 시작되며, 플레이어가 목표 지점에 도달하면 기록 측정이 종료
기록에 따라 각 NPC의 대사가 달라지며, UI를 통해 해당 결과가 출력

### ⚙️ 핵심 구현 포인트
- 대화 종료 후 흐름 전환
마지막 NPC 대화가 끝나면 자동으로 타임어택 모드로 전환
- 5초 준비 타이머 구현
 CountdownTimer 클래스를 통해 5초 동안 준비 시간을 카운트하고 UI에 실시간으로 표시
 타이머 종료 시, Stopwatch가 자동으로 시작
- 기록 측정 기능
 Stopwatch 클래스를 사용하여 플레이어가 출발한 시점부터 도착 지점까지의 시간을 측정됨
- 기록 기반 대사 출력
 ScriptableObject를 활용해 기록 범위별로 각 NPC의 대사를 정의하고, 조건에 따라 적절한 대사를 출력됨.
  - 예시 – 1분 30초 이하 기록일 경우:
  - 지김: "시간은 나쁘지 않다"
  - 세명: "대단해...!"
- UIManager를 통한 UI 제어
 UI 요소(타이머, 결과 팝업, 대사 등)는 모두 UIManager를 통해 생성 및 제어

### 🧩 구조 및 연동
NPC 대화 종료 > 
[UIManager] 맵 및 기본 UI 표시 >
[CountdownTimer] 5초 준비 타이머 시작 >
[Stopwatch] 스톱워치 시작 >
플레이어 조작 & 목표 도달 >
[Stopwatch] 기록 정지 >
[ScriptableObject] 기록에 따라 결과 대사 선택 >
[UIManager] 결과 팝업 및 종료 UI 표시

### 📥사용된 주요 컴포넌트
- CountdownTimer / Stopwatch: 준비 시간 및 기록 측정을 담당
- ScriptableObject: 기록 범위에 따른 NPC 대사 데이터 정의 및 참조 

</details>

<details>
<summary>🔧 금고 퍼즐 (김태겸)</summary>

### 📝 기능 설명
- 금고 퍼즐은 회전 이미지 퍼즐과 회전 다이얼을 맞추는 방식의 미니게임

### ⚙️ 핵심 구현 포인트
- 회전 이미지 퍼즐 풀어 회전 다이얼 퍼즐을 해금하고 맞추는 방식
- 다이얼 수, 정답 조합, 이미지 등의 데이터를 `ScriptableObject`로 분리하여 유연하게 관리
- 챕터 5의 각 씬(501, 502, 504 등)에 따라 퍼즐 이미지나 조합이 달라지도록 구성하여 다양성을 확보

### 🧩 구조 및 연동
- `TreePuzzleData(ScriptableObject)`를 통해 각 퍼즐 구성 정보를 담고 있음
- 챕터 5의 각 씬에서 동일한 퍼즐 UI 프리팹(`SafePopup`)을 사용하면서도, 씬 번호에 따라 서로 다른 `TreePuzzleData`를 로드하여 퍼즐을 

</details>

<details>
<summary>🔧 거미줄 기믹 (김태겸)</summary>

### 📝 기능 설명
- 거미줄은 플레이어와 충돌 시 이동과 점프에 제약을 주는 오브젝트

### ⚙️ 핵심 구현 포인트
- 플레이어와 충돌했을 때 이동과 점프를 제한 처리
- 특정 형태(Dog)를 통해서 거미줄을 1초 뒤 파괴
- 다른 형태일 때 플레이어가 1.5초 머무르면, 안내 팝업이 뜨도록 구현

### 🧩 구조 및 연동
- `PlayerController`의 이동속도와 점프파워를 받아와 제한
- `PlayerFormController`의 플레이어의 현재 폼에 따른 상호작용

</details>

<details>
<summary>🔧 플레이어 조작 (김혜지)</summary>
    
### 📝 기능 설명
입력 시스템을 통해 이동 및 점프 동작을 처리하며, 상태 패턴(State Pattern)을 통해 Idle, Move, Jump, WallCling, WallJump 등으로 분기

### ⚙️ 핵심 구현 포인트
상태 패턴 기반 동작 제어
- 플레이어의 상태(Idle, Move, Jump, WallCling 등)를 클래스로 분리하여 유연한 상태 전환 가능
- 각 상태가 애니메이터 파라미터를 직접 조작하여 애니메이션 연동 안정성 확보
- `Player State Factory`에서 상태 클래스를 생성 및 관리  

입력 처리 통합
- Unity Input System 기반으로 키보드/조이스틱을 모두 지원
- 입력값은 MoveDir을 통해 방향성 유지, 점프·변신 등은 이벤트 트리거 방식으로 분리 처리

</details>

<details>
<summary>🔧 형태변화 시스템 (김혜지)</summary>

### 📝 기능 설명    
ScriptableObject 기반의 형태 데이터(SO)를 활용하여 각 형태의 스탯(이동 속도, 점프력, 무게 등)을 관리하고, 변신 시 플레이어 외형과 능력치를 동적으로 교체

### ⚙️ 핵심 구현 포인트
ScriptableObject 기반 형태 데이터 관리
- 각 형태별 능력치(이동 속도, 점프력, 무게 등)와 스프라이트, 애니메이션 클립을 PlayerFormData로 관리
- 변신 시 해당 데이터를 PlayerFormController가 교체하고 외형·애니메이션 적용

AnimatorOverrideController 활용
- Animator 상태 이름을 유지하면서 형태별 애니메이션 클립만 동적으로 교체
- Animator 파라미터 해시값은 PlayerAnimHash 클래스에서 일괄 관리하여 코드 가독성과 유지보수성 강화

연출 및 제어 흐름 구성
- 변신 시 SpriteRenderer를 비활성화하고 변신 이펙트를 재생한 뒤, 폼 데이터를 교체하고 렌더러 복구
- 조작은 IsControllable 값을 통해 변신 연출 동안 제한됨

</details>

<details>
<summary>🔧 엘레베이터 배선 퍼즐 (김혜지)</summary>

### 📝 기능 설명
제한 시간 내  퍼즐 조각을 회전시켜 각 열의 배선 색상을 올바르게 정렬하는 미니게임
퍼즐은 플레이어가 인간 형태일때만 진입할 수 있으며, 성공 시 엘레베이터가 작동

### ⚙️ 핵심 구현 포인트
- UI 그리드 레이아웃을 이용해 퍼즐 조각을 정렬하고, 중심 나사 위치를 클릭해 2x2 영역을 시계방향으로 회전시키는 방식
- 퍼즐 진입 조건, 제한 시간 타이머, 정답 판정, 클리어시 엘레베이터 해제 등 전체 흐름을 통합적으로 제어

</details>

</details>

---
# 🧩 아키텍처 / 구조

<details>
<summary>클릭해서 열기</summary>

- **씬 구성 및 흐름**
- **TitleScene**: 타이틀 및 시작 버튼
- **LoadingScene**: 씬 전환 시 로딩 화면
- **Chapter (1~5)**: 주요 플레이 구간, 각 스테이지는 별도 씬으로 관리

씬 전환은 `SceneLoader`에서 비동기(AsyncOperation)로 처리하며,  
중간에 `LoadingScene`을 Additive로 로드해 전환 연출을 담당

씬 별 초기화는 `SceneBase` 클래스를 통해 구성되어 있으며,  
씬이 활성화되면 매니저 및 UI 세팅, 데이터 바인딩 등을 일괄 처리

- **매니저 구조**: 

  Managers 클래스는 싱글턴 기반으로 설계되어 있으며, 게임 내 모든 시스템 매니저(`GameManager`, `SoundManager`, `UIManager` 등)를 통합 관리  
  대부분의 매니저는 `MonoBehaviour`를 상속받지 않고 `new`로 생성되며, 명시적인 `Init()` 호출 없이 생성자에서 필요한 초기화를 수행 
  이를 통해 Unity 생명주기 순서에 의존하지 않고, **초기화 순서를 정확히 제어**할 수 있음

  이러한 구조는 **테스트 용이성**, **상태 일관성 유지**, **유지보수 편의성**을 목표로 설계됨

- **데이터 관리**: Unity Google Sheet

</details>

---

# 🤝 협업 방식

<details>
<summary>클릭해서 열기</summary>

- **Git 전략**: 메인 브랜치, 개인 브랜치, Dev 브랜치로 분류. 개인 브랜치에서 작업한 뒤 Dev 브랜치로 머지하고, 모든 개발이 완료되면 데브에서 메인 브랜치로 머지. 
- **이슈/업무 관리**: Notion 
- **커밋 컨벤션**

| 태그     | 설명 |
|----------|------|
| **Feat**     | 새로운 기능 추가 |
| **Fix**      | 버그 수정 또는 오타(Typo) 수정 |
| **Refactor**| 리팩토링 (기능 변화 없이 구조 개선) |
| **Design**   | CSS 등 사용자 UI 디자인 변경 |
| **Comment**  | 주석 추가 및 변경 |
| **Style**    | 코드 포맷팅, 세미콜론 누락 등 (논리 변경 없음) |
| **Test**     | 테스트 코드 추가, 수정, 삭제 (비즈니스 로직 영향 없음) |
| **Chore**    | 기타 잡다한 변경 (빌드 스크립트, 이미지, 패키지 등) |
| **Init**     | 프로젝트 초기 생성 |
| **Rename**   | 파일 혹은 폴더 이름 수정 또는 이동 |
| **Remove**   | 파일 삭제 작업 |
| **Add**      | 코드, 테스트, 예제, 문서 등의 신규 추가 |
| **Improve**  | 성능/호환성/접근성 등 향상된 변경 |
| **Move**     | 코드 위치 이동 |
| **Updated**  | 버전, 계정, 문서, 라이브러리 등 업데이트 |
| **WIP**      | 작업 중인 내용을 임시로 커밋 (Work In Progress) |

- **회의 방식**: 평일 오전 9시 30분, 오후 8시 데일리 스크럼 진행, 매주 금요일 오후 데일리 스크럼은 위클리 스크럼으로 진행.  

</details>

---

# 🎬 시연 영상

<details>
<summary>클릭해서 열기</summary>

- 클릭 시 이동합니다.

[![별의 아이](https://velog.velcdn.com/images/ehddud9608/post/e6e64350-9b2c-4ed4-ac53-e00bb82a6b3e/image.gif)](https://www.youtube.com/watch?v=_dIIvxihAGY)

</details>

---

# 📦 실행 방법

<details>
<summary>클릭해서 열기</summary>

- Unity 버전:2022.3.17f1
- 실행 전 주의사항 
컴퓨터로 실행할 경우 WASD(이동), 스페이스바(점프), 숫자키(1, 2, 3, 4)(변신)으로 플레이 가능함.
모바일로 실행할 경우 왼쪽 하단의 조이스틱, 오른쪽 하단의 버튼(점프, 변신)으로 플레이 가능함.               
- 빌드 방법: WebGL(itch.io,Unity)
- 플레이 링크 
  - https://beautifulmaple.itch.io/kids-of-star
  - https://play.unity.com/ko/games/1afc6210-f107-41f6-9a27-32a348721b26/kids-of-star

</details>

---

# 📄 라이선스 / 참고 자료

<details>
<summary>클릭해서 열기</summary>

**🖼️ 배경**
- 챕터1 배경이미지: https://free-game-assets.itch.io/free-underwater-world-pixel-art-backgrounds
- 챕터2 배경이미지: https://theflavare.itch.io/mondstadt-theme-background-pixel-art
- 챕터3 배경이미지: https://edermunizz.itch.io/free-pixel-art-hill
- 챕터4 배경이미지: https://free-game-assets.itch.io/free-city-backgrounds-pixel-art
- 일부 배경 및 엔딩이미지: https://imgex.ai/ko

**🎨 아트**
- 로딩 UI 아이콘:https://opengameart.org/content/pixel-art-loading-icon-2
- 하니 캐릭터 스프라이트: https://opengameart.org/content/cat-sprites
- 지김 캐릭터 스프라이트: https://opengameart.org/content/husky-sprites

**✨ 이펙트**
- 변신 및 전선 이펙트트: https://bdragon1727.itch.io/750-effect-and-fx-pixel-all
- 저장NPC 스프라이트: https://nyknck.itch.io/staranimation

**🎤 사운드**
- 일부 챕터 SFX: https://shapeforms.itch.io/shapeforms-audio-free-sfx
- 일부 챕터 BGM: https://soundraw.io

</details>
