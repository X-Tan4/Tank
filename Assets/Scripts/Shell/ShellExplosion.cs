using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                    //Player的层级
    public ParticleSystem m_ExplosionParticles;     //爆炸的粒子系统  
    public AudioSource m_ExplosionAudio;            //Audio 
    public float m_MaxDamage = 100f;                //最大伤害  
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 2f;
    public float m_ExplosionRadius = 5f;            //子弹爆炸半径  


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);         //在子弹的存活时间过后，自动销毁
    }

    /**
     *  当子弹与其他物体发生碰撞并且碰撞器的Is Trigger勾选的情况下，会调用该函数
     */
    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

        /**
         * Physics.OverlapSphere(Vector3 position,float radius,int layerMask mask)
         * @parameter position 球体的球心
         * @parameter radius   球体的半径
         * @parameter mask     只有该层级与球体碰撞才会被选择
         * 该函数用于返回在球体范围内与球体产生碰撞的特定层级的碰撞器
         */
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();  //寻找碰撞器的刚体

            if (!targetRigidbody)
            {
                continue;
            }

            //为符合条件的受撞体添加爆炸力
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //获取受撞体的TankHealth脚本
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            if (!targetHealth)
            {
                continue;
            }

            //计算伤害并扣除血量
            float damage = CalculateDamage(targetRigidbody.position);
            targetHealth.TakeDamage(damage);
        }

        //把粒子系统与Shell的关联解除
        m_ExplosionParticles.transform.parent = null;

        //播放爆炸效果及音效
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        //粒子系统的爆炸效果播放完毕后，删除该object
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject); //回收Shell
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

        //1、创建一个向量，由Shell指向目标
        Vector3 explosionToTarget = targetPosition - transform.position;

        //2、计算Shell与目标之间的距离
        float explosionDistance = explosionToTarget.magnitude;

        //3、根据上一步的距离计算出伤害权重
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        //4、根据伤害权重计算出最终伤害
        float damage = relativeDistance * m_MaxDamage;

        //5、最低伤害为0，这是因为第三步的数值有可能是负值，这样就相当于没有伤害
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}