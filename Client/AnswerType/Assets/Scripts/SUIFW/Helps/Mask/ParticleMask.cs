using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 参考地址 代码有调整
/// https://blog.csdn.net/wangjiangrong/article/details/79483257
/// </summary>
public class ParticleMask : MonoBehaviour
{
    [SerializeField] RectTransform m_rectTrans;//遮挡容器，即ScrollView

    List<Material> m_materialList = new List<Material>();//存放需要修改Shader的Material

    public bool isStart = false;
    public bool isEnable = false;
    public bool isDelay = false;
    public float delayTime = 0.5f;

    [GL_Name("是否计算绝对位置")]
    public bool _isAbsolutePosition = false;




    void Start()
    {
        if (!isEnable)
        {
            if (isDelay)
            {
                Invoke(nameof(Delay), delayTime);
                if (isStart)
                {
                    InitParticleMask(gameObject);
                }
            }
            else
            {
                InitParticleMask(gameObject);
            }
        }

        
    }

    private void OnEnable()
    {
        if (isEnable)
        {
            if (isDelay)
            {
                Invoke(nameof(Delay), delayTime);
            }
            else
            {
                InitParticleMask(gameObject);
            }
          
        }
    }

    private void Delay()
    {
        InitParticleMask(gameObject);
    }

    /// <summary>
    /// 添加新的item
    /// </summary>
    /// <param name="obj"></param>
    public void AddNewItem(GameObject obj)
    {
        InitParticleMask(obj);
    }

    private void InitParticleMask(GameObject parent)
    {
        //获取所有需要修改shader的material，并替换shader
        var particleSystems = parent.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0, j = particleSystems.Length; i < j; i++)
        {
            var ps = particleSystems[i];
            var mat = ps.GetComponent<Renderer>().material;
            if (mat.shader.name.Contains("Mask"))
                continue;

            if (!m_materialList.Contains(mat))
                m_materialList.Add(mat);
            if (!mat.shader.name.Contains("Mask"))
                mat.shader = Shader.Find(mat.shader.name + "Mask");

            TrailRenderer tRender = ps.GetComponent<TrailRenderer>();
            if (tRender != null)
            {
                var tmat = tRender.material;
                if (tmat != null)
                {
                    if (!m_materialList.Contains(mat))
                        m_materialList.Add(mat);
                    if (!mat.shader.name.Contains("Mask"))
                        mat.shader = Shader.Find(mat.shader.name + "Mask");
                }
            }
        }

        var renders = parent.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0, j = renders.Length; i < j; i++)
        {
            var ps = renders[i];
            var mat = ps.material;

            if(!m_materialList.Contains(mat))
                m_materialList.Add(mat);
            if (!mat.shader.name.Contains("Mask"))
                mat.shader = Shader.Find(mat.shader.name + "Mask");
        }

        var canvasMasks = parent.GetComponentsInChildren<CanvasMask>(true);
        for (int i = 0, j = canvasMasks.Length; i < j; i++)
        {
            var ps = canvasMasks[i];
            var mat = ps.mat;
            if (mat == null)
            {
                ps.Init();
            }
            if (mat == null)
                continue;
            if (!m_materialList.Contains(mat))
                m_materialList.Add(mat);
            if (!mat.shader.name.Contains("Mask"))
                mat.shader = Shader.Find(mat.shader.name + "Mask");
        }
        CalculateArea();
    }

    public void CalculateArea()
    {
        Vector3[] corners = new Vector3[4];
        m_rectTrans.GetWorldCorners(corners);
        Vector4 area = new Vector4(corners[0].x, corners[0].y, corners[2].x, corners[2].y);

        if(_isAbsolutePosition)
        {
            float value = m_rectTrans.position.y - area.y + m_rectTrans.localPosition.y;
            area.y -= value;
            area.w -= value;
        }


        for (int i = 0, len = m_materialList.Count; i < len; i++)
        {
            m_materialList[i].SetInt("_IsClip", 1);
            m_materialList[i].SetVector("_Area", area);
        }
    }
}
