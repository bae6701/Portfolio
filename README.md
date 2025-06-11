[Project] 운빨존많겜 : Unity NetCode 기반 실시간 멀티플레이 타워 디펜스

![로비 스크린샷]([https://github.com/user-attachments/assets/4f409d3f-eb78-418f-80ac-103512c743dd](https://github.com/user-attachments/assets/0e5dadbe-80e5-4825-a028-44fe32d29c29))

<br>
핵심 플레이 영상(https://youtu.be/rImSYQ_9wTE)
<br>

핵심 기능
- 실시간 멀티플레이 : Unity NetCode for GameObject를 이용한 호스트/클라이언트 방식의 2인 협동 플레이
- 네트워크 동기화 : 영웅 유닛의 소환, 이동, 공격 대상 지정 등 주요 로직 실시간 동기화
- 클라우드 저장 : Unity Cloud 서비스를 연동하여 닉네임, 재화, 최고 스테이지, 보유 영웅 등 유저 핵심 데이터 저장/로드
- 유닛 조합 : ScriptableObject를 활용한 코드 수정 없이 새로운 조합법을 추가/제거/변경

기술 상세
1. Networking
  - 구현 방식 : Listen Server (Host) 방식으로 클라이언트가 호스트에 접속하여 함께 플레이하는 구조를 채택
  - 주요 동기화 로직
    - NetworkObject : 동기화가 필요한 모든 유닛(영웅, 몬스터)에 적용하여 식별
    - ServerRpc / ClientRpc : 클라이언트의 요청을 호스트에서 처리하고, 그 결과를 모든 클라이언트에게 전파
  - 어려웠던 점 및 해결 : 호스트에서 소환된 영웅이 클라이언트에서는 다른 몬스터를 공격하는 데이터 불일치 문제가 발생했습니다.
                        원인은 공격 대상(Target)을 각 클라이언트가 개별적으로 탐색했기 때문입니다. 이를 해결하기 위해,
                        호스트가 공격 대상을 결정하면 해당 몬스터의 NetworkObjectId를 ServerRpc로 모든 클라이언트에 전파하여 공격 대상을 통일시키는 방식으로 문제를 해결했습니다.
                        이 경험을 통해 서버 권위적(Server-Authoritative) 구조의 필요성을 깊이 이해하게 되었습니다.

2. Data Management
  - Unity Cloud Save : 유저의 핵심 성장 데이터를 Json 형태로 관리하여 서버에 저장하고, 게임 시작시 불러오는 기능을 구현
  - ScriptableObject : 영웅, 몬스터의 기본 스탯 데이터를 ScriptableObject로 관리하여, 기획 단계에서의 밸런스 수정 및 데이터 확장이 용이하도록 설계했습니다.

3. Data-Driven Design
  - 목적 : 기획자와 협업 효율울 높이고, 코드 수정 없이 게임 밸런스와 콘텐츠를 관리하기 위해 데이터 기반 설계를 적용했습니다.
  - 유닛 스탯 관리 : 영웅, 몬스터의 기본 스탯 데이터를 ScriptableObject로 관리하여, 기획 단계에서의 밸런스 수정 및 데이터 확장이 용이하도록 설계했습니다.
  - 유닛 조합 시스템 : 모든 유닛 조합법 또한  ScriptableObject에셋으로 분리했습니다. 이를 통해 코드 수정 없이 새로운 조합법을 추가/변경할 수 있어 라이브 서비스 중 콘텐츠 업데이트에 유연하게 대처할 수 있는 구조를 만들었습니다.

사용 기술
- Engine : Unity 6000.0.23f1
- Language : C#
- Networking : Unity NetCode for GameObjects
- Backend : Unity Cloud Save
- IDE : Visual Studio
