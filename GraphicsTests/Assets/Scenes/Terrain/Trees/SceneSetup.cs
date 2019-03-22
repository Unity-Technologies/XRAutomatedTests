using UnityEngine;
using System.Collections;

public class SceneSetup : MonoBehaviour
{
    public float visibleRadius = 9;
    public GameObject cameraTarget;
    public int rows = 3;
    public int cols = 4;

    public Camera[] cameras;

    void Start()
    {
        Vector3 cameraTargetPos = cameraTarget.transform.position;
        Object.Destroy(cameraTarget);

        //rows = 1; cols = 3;

        int count = rows * cols;
        int index = 0;

        Vector2 viewSize = new Vector2(1.0f / cols, 1.0f / rows);

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                if (index >= cameras.Length)
                    break;

                if (cameras[index] == null)
                    continue;

                Camera cameraClone = cameras[index];
                cameraClone.backgroundColor = new Color(0.4f, 0.4f, (float)index / count * 0.4f + 0.6f);
                cameraClone.aspect *= viewSize.x / viewSize.y;

                {
                    Vector3 viewPos = cameraClone.WorldToViewportPoint(cameraTargetPos);

                    float scale = 1;
                    {
                        Vector3[] pos = new Vector3[] {
                            cameraClone.WorldToViewportPoint(cameraTargetPos + Vector3.left * visibleRadius),
                            cameraClone.WorldToViewportPoint(cameraTargetPos + Vector3.up * visibleRadius),
                            cameraClone.WorldToViewportPoint(cameraTargetPos + Vector3.forward * visibleRadius),
                            cameraClone.WorldToViewportPoint(cameraTargetPos - Vector3.left * visibleRadius),
                            cameraClone.WorldToViewportPoint(cameraTargetPos - Vector3.up * visibleRadius),
                            cameraClone.WorldToViewportPoint(cameraTargetPos - Vector3.forward * visibleRadius)
                        };

                        float maxD = -1;
                        foreach (Vector3 p in pos)
                        {
                            Vector3 dist = p - viewPos;
                            float d = new Vector2(dist.x, dist.y).magnitude;
                            if (d > maxD)
                                maxD = d;
                        }

                        scale = 0.5f / maxD;
                        //Debug.Log(maxD + " " + scale + " " + ToVec2(pos[0] - viewPos) + " " + ToVec2(pos[1] - viewPos) + " " + ToVec2(pos[2] - viewPos));
                    }

                    viewPos.x = 1 - viewPos.x * 2;
                    viewPos.y = 1 - viewPos.y * 2;

                    Matrix4x4 m = Matrix4x4.identity;
                    m[0, 3] = viewPos.x * scale;
                    m[1, 3] = viewPos.y * scale;

                    m[0, 0] = m[1, 1] = scale;

                    cameraClone.projectionMatrix = m * cameraClone.projectionMatrix;
                }

                cameraClone.rect = new Rect((float)j / cols, (float)(rows - i - 1) / rows, viewSize.x, viewSize.y);
                cameraClone.depth = 10;

                ++index;
            }

            if (index >= cameras.Length)
                break;
        }
    }

    Vector2 ToVec2(Vector3 v) { return new Vector2(v.x, v.y); }

    void Update()
    {
    }
}
