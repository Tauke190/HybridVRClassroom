using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using EPOOutline;

public class BonePuzzleManager : MonoBehaviourPun
{
    public GameObject[] bonesPuzzles;
    public GameObject spawnButtonPrefab;
    public GameObject modelMoveRay;
    public GameObject moveAreaPrefab;
    public Transform spawnButtonParent;
    public Transform reticle;
    public Material holderMaterial;
    public float bonePickUpSpeed = 10f;
    public float minDistance = 0.1f;

    private List<Button> spawnButtons = new List<Button>();
    private GameObject moveAreaRef;
    private bool spawning;
    private int spawnIndex;
    private CustomPlayerLocomotion player;

    private bool moving;

    public GameObject Current { get; private set; }

    private void Start()
    {
        for (int i = 0; i <= bonesPuzzles.Length; i++)
        {
            GameObject go = Instantiate(spawnButtonPrefab, spawnButtonParent);
            go.GetComponent<ModelSpawnButton>().SetUp(i, i < bonesPuzzles.Length ? bonesPuzzles[i].name : "Delete");

            Button btn = go.GetComponent<Button>();

            spawnButtons.Add(btn);

            if (i == bonesPuzzles.Length)
            {
                btn.interactable = false;
            }
        }

        player = FindObjectOfType<CustomPlayerLocomotion>();

        UiEvents.Instance.OnMoveComplete += MoveComplete;
    }

    private void OnDestroy()
    {
        UiEvents.Instance.OnMoveComplete -= MoveComplete;
    }

    private void MoveComplete()
    {
        if (spawning)
        {
            Spawn();
            spawning = false;
            return;
        }

        if (Current && Current.GetComponent<BoneRef>())
        {
            Current.GetComponent<BoneRef>().SyncTargetPosition();
            Current.GetComponent<BoneRef>().Freeze(false);
        }

        moving = false;
    }

    public void StartMoving()
    {
        if (Current.GetComponent<BoneRef>())
        {
            Current.GetComponent<BoneRef>().Freeze(true);
        }

        CreateMoveArea();

        ToggleAllBoneOutlines(false);

        reticle.transform.position = new Vector3(Current.transform.position.x, reticle.position.y, Current.transform.position.z);

        moving = true;
    }

    public void ToggleAllBoneOutlines(bool value)
    {
        if (Current.GetComponent<BoneRef>() == null)
        {
            return;
        }

        foreach (var item in Current.GetComponent<BoneRef>().Bones)
        {
            item.Target.GetComponent<Outlinable>().enabled = value;
            item.GetComponent<Outlinable>().enabled = value;
        }
    }

    public void CreateMoveArea()
    {
        DeleteMoveArea();
        moveAreaRef = Instantiate(moveAreaPrefab);
    }

    public void DeleteMoveArea()
    {
        if (moveAreaRef)
        {
            Destroy(moveAreaRef);
        }
    }

    private void Spawn()
    {
        if (Current)
        {
            PhotonNetwork.Destroy(Current);
        }

        GameObject objToSpawn = bonesPuzzles[spawnIndex];
        photonView.RPC("RpcToggleXRay", RpcTarget.All, objToSpawn.GetComponent<SeeThroughSync>() != null ? 1 : 0);

        Vector3 spawnPos = reticle.transform.position;
        //spawnPos.y = objToSpawn.transform.position.y;

        Current = PhotonNetwork.Instantiate(objToSpawn.name, spawnPos, objToSpawn.transform.rotation);

        SetButton(spawnIndex);

        spawning = false;
    }

    private void SetButton(int index)
    {
        foreach (var item in spawnButtons)
        {
            item.interactable = true;
        }

        if (index == -1)
        {
            spawnButtons[spawnButtons.Count - 1].interactable = false;
        }
        else
        {
            spawnButtons[index].interactable = false;
        }
    }

    public void Spawn(int index)
    {
        if (index == bonesPuzzles.Length)
        {
            if (Current)
            {
                PhotonNetwork.Destroy(Current);
            }

            SetButton(-1);
            return;
        }

        CreateMoveArea();

        modelMoveRay.SetActive(true);
        spawning = true;

        spawnIndex = index;
    }

    [PunRPC]
    private void RpcToggleXRay(int value)
    {
        FindObjectOfType<XRayToggler>().ToggleXRay(value == 1);
    }

    private void Update()
    {
        if (moving)
        {
            Vector3 dest = new Vector3(reticle.transform.position.x, transform.position.y, reticle.transform.position.z);
            Current.transform.position = Vector3.Lerp(Current.transform.position, dest, Time.deltaTime * 10f);

            if (player)
            {
                Vector2 joystickvalue = player.GetPrimaryJoystickValue() * VRPointer.Instance.rotateSensitivity;

                transform.Rotate(Vector3.up * joystickvalue.x);
            }
        }
    }
}
