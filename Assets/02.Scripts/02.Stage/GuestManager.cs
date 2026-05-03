using UnityEngine;
using System.Collections;

public class GuestManager : MonoBehaviour
{
    public static GuestManager instance { get; private set; }

    [Header("Guest Setting")]
    [SerializeField] private GameObject[] guestPrefab;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private float spawnDelay = 2.0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        //SpawnGuest();
        StartCoroutine(InitialSpawnRoutine());
    }

    private IEnumerator InitialSpawnRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        SpawnGuest();
    }

    private void SpawnGuest()
    {
        if (guestPrefab==null||guestPrefab.Length==0||spawnPos==null)
        {
            Debug.LogError("프리팹/스폰Pos 공란"); return;            
        }

        int randomIndex = Random.Range(0, guestPrefab.Length);
        GameObject selectedPrefab = guestPrefab[randomIndex];

        Instantiate(selectedPrefab, spawnPos.position, Quaternion.identity);
        //Instantiate(guestPrefab,spawnPos.position,Quaternion.identity);
        SoundManager.Instance.PlaySFX(SFXType.MeowHello);

    }

    public void CallNextGuest()
    {
        StartCoroutine(SpawnGuestRoutine());
    }

    private IEnumerator SpawnGuestRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnGuest();
    }

}

