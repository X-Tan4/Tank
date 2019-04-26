using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          //初始生命值
    public Slider m_Slider;                        //slider，生命条
    public Image m_FillImage;                      //生命条的填充
    public Color m_FullHealthColor = Color.green;  //满血的时候是绿色
    public Color m_ZeroHealthColor = Color.red;    //低血量状态是红色
    public GameObject m_ExplosionPrefab;           //爆炸效果的预制件


    private AudioSource m_ExplosionAudio;          //Audio
    private ParticleSystem m_ExplosionParticles;   //爆炸的粒子效果
    private float m_CurrentHealth;
    private bool m_Dead;                           //是否已经死亡


    private void Awake()
    {
        //初始化的时候就准备好爆炸效果的实例
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        //未激活状态
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    /**
     * 外部调用，当坦克受伤的时候调用该函数
     */
    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;

        SetHealthUI();

        //如果坦克的血量低于0并且是存活的，那么判定为死亡
        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;

        //对Fill填充物的颜色做出改变
        //Color.Lerp 颜色的线性插值，通过第三个参数在颜色1和2之间插值。
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        //修改粒子系统的坐标为坦克死亡时候的坐标
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        //播放爆炸效果以及爆炸音效
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }
}
