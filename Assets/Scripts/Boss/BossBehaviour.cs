using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ***FASE DE SPAWN***
// *FUNCIONAMIENTO LITERAL*
// Tomando los enemigos previamente colocados en el editor, en función de la oleada correspondiente
// van siendo teletransportados al lado del Boss. 
// Una vez eliminados, se inicia la siguiente oleada.
// Se repite este comportamiento hasta acabar con todas oleadas.
//-----------------------------------------------------------------
// *INFORMACIÓN DE EDITOR*
// En el editor se introduce el número de oleadas (array de structs Wave[])  
// Dentro de cada oleada, el número de enemigos que invocará (array de structs Enemy[])
// Por último, se rellenan los datos del enemigo (struct Enemy) con la referencia y la posición relativa al jugador
//-----------------------------------------------------------------
// *IMPORTANTE*
// Los enemigos requieren del script SPAWNCONTROL para ser identificados como enemigos de la oleada y ESTAR COLOCADOS EN LA ESCENA

public class BossBehaviour : MonoBehaviour
{
    TransformList enemiesOnScreen = new TransformList();
    Animator[] bossAnimator;
    
    [SerializeField] private int actualWave = 0;
    private float delayHit = 0.41f;
    private float delaySpawn = 0.5f;
    bool dead = false;
    float time = 2f;

    BoxCollider2D col;

    [System.Serializable]
    private struct Wave{
        public Enemy[] enemyRound; // Enemigos en una oleada
    }

    [System.Serializable]   
    private struct Enemy{
        public Vector2 relativePos; // Posición respecto al Boss
        public GameObject enemyRef; // Referencia de los enemigos en escena
    }

    [SerializeField] 
    Wave[] waves = null; // Array de oleadas de Spawn

    [SerializeField]
    GameObject[] crystals = null;

    [SerializeField]
    GameObject[] walls = null;

    private void Start()
    {
        actualWave = 0;
        bossAnimator = GetComponentsInChildren<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (dead)
        {
            if (time > 0)
                time -= Time.deltaTime;
            else
                OnDeath();
        }
    }

    public void FirstInstance()
    {
        Instance();
    }

    private void Instance() //Invocación de los enemigos (tp de transform)
    {
        bossAnimator[0].SetTrigger("AttakBoss");
        for (float i = 0f; i < delayHit; i += Time.deltaTime) { }
        AudioManager.instance.Play(AudioManager.ESounds.Bastonazo);
        if (actualWave == 0)
            AudioManager.instance.Play(AudioManager.ESounds.Boss);
        if (waves != null) 
            for (int i = 0; i < waves[actualWave].enemyRound.Length; i++){
                for (float e = 0f; e < delaySpawn; e += Time.deltaTime) { }
                waves[actualWave].enemyRound[i].enemyRef.transform.position = GetRelativePos(i);
                enemiesOnScreen.InsertInEnd(waves[actualWave].enemyRound[i].enemyRef.transform);
            }
    }



    private void UpdateWave() //LLamado por SpawnControl para hacer la nueva invocación si no quedan enemigos vivos
    {
        if (actualWave < waves.Length && enemiesOnScreen.Lenght() == 0){
            if(crystals[actualWave].GetComponent<BossCrystal>() != null)
                crystals[actualWave].GetComponent<BossCrystal>().SetActive();
        }

    }

    public void UpdateEnemies(Transform e)
    {
        enemiesOnScreen.DeleteElement(e);
        UpdateWave();
    }

    public void UpdateCrystal()
    {
        if (actualWave < waves.Length - 1) //Si aún hay oleadas, saca la siguiente
        {
            actualWave++;
            Instance();
        }
        else // De lo contrario, el Boss está listo para ser derrotado
        {
            for(int i = 0; i < walls.Length; i++)
            {
                Destroy(walls[i].gameObject);
            }

        }

    }

    private Vector2 GetRelativePos(int i) //Método auxiliar para calcular la posición con respecto al Boss de los enemigos
    {
        Vector2 newRelativePos;
        newRelativePos.x = waves[actualWave].enemyRound[i].relativePos.x + this.transform.position.x;
        newRelativePos.y = waves[actualWave].enemyRound[i].relativePos.y + this.transform.position.y;
        return newRelativePos;
    }
    
    public void OnAttack()
    {
        Time.timeScale = 0.3f;
        AudioManager.instance.Stop(AudioManager.ESounds.Boss);
        bossAnimator[0].SetBool("DeathBoss", true);
        col.enabled = false;
        dead = true;
    }

    public void OnDeath()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Creditos");
    }
}
