using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour {
    public static BGMManager Instance { get; private set; }

    [Header("Audio Sources (Bisa Dikosongkan)")]
    public AudioSource sourcePatroli; 
    public AudioSource sourceTegang;  

    [Header("BGM Clips")]
    public AudioClip musikPatroli;
    public AudioClip musikTegang;

    [Header("Settings")]
    public float fadeSpeed = 2f; 
    
    private float targetVolPatroli = 1f;
    private float targetVolTegang = 0f;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        } else {
            Destroy(gameObject); 
            return; 
        }
    }

    void Start() {
        // KODE OTOMATIS: Membuat & mencari Audio Source di objek ini
        AudioSource[] sources = GetComponents<AudioSource>();
        
        // Jika belum ada komponen Audio Source sama sekali di Inspector, kita buatkan otomatis
        if (sources.Length < 2) {
            sourcePatroli = gameObject.AddComponent<AudioSource>();
            sourceTegang = gameObject.AddComponent<AudioSource>();
            
            sourcePatroli.loop = true;
            sourceTegang.loop = true;
        } else {
            sourcePatroli = sources[0];
            sourceTegang = sources[1];
        }

        if (sourcePatroli != null && !sourcePatroli.isPlaying && !sourceTegang.isPlaying) {
            MulaiMainkanMusik();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void MulaiMainkanMusik() {
        if (sourcePatroli != null && musikPatroli != null) {
            sourcePatroli.clip = musikPatroli;
            sourcePatroli.volume = 1f;
            sourcePatroli.Play();
        }
        if (sourceTegang != null && musikTegang != null) {
            sourceTegang.clip = musikTegang;
            sourceTegang.volume = 0f;
            sourceTegang.Play(); 
        }
    }

    void Update() {
        if (sourcePatroli != null && sourceTegang != null) {
            sourcePatroli.volume = Mathf.MoveTowards(sourcePatroli.volume, targetVolPatroli, fadeSpeed * Time.deltaTime);
            sourceTegang.volume = Mathf.MoveTowards(sourceTegang.volume, targetVolTegang, fadeSpeed * Time.deltaTime);
        }
    }

    public void SetMusicState(bool isTegang) {
        if (isTegang) {
            targetVolPatroli = 0f;
            targetVolTegang = 1f; 
        } else {
            targetVolPatroli = 1f; 
            targetVolTegang = 0f;
        }
    }

    public void MatikanSemuaMusik() {
        targetVolPatroli = 0f;
        targetVolTegang = 0f;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "HomePage" || scene.name == "HomeMenu") { 
            SetMusicState(false);
        } else {
            SetMusicState(false);
            if (sourcePatroli != null && !sourcePatroli.isPlaying) {
                MulaiMainkanMusik();
            }
        }
    }
}