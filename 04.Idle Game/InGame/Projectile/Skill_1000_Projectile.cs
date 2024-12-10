using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_1000_Projectile : Projectile
{
    [SerializeField] private GameObject[] fxs;
    
    private Vector3[] bezierCurvePoints; //베지어 곡선 좌표들 저장할 배열

    private CircleCollider2D collider2D;
    private bool isMoving;

    protected override void Awake()
    {
        base.Awake();
        collider2D = GetComponent<CircleCollider2D>();
    }

    public override void Init(UnitAI caster,UnitHp target, double damage, float speed)
    {
        base.Init(caster,target,damage, speed);

        collider2D.enabled = true;

        ActivateFX(false);
        Move();
    }

    private void CalculateCurvePoints(int count, Vector3 startPos, Vector3 targetPos)
    {
        Vector3 v3A = startPos;
        Vector3 v3C = targetPos;

        Vector3 v3Dir = (targetPos - startPos).normalized;
        float angle = v3Dir.x > 0.0f ? 90f : -90f;
        Vector3 v3B = v3A + Quaternion.Euler(0, 0, angle) * v3Dir * (Vector3.Magnitude(targetPos - startPos) * 0.5f > 1.0f ? 1.0f : Vector3.Magnitude(targetPos - startPos) * 0.5f);

        bezierCurvePoints = new Vector3[count + 1];
        float unit = 1.0f / count;

        int i = 0; float t = 0f;
        for (; i < count + 1; i++, t += unit)
        {
            float u = (1 - t);
            float t2 = t * t;
            float u2 = u * u;

            bezierCurvePoints[i] =
                v3A * u2 +
                v3B * (t * u * 2) +
                v3C * t2
                ;
        }
    }
    
    protected override async UniTaskVoid Move()
    {
        int pointIndex = 0;
        isMoving = true;
        CalculateCurvePoints(30,transform.position,transform.position + (Vector3.right * 4));
        
        while (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, bezierCurvePoints[pointIndex], speed * Time.deltaTime);
            
            //좌표 배열의 위치에 가까워질때
            if (Vector3.SqrMagnitude(transform.position - bezierCurvePoints[pointIndex]) < Mathf.Pow(0.0001f, 2))
            {
                pointIndex++;
               
                //좌표 인덱스가 마지막 인덱스면 삭제
                if (pointIndex == bezierCurvePoints.Length)
                {
                    Despawn();
                    break;
                }
            }
            await UniTask.Yield();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<UnitHp>().Hit(damage);
            
            isMoving = false;
            collider2D.enabled = false;
            
            transform.position = col.transform.position;
            
            ActivateFX(true);
        }
    }

    private void ActivateFX(bool isActive)
    {
        fxs[0].SetActive(!isActive);
        fxs[1].SetActive(isActive);
    }
}
