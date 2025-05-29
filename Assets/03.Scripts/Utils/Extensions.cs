using System;

public static class Extensions
{
    public static string GetName(this BgmSoundType bgm)
    {
        return bgm switch
        {
            BgmSoundType.Night          => "Night",
            BgmSoundType.Maorum         => "Maorum",
            BgmSoundType.MaorumChase    => "MaorumChase",
            BgmSoundType.InForest       => "InForest",
            BgmSoundType.InForestPuzzle => "InForestPuzzle",
            BgmSoundType.WithDogs       => "WithDogs",
            BgmSoundType.WithDogsRun    => "WithDogs_Run",
            BgmSoundType.City           => "City",
            BgmSoundType.CityPuzzle     => "CityPuzzle",
            BgmSoundType.Aquarium       => "Aquarium",
            BgmSoundType.Intro          => "Intro",
            _                           => throw new ArgumentOutOfRangeException(nameof(bgm), bgm, null)
        };
    }

    public static string GetName(this AmbienceSoundType ambience)
    {
        return ambience switch
        {
            AmbienceSoundType.UnderWater => "UnderWater",
            AmbienceSoundType.ForestBird => "ForestBird",
            AmbienceSoundType.Wind       => "Wind",
            AmbienceSoundType.City       => "City",
            AmbienceSoundType.Aquarium   => "Aquarium",
            _                            => throw new ArgumentOutOfRangeException(nameof(ambience), ambience, null)
        };
    }

    public static string GetName(this SfxSoundType sfx)
    {
        return sfx switch
        {
            SfxSoundType.ButtonPush      => "ButtonPush",
            SfxSoundType.Communication   => "Communication",
            SfxSoundType.ElevatorMove    => "ElevatorMove",
            SfxSoundType.FormChange      => "FormChange",
            SfxSoundType.JumpField       => "JumpField",
            SfxSoundType.JumpFloor       => "JumpFloor",
            SfxSoundType.JumpWater       => "JumpWater",
            SfxSoundType.JumpWood        => "JumpWood",
            SfxSoundType.LeafTrampoline  => "LeafTrampoline",
            SfxSoundType.PuzzleClear     => "PuzzleClear",
            SfxSoundType.PuzzleFail      => "PuzzleFail",
            SfxSoundType.RunField        => "RunField",
            SfxSoundType.TurnPuzzle      => "TurnPuzzle",
            SfxSoundType.UIButton        => "UIButton",
            SfxSoundType.UICancel        => "UICancel",
            SfxSoundType.WalkFloor       => "WalkFloor",
            SfxSoundType.WalkForest      => "WalkForest",
            SfxSoundType.WalkWater       => "WalkWater",
            SfxSoundType.WalkWood        => "WalkWood",
            SfxSoundType.Walla           => "Walla",
            SfxSoundType.WallBreak       => "WallBreak",
            SfxSoundType.Thunder         => "Thunder",
            SfxSoundType.ImportantChoice => "ImportantChoice",
            SfxSoundType.BrokenElevator  => "BrokenElevator",
            SfxSoundType.Beep            => "Beep",
            _                           => throw new ArgumentOutOfRangeException(nameof(sfx), sfx, null)
        };
    }

    public static string GetName(this FootstepType footstepType)
    {
        return string.Empty;
    }

    public static string GetName(this SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.Title          => "TitleScene",
            SceneType.Loading        => "LoadingScene",
            SceneType.Chapter1       => "Chapter_1",
            SceneType.Chapter1Puzzle => "Chapter_103",
            SceneType.Chapter2       => "Chapter_2",
            SceneType.Chapter3       => "Chapter_3",
            SceneType.Chapter4       => "Chapter_4",
            SceneType.Chapter501     => "Chapter_501",
            SceneType.Chapter502     => "Chapter_502",
            SceneType.Chapter503     => "Chapter_503",
            SceneType.Chapter504     => "Chapter_504",
            _                        => throw new ArgumentOutOfRangeException(nameof(sceneType), sceneType, null)
        };
    }

    public static string GetName(this ChapterType chapterType)
    {
        return chapterType switch
        {
            ChapterType.Chapter1 => "Chapter1",
            ChapterType.Chapter2 => "Chapter2",
            ChapterType.Chapter3 => "Chapter3",
            ChapterType.Chapter4 => "Chapter4",
            ChapterType.Chapter5 => "Chapter5",
            _                     => throw new ArgumentOutOfRangeException(nameof(chapterType), chapterType, null)
        };
    }

    public static string GetName(this EndingType endingType)
    {
        return endingType switch
        {
            EndingType.ComfortableLife  => "안락한 일상",
            EndingType.WinRecognition   => "쟁취한 인정",
            EndingType.DreamingCat      => "낭만 고양이",
            EndingType.IntoTheOcean     => "드넓은 바다로",
            EndingType.DifferentButSame => "같지만 다르게",
            EndingType.Absorb           => "흡수",
            EndingType.Stable           => "안정",
            EndingType.Obedience        => "복종",
            EndingType.Adaptation       => "적응",
            EndingType.Mistake          => "실수",
            EndingType.Detection        => "발각",
            _                           => throw new ArgumentOutOfRangeException(nameof(endingType), endingType, null)
        };
    }

    public static string GetName(this CutSceneType cutSceneType)
    {
        return cutSceneType switch
        {
            CutSceneType.FallingDown     => "FallingDown",
            CutSceneType.Rescued         => "Rescued",
            CutSceneType.LeavingForest   => "LeavingForest",
            CutSceneType.DaunRoom        => "DaunRoom",
            CutSceneType.JigimOrder      => "JigimOrder",
            CutSceneType.FieldNormalLife => "FieldNormalLife",
            CutSceneType.SemyungGoOut    => "SemyungGoOut",
            CutSceneType.RescueKitten    => "RescueKitten",
            CutSceneType.DogFormChange   => "DogFormChange",
            CutSceneType.JieuiRequest    => "JieuiRequest",
            CutSceneType.MeetingWomen    => "MeetingWomen",
            CutSceneType.HaniRequest     => "HaniRequest",
            CutSceneType.TheCatReturns   => "TheCatReturns",
            CutSceneType.FinalChoice     => "FinalChoice",
            CutSceneType.ComebackForest  => "ComebackForest",
            CutSceneType.ComebackField   => "ComebackField",
            CutSceneType.ComebackCity    => "ComebackCity",
            CutSceneType.MeetingBihyi    => "MeetingBihyi",
            _                            => throw new ArgumentOutOfRangeException(nameof(cutSceneType), cutSceneType, null)
        };
    }

    public static string GetName(this Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => "Easy",
            Difficulty.Hard => "Hard",
            _               => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };
    }
}