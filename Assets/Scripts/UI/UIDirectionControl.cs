using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;   //��ʼ�Ƕ�
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;         //ÿһ֡�ĸ��¶��ѽǶ�����Ϊ��ʼֵ
                                                             //Ҳ���ǹ̶�Slider�ĽǶ�
    }
}
