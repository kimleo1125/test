using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RhythmEditor : MonoBehaviour
{
    public AudioSource music;
    public float BPM = 120f;  // 이 값은 참고용

    public KeyCode[] inputKeys = new KeyCode[4] { KeyCode.S, KeyCode.D, KeyCode.K, KeyCode.L };

    public List<BeatNote> beatNotes = new List<BeatNote>();

    void Update()
    {
        // 음악 재생 / 일시정지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (music.isPlaying)
                music.Pause();
            else
                music.Play();
        }

        // 키 입력 감지 → 음악 재생 시간 기준으로 정확한 시간 저장
        for (int i = 0; i < inputKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
            {
                float inputTime = music.time;  // 입력 순간의 정확한 시간
                beatNotes.Add(new BeatNote(inputTime, i));
                Debug.Log($"🎵 Note 입력: Time {inputTime:F3}s, Line {i}");
            }
        }

        // 저장
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveToJson("mySong");
        }

        // 불러오기
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadFromJson("mySong");
        }
    }

    void SaveToJson(string filename)
    {
        string json = JsonUtility.ToJson(new BeatNoteListWrapper { beatNotes = beatNotes }, true);
        string path = Path.Combine(Application.dataPath, $"{filename}.json");
        File.WriteAllText(path, json);
        Debug.Log($"💾 저장 완료: {path}");
    }

    void LoadFromJson(string filename)
    {
        string path = Path.Combine(Application.dataPath, $"{filename}.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            beatNotes = JsonUtility.FromJson<BeatNoteListWrapper>(json).beatNotes;
            Debug.Log($"📥 불러오기 완료: {beatNotes.Count}개 노트");
        }
        else
        {
            Debug.LogWarning("❗️ 파일을 찾을 수 없습니다: " + path);
        }
    }

    [System.Serializable]
    public class BeatNoteListWrapper
    {
        public List<BeatNote> beatNotes;
    }
}

// BeatNote 클래스 수정: beat 대신 time 저장
[System.Serializable]
public class BeatNote
{
    public float time;  // 노트가 입력된 정확한 시간(초)
    public int line;    // 노트 라인 번호

    public BeatNote(float time, int line)
    {
        this.time = time;
        this.line = line;
    }
}
