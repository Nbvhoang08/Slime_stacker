using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class QueueSystem : MonoBehaviour
{
    [SerializeField] private Transform[] queueSlots;
    [SerializeField] private Vector2 slotSize = new Vector2(1f, 1f);
    [SerializeField] private float moveSpeed = 5f; // Tốc độ di chuyển của object
    private const int MAX_SLOTS = 5;
    [SerializeField] private GameManager gameManager;
    
    public List<QueueObject> objectQueue = new List<QueueObject>();
    [SerializeField]private List<GameObject> pendingObjects;
    [SerializeField]private GameObject selectedObject;
    public bool isDragging = false;
    private Vector3 dragOffset;

   private void Awake() 
   {

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        queueSlots = new Transform[MAX_SLOTS];
        // Tạo slot từ phải sang trái
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            GameObject slot = new GameObject($"Slot_{i}");
            slot.transform.parent = transform;
            // Vị trí slot từ phải sang trái
            slot.transform.localPosition =  new Vector3((MAX_SLOTS-3- i) * slotSize.x + (MAX_SLOTS - 1 - 0.5f*i), 0, 0);
            queueSlots[i] = slot.transform;

            
        }
    }

    private void Update()
    {
        HandleInput();
        //CheckAndSpawnNewObject();
        UpdateObjectPositions();
        FitScale();
         
    }

    private void UpdateObjectPositions()
    {
        bool allObjectsMoved = true;

        foreach (var queueObj in objectQueue.ToArray())
        {
            if (queueObj.isMoving)
            {
                Vector3 currentPos = queueObj.gameObject.transform.position;
                queueObj.gameObject.transform.position = Vector3.MoveTowards
                (
                    currentPos, 
                    queueObj.targetPosition, 
                    moveSpeed * Time.deltaTime
                    
                );
                if (Vector3.Distance(queueObj.gameObject.transform.position, queueObj.targetPosition) < 0.01f)
                {
                    queueObj.isMoving = false;
                }
                else
                {
                    allObjectsMoved = false;
                }
            }
        }

        if (allObjectsMoved)
        {
            CheckAndSpawnNewObject();
        }
        
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < objectQueue.Count; i++)
        {
            MoveObjectToSlot(objectQueue[i], i);
            
        }
    }
    private void CheckAndSpawnNewObject()
    {
        if (objectQueue.Count < MAX_SLOTS && pendingObjects.Count > 0)
        {
            SpawnNextObject();
        }
    }
    private void SpawnNextObject()
    {
        if (pendingObjects.Count > 0 && objectQueue.Count < MAX_SLOTS)
        {
            GameObject newObj = Instantiate(pendingObjects[0],transform.position,pendingObjects[0].transform.rotation);
            pendingObjects.RemoveAt(0);
            
            QueueObject queueObj = new QueueObject(newObj);
            // Thêm vào đầu list (vị trí bên trái)
            objectQueue.Insert(0, queueObj);
            
            // Di chuyển object vào slot cuối (bên trái)
            UpdateQueuePositions();
            if(!gameManager.slime.Contains(newObj))
            {
                gameManager.slime.Add(newObj);
            }
        }
    }


    private void MoveObjectToSlot(QueueObject queueObj, int slotIndex)
    {
        if (slotIndex < queueSlots.Length)
        {
            GameObject obj = queueObj.gameObject;
            queueObj.targetPosition = queueSlots[objectQueue.Count - 1 - slotIndex].position;
            queueObj.isMoving = true;
            // Scale object to fit slot
        }
    }
    private void FitScale()
    {
        foreach (QueueObject queueObj in objectQueue)
        {
            if (queueObj.gameObject != selectedObject)
            {  
                if( queueObj.gameObject.transform.localScale == queueObj.originalScale)
                {
                    Bounds objBounds = GetObjectBounds(queueObj.gameObject);
                    float scaleX = slotSize.x / objBounds.size.x;
                    float scaleY = slotSize.y / objBounds.size.y;
                    float scaleFactor = Mathf.Min(scaleX, scaleY);
                    queueObj.gameObject.transform.localScale = queueObj.originalScale * scaleFactor;
                }
                
            }

                if(queueObj.rb != null)
                {
                    queueObj.rb.gravityScale = 0;
                }
                
        }
    }

    private void HandleInput()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            // Kiểm tra xem object có phải là object trên cùng của list không
            QueueObject hitObject = objectQueue.Find(x => x.gameObject == hit.collider.gameObject);
            if (hitObject != null && objectQueue.IndexOf(hitObject) == objectQueue.Count - 1)
            {
                StartDragging(hitObject);
            }
        }
    }
    else if (Input.GetMouseButtonUp(0) && isDragging)
    {
        StopDragging();
    }

    if (isDragging)
    {
        UpdateDragPosition();
    }
}

private void StartDragging(QueueObject queueObj)
{
    if (queueObj == null) return;

    isDragging = true;
    selectedObject = queueObj.gameObject;
    dragOffset = queueObj.gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    
    // Chỉ restore scale cho selected object
    selectedObject.transform.localScale = queueObj.originalScale;
    queueObj.gameObject.layer = LayerMask.NameToLayer("slime");
    if(queueObj.magneticObject != null)
    {
        queueObj.magneticObject.enabled = true;
        queueObj.magneticObject.isHolding = true;
    } 
   
    // Remove object from queue
    objectQueue.Remove(queueObj);
    UpdateQueuePositions();
}

private void StopDragging()
{
    if (selectedObject == null) return;

    // Giả sử selectedObject đã được gán đúng kiểu QueueObject
    GameObject queueObj = selectedObject;
    if (queueObj != null)
    {
        queueObj.GetComponent<MagneticObject>().rb.gravityScale = 1;
        queueObj.GetComponent<MagneticObject>().isHolding = false;
    }

    isDragging = false;
    selectedObject = null;
}

    private void UpdateDragPosition()
    {
        if (selectedObject != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedObject.transform.position = mousePosition + dragOffset;
        }
    }

    

    public void InitializePendingObjects(List<GameObject> objects)
    {
        pendingObjects = new List<GameObject>(objects);
        SpawnInitialObjects();
    }

    private void SpawnInitialObjects()
    {
        int objectsToSpawn = Mathf.Min(MAX_SLOTS, pendingObjects.Count);
        for (int i = 0; i < objectsToSpawn; i++)
        {
            GameObject newObj = Instantiate(pendingObjects[0],queueSlots[i].position,pendingObjects[0].transform.rotation);
            pendingObjects.RemoveAt(0);
            
            QueueObject queueObj = new QueueObject(newObj);
            // Thêm vào đầu list (vị trí bên trái)
            objectQueue.Insert(0, queueObj);
            if(!gameManager.slime.Contains(newObj))
            {
                gameManager.slime.Add(newObj);
            }
        }
    }

   

    private Bounds GetObjectBounds(GameObject obj)
    {
        Bounds bounds = new Bounds();
        
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                bounds = new Bounds(rectTransform.position, rectTransform.rect.size);
            }
        }
        
        return bounds;
    }

    public bool IsSelectedObject(GameObject obj)
    {
        return objectQueue.Count > 0 && objectQueue[objectQueue.Count - 1].gameObject == obj;
    }

    public int GetQueueCount()
    {
        return objectQueue.Count;
    }
}

 // Class để lưu thông tin về object trong queue
[System.Serializable] 
public class QueueObject
{
    public GameObject gameObject;
    public Vector3 originalScale;
    public Vector3 targetPosition;
    public bool isMoving;
    public Rigidbody2D rb;
    public MagneticObject magneticObject ;
    
    public QueueObject(GameObject obj)
    {
        gameObject = obj;
        originalScale = obj.transform.localScale;
        isMoving = false;
        if(obj.GetComponent<Rigidbody2D>() != null)
        {
            rb = obj.GetComponent<Rigidbody2D>();
        }
        if(obj.GetComponent<MagneticObject>() != null)
        {
            magneticObject= obj.GetComponent<MagneticObject>();
        }
       
    }
}

