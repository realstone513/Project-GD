using System.Collections.Generic;
using UnityEngine;

public class EmptyWorkspace : GenericWindow
{
    public GameObject infoPrefab;
    public Transform infoSpawnTransform;
    public List<GameObject> infos;

    private void OnEnable()
    {
        List<GameObject> unassignEmployeeList = EmployeeManager.instance.GetUnassgin();
        foreach (GameObject emp in unassignEmployeeList)
        {
            GameObject info = Instantiate(infoPrefab, infoSpawnTransform);
            info.GetComponent<EmployeeInfo>().SetInit(emp);
            infos.Add(info);
        }
    }

    private void OnDisable()
    {
        foreach (GameObject info in infos)
            Destroy(info);
        infos.Clear();
    }
}