using System;
using UnityEngine;

[Serializable]
public struct Box
{
    public Vector2 size;
    public Vector2 center;
    public float sizeX { get => size.x; set { size.x = value; } }
    public float sizeY { get => size.y; set { size.y = value; } }

    public float centerX { get => center.x; set { center.x = value; } }
    public float centerY { get => center.y; set { center.y = value; } }

    public float Left => center.x - 0.5f * size.x;
    public float Right => center.x + 0.5f * size.x;
    public float Top => center.y + 0.5f * size.y;
    public float Bottom => center.y - 0.5f * size.y;

    public Vector2 RightUp => new Vector2(Right, Top);
    public Vector2 LeftUp => new Vector2(Left, Top);
    public Vector2 RightDown => new Vector2(Right, Bottom);
    public Vector2 LeftDown => new Vector2(Left, Bottom);

    public Box(Vector2 size, Vector2 center)
    {
        this.size = size;
        this.center = center;
    }
    public Box(Collider2D coll)
    {
        size = coll.bounds.size;
        center = coll.offset;
    }

    public Vector2 GetCenterPos(Vector2 pos)
    {
        return pos + center;
    }

    #region Contact

    /// <param name="y">box�� Y ��ǥ </param>
    /// <returns>Box�� line�� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactUp(float y, float line) => line < y + center.y + 0.5f * size.y;

    /// <param name="y">box�� Y ��ǥ </param>
    /// <returns>Box�� line�� ���ϰų� �� �Ʒ��� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactDown(float y, float line) => line > y + center.y - 0.5f * size.y;

    /// <param name="x">box�� X ��ǥ</param>
    /// <returns>Box�� line�� ���ϰų� �׺��� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactRight(float x, float line) => line < x + center.x + 0.5f * size.x;

    /// <param name="x">box�� X ��ǥ</param>
    /// <returns>Box�� line�� ���ϰų� �׺��� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactLeft(float x, float line) => line > x + center.x - 0.5f * size.x;

    /// <param name="pos">box�� WorldPosition</param>
    /// <param name="up">�� ���ؼ�</param>
    /// <param name="down">�Ʒ� ���ؼ�</param>
    /// <param name="right">���� ���ؼ�</param>
    /// <param name="left">���� ���ؼ�</param>
    /// <returns>Box�� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContact(Vector2 pos, float up, float down, float right, float left)
    {
        return IsContactGameTop(pos.y)
            || IsContactGameBottom(pos.y)
            || IsContactGameRight(pos.x)
            || IsContactGameLeft(pos.x);
    }

    #endregion

    #region Contact + Out

    /// <param name="y">box�� Y ��ǥ </param>
    /// <param name="line">�� ���ؼ�</param>
    /// <returns>Box�� line�� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactTop(float y, float line, out float contact)
    {
        contact = line - center.y - 0.5f * size.y;
        return y > contact;
    }

    /// <param name="y">box�� Y ��ǥ</param>
    /// <param name="line">�Ʒ� ���ؼ�</param>
    /// <returns>Box�� line�� ���ϰų� �� �Ʒ��� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactBottom(float y, float line, out float contact)
    {
        contact = line - center.y + 0.5f * size.y;
        return y < contact;
    }

    /// <param name="x">box�� X ��ǥ</param>
    /// <param name="line">���� ���ؼ�</param>
    /// <returns>Box�� line�� ���ϰų� �׺��� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactRight(float x, float line, out float contact)
    {
        contact = line - center.x - 0.5f * size.x;
        return x > contact;
    }

    /// <param name="x">box�� X ��ǥ</param>
    /// <param name="line">���� ���ؼ�</param>
    /// <returns>Box�� line�� ���ϰų� �׺��� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactLeft(float x, float line, out float contact)
    {
        contact = line - center.x + 0.5f * size.x;
        return x < contact;
    }

    /// <param name="pos">box�� WorldPosition</param>
    /// <param name="top">�� ���ؼ�</param>
    /// <param name="bottom">�Ʒ� ���ؼ�</param>
    /// <param name="right">���� ���ؼ�</param>
    /// <param name="left">���� ���ؼ�</param>
    /// <param name="contact"></param>
    /// <returns>Box�� ���ؼ��� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�. // false�� �� contact = pos</returns>
    public bool IsContact(Vector2 pos, float top, float bottom, float right, float left, out Vector2 contact)
    {
        /*if (IsContactUp(pos.y, up, out float contactUp))
        {
            contact = new Vector2(pos.x, contactUp);
            return true;
        }
        if (IsContactDown(pos.y, down, out float contactDown))
        {
            contact = new Vector2(pos.x, contactDown);
            return true;
        }
        if (IsContactRight(pos.x, right, out float contactRight))
        {
            contact = new Vector2(contactRight, pos.y);
            return true;
        }
        if (IsContactLeft(pos.x, left, out float contactLeft))
        {
            contact = new Vector2(contactLeft, pos.y);
            return true;
        }
        contact = pos;
        return false;*/

        bool result = false;

        contact = pos;

        if (IsContactTop(pos.y, top, out float contactUp))
        {
            contact.y = contactUp;
            result = true;
        }
        else if (IsContactBottom(pos.y, bottom, out float contactDown))
        {
            contact.y = contactDown;
            result = true;
        }
        if (IsContactRight(pos.x, right, out float contactRight))
        {
            contact.x = contactRight;
            result = true;
        }
        else if (IsContactLeft(pos.x, left, out float contactLeft))
        {
            contact.x = contactLeft;
            result = true;
        }
        return result;

    }

    /// <param name="pos">box�� WorldPosition</param>
    /// <param name="up">�� ���ؼ�</param>
    /// <param name="down">�Ʒ� ���ؼ�</param>
    /// <param name="right">���� ���ؼ�</param>
    /// <param name="left">���� ���ؼ�</param>
    /// <param name="contact"></param>
    /// <returns>Box�� ���ؼ��� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�. // false�� �� contact = pos</returns>
    public bool IsContact(Vector3 pos, float up, float down, float right, float left, out Vector3 contact)
    {
        bool result = false;

        contact = pos;

        if (IsContactTop(pos.y, up, out contact.y)) result = true;
        else if (IsContactBottom(pos.y, down, out contact.y)) result = true;

        if (IsContactRight(pos.x, right, out contact.x)) result = true;
        else if (IsContactLeft(pos.x, left, out contact.x)) result = true;
        
        return result;
    }
    #endregion

    #region ContactGame

    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameTop(float x) => IsContactUp(x, Window.gameUp);

    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� �Ʒ��� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameBottom(float x) => IsContactDown(x, Window.gameDown);

    /// <returns>Box�� ���� �ܰ����� ���ϰų� �׺��� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameRight(float y) => IsContactRight(y, Window.gameRight);

    /// <returns>Box�� ���� �ܰ����� ���ϰų� �׺��� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameLeft(float y) => IsContactLeft(y, Window.gameLeft);

    /// <param name="pos">box�� WorldPosition</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGame(Vector2 pos)
    {
        return IsContactGameTop(pos.y)
            || IsContactGameBottom(pos.y)
            || IsContactGameRight(pos.x)
            || IsContactGameLeft(pos.x);
    }

    #endregion

    #region ContactGame + Out

    /// <param name="x">box�� X ��ǥ</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameTop(float x, out float contact) => IsContactTop(x, Window.gameUp, out contact);

    /// <param name="x">box�� X ��ǥ</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� �Ʒ��� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameBottom(float x, out float contact) => IsContactBottom(x, Window.gameDown, out contact);

    /// <param name="y">box�� Y ��ǥ</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �׺��� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameRight(float y, out float contact) => IsContactRight(y, Window.gameRight, out contact);

    /// <param name="y">box�� Y ��ǥ</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �׺��� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsContactGameLeft(float y, out float contact) => IsContactLeft(y, Window.gameLeft, out contact);

    /// <param name="pos">box�� WorldPosition</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�. // false�� �� contact = pos</returns>
    public bool IsContactGame(Vector2 pos, out Vector2 contact)
    {
        return IsContact(pos, Window.gameUp, Window.gameDown, Window.gameRight, Window.gameLeft, out contact);
    }

    /// <param name="pos">box�� WorldPosition</param>
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�. // false�� �� contact = pos</returns>
    public bool IsContactGame(Vector3 pos, out Vector3 contact)
    {
        return IsContact(pos, Window.gameUp, Window.gameDown, Window.gameRight, Window.gameLeft, out contact);
    }

    #endregion

    #region Exit

    /// <returns>Box�� line���� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitTop(float y, float line) => line < y + center.y - 0.5f * size.y;
    
    /// <returns>Box�� line���� �Ʒ��� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitBottom(float y, float line) => line > y + center.y + 0.5f * size.y;
    
    /// <returns>Box�� line���� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitRight(float x, float line) => line < x + center.x - 0.5f * size.x;

    /// <returns>Box�� line���� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitLeft(float x, float line) => line > x + center.x + 0.5f * size.x;

    /// <param name="pos">box�� WorldPosition</param>
    /// <param name="top">�� ���ؼ�</param>
    /// <param name="bottom">�Ʒ� ���ؼ�</param>
    /// <param name="right">���� ���ؼ�</param>
    /// <param name="left">���� ���ؼ�</param>
    /// <returns>Box�� ���ؼ� �ۿ� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExit(Vector2 pos, float top, float bottom, float right, float left)
    {
        return IsExitTop(pos.y, top) 
            || IsExitBottom(pos.y, bottom) 
            || IsExitRight(pos.x, right) 
            || IsExitLeft(pos.x, left);
    }

    #endregion

    /*
    #region Exit + Out

    /// <returns>Box�� line���� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitUp(float y, float line, out float contact)
    {
        contact = line - center.y + 0.5f * size.y;
        return y >= contact;
    }
    /// <returns>Box�� line���� �Ʒ��� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitDown(float y, float line, out float contact)
    {
        contact = line - center.y - 0.5f * size.y;
        return y <= contact;
    }
    /// <returns>Box�� line���� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitRight(float x, float line, out float contact)
    {
        contact = line - center.x + 0.5f * size.x;
        return x >= contact;
    }

    /// <returns>Box�� line���� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitLeft(float x, float line, out float contact)
    {
        contact = line - center.x - 0.5f * size.x;
        return x <= contact;
    }

    /// <param name="pos">box�� WorldPosition</param>
    /// <param name="up">�� ���ؼ�</param>
    /// <param name="down">�Ʒ� ���ؼ�</param>
    /// <param name="right">���� ���ؼ�</param>
    /// <param name="left">���� ���ؼ�</param>
    /// <param name="contact">���� ��ġ</param>
    /// <returns>Box�� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�. // false�� �� contact = pos</returns>
    public bool IsExit(Vector2 pos, float up, float down, float right, float left, out Vector2 contact)
    {
        if (IsExitUp(pos.y, up, out float contactUp))
        {
            contact = new Vector2(pos.x, contactUp);
            return true;
        }
        if (IsExitDown(pos.y, down, out float contactDown))
        {
            contact = new Vector2(pos.x, contactDown);
            return true;
        }
        if (IsExitRight(pos.x, right, out float contactRight))
        {
            contact = new Vector2(contactRight, pos.y);
            return true;
        }
        if (IsExitLeft(pos.x, left, out float contactLeft))
        {
            contact = new Vector2(contactLeft, pos.y);
            return true;
        }
        contact = pos;
        return false;
    }

    #endregion
    */

    #region ExitGame

    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitGameTop(float x) => IsExitTop(x, Window.gameUp);

    /// <returns>Box�� ���� �ܰ����� ���ϰų� �� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitGameBottom(float x) => IsExitBottom(x, Window.gameDown);
    
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �׺��� �������� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitGameRight(float y) => IsExitRight(y, Window.gameRight);
    
    /// <returns>Box�� ���� �ܰ����� ���ϰų� �׺��� ������ �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitGameLeft(float y) => IsExitLeft(y, Window.gameLeft);

    /// <param name="pos">box�� WorldPosition</param>
    /// <returns>Box�� ���� �ܰ��� �ۿ� ���� �� true�� ��ȯ�մϴ�.</returns>
    public bool IsExitGame(Vector2 pos)
    {
        return IsExitGameTop(pos.y)
            || IsExitGameBottom(pos.y)
            || IsExitGameRight(pos.x)
            || IsExitGameLeft(pos.x);
    }

    #endregion
}