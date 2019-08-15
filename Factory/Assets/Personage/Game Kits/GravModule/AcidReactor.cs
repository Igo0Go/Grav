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
                GameObject bufer = Instantiate(structureItem.standart, transform.position + transform.up * transform.localScale.y, transform.rotation, transform.parent);
                up = bufer.GetComponent<AcidReactor>();
                up.down = this;
                if(forward != null && forward.up != null && forward.up.backward == null)
                {
                    forward.up.backward = up;
                    up.forward = forward.up;
                    EditorUtility.SetDirty(forward.up.backward);
                }
                if (backward != null && backward.up != null && backward.up.forward == null)
                {
                    backward.up.forward = up;
                    up.backward = backward.up;
                    EditorUtility.SetDirty(backward.up.forward);
                }
                if (left != null && left.up != null && left.up.right == null)
                {
                    left.up.right = up;
                    up.left = left.up;
                    EditorUtility.SetDirty(left.up.right);
                }
                if (right != null && right.up != null && right.up.left == null)
                {
                    right.up.left = up;
                    up.right = right.up;
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
                GameObject bufer = Instantiate(structureItem.standart, transform.position - transform.up * transform.localScale.y, transform.rotation, transform.parent);
                down = bufer.GetComponent<AcidReactor>();
                down.up = this;

                if (forward != null && forward.down != null && forward.down.backward == null)
                {
                    forward.down.backward = down;
                    down.forward = forward.down;
                    EditorUtility.SetDirty(forward.up.backward);
                }
                if (backward != null && backward.down != null && backward.down.forward == null)
                {
                    backward.down.forward = down;
                    down.backward = backward.down;
                    EditorUtility.SetDirty(backward.down.forward);
                }
                if (left != null && left.down != null && left.down.right == null)
                {
                    left.down.right = down;
                    down.left = left.down.right;
                    EditorUtility.SetDirty(left.down.right);
                }
                if (right != null && right.down != null && right.down.left == null)
                {
                    right.down.left = down;
                    down.right = right.down;
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
                GameObject bufer = Instantiate(structureItem.standart, transform.position + transform.forward * transform.localScale.z, transform.rotation, transform.parent);
                forward = bufer.GetComponent<AcidReactor>();
                forward.backward = this;

                if (up != null && up.forward != null && up.forward.down == null)
                {
                    up.forward.down = forward;
                    forward.up = up.forward.down;
                    EditorUtility.SetDirty(up.forward.down);
                }
                if (down != null && down.forward != null && down.forward.up == null)
                {
                    down.forward.up = forward;
                    forward.down = down.forward;
                    EditorUtility.SetDirty(backward.down.up);
                }
                if (left != null && left.forward != null && left.forward.right == null)
                {
                    left.forward.right = forward;
                    forward.left = left.forward;
                    EditorUtility.SetDirty(left.forward.right);
                }
                if (right != null && right.forward != null && right.forward.left == null)
                {
                    right.forward.left = forward;
                    forward.right = right.forward;
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
                GameObject bufer = Instantiate(structureItem.standart, transform.position - transform.forward * transform.localScale.z, transform.rotation, transform.parent);
                backward = bufer.GetComponent<AcidReactor>();
                backward.forward = this;

                if (up != null && up.backward != null && up.backward.down == null)
                {
                    up.backward.down = backward;
                    backward.up = up.backward;
                    EditorUtility.SetDirty(up.backward.down);
                }
                if (down != null && down.backward != null && down.backward.up == null)
                {
                    down.backward.up = forward;
                    backward.down = down.backward;
                    EditorUtility.SetDirty(backward.down.up);
                }
                if (left != null && left.backward != null && left.backward.right == null)
                {
                    left.backward.right = forward;
                    backward.left = left.backward;
                    EditorUtility.SetDirty(left.backward.right);
                }
                if (right != null && right.backward != null && right.backward.left == null)
                {
                    right.backward.left = forward;
                    backward.right = right.backward;
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
                GameObject bufer = Instantiate(structureItem.standart, transform.position + transform.right * transform.localScale.x, transform.rotation, transform.parent);
                right = bufer.GetComponent<AcidReactor>();
                right.left = this;

                if (up != null && up.right != null && up.right.down == null)
                {
                    up.right.down = right;
                    right.up = up.right;
                    EditorUtility.SetDirty(up.right.down);
                }
                if (down != null && down.right != null && down.right.up == null)
                {
                    down.right.up = right;
                    right.down = down.right;
                    EditorUtility.SetDirty(down.right.up);
                }
                if (forward != null && forward.right != null && forward.right.backward == null)
                {
                    forward.right.backward = right;
                    right.forward = forward.right;
                    EditorUtility.SetDirty(forward.right.right);
                }
                if (backward != null && backward.right != null && backward.right.forward == null)
                {
                    backward.right.forward = right;
                    right.backward = backward.right;
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
                GameObject bufer = Instantiate(structureItem.standart, transform.position - transform.right * transform.localScale.x, transform.rotation, transform.parent);
                left = bufer.GetComponent<AcidReactor>();
                left.right = this;

                if (up != null && up.left != null && up.left.down == null)
                {
                    up.left.down = left;
                    left.up = up.left;
                    EditorUtility.SetDirty(up.left.down);
                }
                if (down != null && down.left != null && down.left.up == null)
                {
                    down.left.up = left;
                    left.down = down.left;
                    EditorUtility.SetDirty(down.left.up);
                }
                if (forward != null && forward.left != null && forward.left.backward == null)
                {
                    forward.left.backward = left;
                    left.forward = forward.left;
                    EditorUtility.SetDirty(forward.left.right);
                }
                if (backward != null && backward.left != null && backward.left.forward == null)
                {
                    backward.left.forward = left;
                    left.backward = backward.left;
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
