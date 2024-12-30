using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelConfig
{
    public string LevelName;
    public List<GameObject> Letters;
    public List<Transform> LettersSpawnPositions;
    public List<GameObject> SlotPrefabs;
    public List<Transform> SlotSpawnPositions;
    public GameObject AnimalPrefab;
    public Transform AnimalSpawnPosition;
}



[System.Serializable]
public class ParticleSystemConfig
{
    [HideInInspector]
    public string ParticleName;

/*    public ParticleType ParticleType;
*/    public GameObject ParticlePrefab;
    public Transform PositionTransform;
    public Vector3 PositionVector;
    public Vector3 Rotation;
    public Vector3 Scale = Vector3.one;
    public float Duration = 2f;
    public float Delay = 1;
    public bool Loop = false;
  /*  public string GetParticleName()
    {
        return ParticleType.ToString();  
    }*/
  /*  public void OnValidate()
    {
        ParticleName = ParticleType.ToString(); 
    }*/
  

}


public class LetterSpawner : MonoBehaviour
{
    private static LetterSpawner _instance;
    public List<LevelConfig> Levels;
    public List<ParticleSystemConfig> ParticleConfigs; //Setup
    private ParticleSystemConfig ParticleSystemConfig; // Use For Editor
   
    public GameObject FinishPanel;

    private int currentLevelIndex = 0;
    private int placedObjectCount = 0;
/*    private int MaxWrongMove = 5; // tutoriali gatishulia troebit 
*/    [HideInInspector]public int WrongCount = 0;

/*    public GameObject TutorialManager;
*/
    private List<GameObject> animals = new List<GameObject>();
    private List<GameObject> letters = new List<GameObject>();
    private List<GameObject> slots = new List<GameObject>();

    private LevelConfig CurrentLevel => Levels[currentLevelIndex];
    private string HappyAnimation = "Happy-Talking";
    private string IdleAnimation = "Idle";
    public static LetterSpawner Instance { get; private set; }

    private LetterGameEntryPoint _entryPoint;
    [SerializeField] private Button homeButton;
    private void Awake()
    {
        homeButton.onClick.AddListener(FinishOnButton);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void FinishOnButton()
    {
        _entryPoint.InvokeGameFinished();
    }

    void Start()
    {
        if (ParticleConfigs.Count > 0)
        {
            foreach (var particleConfig in ParticleConfigs)
            {
/*                particleConfig.OnValidate();
*/
                string particleName = particleConfig.ParticleName;

            }
        }
        if (_instance == null)
        {
            _instance = this;
        }

        LoadLevel(currentLevelIndex);
    }
    
    void LoadLevel(int levelIndex)
    {
        if (levelIndex >= Levels.Count) return;

        ClearPreviousLevel();

        LevelConfig level = Levels[levelIndex];

        if (level.AnimalPrefab != null)
        {
            GameObject animal = Instantiate(level.AnimalPrefab, level.AnimalSpawnPosition.position, Quaternion.identity);
            animal.AddComponent<TagValue>().Tag = new string[] { "Animal" }; 
            animals.Add(animal);
        }

        for (int i = 0; i < level.Letters.Count; i++)
        {
            if (i < level.LettersSpawnPositions.Count)
            {
                GameObject letter = Instantiate(level.Letters[i], level.LettersSpawnPositions[i].position, Quaternion.identity);
                letter.AddComponent<TagValue>().Tag = new string[] { "Letter" }; 
                letters.Add(letter);
            }
        }

        for (int i = 0; i < level.SlotPrefabs.Count; i++)
        {
            if (i < level.SlotSpawnPositions.Count)
            {
                GameObject slot = Instantiate(level.SlotPrefabs[i], level.SlotSpawnPositions[i].position, Quaternion.identity);
                slot.AddComponent<TagValue>().Tag = new string[] { "Slot" };
                slots.Add(slot);
            }
        }

        placedObjectCount = 0;
    }

    void ClearPreviousLevel()
    {
        foreach (var animal in animals)
        {
            Destroy(animal);
        }
        animals.Clear();

        foreach (var letter in letters)
        {
            Destroy(letter);
        }
        letters.Clear();

        foreach (var slot in slots)
        {
            Destroy(slot);
        }
        slots.Clear();
        WrongCount = 0;
    }

    void InstantiateParticles()
    {
        foreach (var particleConfig in ParticleConfigs)
        {
            Vector3 position;

            if (particleConfig.PositionTransform != null)
            {
                position = particleConfig.PositionTransform.position;
            }
            else if (particleConfig.PositionVector != Vector3.zero)
            {
                position = particleConfig.PositionVector;
            }
            else
            {
                Debug.LogError("პარტიკლის პოზიციები არარის შევსებული, შეავსე ან ტრანსფორნით ან ვექტორით");
                Debug.LogError("ვექტორით შევსებისას არ გამოიყენო ტრანსფორმი, მხოლოდ ტრანსფორმი იმუშავებს, შეავსე მხოლოდ ერთი");

                continue;
            }

            StartCoroutine(InstantiateParticleWithDelay(particleConfig, position));
        }
    }

    IEnumerator InstantiateParticleWithDelay(ParticleSystemConfig particleConfig, Vector3 position)
    {
        yield return new WaitForSeconds(particleConfig.Delay);

        GameObject particle = Instantiate(particleConfig.ParticlePrefab, position, Quaternion.Euler(particleConfig.Rotation));
        particle.transform.localScale = particleConfig.Scale;

        var particleSystem = particle.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var mainModule = particleSystem.main;
            mainModule.loop = particleConfig.Loop;
        }

        if (!particleConfig.Loop)
        {
            Destroy(particle, particleConfig.Duration);
        }
    }

    public void ObjectPlaced(Slot slot)
    {
        placedObjectCount++;
        slot.IsOccupied = true;

        if (placedObjectCount >= CurrentLevel.Letters.Count)
        {
            StartCoroutine(FinishLevel());
        }
    }

   /* public IEnumerator Tutorial()
    {
        if (WrongCount == MaxWrongMove)
        {
            TutorialManager.SetActive(true);
            yield return new WaitForSeconds(4);
            TutorialManager.SetActive(false);
            WrongCount = 0;
        }
    }*/
    IEnumerator FinishLevel()
    {
        placedObjectCount = 0;
        PlayAnimation();
        yield return new WaitForSeconds(2f);
        currentLevelIndex++;

        if (currentLevelIndex < Levels.Count)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            SetFinishForPackage();
/*            FinishPanel.SetActive(true);
            InstantiateParticles();*/
        }
    }

    void PlayAnimation()
    {
        foreach (GameObject animal in animals)
        {
            var skeletonAnimation = animal.GetComponent<SkeletonAnimation>();
            if (skeletonAnimation != null)
            {
                StartCoroutine(AnimationDelay(animal));
            }
        }
    }

    IEnumerator AnimationDelay(GameObject animal)
    {
        var skeletonAnimation = animal.GetComponent<SkeletonAnimation>();

        if (skeletonAnimation != null)
        {
            var trackEntry = skeletonAnimation.state.SetAnimation(0, HappyAnimation, false);
            yield return new WaitForSeconds(trackEntry.Animation.Duration);
            skeletonAnimation.state.SetAnimation(0, IdleAnimation, true);
        }
    }
    public void SetEntryPoint(LetterGameEntryPoint entryPoint)
    {
        _entryPoint = entryPoint;
    }

    private void SetFinishForPackage()
    {
        StartCoroutine(FinishAfterFirework());
    }

    private IEnumerator FinishAfterFirework()
    {
        yield return new WaitForSecondsRealtime(3f);
        _entryPoint.InvokeGameFinished();
    }
}


//Editor

#if UNITY_EDITOR

public enum ParticleType
{
    Conffetti,
    FireWork,
    Star,
    ConffetiImpact
}


#endif

