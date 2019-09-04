using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public delegate void DamageHandler(int damage);

public class AcidReactor : MonoBehaviour
{
    public CubeStructureItem structureItem;
    [Range(0.01f,5)]
    public float destroySpeed = 1;

    public event DamageHandler OnDamage;

    private int damage;

    public AcidReactor up, down, right, left, forward, backward;

    private void Start()
    {
        damage = -1;
        CheckEvents();
    }
    private void Update()
    {
        if(damage > 0)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * destroySpeed;
            if(transform.lossyScale.x < 0.05f && transform.lossyScale.y < 0.05f && transform.lossyScale.z < 0.05f)
            {
                OnDamage?.Invoke(damage - 1);
                DestroyCompletely();
                damage = -1;
            }
        }
        else if(damage == 0)
        {
            InstanceDamaged();
        }
    }

    public void CreateUp()
    {
        if(structureItem != null)
        {
            if(up == null)
            {
                GameObject bufer = (GameObject)PrefabUtility.InstantiatePrefab(structureItem.standart);
                bufer.transform.parent = transform.parent;
                bufer.transform.rotation = transform.rotation;
                bufer.transform.position = transform.position + transform.up * transform.localScale.y;
                up = bufer.GetComponent<AcidReactor>();
                up.down = this;
                if(forward != null && forward.up != null && forward.up.backward == null)
                {
                    forward.up.backward = up;
                    up.forward = forward.up;
                    if (forward.up.up != null && forward.up.up.backward != null && forward.up.up.backward.down == null)
                    {
                        forward.up.up.backward.down = up;
                        up.up = forward.up.up.backward;
                        EditorUtility.SetDirty(forward.up.up.backward);
                    }
                    EditorUtility.SetDirty(forward.up.backward);
                }
                if (backward != null && backward.up != null && backward.up.forward == null)
                {
                    backward.up.forward = up;
                    up.backward = backward.up;
                    if (backward.up.up != null && backward.up.up.forward != null && backward.up.up.forward.down == null)
                    {
                        backward.up.up.forward.down = up;
                        up.up = backward.up.up.forward;
                        EditorUtility.SetDirty(backward.up.up.forward);
                    }
                    EditorUtility.SetDirty(backward.up.forward);
                }
                if (left != null && left.up != null && left.up.right == null)
                {
                    left.up.right = up;
                    up.left = left.up;
                    if (left.up.up != null && left.up.up.right != null && left.up.up.right.down == null)
                    {
                        left.up.up.right.down = up;
                        up.up = left.up.up.right;
                        EditorUtility.SetDirty(left.up.up.right);
                    }
                    EditorUtility.SetDirty(left.up.right);
                }
                if (right != null && right.up != null && right.up.left == null)
                {
                    right.up.left = up;
                    up.right = right.up;
                    if (right.up.up != null && right.up.up.left != null && right.up.up.left.down == null)
                    {
                        right.up.up.left.down = up;
                        up.up = right.up.up.left;
                        EditorUtility.SetDirty(right.up.up.left);
                    }
                    EditorUtility.SetDirty(right.up.left);
                }
                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(up);
            }
            else
            {
                Debug.LogWarning("Связь уже существует!");
            }
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }
    }
    public void CreateDown()
    {
        if (structureItem != null)
        {
            if (down == null)
            {
                GameObject bufer = (GameObject)PrefabUtility.InstantiatePrefab(structureItem.standart);
                bufer.transform.parent = transform.parent;
                bufer.transform.rotation = transform.rotation;
                bufer.transform.position = transform.position - transform.up * transform.localScale.y;
                down = bufer.GetComponent<AcidReactor>();
                down.up = this;

                if (forward != null && forward.down != null && forward.down.backward == null)
                {
                    forward.down.backward = down;
                    down.forward = forward.down;
                    if (forward.down.down != null && forward.down.down.backward != null && forward.down.down.backward.up == null)
                    {
                        forward.down.down.backward.up = down;
                        down.down = forward.down.down.backward;
                        EditorUtility.SetDirty(forward.down.down.backward);
                    }
                    EditorUtility.SetDirty(forward.up.backward);
                }
                if (backward != null && backward.down != null && backward.down.forward == null)
                {
                    backward.down.forward = down;
                    down.backward = backward.down;
                    if (backward.down.down != null && backward.down.down.forward != null && backward.down.down.forward.up == null)
                    {
                        backward.down.down.forward.up = down;
                        down.down = backward.down.down.forward;
                        EditorUtility.SetDirty(backward.down.down.forward);
                    }
                    EditorUtility.SetDirty(backward.down.forward);
                }
                if (left != null && left.down != null && left.down.right == null)
                {
                    left.down.right = down;
                    down.left = left.down.right;
                    if (left.down.down != null && left.down.down.right != null && left.down.down.right.up == null)
                    {
                        left.down.down.right.up = down;
                        down.down = left.down.down.right;
                        EditorUtility.SetDirty(left.down.down.right);
                    }
                    EditorUtility.SetDirty(left.down.right);
                }
                if (right != null && right.down != null && right.down.left == null)
                {
                    right.down.left = down;
                    down.right = right.down;
                    if (right.down.down != null && right.down.down.left != null && right.down.down.left.up == null)
                    {
                        right.down.down.left.up = down;
                        down.down = right.down.down.left;
                        EditorUtility.SetDirty(left.down.down.left);
                    }
                    EditorUtility.SetDirty(right.down.left);
                }

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(down);
            }
            else
            {
                Debug.LogWarning("Связь уже существует!");
            }
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }
        
    }
    public void CreateForward()
    {
        if (structureItem != null)
        {
            if (forward == null)
            {
                GameObject bufer = (GameObject)PrefabUtility.InstantiatePrefab(structureItem.standart);
                bufer.transform.parent = transform.parent;
                bufer.transform.rotation = transform.rotation;
                bufer.transform.position = transform.position + transform.forward * transform.localScale.z;
                forward = bufer.GetComponent<AcidReactor>();
                forward.backward = this;

                if (up != null && up.forward != null && up.forward.down == null)
                {
                    up.forward.down = forward;
                    forward.up = up.forward.down;
                    if(up.forward.forward != null && up.forward.forward.down != null && up.forward.forward.down.backward == null)
                    {
                        up.forward.forward.down.backward = forward;
                        forward.forward = up.forward.forward.down;
                        EditorUtility.SetDirty(up.forward.forward.down);
                    }
                    EditorUtility.SetDirty(up.forward.down);
                }
                if (down != null && down.forward != null && down.forward.up == null)
                {
                    down.forward.up = forward;
                    forward.down = down.forward;
                    if (down.forward.forward != null && down.forward.forward.up != null && down.forward.forward.up.backward == null)
                    {
                        down.forward.forward.up.backward = forward;
                        forward.forward = down.forward.forward.up;
                        EditorUtility.SetDirty(down.forward.forward.up);
                    }
                    EditorUtility.SetDirty(down.forward.up);
                }
                if (left != null && left.forward != null && left.forward.right == null)
                {
                    left.forward.right = forward;
                    forward.left = left.forward;
                    if (left.forward.forward != null && left.forward.forward.right != null && left.forward.forward.right.backward == null)
                    {
                        left.forward.forward.right.backward = forward;
                        forward.forward = left.forward.forward.right;
                        EditorUtility.SetDirty(left.forward.forward.right);
                    }
                    EditorUtility.SetDirty(left.forward.right);
                }
                if (right != null && right.forward != null && right.forward.left == null)
                {
                    right.forward.left = forward;
                    forward.right = right.forward;
                    if (right.forward.forward != null && right.forward.forward.left != null && right.forward.forward.left.backward == null)
                    {
                        right.forward.forward.left.backward = forward;
                        forward.forward = right.forward.forward.left;
                        EditorUtility.SetDirty(left.forward.forward.right);
                    }
                    EditorUtility.SetDirty(right.forward.left);
                }

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(forward);
            }
            else
            {
                Debug.LogWarning("Связь уже существует!");
            }
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }
        
    }
    public void CreateBackward()
    {
        if (structureItem != null)
        {
            if (backward == null)
            {
                GameObject bufer = (GameObject)PrefabUtility.InstantiatePrefab(structureItem.standart);
                bufer.transform.parent = transform.parent;
                bufer.transform.rotation = transform.rotation;
                bufer.transform.position = transform.position - transform.forward * transform.localScale.z;
                backward = bufer.GetComponent<AcidReactor>();
                backward.forward = this;

                if (up != null && up.backward != null && up.backward.down == null)
                {
                    up.backward.down = backward;
                    backward.up = up.backward;
                    if (up.backward.backward != null && up.backward.backward.down!= null && up.backward.backward.down.forward == null)
                    {
                        up.backward.backward.down.forward = backward;
                        backward.backward = up.backward.backward.down;
                        EditorUtility.SetDirty(up.backward.backward.down);
                    }
                    EditorUtility.SetDirty(up.backward.down);
                }
                if (down != null && down.backward != null && down.backward.up == null)
                {
                    down.backward.up = backward;
                    backward.down = down.backward;
                    if (down.backward.backward != null && down.backward.backward.up != null && down.backward.backward.up.forward == null)
                    {
                        down.backward.backward.up.forward = backward;
                        backward.backward = down.backward.backward.up;
                        EditorUtility.SetDirty(down.backward.backward.up);
                    }
                    EditorUtility.SetDirty(backward.down.up);
                }
                if (left != null && left.backward != null && left.backward.right == null)
                {
                    left.backward.right = backward;
                    backward.left = left.backward;
                    if (left.backward.backward != null && left.backward.backward.right != null && left.backward.backward.right.forward == null)
                    {
                        left.backward.backward.right.forward = backward;
                        backward.backward = left.backward.backward.right;
                        EditorUtility.SetDirty(left.backward.backward.right);
                    }
                    EditorUtility.SetDirty(left.backward.right);
                }
                if (right != null && right.backward != null && right.backward.left == null)
                {
                    right.backward.left = backward;
                    backward.right = right.backward;
                    if (right.backward.backward != null && right.backward.backward.left != null && right.backward.backward.left.forward == null)
                    {
                        right.backward.backward.left.forward = backward;
                        backward.backward = right.backward.backward.left;
                        EditorUtility.SetDirty(right.backward.backward.left);
                    }
                    EditorUtility.SetDirty(right.backward.left);
                }

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(backward);
            }
            else
            {
                Debug.LogWarning("Связь уже существует!");
            }
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }
        
    }
    public void CreateRight()
    {
        if (structureItem != null)
        {
            if (right == null)
            {
                GameObject bufer = (GameObject)PrefabUtility.InstantiatePrefab(structureItem.standart);
                bufer.transform.parent = transform.parent;
                bufer.transform.rotation = transform.rotation;
                bufer.transform.position = transform.position + transform.right * transform.localScale.x;
                right = bufer.GetComponent<AcidReactor>();
                right.left = this;

                if (up != null && up.right != null && up.right.down == null)
                {
                    up.right.down = right;
                    right.up = up.right;
                    if (up.right.right != null && up.right.right.down != null && up.right.right.down.left == null)
                    {
                        up.right.right.down.left = right;
                        right.right = up.right.right.down;
                        EditorUtility.SetDirty(up.right.right.down);
                    }
                    EditorUtility.SetDirty(up.right.down);
                }
                if (down != null && down.right != null && down.right.up == null)
                {
                    down.right.up = right;
                    right.down = down.right;
                    if (down.right.right != null && down.right.right.up != null && down.right.right.up.left == null)
                    {
                        down.right.right.up.left = right;
                        right.right = down.right.right.up;
                        EditorUtility.SetDirty(down.right.right.up);
                    }
                    EditorUtility.SetDirty(down.right.up);
                }
                if (forward != null && forward.right != null && forward.right.backward == null)
                {
                    forward.right.backward = right;
                    right.forward = forward.right;
                    if (forward.right.right != null && forward.right.right.backward != null && forward.right.right.backward.left == null)
                    {
                        forward.right.right.backward.left = right;
                        right.right = forward.right.right.backward;
                        EditorUtility.SetDirty(forward.right.right.backward);
                    }
                    EditorUtility.SetDirty(forward.right.backward);
                }
                if (backward != null && backward.right != null && backward.right.forward == null)
                {
                    backward.right.forward = right;
                    right.backward = backward.right;
                    if (backward.right.right != null && backward.right.right.forward != null && backward.right.right.forward.left == null)
                    {
                        backward.right.right.forward.left = right;
                        right.right = backward.right.right.forward;
                        EditorUtility.SetDirty(backward.right.right.forward);
                    }
                    EditorUtility.SetDirty(backward.right.forward);
                }

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(right);
            }
            else
            {
                Debug.LogWarning("Связь уже существует!");
            }
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }
        
    }
    public void CreateLeft()
    {
        if (structureItem != null)
        {
            if (left == null)
            {
                GameObject bufer = (GameObject)PrefabUtility.InstantiatePrefab(structureItem.standart);
                bufer.transform.parent = transform.parent;
                bufer.transform.rotation = transform.rotation;
                bufer.transform.position = transform.position - transform.right * transform.localScale.x;
                left = bufer.GetComponent<AcidReactor>();
                left.right = this;

                if (up != null && up.left != null && up.left.down == null)
                {
                    up.left.down = left;
                    left.up = up.left;
                    if (up.left.left != null && up.left.left.down != null && up.left.left.down.right == null)
                    {
                        up.left.left.down.right = left;
                        left.left = up.left.left.down;
                        EditorUtility.SetDirty(up.left.left.down.right);
                    }
                    EditorUtility.SetDirty(up.left.down);
                }
                if (down != null && down.left != null && down.left.up == null)
                {
                    down.left.up = left;
                    left.down = down.left;
                    if (down.left.left != null && down.left.left.up != null && down.left.left.up.right == null)
                    {
                        down.left.left.up.right = left;
                        left.left = down.left.left.up;
                        EditorUtility.SetDirty(down.left.left.up.right);
                    }
                    EditorUtility.SetDirty(down.left.up);
                }
                if (forward != null && forward.left != null && forward.left.backward == null)
                {
                    forward.left.backward = left;
                    left.forward = forward.left;
                    if (forward.left.left != null && forward.left.left.backward != null && forward.left.left.backward.right == null)
                    {
                        forward.left.left.backward.right = left;
                        left.left = forward.left.left.backward;
                        EditorUtility.SetDirty(forward.left.left.backward.right);
                    }
                    EditorUtility.SetDirty(forward.left.right);
                }
                if (backward != null && backward.left != null && backward.left.forward == null)
                {
                    backward.left.forward = left;
                    left.backward = backward.left;
                    if (backward.left.left != null && backward.left.left.forward != null && backward.left.left.forward.right == null)
                    {
                        backward.left.left.forward.right = left;
                        left.left = backward.left.left.forward;
                        EditorUtility.SetDirty(backward.left.left.forward.right);
                    }
                    EditorUtility.SetDirty(backward.left.forward);
                }

                EditorUtility.SetDirty(this);
                EditorUtility.SetDirty(left);
            }
            else
            {
                Debug.LogWarning("Связь уже существует!");
            }
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }

    }
    public void GetDamage(int point)
    {
        damage = point;
    }


    private void CheckEvents()
    {
        if (up != null)
        {
            up.OnDamage += GetDamage;
            OnDamage += up.GetDamage;
        }
        if (down != null)
        {
            down.OnDamage += GetDamage;
            OnDamage += down.GetDamage;
        }
        if (forward != null)
        {
            forward.OnDamage += GetDamage;
            OnDamage += forward.GetDamage;
        }
        if (backward != null)
        {
            backward.OnDamage += GetDamage;
            OnDamage += backward.GetDamage;
        }
        if (right != null)
        {
            right.OnDamage += GetDamage;
            OnDamage += right.GetDamage;
        }
        if (left != null)
        {
            left.OnDamage += GetDamage;
            OnDamage += left.GetDamage;
        }
    }
    private void InstanceDamaged()
    {
        if (structureItem != null)
        {
            GameObject bufer = Instantiate(structureItem.destroyed, transform.position, transform.rotation, transform.parent);
            AcidReactor acidReactor = bufer.GetComponent<AcidReactor>();
            if (up != null)
            {
                up.OnDamage -= GetDamage;
                up.down = null;
                up.down = acidReactor;
                acidReactor.up = up;
                up.OnDamage += acidReactor.OnDamage;
            }
            if (down != null)
            {
                down.OnDamage -= GetDamage;
                down.up = null;
                down.up = acidReactor;
                acidReactor.down = down;
                down.OnDamage += acidReactor.OnDamage;
            }
            if (forward != null)
            {
                forward.OnDamage -= GetDamage;
                forward.backward = null;
                forward.backward = acidReactor;
                acidReactor.forward = forward;
                forward.OnDamage += acidReactor.OnDamage;
            }
            if (backward != null)
            {
                backward.OnDamage -= GetDamage;
                backward.forward = null;
                backward.forward = acidReactor;
                acidReactor.backward = backward;
                backward.OnDamage += acidReactor.OnDamage;
            }
            if (right != null)
            {
                right.OnDamage -= GetDamage;
                right.left = null;
                right.left = acidReactor;
                acidReactor.right = right;
                right.OnDamage += acidReactor.OnDamage;
            }
            if (left != null)
            {
                left.OnDamage -= GetDamage;
                left.right = null;
                left.right = acidReactor;
                acidReactor.left = left;
                left.OnDamage += acidReactor.OnDamage;
            }
            OnDamage = null;
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Не заполнен параметр structureItem объекта " + gameObject.name + ". Действие невозможно.");
        }
    }
    private void DestroyCompletely()
    {
        if(up != null)
        {
            up.OnDamage -= GetDamage;
            up.down = null;
        }
        if (down != null)
        {
            down.OnDamage -= GetDamage;
            down.up = null;
        }
        if (forward != null)
        {
            forward.OnDamage -= GetDamage;
            forward.backward = null;
        }
        if (backward != null)
        {
            backward.OnDamage -= GetDamage;
            backward.forward = null;
        }
        if (right != null)
        {
            right.OnDamage -= GetDamage;
            right.left = null;
        }
        if (left != null)
        {
            left.OnDamage -= GetDamage;
            left.right = null;
        }
        OnDamage = null;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (up != null)
        {
            Gizmos.DrawLine(transform.position, up.transform.position);
        }
        if (down != null)
        {
            Gizmos.DrawLine(transform.position, down.transform.position);
        }
        if (forward != null)
        {
            Gizmos.DrawLine(transform.position, forward.transform.position);
        }
        if (backward != null)
        {
            Gizmos.DrawLine(transform.position, backward.transform.position);
        }
        if (right != null)
        {
            Gizmos.DrawLine(transform.position, right.transform.position);
        }
        if (left != null)
        {
            Gizmos.DrawLine(transform.position, left.transform.position);
        }
    }
}
