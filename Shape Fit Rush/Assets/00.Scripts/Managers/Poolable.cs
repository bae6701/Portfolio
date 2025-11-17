using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// (리빌드 v2.5 아키텍처 필수 스크립트)
/// 풀(Pool)로 관리될 모든 프리팹(블록, VFX)에 부착되는 '꼬리표'.
/// PoolManager가 이 컴포넌트를 기준으로 Push/Pop을 관리합니다.
/// </summary>
public class Poolable : MonoBehaviour
{
	public bool IsUsing;
}