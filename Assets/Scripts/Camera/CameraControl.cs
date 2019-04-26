using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 //移动Camera到目的position的时间
    public float m_ScreenEdgeBuffer = 4f;           //确保Tanks不会在屏幕边界之外
    public float m_MinSize = 6.5f;                  //Camera的最小尺寸
                                                    /*[HideInInspector]*/
    public Transform[] m_Targets; //坦克，先把[HideInInspector]注释掉


    private Camera m_Camera;
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;              //需要移动到的位置


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        /**
         * function Vector3.SmoothDamp(Vector3 current,Vector3 target
         *                 ,ref Vector3 currentVelocity,float smoothTime)
         *  @parameters
         *  current:当前的位置
         *  target:试图接近的位置
         *  currentVelocity:当前速度，这个值由你每次调用这个函数时修改
         *  smoothTime:到达目标的大约时间，较小的值将快速到达目标
         */
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            //判断当前坦克是否已经不是激活状态（死亡），如果未激活，
            //则不需要跟随该坦克
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        //CameraRig的Y position不会被改变
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        //根据目标位置来计算合适的Size
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        //把目标位置的世界坐标转换成本地坐标
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            //把坦克所在的位置转换成CameraRig的本地坐标
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            //在CameraRig的本地坐标下，求出坦克与CameraRig的目标位置的距离
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }
        //加上ScreenEdgeBuffer值，即坦克与屏幕边界的距离
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}