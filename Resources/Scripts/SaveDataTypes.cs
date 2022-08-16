using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IndexRecordData
{
    public List<float> time;
    public List<int> index;

    public IndexRecordData()
    {
        time = new List<float>();
        index = new List<int>();
    }

    public void Add(int _index)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        index.Add(_index);
    }
}

[System.Serializable]
public class TwoIndexRecordData
{
    public List<float> time;
    public List<int> index1;
    public List<int> index2;

    public TwoIndexRecordData()
    {
        time = new List<float>();
        index1 = new List<int>();
        index2 = new List<int>();
    }

    public void Add(int _index1, int _index2)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        index1.Add(_index1);
        index2.Add(_index2);
    }
}

[System.Serializable]
public class PdfRecordData
{
    public List<float> time;
    public List<byte[]> texture;

    public PdfRecordData()
    {
        time = new List<float>();
        texture = new List<byte[]>();
    }

    public void Add(byte[] _texture)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        texture.Add(_texture);
    }
}

[System.Serializable]
public class WhiteboardRecordData
{
    public List<float> time;
    public List<int> id;
    public List<float[]> pos;
    public List<int> pensize;
    public List<float[]> color;

    public WhiteboardRecordData()
    {
        time = new List<float>();
        id = new List<int>();
        pos = new List<float[]>();
        pensize = new List<int>();
        color = new List<float[]>();
    }

    public void Add(int _id, float[] _pos, int _pensize, float[] _color)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        id.Add(_id);
        pos.Add(_pos);
        pensize.Add(_pensize);
        color.Add(_color);
    }
}

[System.Serializable]
public class TransformRecordData
{
    public List<float> time;
    public List<float> positionX;
    public List<float> positionY;
    public List<float> positionZ;
    public List<float> rotationX;
    public List<float> rotationY;
    public List<float> rotationZ;

    public TransformRecordData()
    {
        time = new List<float>();
        positionX = new List<float>();
        positionY = new List<float>();
        positionZ = new List<float>();
        rotationX = new List<float>();
        rotationY = new List<float>();
        rotationZ = new List<float>();
    }

    public void Add(Vector3 _position, Vector3 _angles)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        positionX.Add(_position.x);
        positionY.Add(_position.y);
        positionZ.Add(_position.z);
        rotationX.Add(_angles.x);
        rotationY.Add(_angles.y);
        rotationZ.Add(_angles.z);
    }
}

[System.Serializable]
public class PeopleData
{
    public enum State { Spawned, Moved, Left }

    public List<float> time;
    public List<int> actorId;
    public List<string> playerName;
    public List<int> chairIndex;
    public List<State> state;

    public PeopleData()
    {
        time = new List<float>();
        actorId = new List<int>();
        playerName = new List<string>();
        chairIndex = new List<int>();
        state = new List<State>();
    }

    public void Add(int _actorid, string _name, int _chair, State _state)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        actorId.Add(_actorid);
        playerName.Add(_name);
        chairIndex.Add(_chair);
        state.Add(_state);
    }
}

[System.Serializable]
public class ChatRecordData 
{
    public List<float> time;
    public List<string> chats;
    public List<int> actorid;

    public ChatRecordData()
    {
        time = new List<float>();
        chats = new List<string>();
        actorid = new List<int>();
    }

    public void Add(string chat, int id)
    {
        time.Add(RecordManager.Instance.RecordTimer);
        chats.Add(chat);
        actorid.Add(id);
    }
}

public class UID
{
    public static string BoardSelection = "BoardSelectionData";
    public static string ChatData = "ChatData";
    public static string ChatInitialization = "ChatInitializationData";
    public static string WhiteboardData = "WhiteboardData";
    public static string WhiteboardInitialization = "WhiteboardInitializationData";
    public static string WhiteboardTransformData = "WhiteboardTransformData";
    public static string PeopleData = "PeopleData";
    public static string PeopleInitializationData = "PeopleInitializationData";
    public static string HandRaiseData = "HandRaiseData";
    public static string EmojiData = "EmojiData";
    public static string SlidesData = "SlidesData";
    public static string HeadPositionData = "HeadPositionData";
    public static string Hand1PositionData = "Hand1PositionData";
    public static string Hand2PositionData = "Hand2PositionData";
    public static string MarkerPositionData = "MarkerPositionData";
} 

