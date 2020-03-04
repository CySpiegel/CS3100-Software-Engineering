using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimatior : MonoBehaviour
{

    public GameObject MenuOffScreenPos;
    public GameObject MenuOnScreenPos;
    public GameObject MenuPanel;

    static private bool MoveMenu;
    private bool MoveMenuBack;
    private float MenuMovementSpeed = 10.0f;
    private float tempMenuPos;

    // Start is called before the first frame update
    void Start()
    {
        MoveMenu = false;
        MenuPanel.transform.position = MenuOffScreenPos.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(MoveMenu)
        {
            MenuPanel.transform.position = Vector3.Lerp(MenuPanel.transform.position, 
                MenuOnScreenPos.transform.position, MenuMovementSpeed * Time.deltaTime);

            if(MenuPanel.transform.localPosition.x == tempMenuPos)
            {
                MoveMenu = false;
                MenuPanel.transform.position = MenuOffScreenPos.transform.position;
                tempMenuPos = -99999999.99f;
            }
            if(MoveMenu)
            {
                tempMenuPos = MenuPanel.transform.position.x;
            }

        }

        if(MoveMenuBack)
        {
            MenuPanel.transform.position = Vector3.Lerp(MenuPanel.transform.position,
                MenuOffScreenPos.transform.position, MenuMovementSpeed * Time.deltaTime);

            if (MenuPanel.transform.localPosition.x == tempMenuPos)
            {
                MoveMenuBack = false;
                MenuPanel.transform.position = MenuOffScreenPos.transform.position;
                tempMenuPos = -99999999.99f;
            }
            if (MoveMenuBack)
            {
                tempMenuPos = MenuPanel.transform.position.x;
            }
        }
    }


    public void MoveMenuPanelToScreenClick()
    {
        MoveMenuBack = MoveMenu;
        MoveMenu = !MoveMenu;
        
    }
}