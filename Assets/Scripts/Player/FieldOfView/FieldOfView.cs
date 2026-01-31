using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;

    [SerializeField] private float fov;
    [SerializeField] private int rayCount;
    [SerializeField] private float viewDistance;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        Vector3 origin = Vector3.zero;

        int vertexCount = rayCount + 2;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        float startAngle = -fov / 2f;
        float angleIncrease = fov / rayCount;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startAngle + angleIncrease * i;

            Vector3 dir = GetVectorFromAngle(angle);
            Vector3 vertex = origin + dir * viewDistance;

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }
}