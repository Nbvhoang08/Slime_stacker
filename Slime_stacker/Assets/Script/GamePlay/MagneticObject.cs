using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    [Header("Magnetic Properties")]
    public float magneticStrength = 10f;        // Độ mạnh của từ trường
    public float magneticRadius = 5f;           // Bán kính ảnh hưởng của từ trường
    public float minimumForce = 0.1f;          // Lực hút tối thiểu
    public LayerMask magneticLayer;            // Layer chứa các object có thể bị hút
    
    [Header("Object Properties")]
    public float objectMass = 1f;              // Khối lượng của object
    public Rigidbody2D rb;                      // Rigidbody của object
    private List<Rigidbody2D> attractedObjects;  // Danh sách các object đang bị hút
    public bool isHolding;
    public bool istouching;
    private Collider2D colli;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = objectMass;
        attractedObjects = new List<Rigidbody2D>();
        colli = GetComponent<Collider2D>();
    }
    void Update()
    {
        
        if (colli == null)
        {
            Debug.LogWarning("Collider đã bị xóa hoặc vô hiệu hóa!" + this.name);
        }
    }
    
    private void FixedUpdate()
    {
      
            // // Tính toán và áp dụng lực hút cho từng object
            ApplyMagneticForces();
        
            // // Tìm các object trong phạm vi từ trường
            DetectMagneticObjects();
            // // Tính tổng hợp lực tác động lên object này
            CalculateTotalForces();
            
            
    }
    
    private void DetectMagneticObjects()
    {
        // Tìm các collider trong phạm vi từ trường
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, magneticRadius, magneticLayer);
        attractedObjects = new List<Rigidbody2D>();
        
        foreach (Collider2D collider in hitColliders)
        {
            Rigidbody2D targetRb = collider.GetComponent<Rigidbody2D>();
            if (targetRb != null && targetRb != rb)
            {
                attractedObjects.Add(targetRb);
            }
        }
    }
    
    private void ApplyMagneticForces()
    {
        foreach (Rigidbody2D targetRb in attractedObjects)
        {
            if(!targetRb.gameObject.GetComponent<MagneticObject>().isHolding)
            {
                // Tính vector hướng và khoảng cách
                Vector2 direction = (Vector2)transform.position - targetRb.position;
                float distance = direction.magnitude;
              

            
                if (distance == 0f) continue;
            
                // Tính lực hút dựa trên định luật nghịch đảo bình phương
                float forceMagnitude = (magneticStrength * rb.mass * targetRb.mass) /(distance*distance);
                forceMagnitude = math.clamp(forceMagnitude, 0f,100f);
                // Giới hạn lực tối thiểu
                if (forceMagnitude < minimumForce) continue;
            
                // Chuẩn hóa vector hướng
                Vector3 force = direction.normalized * forceMagnitude;
            
                // Áp dụng lực lên object đích
                targetRb.AddForce(force);
                
                // Áp dụng lực phản tác dụng lên chính object này (Định luật 3 Newton)
                rb.AddForce(-force);
            }
            
        }
    }
    
    private void CalculateTotalForces()
    {
        // Tính trọng lực
        Vector2 gravity = Physics2D.gravity * rb.mass;
        
        // Tính tổng các lực từ trường
        Vector2 totalMagneticForce = Vector2.zero;
        foreach (Rigidbody2D targetRb in attractedObjects)
        {
            Vector2 direction = (Vector2)transform.position - targetRb.position;
            float distance = direction.magnitude;
            
            if (distance == 0f) continue;
            
            float forceMagnitude = (magneticStrength * rb.mass * targetRb.mass) / (distance * distance);
            totalMagneticForce += direction.normalized * forceMagnitude;
        }
        
        // Tổng hợp lực cuối cùng
        Vector2 totalForce = gravity + totalMagneticForce;
        
        // Debug để kiểm tra
        Debug.DrawRay(transform.position, totalForce.normalized * 2f, Color.red);
        
        
    }

   
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("slime"))
        {
            if(!istouching)
            {
                 SoundManager.Instance.PlayVFXSound(0);   
                istouching= true;
                 
            }
            Debug.Log(other.gameObject.name + " " + this.name );
                
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("slime"))
        {
           istouching = false;
                
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi từ trường trong Scene view
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magneticRadius);
    }
}
