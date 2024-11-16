using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody rb;                      // Rigidbody của object
    private List<Rigidbody> attractedObjects;  // Danh sách các object đang bị hút
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = objectMass;
        attractedObjects = new List<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        // Tìm các object trong phạm vi từ trường
        DetectMagneticObjects();
        
        // Tính toán và áp dụng lực hút cho từng object
        ApplyMagneticForces();
        
        // Tính tổng hợp lực tác động lên object này
        CalculateTotalForces();
    }
    
    private void DetectMagneticObjects()
    {
        // Tìm các collider trong phạm vi từ trường
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, magneticRadius, magneticLayer);
        attractedObjects.Clear();
        
        foreach (Collider collider in hitColliders)
        {
            Rigidbody targetRb = collider.GetComponent<Rigidbody>();
            if (targetRb != null && targetRb != rb)
            {
                attractedObjects.Add(targetRb);
            }
        }
    }
    
    private void ApplyMagneticForces()
    {
        foreach (Rigidbody targetRb in attractedObjects)
        {
            // Tính vector hướng và khoảng cách
            Vector3 direction = transform.position - targetRb.position;
            float distance = direction.magnitude;
            
            if (distance == 0f) continue;
            
            // Tính lực hút dựa trên định luật nghịch đảo bình phương
            float forceMagnitude = (magneticStrength * rb.mass * targetRb.mass) / (distance * distance);
            
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
    
    private void CalculateTotalForces()
    {
        // Tính trọng lực
        Vector3 gravity = Physics.gravity * rb.mass;
        
        // Tính tổng các lực từ trường
        Vector3 totalMagneticForce = Vector3.zero;
        foreach (Rigidbody targetRb in attractedObjects)
        {
            Vector3 direction = transform.position - targetRb.position;
            float distance = direction.magnitude;
            
            if (distance == 0f) continue;
            
            float forceMagnitude = (magneticStrength * rb.mass * targetRb.mass) / (distance * distance);
            totalMagneticForce += direction.normalized * forceMagnitude;
        }
        
        // Tổng hợp lực cuối cùng
        Vector3 totalForce = gravity + totalMagneticForce;
        
        // Debug để kiểm tra
        Debug.DrawRay(transform.position, totalForce.normalized * 2f, Color.red);
        Debug.Log($"Total Force Magnitude: {totalForce.magnitude}");
    }
    
    private void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi từ trường trong Scene view
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magneticRadius);
    }
}
