using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        //5回合获胜则游戏获胜
    public float m_StartDelay = 3f;         //每回合开始的等待时间
    public float m_EndDelay = 3f;           //每回合结束之后的等待时间
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;           //两个坦克管理者


    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay); //用来协同yield指令，等待若干秒
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();             //生成坦克       
        SetCameraTargets();          //设置摄像机

        StartCoroutine(GameLoop());  //
    }

    /**
     * 在出生点生成坦克
     */
    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;  //为坦克标号
            m_Tanks[i].Setup();                 //调用TankManager的setup方法
        }
    }

    /**
     * 设置摄像头的初始位置
     */
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }

    //游戏循环
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());   //等待一段时间后执行
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        //如果有胜者，则重新加载游戏场景
        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(GameLoop());     //如果没有胜者，则继续循环
        }
    }

    /**
     * 每一回合的开始
     */
    private IEnumerator RoundStarting()
    {
        ResetAllTanks();                   //重置坦克位置
        DisableTankControl();              //取消对坦克的控制
        m_CameraControl.SetStartPositionAndSize();  //摄像机聚焦位置重置

        m_RoundNumber++;                   //回合数增加
        m_MessageText.text = "ROUND" + m_RoundNumber; //更改UI的显示
        yield return m_StartWait;
    }


    /**
     * 每一回合的游戏过程
     */
    private IEnumerator RoundPlaying()
    {
        EnableTankControl();    //激活对坦克的控制

        m_MessageText.text = string.Empty; //UI不显示

        //如果只剩下一个玩家，则跳出循环
        while (!OneTankLeft())
        {
            yield return null;
        }

    }


    /**
     * 每一回合的结束
     */
    private IEnumerator RoundEnding()
    {
        //取消对坦克的控制
        DisableTankControl();

        m_RoundWinner = null;

        //判断当前回合获胜的玩家
        m_RoundWinner = GetRoundWinner();

        //累积胜利次数
        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        //判断是否有玩家达到了游戏胜利的条件
        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }

    /**
     * 该方法用于判断是否只剩下一个玩家在场景中
     */
    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }

    /**
     * 该方法用于判断回合胜者
     */
    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    /**
     * 该方法用于判断游戏获胜者
     */
    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();     //调用TankManager的Reset()方法
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();   //调用TankManager的EnableControl()方法
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();   //调用TankManager的DisableControl()方法
        }
    }
}