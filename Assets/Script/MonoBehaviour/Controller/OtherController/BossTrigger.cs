using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEndStates;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPos;
    public GameObject transPoint;

    public float textKeepTime;
    public string[] texts;

    [System.NonSerialized] public FiniteStateMachine<GameEndType> state;
    private FighterController boss;

    private void Awake()
    {
        state = new FiniteStateMachine<GameEndType>();
        state.Relate(GameEndType.Ready, new Ready(this));
        state.Relate(GameEndType.Start, new Start(this));
        state.Relate(GameEndType.End, new End(this));
    }

    private void OnEnable()
    {
        state.Transition(GameEndType.Ready);
        GameManager.OnTransScene += TransDefaultMusic;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.music.clip = AudioManager.instance.bossMusic;
            AudioManager.instance.music.Play();
            transPoint.SetActive(false);
            boss = Instantiate(bossPrefab, bossSpawnPos).GetComponent<FighterController>();
            boss.transform.SetParent(null);
            boss.OnDeath += () => state.Transition(GameEndType.Start);

            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void Update()
    {
        state.currentState.UpdateState();
    }

    private void OnDisable()
    {
        GameManager.OnTransScene -= TransDefaultMusic;
    }

    private void TransDefaultMusic()
    {
        if (state.currentState is Ready && AudioManager.instance.music.clip != AudioManager.instance.defaultMusic)
        {
            AudioManager.instance.music.clip = AudioManager.instance.defaultMusic;
            AudioManager.instance.music.Play();
        }
    }
}
