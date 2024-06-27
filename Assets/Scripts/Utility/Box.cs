using System;
using UnityEngine;

[Serializable]
public struct Box
{
    public Vector2 size;
    public Vector2 center;

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

    #region Contact

    /// <param name="y">box의 Y 좌표 </param>
    /// <returns>Box가 line에 접하거나 그 위일 때 true를 반환합니다.</returns>
    public bool IsContactUp(float y, float line) => line < y + center.y + 0.5f * size.y;

    /// <param name="y">box의 Y 좌표 </param>
    /// <returns>Box가 line에 접하거나 그 아래일 때 true를 반환합니다.</returns>
    public bool IsContactDown(float y, float line) => line > y + center.y - 0.5f * size.y;

    /// <param name="x">box의 X 좌표</param>
    /// <returns>Box가 line에 접하거나 그보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsContactRight(float x, float line) => line < x + center.x + 0.5f * size.x;

    /// <param name="x">box의 X 좌표</param>
    /// <returns>Box가 line에 접하거나 그보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsContactLeft(float x, float line) => line > x + center.x - 0.5f * size.x;

    /// <param name="pos">box의 WorldPosition</param>
    /// <param name="up">위 기준선</param>
    /// <param name="down">아래 기준선</param>
    /// <param name="right">우측 기준선</param>
    /// <param name="left">좌측 기준선</param>
    /// <returns>Box가 접하거나 그 밖일 때 true를 반환합니다.</returns>
    public bool IsContact(Vector2 pos, float up, float down, float right, float left)
    {
        return IsContactGameUp(pos.y)
            || IsContactGameDown(pos.y)
            || IsContactGameRight(pos.x)
            || IsContactGameLeft(pos.x);
    }

    #endregion

    #region Contact + Out

    /// <param name="y">box의 Y 좌표 </param>
    /// <param name="line">위 기준선</param>
    /// <returns>Box가 line에 접하거나 그 위일 때 true를 반환합니다.</returns>
    public bool IsContactUp(float y, float line, out float contact)
    {
        contact = line - center.y - 0.5f * size.y;
        return y >= contact;
    }

    /// <param name="y">box의 Y 좌표</param>
    /// <param name="line">아래 기준선</param>
    /// <returns>Box가 line에 접하거나 그 아래일 때 true를 반환합니다.</returns>
    public bool IsContactDown(float y, float line, out float contact)
    {
        contact = line - center.y + 0.5f * size.y;
        return y <= contact;
    }

    /// <param name="x">box의 X 좌표</param>
    /// <param name="line">우측 기준선</param>
    /// <returns>Box가 line에 접하거나 그보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsContactRight(float x, float line, out float contact)
    {
        contact = line - center.x - 0.5f * size.x;
        return x >= contact;
    }

    /// <param name="x">box의 X 좌표</param>
    /// <param name="line">좌측 기준선</param>
    /// <returns>Box가 line에 접하거나 그보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsContactLeft(float x, float line, out float contact)
    {
        contact = line - center.x + 0.5f * size.x;
        return x <= contact;
    }

    /// <param name="pos">box의 WorldPosition</param>
    /// <param name="up">위 기준선</param>
    /// <param name="down">아래 기준선</param>
    /// <param name="right">우측 기준선</param>
    /// <param name="left">좌측 기준선</param>
    /// <param name="contact"></param>
    /// <returns>Box가 기준선에 접하거나 그 밖일 때 true를 반환합니다. // false일 때 contact = pos</returns>
    public bool IsContact(Vector2 pos, float up, float down, float right, float left, out Vector2 contact)
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

        if (IsContactUp(pos.y, up, out float contactUp))
        {
            contact.y = contactUp;
            result = true;
        }
        else if (IsContactDown(pos.y, down, out float contactDown))
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

    /// <param name="pos">box의 WorldPosition</param>
    /// <param name="up">위 기준선</param>
    /// <param name="down">아래 기준선</param>
    /// <param name="right">우측 기준선</param>
    /// <param name="left">좌측 기준선</param>
    /// <param name="contact"></param>
    /// <returns>Box가 기준선에 접하거나 그 밖일 때 true를 반환합니다. // false일 때 contact = pos</returns>
    public bool IsContact(Vector3 pos, float up, float down, float right, float left, out Vector3 contact)
    {
        bool result = false;

        contact = pos;

        if (IsContactUp(pos.y, up, out float contactUp))
        {
            contact.y = contactUp;
            result = true;
        }
        else if (IsContactDown(pos.y, down, out float contactDown))
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
    #endregion

    #region ContactGame

    /// <returns>Box가 게임 외곽선에 접하거나 그 위일 때 true를 반환합니다.</returns>
    public bool IsContactGameUp(float x) => IsContactUp(x, Window.GameUp);

    /// <returns>Box가 게임 외곽선에 접하거나 그 아래일 때 true를 반환합니다.</returns>
    public bool IsContactGameDown(float x) => IsContactDown(x, Window.GameDown);

    /// <returns>Box가 게임 외곽선에 접하거나 그보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsContactGameRight(float y) => IsContactRight(y, Window.GameRight);

    /// <returns>Box가 게임 외곽선에 접하거나 그보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsContactGameLeft(float y) => IsContactLeft(y, Window.GameLeft);

    /// <param name="pos">box의 WorldPosition</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그 밖일 때 true를 반환합니다.</returns>
    public bool IsContactGame(Vector2 pos)
    {
        return IsContactGameUp(pos.y)
            || IsContactGameDown(pos.y)
            || IsContactGameRight(pos.x)
            || IsContactGameLeft(pos.x);
    }

    #endregion

    #region ContactGame + Out

    /// <param name="x">box의 X 좌표</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그 위일 때 true를 반환합니다.</returns>
    public bool IsContactGameUp(float x, out float contact) => IsContactUp(x, Window.GameUp, out contact);

    /// <param name="x">box의 X 좌표</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그 아래일 때 true를 반환합니다.</returns>
    public bool IsContactGameDown(float x, out float contact) => IsContactDown(x, Window.GameDown, out contact);

    /// <param name="y">box의 Y 좌표</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsContactGameRight(float y, out float contact) => IsContactRight(y, Window.GameRight, out contact);

    /// <param name="y">box의 Y 좌표</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsContactGameLeft(float y, out float contact) => IsContactLeft(y, Window.GameLeft, out contact);

    /// <param name="pos">box의 WorldPosition</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그 밖일 때 true를 반환합니다. // false일 때 contact = pos</returns>
    public bool IsContactGame(Vector2 pos, out Vector2 contact)
    {
        return IsContact(pos, Window.GameUp, Window.GameDown, Window.GameRight, Window.GameLeft, out contact);
    }

    /// <param name="pos">box의 WorldPosition</param>
    /// <returns>Box가 게임 외곽선에 접하거나 그 밖일 때 true를 반환합니다. // false일 때 contact = pos</returns>
    public bool IsContactGame(Vector3 pos, out Vector3 contact)
    {
        return IsContact(pos, Window.GameUp, Window.GameDown, Window.GameRight, Window.GameLeft, out contact);
    }

    #endregion

    #region Exit

    /// <returns>Box가 line보다 위일 때 true를 반환합니다.</returns>
    public bool IsExitUp(float y, float line) => line < y + center.y - 0.5f * size.y;
    
    /// <returns>Box가 line보다 아래일 때 true를 반환합니다.</returns>
    public bool IsExitDown(float y, float line) => line > y + center.y + 0.5f * size.y;
    
    /// <returns>Box가 line보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsExitRight(float x, float line) => line < x + center.x - 0.5f * size.x;

    /// <returns>Box가 line보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsExitLeft(float x, float line) => line > x + center.x + 0.5f * size.x;

    /// <param name="pos">box의 WorldPosition</param>
    /// <param name="up">위 기준선</param>
    /// <param name="down">아래 기준선</param>
    /// <param name="right">우측 기준선</param>
    /// <param name="left">좌측 기준선</param>
    /// <returns>Box가 기준선 밖에 있을 때 true를 반환합니다.</returns>
    public bool IsExit(Vector2 pos, float up, float down, float right, float left)
    {
        return IsExitUp(pos.y, up) 
            || IsExitDown(pos.y, down) 
            || IsExitRight(pos.x, right) 
            || IsExitLeft(pos.x, left);
    }

    #endregion

    /*
    #region Exit + Out

    /// <returns>Box가 line보다 위일 때 true를 반환합니다.</returns>
    public bool IsExitUp(float y, float line, out float contact)
    {
        contact = line - center.y + 0.5f * size.y;
        return y >= contact;
    }
    /// <returns>Box가 line보다 아래일 때 true를 반환합니다.</returns>
    public bool IsExitDown(float y, float line, out float contact)
    {
        contact = line - center.y - 0.5f * size.y;
        return y <= contact;
    }
    /// <returns>Box가 line보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsExitRight(float x, float line, out float contact)
    {
        contact = line - center.x + 0.5f * size.x;
        return x >= contact;
    }

    /// <returns>Box가 line보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsExitLeft(float x, float line, out float contact)
    {
        contact = line - center.x - 0.5f * size.x;
        return x <= contact;
    }

    /// <param name="pos">box의 WorldPosition</param>
    /// <param name="up">위 기준선</param>
    /// <param name="down">아래 기준선</param>
    /// <param name="right">우측 기준선</param>
    /// <param name="left">좌측 기준선</param>
    /// <param name="contact">접한 위치</param>
    /// <returns>Box가 접하거나 그 밖일 때 true를 반환합니다. // false일 때 contact = pos</returns>
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

    /// <returns>Box가 게임 외곽선에 접하거나 그 위일 때 true를 반환합니다.</returns>
    public bool IsExitGameUp(float x) => IsExitUp(x, Window.GameUp);

    /// <returns>Box가 게임 외곽선에 접하거나 그 위일 때 true를 반환합니다.</returns>
    public bool IsExitGameDown(float x) => IsExitDown(x, Window.GameDown);
    
    /// <returns>Box가 게임 외곽선에 접하거나 그보다 오른쪽일 때 true를 반환합니다.</returns>
    public bool IsExitGameRight(float y) => IsExitRight(y, Window.GameRight);
    
    /// <returns>Box가 게임 외곽선에 접하거나 그보다 왼쪽일 때 true를 반환합니다.</returns>
    public bool IsExitGameLeft(float y) => IsExitLeft(y, Window.GameLeft);

    /// <param name="pos">box의 WorldPosition</param>
    /// <returns>Box가 게임 외곽선 밖에 있을 때 true를 반환합니다.</returns>
    public bool IsExitGame(Vector2 pos)
    {
        return IsExitGameUp(pos.y)
            || IsExitGameDown(pos.y)
            || IsExitGameRight(pos.x)
            || IsExitGameLeft(pos.x);
    }

    #endregion
}