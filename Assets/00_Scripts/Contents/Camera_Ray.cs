using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class Camera_Ray : NetworkBehaviour
{
    HeroHolder holder = null;
    HeroHolder moveHolder = null;
    string HostAndClient = "";
    private void Start()
    {
        HostAndClient = Net_Utils.LocalId() == 0 ? "HostHolder" : "ClientHolder";
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MouseButtonDown();
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MouseButton();
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            MouseButtonUp();
        }
    }

    private void MouseButtonDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hitpoint = Physics2D.Raycast(ray.origin, ray.direction);

        if (holder != null)
        {
            holder.HideRange();
            holder = null;
        }

        if (hitpoint.collider != null)
        {           
            holder = hitpoint.collider.GetComponent<HeroHolder>();
            if (holder == null) return;
            if (holder._holderType == Data.HeroType.NONE)
            {
                holder = null;
                return;
            }
            bool CanGet = false;
            int value = (int)NetworkManager.Singleton.LocalClientId;
            if (value == 0) CanGet = holder._holder_Name.Contains("Host");
            else if (value == 1) CanGet = holder._holder_Name.Contains("Client");

            if (!CanGet) holder = null;
        }
    }

    private void MouseButton()
    {
        if (holder != null)
        {
            holder.SelectedHolder();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitpoint = Physics2D.Raycast(ray.origin, ray.direction);

            if (hitpoint.collider != null && hitpoint.collider.transform != holder.transform)
            {
                if (moveHolder != null && hitpoint.collider.transform != moveHolder.transform)
                {
                    moveHolder.UnSelectMovePlace();
                }
                moveHolder = hitpoint.collider.GetComponent<HeroHolder>();

                if (!moveHolder._holder_Name.Contains(HostAndClient))
                {
                    moveHolder = null;
                }

                if(moveHolder != null)
                    moveHolder.SelectMovePlace();
            }
        }
    }
    private void MouseButtonUp()
    {
        if (holder == null) return;
        if (moveHolder == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitpoint = Physics2D.Raycast(ray.origin, ray.direction);
            if (hitpoint.collider != null)
            {
                // 마우스 Down한 곳과 UP한 곳이 같다면
                if (holder.transform == hitpoint.collider.transform)
                {
                    holder.ShowRange();
                    holder.UnSelectedHolder();
                }
            }          
        }
        else
        {         
            moveHolder.UnSelectMovePlace();
            holder.UnSelectedHolder();
            Managers.Spawn.Spawner.HolderPositionChange(holder, moveHolder, Net_Utils.LocalId());
        }
        if (holder != null)
            holder.UnSelectedHolder();

        moveHolder = null;
    }
}
