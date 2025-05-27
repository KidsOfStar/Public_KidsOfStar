using GoogleSheet.Core.Type;
using System;

#region Player

[Flags]
public enum PlayerFormType
{
    Stone    = 1 << 0, // 000001
    Human    = 1 << 1, // 000010
    Squirrel = 1 << 2, // 000100
    Dog      = 1 << 3, // 001000
    Cat      = 1 << 4, // 010000
    Hide     = 1 << 5, // 100000
}

#endregion

#region Dialogue

[UGS(typeof(CharacterType))]
public enum CharacterType
{
    Maorum,
    WaterPlant1,
    WaterPlant2,
    Daun,
    Dongdo,
    Jigim,
    Semyung,
    Hanyung,
    Ea,
    Hani,
    Jieui,
    Bihyi,
    Stone,
    Songsari,
    Dolmengee,
    People1,
    People2,
    People3,
    People4,
    Women,
    Save,
}

[UGS(typeof(DialogActionType))]
public enum DialogActionType
{
    None,
    ShowSelect,
    HighlightSelect,
    DataSave,
    FinalSelect,
    ModifyTrust,
    PlayCutScene,
    LoadScene,
    UpdateProgress,
    ChangeForm,
    GoToEnding,
}

[UGS(typeof(DialogActionType))]
public enum CustomActionType
{
    GoToEnding,
    MoveTo,
    PlayCutScene,
    Return,
}

public enum DialogCustomMethodType
{
    ChangeHumanForm,
}

#endregion

#region Sound

public enum BgmSoundType
{
    Maorum,
    MaorumChase,
    InForest,
    InForestPuzzle,
    WithDogs,
    WithDogsRun,
    City,
    CityPuzzle,
    Aquarium,
}

public enum AmbienceSoundType
{
    UnderWater,
    ForestBird,
    Wind,
    City,
    Aquarium,
}

public enum SfxSoundType
{
    ButtonPush,
    Communication,
    ElevatorMove,
    FormChange,
    JumpField,
    JumpFloor,
    JumpWater,
    JumpWood,
    LeafTrampoline,
    PuzzleClear,
    PuzzleFail,
    RunField,
    TurnPuzzle,
    UIButton,
    UICancel,
    WalkFloor,
    WalkForest,
    WalkWater,
    WalkWood,
    Walla,
    WallBreak,
	Thunder,
    ImportantChoice,
    BrokenElevator,
    Beep,
}

public enum FootstepType
{
}

#endregion

#region Stage

public enum SceneType
{
    Title,
    Loading,
    Chapter1,
    Chapter1Puzzle,
    Chapter2,
    Chapter3,
    Chapter4,
    Chapter501,
    Chapter502,
    Chapter503,
    Chapter504,
}

public enum ChapterType
{
    Chapter1,
    Chapter2,
    Chapter3,
    Chapter4,
    Chapter5,
}

public enum Difficulty
{
    Easy,
    Hard,
}

public enum InteractionType
{
    StartGame,
    EndGame
}

#endregion

#region CutScene

[Flags]
public enum EndingType
{
    None = 0,
    
    //기본 엔딩
    Absorb     = 1 << 0,
    Stable     = 1 << 1,
    Obedience  = 1 << 2,
    Adaptation = 1 << 3,
    Mistake    = 1 << 4,
    Detection  = 1 << 5,

    // 중요 엔딩
    ComfortableLife  = 1 << 6,
    WinRecognition   = 1 << 7,
    DreamingCat      = 1 << 8,
    IntoTheOcean    = 1 << 9,
    DifferentButSame = 1 << 10,
}

public enum CutSceneType
{
    FallingDown,
    Rescued,
    DaunRoom,
    LeavingForest,
    DogFormChange,
    FieldNormalLife,
    SemyungGoOut,
    JigimOrder,
    RescueKitten,
    JieuiRequest,
    MeetingWomen,
    MeetingBihyi,
    HaniRequest,
    TheCatReturns,
    FinalChoice,
    ComebackForest,
    ComebackField,
    ComebackCity,
}

#endregion

#region Setting

public enum UIPosition
{
    BG, // 배경 UI
    UI,         // 기본 씬 UI
    Popup,      // 팝업 창
    Top,        // 에러 창 
}

public enum ObstacleType
{
    SmallSeaweed,
    MediumSeaweed,
    LargeSeaweed,
    Stone
}

public enum ReferenceMode //레터박스 화면비율 선정 Mode
{
    DesignedAspectRatio,
    OrginalResolution
};

public enum WarningType
{
    None,
    Squirrel,
    BoxMissing,
    BoxFalling,
    BackMove,
    Coweb,
    Ladder,
}

#endregion