using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;         //玩家序号
    public Rigidbody m_Shell;              //炮弹
    public Transform m_FireTransform;      //发射点位置
    public Slider m_AimSlider;             //蓄能条
    public AudioSource m_ShootingAudio;    //Audio
    public AudioClip m_ChargingClip;       //蓄能的音效
    public AudioClip m_FireClip;           //发射的音效
    public float m_MinLaunchForce = 15f;   //最小发射力量
    public float m_MaxLaunchForce = 30f;   //最大发射力量
    public float m_MaxChargeTime = 0.75f;  //最大充能时间


    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;

    /**
     * 初始化
     */
    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    private void Start()
    {
        //保存当前player发射按键的字符串
        m_FireButton = "Fire" + m_PlayerNumber;

        //充能速度
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }


    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        //如果充能超过最大充能并且还没发射子弹
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        //根据按键判断是否按下了开火键
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;  //从最小充能开始

            m_ShootingAudio.clip = m_ChargingClip;    //切换成充能音效
            m_ShootingAudio.Play();
        }
        //持续按下开火键的过程中，不断增加充能
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            m_AimSlider.value = m_CurrentLaunchForce;
        }
        //用户松开开火键，那么开火
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        //实例化一个炮弹
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}