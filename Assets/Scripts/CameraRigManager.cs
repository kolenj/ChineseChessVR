// --------------------------------------------------------------------------------------------------------------------
// <copyright file=CameraRigManager.cs company=League of HTC Vive Developers>
/*
11111111111111111111111111111111111111001111111111111111111111111
11111111111111111111111111111111111100011111111111111111111111111
11111111111111111111111111111111100001111111111111111111111111111
11111111111111111111111111111110000111111111111111111111111111111
11111111111111111111111111111000000111111111111111111111111111111
11111111111111111111111111100000011110001100000000000000011111111
11111111111111111100000000000000000000000000000000011111111111111
11111111111111110111000000000000000000000000000011111111111111111
11111111111111111111111000000000000000000000000000000000111111111
11111111111111111110000000000000000000000000000000111111111111111
11111111111111111100011100000000000000000000000000000111111111111
11111111111111100000110000000000011000000000000000000011111111111
11111111111111000000000000000100111100000000000001100000111111111
11111111110000000000000000001110111110000000000000111000011111111
11111111000000000000000000011111111100000000000000011110001111111
11111110000000011111111111111111111100000000000000001111100111111
11111111000001111111111111111111110000000000000000001111111111111
11111111110111111111111111111100000000000000000000000111111111111
11111111111111110000000000000000000000000000000000000111111111111
11111111111111111100000000000000000000000000001100000111111111111
11111111111111000000000000000000000000000000111100000111111111111
11111111111000000000000000000000000000000001111110000111111111111
11111111100000000000000000000000000000001111111110000111111111111
11111110000000000000000000000000000000111111111110000111111111111
11111100000000000000000001110000001111111111111110001111111111111
11111000000000000000011111111111111111111111111110011111111111111
11110000000000000001111111111111111100111111111111111111111111111
11100000000000000011111111111111111111100001111111111111111111111
11100000000001000111111111111111111111111000001111111111111111111
11000000000001100111111111111111111111111110000000111111111111111
11000000000000111011111111111100011111000011100000001111111111111
11000000000000011111111111111111000111110000000000000011111111111
11000000000000000011111111111111000000000000000000000000111111111
11001000000000000000001111111110000000000000000000000000001111111
11100110000000000001111111110000000000000000111000000000000111111
11110110000000000000000000000000000000000111111111110000000011111
11111110000000000000000000000000000000001111111111111100000001111
11111110000010000000000000000001100000000111011111111110000001111
11111111000111110000000000000111110000000000111111111110110000111
11111110001111111100010000000001111100000111111111111111110000111
11111110001111111111111110000000111111100000000111111111111000111
11111111001111111111111111111000000111111111111111111111111100011
11111111101111111111111111111110000111111111111111111111111001111
11111111111111111111111111111110001111111111111111111111100111111
11111111111111111111111111111111001111111111111111111111001111111
11111111111111111111111111111111100111111111111111111111111111111
11111111111111111111111111111111110111111111111111111111111111111
*/
//   
// </copyright>
// <summary>
//  Chinese Chess VR
// </summary>
// <author>胡良云（CloudHu）</author>
//中文注释：胡良云（CloudHu） 3/21/2017

// --------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// FileName: CameraRigManager.cs
/// Author: 胡良云（CloudHu）
/// Corporation: 
/// Description: 这个脚本用于管理CameraRig
/// DateTime: 3/21/2017
/// </summary>
public class CameraRigManager : Photon.PunBehaviour, IPunObservable
{

    #region Public Variables  //公共变量区域

	[Tooltip("玩家UI游戏对象预设")]
	public GameObject PlayerUiPrefab;

    [Tooltip("本地玩家实例。使用这个来了解本地玩家是否在场景中")]
    public static GameObject LocalPlayerInstance;

	[Tooltip("玩家当前的体力值")]
	public float Health = 1f;

	[Tooltip("位置索引:1是头；2是左手；3是右手")]
	public int index = 1;	
    #endregion


    #region Private Variables   //私有变量区域


    #endregion


    #region MonoBehaviour CallBacks //回调函数区域

    void Awake()
    {
        // 用于GameManager.cs: 我们跟踪的本地玩家实例来防止在关卡被同步时实例化
        if (photonView.isMine)
        {
			if(index==1)
			CameraRigManager.LocalPlayerInstance = this.gameObject;
        }
        // #关键  我们标识不在加载时被摧毁，使实例在关卡同步时保留下来，从而使关卡加载时有无缝体验。
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {

		//创建玩家UI
		if (this.PlayerUiPrefab != null)
		{
			GameObject _uiGo = Instantiate(this.PlayerUiPrefab,Vector3.zero,Quaternion.identity,transform) as GameObject;
			//_uiGo.transform.SetParent (transform,false);
			_uiGo.transform.localPosition = new Vector3 (0, 1f, 0);
			_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
		}
		else
		{
			Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (photonView.isMine)
		{
			switch (index) {
			case 1:
				transform.position = GameManager.Instance.head.position;
				transform.rotation = GameManager.Instance.head.rotation;
				if (this.Health <= 0f)
				{
					GameManager.Instance.LeaveRoom();
				}
				break;
			case 2:
				transform.position = GameManager.Instance.leftHand.position;
				transform.rotation = GameManager.Instance.leftHand.rotation;
				break;
			default:
				transform.position = GameManager.Instance.rightHand.position;
				transform.rotation = GameManager.Instance.rightHand.rotation;
				break;
			}

		}

	}
    #endregion

    #region Public Methods	//公共方法区域


    #endregion
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // 我们是本地玩家，则把数据发送给远程玩家
           // stream.SendNext(this.IsFiring);
            stream.SendNext(this.Health);
        }
        else
        {
            //网络玩家则接收数据
          //  this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
